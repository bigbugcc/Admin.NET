// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 作业持久化（数据库）
/// </summary>
public class DbJobPersistence : IJobPersistence
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DbJobPersistence(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    /// <summary>
    /// 作业调度服务启动时
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public async Task<IEnumerable<SchedulerBuilder>> PreloadAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>().CopyNew();
        var dynamicJobCompiler = scope.ServiceProvider.GetRequiredService<DynamicJobCompiler>();

        // 获取所有定义的作业
        var allJobs = App.EffectiveTypes.ScanToBuilders().ToList();
        // 若数据库不存在任何作业，则直接返回
        if (!db.Queryable<SysJobDetail>().Any(u => true)) return allJobs;

        // 遍历所有定义的作业
        foreach (var schedulerBuilder in allJobs)
        {
            // 获取作业信息构建器
            var jobBuilder = schedulerBuilder.GetJobBuilder();

            // 加载数据库数据
            var dbDetail = await db.Queryable<SysJobDetail>().FirstAsync(u => u.JobId == jobBuilder.JobId);
            if (dbDetail == null) continue;

            // 同步数据库数据
            jobBuilder.LoadFrom(dbDetail);

            // 获取作业的所有数据库的触发器
            var dbTriggers = await db.Queryable<SysJobTrigger>().Where(u => u.JobId == jobBuilder.JobId).ToListAsync();
            // 遍历所有作业触发器
            foreach (var (_, triggerBuilder) in schedulerBuilder.GetEnumerable())
            {
                // 加载数据库数据
                var dbTrigger = dbTriggers.FirstOrDefault(u => u.JobId == jobBuilder.JobId && u.TriggerId == triggerBuilder.TriggerId);
                if (dbTrigger == null) continue;

                triggerBuilder.LoadFrom(dbTrigger).Updated(); // 标记更新
            }
            // 遍历所有非编译时定义的触发器加入到作业中
            foreach (var dbTrigger in dbTriggers)
            {
                if (schedulerBuilder.GetTriggerBuilder(dbTrigger.TriggerId)?.JobId == jobBuilder.JobId) continue;
                var triggerBuilder = TriggerBuilder.Create(dbTrigger.TriggerId).LoadFrom(dbTrigger);
                schedulerBuilder.AddTriggerBuilder(triggerBuilder); // 先添加
                triggerBuilder.Updated(); // 再标记更新
            }

            // 标记更新
            schedulerBuilder.Updated();
        }

        // 获取数据库所有通过脚本创建的作业
        var allDbScriptJobs = await db.Queryable<SysJobDetail>().Where(u => u.CreateType != JobCreateTypeEnum.BuiltIn).ToListAsync();
        foreach (var dbDetail in allDbScriptJobs)
        {
            // 动态创建作业
            Type jobType;
            switch (dbDetail.CreateType)
            {
                case JobCreateTypeEnum.Script:
                    jobType = dynamicJobCompiler.BuildJob(dbDetail.ScriptCode);
                    break;

                case JobCreateTypeEnum.Http:
                    jobType = typeof(HttpJob);
                    break;

                default:
                    throw new NotSupportedException();
            }

            // 动态构建的 jobType 的程序集名称为随机名称，需重新设置
            dbDetail.AssemblyName = jobType.Assembly.FullName!.Split(',')[0];
            var jobBuilder = JobBuilder.Create(jobType).LoadFrom(dbDetail);

            // 强行设置为不扫描 IJob 实现类 [Trigger] 特性触发器，否则 SchedulerBuilder.Create 会再次扫描，导致重复添加同名触发器
            jobBuilder.SetIncludeAnnotations(false);

            // 获取作业的所有数据库的触发器加入到作业中
            var dbTriggers = await db.Queryable<SysJobTrigger>().Where(u => u.JobId == jobBuilder.JobId).ToListAsync();
            var triggerBuilders = dbTriggers.Select(u => TriggerBuilder.Create(u.TriggerId).LoadFrom(u).Updated());
            var schedulerBuilder = SchedulerBuilder.Create(jobBuilder, triggerBuilders.ToArray());

            // 标记更新
            schedulerBuilder.Updated();

            allJobs.Add(schedulerBuilder);
        }

        return allJobs;
    }

    /// <summary>
    /// 作业计划初始化通知
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    public Task<SchedulerBuilder> OnLoadingAsync(SchedulerBuilder builder, CancellationToken stoppingToken)
    {
        return Task.FromResult(builder);
    }

    /// <summary>
    /// 作业计划Scheduler的JobDetail变化时
    /// </summary>
    /// <param name="context"></param>
    public async Task OnChangedAsync(PersistenceContext context)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>().CopyNew();

            var jobDetail = context.JobDetail.Adapt<SysJobDetail>();
            switch (context.Behavior)
            {
                case PersistenceBehavior.Appended:
                    await db.Insertable(jobDetail).ExecuteCommandAsync();
                    break;

                case PersistenceBehavior.Updated:
                    await db.Updateable(jobDetail).WhereColumns(u => new { u.JobId }).IgnoreColumns(u => new { u.Id, u.CreateType, u.ScriptCode }).ExecuteCommandAsync();
                    break;

                case PersistenceBehavior.Removed:
                    await db.Deleteable<SysJobDetail>().Where(u => u.JobId == jobDetail.JobId).ExecuteCommandAsync();
                    break;
            }
        }
    }

    /// <summary>
    /// 作业计划Scheduler的触发器Trigger变化时
    /// </summary>
    /// <param name="context"></param>
    public async Task OnTriggerChangedAsync(PersistenceTriggerContext context)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>().CopyNew();

            var jobTrigger = context.Trigger.Adapt<SysJobTrigger>();
            switch (context.Behavior)
            {
                case PersistenceBehavior.Appended:
                    await db.Insertable(jobTrigger).ExecuteCommandAsync();
                    break;

                case PersistenceBehavior.Updated:
                    await db.Updateable(jobTrigger).WhereColumns(u => new { u.TriggerId, u.JobId }).IgnoreColumns(u => new { u.Id }).ExecuteCommandAsync();
                    break;

                case PersistenceBehavior.Removed:
                    await db.Deleteable<SysJobTrigger>().Where(u => u.TriggerId == jobTrigger.TriggerId && u.JobId == jobTrigger.JobId).ExecuteCommandAsync();
                    break;
            }
        }
    }

    /// <summary>
    /// 作业触发器运行记录
    /// </summary>
    /// <param name="timeline"></param>
    public async Task OnExecutionRecordAsync(TriggerTimeline timeline)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>().CopyNew();

            var jobTriggerRecord = timeline.Adapt<SysJobTriggerRecord>();
            await db.Insertable(jobTriggerRecord).ExecuteCommandAsync();
        }
    }
}