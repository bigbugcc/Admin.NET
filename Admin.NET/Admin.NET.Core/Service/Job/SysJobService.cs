// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统作业任务服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 320, Description = "作业任务")]
public class SysJobService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysJobDetail> _sysJobDetailRep;
    private readonly SqlSugarRepository<SysJobTrigger> _sysJobTriggerRep;
    private readonly SqlSugarRepository<SysJobTriggerRecord> _sysJobTriggerRecordRep;
    private readonly SqlSugarRepository<SysJobCluster> _sysJobClusterRep;
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly DynamicJobCompiler _dynamicJobCompiler;

    public SysJobService(SqlSugarRepository<SysJobDetail> sysJobDetailRep,
        SqlSugarRepository<SysJobTrigger> sysJobTriggerRep,
        SqlSugarRepository<SysJobTriggerRecord> sysJobTriggerRecordRep,
        SqlSugarRepository<SysJobCluster> sysJobClusterRep,
        ISchedulerFactory schedulerFactory,
        DynamicJobCompiler dynamicJobCompiler)
    {
        _sysJobDetailRep = sysJobDetailRep;
        _sysJobTriggerRep = sysJobTriggerRep;
        _sysJobTriggerRecordRep = sysJobTriggerRecordRep;
        _sysJobClusterRep = sysJobClusterRep;
        _schedulerFactory = schedulerFactory;
        _dynamicJobCompiler = dynamicJobCompiler;
    }

    /// <summary>
    /// 获取作业分页列表 ⏰
    /// </summary>
    [DisplayName("获取作业分页列表")]
    public async Task<SqlSugarPagedList<JobDetailOutput>> PageJobDetail(PageJobDetailInput input)
    {
        var jobDetails = await _sysJobDetailRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.JobId), u => u.JobId.Contains(input.JobId.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.GroupName), u => u.GroupName.Contains(input.GroupName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Description), u => u.Description.Contains(input.Description.Trim()))
            .Select(d => new JobDetailOutput
            {
                JobDetail = d,
            }).ToPagedListAsync(input.Page, input.PageSize);
        await _sysJobDetailRep.AsSugarClient().ThenMapperAsync(jobDetails.Items, async u =>
        {
            u.JobTriggers = await _sysJobTriggerRep.GetListAsync(t => t.JobId == u.JobDetail.JobId);
        });

        // 提取中括号里面的参数值
        var rgx = new Regex(@"(?i)(?<=\[)(.*)(?=\])");
        foreach (var job in jobDetails.Items)
        {
            foreach (var jobTrigger in job.JobTriggers)
            {
                jobTrigger.Args = rgx.Match(jobTrigger.Args ?? "").Value;
            }
        }
        return jobDetails;
    }

    /// <summary>
    /// 获取作业组名称集合 ⏰
    /// </summary>
    [DisplayName("获取作业组名称集合")]
    public async Task<List<string>> ListJobGroup()
    {
        return await _sysJobDetailRep.AsQueryable().Distinct().Select(e => e.GroupName).ToListAsync();
    }

    /// <summary>
    /// 添加作业 ⏰
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "AddJobDetail"), HttpPost]
    [DisplayName("添加作业")]
    public async Task AddJobDetail(AddJobDetailInput input)
    {
        var isExist = await _sysJobDetailRep.IsAnyAsync(u => u.JobId == input.JobId && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1006);

        // 动态创建作业
        Type jobType;
        switch (input.CreateType)
        {
            case JobCreateTypeEnum.Script when string.IsNullOrEmpty(input.ScriptCode):
                throw Oops.Oh(ErrorCodeEnum.D1701);
            case JobCreateTypeEnum.Script:
                {
                    jobType = _dynamicJobCompiler.BuildJob(input.ScriptCode);

                    if (jobType.GetCustomAttributes(typeof(JobDetailAttribute)).FirstOrDefault() is not JobDetailAttribute jobDetailAttribute)
                        throw Oops.Oh(ErrorCodeEnum.D1702);
                    if (jobDetailAttribute.JobId != input.JobId)
                        throw Oops.Oh(ErrorCodeEnum.D1703);
                    break;
                }
            case JobCreateTypeEnum.Http:
                jobType = typeof(HttpJob);
                break;

            default:
                throw new NotSupportedException();
        }

        _schedulerFactory.AddJob(JobBuilder.Create(jobType).LoadFrom(input.Adapt<SysJobDetail>()).SetJobType(jobType));

        // 延迟一下等待持久化写入，再执行其他字段的更新
        await Task.Delay(500);
        await _sysJobDetailRep.AsUpdateable()
            .SetColumns(u => new SysJobDetail { CreateType = input.CreateType, ScriptCode = input.ScriptCode })
            .Where(u => u.JobId == input.JobId).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新作业 ⏰
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "UpdateJobDetail"), HttpPost]
    [DisplayName("更新作业")]
    public async Task UpdateJobDetail(UpdateJobDetailInput input)
    {
        var isExist = await _sysJobDetailRep.IsAnyAsync(u => u.JobId == input.JobId && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1006);

        var sysJobDetail = await _sysJobDetailRep.GetFirstAsync(u => u.Id == input.Id);
        if (sysJobDetail.JobId != input.JobId) throw Oops.Oh(ErrorCodeEnum.D1704);

        var scheduler = _schedulerFactory.GetJob(sysJobDetail.JobId);
        var oldScriptCode = sysJobDetail.ScriptCode; // 旧脚本代码
        input.Adapt(sysJobDetail);

        if (input.CreateType == JobCreateTypeEnum.Script)
        {
            if (string.IsNullOrEmpty(input.ScriptCode)) throw Oops.Oh(ErrorCodeEnum.D1701);

            if (input.ScriptCode != oldScriptCode)
            {
                // 动态创建作业
                var jobType = _dynamicJobCompiler.BuildJob(input.ScriptCode);

                if (jobType.GetCustomAttributes(typeof(JobDetailAttribute)).FirstOrDefault() is not JobDetailAttribute jobDetailAttribute)
                    throw Oops.Oh(ErrorCodeEnum.D1702);
                if (jobDetailAttribute.JobId != input.JobId) throw Oops.Oh(ErrorCodeEnum.D1703);

                scheduler?.UpdateDetail(JobBuilder.Create(jobType).LoadFrom(sysJobDetail).SetJobType(jobType));
            }
        }
        else
        {
            scheduler?.UpdateDetail(scheduler.GetJobBuilder().LoadFrom(sysJobDetail));
        }

        // Tip: 假如这次更新有变更了 JobId，变更 JobId 后触发的持久化更新执行，会由于找不到 JobId 而更新不到数据
        // 延迟一下等待持久化写入，再执行其他字段的更新
        await Task.Delay(500);
        await _sysJobDetailRep.UpdateAsync(sysJobDetail);
    }

    /// <summary>
    /// 删除作业 ⏰
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "DeleteJobDetail"), HttpPost]
    [DisplayName("删除作业")]
    public async Task DeleteJobDetail(DeleteJobDetailInput input)
    {
        _schedulerFactory.RemoveJob(input.JobId);

        // 如果 _schedulerFactory 中不存在 JodId，则无法触发持久化，下面的代码确保作业和触发器能被删除
        await _sysJobDetailRep.DeleteAsync(u => u.JobId == input.JobId);
        await _sysJobTriggerRep.DeleteAsync(u => u.JobId == input.JobId);
    }

    /// <summary>
    /// 获取触发器列表 ⏰
    /// </summary>
    [DisplayName("获取触发器列表")]
    public async Task<List<SysJobTrigger>> GetJobTriggerList([FromQuery] JobDetailInput input)
    {
        return await _sysJobTriggerRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.JobId), u => u.JobId.Contains(input.JobId))
            .ToListAsync();
    }

    /// <summary>
    /// 添加触发器 ⏰
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "AddJobTrigger"), HttpPost]
    [DisplayName("添加触发器")]
    public async Task AddJobTrigger(AddJobTriggerInput input)
    {
        var isExist = await _sysJobTriggerRep.IsAnyAsync(u => u.TriggerId == input.TriggerId && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1006);

        var jobTrigger = input.Adapt<SysJobTrigger>();
        jobTrigger.Args = "[" + jobTrigger.Args + "]";

        var scheduler = _schedulerFactory.GetJob(input.JobId);
        scheduler?.AddTrigger(Triggers.Create(input.AssemblyName, input.TriggerType).LoadFrom(jobTrigger));
    }

    /// <summary>
    /// 更新触发器 ⏰
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "UpdateJobTrigger"), HttpPost]
    [DisplayName("更新触发器")]
    public async Task UpdateJobTrigger(UpdateJobTriggerInput input)
    {
        var isExist = await _sysJobTriggerRep.IsAnyAsync(u => u.TriggerId == input.TriggerId && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1006);

        var jobTrigger = input.Adapt<SysJobTrigger>();
        if (jobTrigger.EndTime.HasValue && jobTrigger.EndTime.Value.Year < 1901)
        {
            jobTrigger.EndTime = null;
        }
        if (jobTrigger.StartTime.HasValue && jobTrigger.StartTime.Value.Year < 1901)
        {
            jobTrigger.StartTime = null;
        }
        jobTrigger.Args = "[" + jobTrigger.Args + "]";

        var scheduler = _schedulerFactory.GetJob(input.JobId);
        scheduler?.UpdateTrigger(Triggers.Create(input.AssemblyName, input.TriggerType).LoadFrom(jobTrigger));
    }

    /// <summary>
    /// 删除触发器 ⏰
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "DeleteJobTrigger"), HttpPost]
    [DisplayName("删除触发器")]
    public async Task DeleteJobTrigger(DeleteJobTriggerInput input)
    {
        var scheduler = _schedulerFactory.GetJob(input.JobId);
        scheduler?.RemoveTrigger(input.TriggerId);

        // 如果 _schedulerFactory 中不存在 JodId，则无法触发持久化，下行代码确保触发器能被删除
        await _sysJobTriggerRep.DeleteAsync(u => u.JobId == input.JobId && u.TriggerId == input.TriggerId);
    }

    /// <summary>
    /// 暂停所有作业 ⏰
    /// </summary>
    /// <returns></returns>
    [DisplayName("暂停所有作业")]
    public void PauseAllJob()
    {
        _schedulerFactory.PauseAll();
    }

    /// <summary>
    /// 启动所有作业 ⏰
    /// </summary>
    /// <returns></returns>
    [DisplayName("启动所有作业")]
    public void StartAllJob()
    {
        _schedulerFactory.StartAll();
    }

    /// <summary>
    /// 暂停作业 ⏰
    /// </summary>
    [DisplayName("暂停作业")]
    public void PauseJob(JobDetailInput input)
    {
        _schedulerFactory.TryPauseJob(input.JobId, out _);
    }

    /// <summary>
    /// 启动作业 ⏰
    /// </summary>
    [DisplayName("启动作业")]
    public void StartJob(JobDetailInput input)
    {
        _schedulerFactory.TryStartJob(input.JobId, out _);
    }

    /// <summary>
    /// 取消作业 ⏰
    /// </summary>
    [DisplayName("取消作业")]
    public void CancelJob(JobDetailInput input)
    {
        _schedulerFactory.TryCancelJob(input.JobId, out _);
    }

    /// <summary>
    /// 执行作业 ⏰
    /// </summary>
    /// <param name="input"></param>
    [DisplayName("执行作业")]
    public void RunJob(JobDetailInput input)
    {
        if (_schedulerFactory.TryRunJob(input.JobId, out _) != ScheduleResult.Succeed) throw Oops.Oh(ErrorCodeEnum.D1705);
    }

    /// <summary>
    /// 暂停触发器 ⏰
    /// </summary>
    [DisplayName("暂停触发器")]
    public void PauseTrigger(JobTriggerInput input)
    {
        var scheduler = _schedulerFactory.GetJob(input.JobId);
        scheduler?.PauseTrigger(input.TriggerId);
    }

    /// <summary>
    /// 启动触发器 ⏰
    /// </summary>
    [DisplayName("启动触发器")]
    public void StartTrigger(JobTriggerInput input)
    {
        var scheduler = _schedulerFactory.GetJob(input.JobId);
        scheduler?.StartTrigger(input.TriggerId);
    }

    /// <summary>
    /// 强制唤醒作业调度器 ⏰
    /// </summary>
    [DisplayName("强制唤醒作业调度器")]
    public void CancelSleep()
    {
        _schedulerFactory.CancelSleep();
    }

    /// <summary>
    /// 强制触发所有作业持久化 ⏰
    /// </summary>
    [DisplayName("强制触发所有作业持久化")]
    public void PersistAll()
    {
        _schedulerFactory.PersistAll();
    }

    /// <summary>
    /// 获取集群列表 ⏰
    /// </summary>
    [DisplayName("获取集群列表")]
    public async Task<List<SysJobCluster>> GetJobClusterList()
    {
        return await _sysJobClusterRep.GetListAsync();
    }

    /// <summary>
    /// 获取作业触发器运行记录分页列表 ⏰
    /// </summary>
    [DisplayName("获取作业触发器运行记录分页列表")]
    public async Task<SqlSugarPagedList<SysJobTriggerRecord>> PageJobTriggerRecord(PageJobTriggerRecordInput input)
    {
        return await _sysJobTriggerRecordRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.JobId), u => u.JobId == input.JobId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.TriggerId), u => u.TriggerId == input.TriggerId)
            .OrderByDescending(u => u.Id)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 清空作业触发器运行记录 🔖
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "ClearJobTriggerRecord"), HttpPost]
    [DisplayName("清空作业触发器运行记录")]
    public void ClearJobTriggerRecord()
    {
        _sysJobTriggerRecordRep.AsSugarClient().DbMaintenance.TruncateTable<SysJobTriggerRecord>();
    }

    /// <summary>
    /// 清空不保留的作业触发器运行记录 🔖
    /// </summary>
    /// <returns></returns>
    [NonAction]
    [DisplayName("清空过期的作业触发器运行记录")]
    public async Task ClearExpireJobTriggerRecord(SysJobTriggerRecord input)
    {
        int keepRecords = 30;//保留记录条数
        // 使用CopyNew()创建新的数据库连接实例，避免连接冲突
        var db = _sysJobTriggerRecordRep.AsSugarClient().CopyNew();
        await db.Deleteable<SysJobTriggerRecord>().In(it => it.Id,
            db.Queryable<SysJobTriggerRecord>()
                .Skip(keepRecords)
                .OrderByDescending(it => it.LastRunTime)
                .Where(u => u.JobId == input.JobId && u.TriggerId == input.TriggerId)
                .Select(it => it.Id) //注意Select不要ToList(), ToList就2次查询了
        ).ExecuteCommandAsync();
    }
}