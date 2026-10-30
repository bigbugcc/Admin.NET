// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Furion.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Admin.NET.Plugin.DingTalk.Service;

/// <summary>
/// 钉钉服务 🧩
/// </summary>
[ApiDescriptionSettings(DingTalkConst.GroupName, Order = 100)]
[SysAdminOnly]
public class DingTalkService : IDynamicApiController, IScoped
{
    private readonly IDingTalkApi _dingTalkApi;
    private readonly DingTalkOptions _dingTalkOptions;
    private readonly SqlSugarRepository<DingTalkWokerflowLog> _dingTalkWokerflowLogRep;
    private readonly SqlSugarRepository<DingTalkWokerflowConfig> _dingTalkWokerflowConfigRep;
    private readonly SqlSugarRepository<SysOrg> _sysOrgRep;
    private readonly SqlSugarRepository<DingTalkUser> _dingTalkUserRep;
    private readonly SqlSugarRepository<SysUser> _sysUserRep;
    private readonly SqlSugarRepository<DingTalkRoleUser> _dingTalkRoleUserRep;
    private readonly SqlSugarRepository<SysRole> _sysRoleRep;
    private readonly SqlSugarRepository<SysUserRole> _sysUserRoleRep;
    private readonly DingTalkContactPersister _contactPersister;

    public DingTalkService(
        IDingTalkApi dingTalkApi,
        IOptions<DingTalkOptions> dingTalkOptions,
        SqlSugarRepository<DingTalkWokerflowLog> dingTalkWokerflowLogRep,
        SqlSugarRepository<DingTalkWokerflowConfig> dingTalkWokerflowConfigRep,
        SqlSugarRepository<SysOrg> sysOrgRep,
        SqlSugarRepository<DingTalkUser> dingTalkUserRep,
        SqlSugarRepository<SysUser> sysUserRep,
        SqlSugarRepository<DingTalkRoleUser> dingTalkRoleUserRep,
        SqlSugarRepository<SysRole> sysRoleRep,
        SqlSugarRepository<SysUserRole> sysUserRoleRep,
        DingTalkContactPersister contactPersister
    )
    {
        _dingTalkApi = dingTalkApi;
        _dingTalkOptions = dingTalkOptions.Value;
        _dingTalkWokerflowLogRep = dingTalkWokerflowLogRep;
        _dingTalkWokerflowConfigRep = dingTalkWokerflowConfigRep;
        _sysOrgRep = sysOrgRep;
        _dingTalkUserRep = dingTalkUserRep;
        _sysUserRep = sysUserRep;
        _dingTalkRoleUserRep = dingTalkRoleUserRep;
        _sysRoleRep = sysRoleRep;
        _sysUserRoleRep = sysUserRoleRep;
        _contactPersister = contactPersister;
    }

    /// <summary>
    /// 获取企业内部应用的access_token
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取企业内部应用的access_token")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<GetDingTalkTokenOutput> GetDingTalkToken()
    {
        var tokenRes = await _dingTalkApi.GetDingTalkToken(
            _dingTalkOptions.ClientId,
            _dingTalkOptions.ClientSecret
        );
        if (tokenRes.ErrCode != 0)
        {
            throw Oops.Oh(tokenRes.ErrMsg);
        }
        return tokenRes;
    }

    /// <summary>
    /// 获取在职员工列表 🔖
    /// </summary>
    /// <param name="access_token"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost, DisplayName("获取在职员工列表")]
    public async Task<
        DingTalkBaseResponse<GetDingTalkCurrentEmployeesListOutput>
    > GetDingTalkCurrentEmployeesList(
        string access_token,
        [Required] GetDingTalkCurrentEmployeesListInput input
    )
    {
        return await _dingTalkApi.GetDingTalkCurrentEmployeesList(access_token, input);
    }

    /// <summary>
    /// 通过免登码获取用户信息 🔖
    /// </summary>
    /// <param name="input">包含前端传来的免登授权码 authCode</param>
    /// <returns></returns>
    [HttpPost, DisplayName("通过免登码获取用户信息")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<DingTalkBaseResponse<GetDingTalkUserInfoOutput>> GetDingTalkUserInfo(
        [Required] GetDingTalkUserInfoInput input
    )
    {
        return await _dingTalkApi.GetDingTalkUserInfo(GetDingTalkToken().Result.AccessToken, input);
    }

    /// <summary>
    /// 获取员工花名册字段信息 🔖
    /// </summary>
    /// <param name="access_token"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost, DisplayName("获取员工花名册字段信息")]
    public async Task<
        DingTalkBaseResponse<List<DingTalkEmpRosterFieldVo>>
    > GetDingTalkCurrentEmployeesRosterList(
        string access_token,
        [Required] GetDingTalkCurrentEmployeesRosterListInput input
    )
    {
        return await _dingTalkApi.GetDingTalkCurrentEmployeesRosterList(access_token, input);
    }

    /// <summary>
    /// 发送钉钉互动卡片 🔖
    /// </summary>
    /// <param name="token"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("给指定用户发送钉钉互动卡片")]
    [Obsolete]
    public async Task<DingTalkSendInteractiveCardsOutput> DingTalkSendInteractiveCards(
        string token,
        DingTalkSendInteractiveCardsInput input
    )
    {
        return await _dingTalkApi.DingTalkSendInteractiveCards(token, input);
    }

    /// <summary>
    /// 创建并投放钉钉消息卡片 🔖
    /// </summary>
    /// <param name="token"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("给指定用户发送钉钉消息卡片")]
    public async Task<DingTalkCreateAndDeliverOutput> DingTalkCreateAndDeliver(
        string token,
        DingTalkCreateAndDeliverInput input
    )
    {
        return await _dingTalkApi.DingTalkCreateAndDeliver(token, input);
    }

    [DisplayName("用于发起OA审批实例")]
    public async Task<DingTalkWorkflowProcessInstancesOutput> DingTalkWorkflowProcessInstances(
        string token,
        DingTalkWorkflowProcessInstancesInput input
    )
    {
        var temp = await _dingTalkApi.DingTalkWorkflowProcessInstances(token, input);
        return temp;
    }

    [DisplayName("查询审批实例")]
    public async Task<DingTalkGetProcessInstancesOutput> DingTalkWorkflowProcessInstances(
        string token,
        string input
    )
    {
        var temp = await _dingTalkApi.GetProcessInstances(token, input);
        DingTalkWokerflowLog flow = await _dingTalkWokerflowLogRep.GetFirstAsync(t =>
            t.Status == "RUNNING" && t.instanceId == input
        );

        if ((flow != null) && (temp.Result.Status != flow.Status))
        {
            flow.Status = temp.Result.Status;
            flow.UpdateTime = DateTime.Now;
            flow.WorkflowId = temp.Result.BusinessId;
            flow.Result = temp.Result.Result;
            flow.taskId = temp.Result.Tasks.FirstOrDefault(t => t.Status == "RUNNING")?.TaskId;
            await _dingTalkWokerflowLogRep.UpdateAsync(flow);
        }
        return temp;
    }

    /// <summary>
    /// 查询“我审批的”审批任务列表 🔖
    /// </summary>
    /// <remarks>
    /// 依赖钉钉 OA审批高级版（OA高级版专享接口）。isDone=false 查询待处理任务，
    /// isDone=true 查询已处理任务。未开通高级版权益时接口会返回权益校验失败。
    /// </remarks>
    /// <param name="input">查询入参（userId、分页等）</param>
    /// <param name="isDone">false：待处理（待我审批）；true：已处理（我已审批）</param>
    /// <returns></returns>
    [HttpPost, DisplayName("查询我审批的审批任务列表")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<DingTalkProcessCentreTaskOutput> GetMyApproveList(
        [Required] DingTalkProcessCentreQueryInput input,
        bool isDone = false
    )
    {
        var token = (await GetDingTalkToken()).AccessToken;
        return isDone
            ? await _dingTalkApi.GetProcessCentreDoneTasks(token, input)
            : await _dingTalkApi.GetProcessCentreTodoTasks(token, input);
    }

    /// <summary>
    /// 查询“我发起的”审批实例列表 🔖
    /// </summary>
    /// <remarks>依赖钉钉 OA审批高级版（OA高级版专享接口）。</remarks>
    /// <param name="input">查询入参（userId、分页等）</param>
    /// <returns></returns>
    [HttpPost, DisplayName("查询我发起的审批实例列表")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<DingTalkProcessCentreInstanceOutput> GetMySubmittedList(
        [Required] DingTalkProcessCentreQueryInput input
    )
    {
        var token = (await GetDingTalkToken()).AccessToken;
        return await _dingTalkApi.GetProcessCentreSubmittedInstances(token, input);
    }

    /// <summary>
    /// 查询“我收到的”审批实例列表（抄送我的）🔖
    /// </summary>
    /// <remarks>依赖钉钉 OA审批高级版（OA高级版专享接口）。</remarks>
    /// <param name="input">查询入参（userId、分页等）</param>
    /// <returns></returns>
    [HttpPost, DisplayName("查询我收到的审批实例列表")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<DingTalkProcessCentreInstanceOutput> GetMyReceivedList(
        [Required] DingTalkProcessCentreQueryInput input
    )
    {
        var token = (await GetDingTalkToken()).AccessToken;
        return await _dingTalkApi.GetProcessCentreNoticedInstances(token, input);
    }

    /// <summary>
    /// 获取审批实例ID列表（标准版，可按发起人过滤）🔖
    /// </summary>
    /// <remarks>
    /// 未开通 OA高级版时可用本接口作为“我发起的”兜底：传入 processCode + 发起人 userIds，
    /// 返回实例ID列表，再逐个调用 <see cref="DingTalkWorkflowProcessInstances(string, string)"/> 获取详情。
    /// 时间范围：仅传 startTime 时距当前不超过 120 天；同时传 startTime 与 endTime 时范围不超过 120 天，
    /// 且 startTime 距当前不超过 365 天。
    /// </remarks>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost, DisplayName("获取审批实例ID列表")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<DingTalkListInstanceIdsOutput> ListProcessInstanceIds(
        [Required] DingTalkListInstanceIdsInput input
    )
    {
        var token = (await GetDingTalkToken()).AccessToken;
        return await _dingTalkApi.ListProcessInstanceIds(token, input);
    }

    /// <summary>
    /// 标准版审批单归类查询（无需 OA高级版）🔖
    /// </summary>
    /// <remarks>
    /// 实现方式：按 processCode + 时间范围分页拉取实例ID（<see cref="IDingTalkApi.ListProcessInstanceIds"/>），
    /// 再逐个获取实例详情（<see cref="IDingTalkApi.GetProcessInstances"/>），
    /// 在业务侧按当前 userId 归类为“我发起的/我审批的/我收到的”。
    /// 注意：数据量大时耗时较长（受 MaxScan 上限保护）；approverUserIds/ccUserIds 仅对“用接口发起”的审批单返回，
    /// OA 应用手动发起的审批单“我审批的”依赖 tasks[].userId 判断。
    /// </remarks>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost, DisplayName("标准版审批单归类查询")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<DingTalkMyProcessListOutput> GetMyProcessList(
        [Required] DingTalkMyProcessQueryInput input
    )
    {
        var token = (await GetDingTalkToken()).AccessToken;
        var result = new DingTalkMyProcessListOutput();

        long nextToken = 0;
        while (result.ScannedCount < input.MaxScan)
        {
            var idsRes = await _dingTalkApi.ListProcessInstanceIds(
                token,
                new DingTalkListInstanceIdsInput
                {
                    ProcessCode = input.ProcessCode,
                    StartTime = input.StartTime,
                    EndTime = input.EndTime,
                    NextToken = nextToken,
                    MaxResults = input.MaxResults,
                }
            );

            var ids = idsRes?.Result?.List;
            if (ids == null || ids.Count == 0)
                break;

            foreach (var id in ids)
            {
                if (result.ScannedCount >= input.MaxScan)
                {
                    result.Truncated = true;
                    break;
                }

                var detailRes = await _dingTalkApi.GetProcessInstances(token, id);
                result.ScannedCount++;

                var data = detailRes?.Result;
                if (data == null)
                    continue;

                Classify(result, input.UserId, id, data);
            }

            // nextToken 为空表示已无更多数据
            if (string.IsNullOrEmpty(idsRes.Result.NextToken)
                || !long.TryParse(idsRes.Result.NextToken, out nextToken))
                break;
        }

        return result;
    }

    /// <summary>
    /// 将单个审批实例按当前 userId 归类到“我发起的/我审批的/我收到的”
    /// </summary>
    private static void Classify(
        DingTalkMyProcessListOutput result,
        string userId,
        string instanceId,
        ResultData data
    )
    {
        DingTalkMyProcessItem NewItem() =>
            new()
            {
                ProcessInstanceId = instanceId,
                Title = data.Title,
                BusinessId = data.BusinessId,
                OriginatorUserId = data.OriginatorUserId,
                OriginatorDeptName = data.OriginatorDeptName,
                Status = data.Status,
                Result = data.Result,
                CreateTime = data.CreateTime,
            };

        // 我发起的：发起人是当前用户
        if (string.Equals(data.OriginatorUserId, userId, StringComparison.Ordinal))
        {
            result.Submitted.Add(NewItem());
        }

        // 我审批的：作为审批人参与（优先用 tasks 判断，其次 approverUserIds）
        var myTask = data.Tasks?.FirstOrDefault(t =>
            string.Equals(t.UserId, userId, StringComparison.Ordinal)
        );
        var isApprover =
            myTask != null
            || (data.ApproverUserIds?.Contains(userId) ?? false);
        if (isApprover)
        {
            var approveItem = NewItem();
            approveItem.MyTaskStatus = myTask?.Status;
            result.Approve.Add(approveItem);
        }

        // 我收到的：被抄送
        if (data.CcUserIds?.Contains(userId) ?? false)
        {
            result.Received.Add(NewItem());
        }
    }

    /// <summary>
    /// 查询“我发起的”审批单（本地表，数据来自回调落库）🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost, DisplayName("查询我发起的审批单(本地)")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<SqlSugarPagedList<DingTalkWokerflowLog>> GetLocalSubmittedList(
        [Required] DingTalkLocalProcessQueryInput input
    )
    {
        // 发起人是 scalar 列，可直接服务端分页
        return await _dingTalkWokerflowLogRep
            .AsQueryable()
            .Where(t => t.OriginatorUserId == input.UserId)
            .WhereIF(!string.IsNullOrEmpty(input.Status), t => t.Status == input.Status)
            .OrderByDescending(t => t.CreateTime)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 查询“我审批的”审批单（本地表，数据来自回调落库）🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost, DisplayName("查询我审批的审批单(本地)")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<SqlSugarPagedList<DingTalkWokerflowLog>> GetLocalApproveList(
        [Required] DingTalkLocalProcessQueryInput input
    )
    {
        return await QueryByJsonMemberAsync(input, isApprover: true);
    }

    /// <summary>
    /// 查询“我收到的”审批单（本地表，数据来自回调落库）🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost, DisplayName("查询我收到的审批单(本地)")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<SqlSugarPagedList<DingTalkWokerflowLog>> GetLocalReceivedList(
        [Required] DingTalkLocalProcessQueryInput input
    )
    {
        return await QueryByJsonMemberAsync(input, isApprover: false);
    }

    /// <summary>
    /// 按 JSON 列表成员（审批人/抄送人）分页查询本地审批单。
    /// JSON 数组成员判断不便于跨库服务端过滤，故先按 Status 服务端缩小范围，
    /// 再在内存中用精确 Contains 校验列表成员，最后手动分页（与本项目对 JSON 列的既有处理方式一致）。
    /// </summary>
    /// <param name="input"></param>
    /// <param name="isApprover">true 查审批人列表，false 查抄送人列表</param>
    private async Task<SqlSugarPagedList<DingTalkWokerflowLog>> QueryByJsonMemberAsync(
        DingTalkLocalProcessQueryInput input,
        bool isApprover
    )
    {
        var candidates = await _dingTalkWokerflowLogRep
            .AsQueryable()
            .WhereIF(!string.IsNullOrEmpty(input.Status), t => t.Status == input.Status)
            .OrderByDescending(t => t.CreateTime)
            .ToListAsync();

        // 内存精确校验列表成员，再复用项目统一的内存分页扩展
        var matched = candidates
            .Where(t =>
                (isApprover ? t.ApproverUserIds : t.CcUserIds)?.Contains(input.UserId) ?? false
            )
            .ToList();

        return matched.ToPagedList(input.Page, input.PageSize);
    }

    #region 审批配置（DingTalkWokerflowConfig）管理

    /// <summary>
    /// 分页查询审批配置 🔖
    /// </summary>
    /// <param name="input">审批名 / 审批单Id 模糊过滤 + 分页</param>
    /// <returns></returns>
    [HttpPost, DisplayName("分页查询审批配置")]
    public async Task<SqlSugarPagedList<DingTalkWokerflowConfig>> GetWorkflowConfigPage(
        [Required] DingTalkWokerflowConfigQueryInput input
    )
    {
        return await _dingTalkWokerflowConfigRep
            .AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.WorkflowName), t => t.WorkflowName.Contains(input.WorkflowName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.ProcessCode), t => t.ProcessCode.Contains(input.ProcessCode))
            .OrderBy(t => t.WorkflowName)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 查询全部审批配置（不分页，供下拉选择用）🔖
    /// </summary>
    /// <returns></returns>
    [HttpPost, DisplayName("查询全部审批配置")]
    public async Task<List<DingTalkWokerflowConfig>> GetWorkflowConfigList()
    {
        return await _dingTalkWokerflowConfigRep.AsQueryable().OrderBy(t => t.WorkflowName).ToListAsync();
    }

    /// <summary>
    /// 新增审批配置 🔖
    /// </summary>
    /// <remarks>WorkflowName 为主键，不能重复。</remarks>
    /// <param name="input"></param>
    [HttpPost, DisplayName("新增审批配置")]
    public async Task AddWorkflowConfig([Required] DingTalkWokerflowConfig input)
    {
        if (string.IsNullOrWhiteSpace(input.WorkflowName))
            throw Oops.Oh("审批名不能为空");
        if (string.IsNullOrWhiteSpace(input.ProcessCode))
            throw Oops.Oh("审批单Id(ProcessCode)不能为空");
        if (await _dingTalkWokerflowConfigRep.IsAnyAsync(t => t.WorkflowName == input.WorkflowName))
            throw Oops.Oh($"审批名已存在：{input.WorkflowName}");

        await _dingTalkWokerflowConfigRep.InsertAsync(input);
    }

    /// <summary>
    /// 更新审批配置 🔖
    /// </summary>
    /// <remarks>按主键 WorkflowName 更新 ProcessCode。</remarks>
    /// <param name="input"></param>
    [HttpPost, DisplayName("更新审批配置")]
    public async Task UpdateWorkflowConfig([Required] DingTalkWokerflowConfig input)
    {
        if (string.IsNullOrWhiteSpace(input.WorkflowName))
            throw Oops.Oh("审批名不能为空");
        if (string.IsNullOrWhiteSpace(input.ProcessCode))
            throw Oops.Oh("审批单Id(ProcessCode)不能为空");
        if (!await _dingTalkWokerflowConfigRep.IsAnyAsync(t => t.WorkflowName == input.WorkflowName))
            throw Oops.Oh($"审批配置不存在：{input.WorkflowName}");

        await _dingTalkWokerflowConfigRep.UpdateAsync(input);
    }

    /// <summary>
    /// 删除审批配置 🔖
    /// </summary>
    /// <param name="input"></param>
    [HttpPost, DisplayName("删除审批配置")]
    public async Task DeleteWorkflowConfig([Required] DingTalkWokerflowConfig input)
    {
        if (string.IsNullOrWhiteSpace(input.WorkflowName))
            throw Oops.Oh("审批名不能为空");
        await _dingTalkWokerflowConfigRep.DeleteAsync(t => t.WorkflowName == input.WorkflowName);
    }

    #endregion 审批配置（DingTalkWokerflowConfig）管理

    #region 初始化（全量拉取落库）

    /// <summary>
    /// 按审批模板 processCode 初始化审批实例到本地 🔖
    /// </summary>
    /// <remarks>
    /// 遍历该模板下时间范围内的所有审批实例：DingTalkWokerflowLog 不存在则新增，存在则更新钉钉侧字段。
    /// 与回调落库共用主键 instanceId，用 Storageable 按主键合并，不覆盖业务侧独有列
    /// （SourceDocument / CreateUserName / WorkflowName）。时间范围规则同 ListProcessInstanceIds
    /// （仅传 startTime 距当前不超过 120 天；同时传两者范围不超过 120 天且 startTime 距当前不超过 365 天）。
    /// </remarks>
    /// <param name="input">processCode + 时间范围 + 分页/扫描上限</param>
    /// <returns>本次新增数、更新数、扫描数</returns>
    [HttpPost, DisplayName("初始化审批实例(按ProcessCode)")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<DingTalkInitWorkflowOutput> InitWorkflowByProcessCode(
        [Required] DingTalkMyProcessQueryInput input
    )
    {
        var token = (await GetDingTalkToken()).AccessToken;
        var output = new DingTalkInitWorkflowOutput();

        long nextToken = 0;
        while (output.Scanned < input.MaxScan)
        {
            var idsRes = await _dingTalkApi.ListProcessInstanceIds(
                token,
                new DingTalkListInstanceIdsInput
                {
                    ProcessCode = input.ProcessCode,
                    StartTime = input.StartTime,
                    EndTime = input.EndTime,
                    NextToken = nextToken,
                    MaxResults = input.MaxResults,
                }
            );

            var ids = idsRes?.Result?.List;
            if (ids == null || ids.Count == 0)
                break;

            foreach (var id in ids)
            {
                if (output.Scanned >= input.MaxScan)
                {
                    output.Truncated = true;
                    break;
                }
                output.Scanned++;

                var detailRes = await _dingTalkApi.GetProcessInstances(token, id);
                var data = detailRes?.Result;
                if (data == null)
                    continue;

                var isNew = await UpsertWorkflowLogAsync(input.ProcessCode, id, data);
                if (isNew)
                    output.Inserted++;
                else
                    output.Updated++;
            }

            if (output.Truncated
                || string.IsNullOrEmpty(idsRes.Result.NextToken)
                || !long.TryParse(idsRes.Result.NextToken, out nextToken))
                break;
        }

        Log.Information(
            $"[钉钉初始化] 审批实例初始化完成，processCode={input.ProcessCode}，"
                + $"扫描{output.Scanned}，新增{output.Inserted}，更新{output.Updated}，截断={output.Truncated}"
        );
        return output;
    }

    /// <summary>
    /// 将单个审批实例落库（按 instanceId 合并）。返回 true 表示新增，false 表示更新。
    /// </summary>
    private async Task<bool> UpsertWorkflowLogAsync(string processCode, string instanceId, ResultData data)
    {
        var exists = await _dingTalkWokerflowLogRep.IsAnyAsync(t => t.instanceId == instanceId);

        var log = new DingTalkWokerflowLog
        {
            instanceId = instanceId,
            WorkflowName = processCode,
            Title = data.Title,
            WorkflowId = data.BusinessId,
            OriginatorUserId = data.OriginatorUserId,
            Status = data.Status,
            Result = data.Result,
            CreateTime = data.CreateTime ?? DateTime.Now,
            UpdateTime = DateTime.Now,
            // 审批人：优先 approverUserIds，否则从任务节点提取
            ApproverUserIds =
                data.ApproverUserIds is { Count: > 0 }
                    ? data.ApproverUserIds
                    : data
                        .Tasks?.Select(x => x.UserId)
                        .Where(x => !string.IsNullOrEmpty(x))
                        .Distinct()
                        .ToList(),
            CcUserIds = data.CcUserIds,
        };

        // 与回调落库共用主键：不存在整条插入；存在仅更新钉钉侧字段，业务列保持不动
        var storage = await _dingTalkWokerflowLogRep
            .Context.Storageable(log)
            .WhereColumns(t => t.instanceId)
            .ToStorageAsync();
        await storage.AsInsertable.ExecuteCommandAsync();
        await storage
            .AsUpdateable.UpdateColumns(t => new
            {
                t.Title,
                t.WorkflowId,
                t.OriginatorUserId,
                t.ApproverUserIds,
                t.CcUserIds,
                t.Status,
                t.Result,
                t.UpdateTime,
            })
            .ExecuteCommandAsync();

        return !exists;
    }

    /// <summary>
    /// 调用 /workflow/processInstances 获取单个审批实例详情并落库 DingTalkWokerflowLog（无则新增/有则更新）🔖
    /// </summary>
    /// <param name="processInstanceId">审批实例Id</param>
    /// <param name="processCode">审批模板 processCode（用于新增时填充 WorkflowName 占位，可空）</param>
    /// <returns>审批实例详情</returns>
    [HttpPost, DisplayName("获取审批实例并落库")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<ResultData> GetAndSaveProcessInstance(
        [Required] string processInstanceId,
        string processCode = null
    )
    {
        var token = (await GetDingTalkToken()).AccessToken;
        var detailRes = await _dingTalkApi.GetProcessInstances(token, processInstanceId);
        var data = detailRes?.Result;
        if (data == null)
            throw Oops.Oh($"获取审批实例详情失败：{processInstanceId}");

        var isNew = await UpsertWorkflowLogAsync(processCode ?? data.BusinessId, processInstanceId, data);
        Log.Information($"[钉钉同步] 审批实例落库 {(isNew ? "新增" : "更新")}：{processInstanceId}");
        return data;
    }

    /// <summary>
    /// 调用 /v2/department/get 获取单个部门详情并落库 SysOrg（无则新增/有则更新）🔖
    /// </summary>
    /// <param name="deptId">部门Id</param>
    [HttpPost, DisplayName("获取部门详情并落库")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<DingTalkDeptOutput> GetAndSaveDeptDetail([Required] long deptId)
    {
        var token = (await GetDingTalkToken()).AccessToken;
        var res = await _dingTalkApi.GetDingTalkDeptDetail(
            token,
            new GetDingTalkDeptInput { dept_id = deptId }
        );
        if (res.ErrCode != 0 || res.Result == null)
            throw Oops.Oh($"获取部门详情失败[{deptId}]：{res.ErrMsg}");

        await _contactPersister.UpsertDeptsByIdsAsync(new List<long> { deptId });
        return res.Result;
    }

    /// <summary>
    /// 调用 /v2/user/get 获取单个用户详情并落库 DingTalkUser + SysUser（无则新增/有则更新）🔖
    /// </summary>
    /// <param name="userId">钉钉用户Id</param>
    [HttpPost, DisplayName("获取用户详情并落库")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<DingTalkUserDetailOutput> GetAndSaveUserDetail([Required] string userId)
    {
        var token = (await GetDingTalkToken()).AccessToken;
        var res = await _dingTalkApi.GetDingTalkUserDetail(
            token,
            new GetDingTalkUserDetailInput { userid = userId }
        );
        if (res.ErrCode != 0 || res.Result == null)
            throw Oops.Oh($"获取用户详情失败[{userId}]：{res.ErrMsg}");

        await _contactPersister.UpsertUsersByIdsAsync(new List<string> { userId });
        return res.Result;
    }

    /// <summary>
    /// 调用 /role/getrole 获取单个角色详情并落库 SysRole（无则新增/有则更新）🔖
    /// </summary>
    /// <param name="roleId">角色Id</param>
    [HttpPost, DisplayName("获取角色详情并落库")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<DingTalkRoleDetail> GetAndSaveRoleDetail([Required] long roleId)
    {
        var token = (await GetDingTalkToken()).AccessToken;
        var res = await _dingTalkApi.GetDingTalkRoleDetail(
            token,
            new GetDingTalkRoleDetailInput { roleId = roleId }
        );
        // 注意：/role/getrole 的 role 在返回体顶层，成功语义以 ErrCode==0 为准
        if (res.ErrCode != 0 || res.role == null)
            throw Oops.Oh($"获取角色详情失败[{roleId}]：{res.ErrMsg}");

        await _contactPersister.UpsertRolesByIdsAsync(new List<long> { roleId });
        return res.role;
    }

    /// <summary>
    /// 初始化钉钉部门到本地 SysOrg 🔖
    /// </summary>
    /// <remarks>参考 SyncDingTalkDeptJob：从根部门递归拉取全部子部门，与本地“钉钉同步”机构做全量对账（增/改/删）。</remarks>
    /// <returns>新增/更新/删除数量</returns>
    [HttpPost, DisplayName("初始化钉钉部门")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<DingTalkInitCountOutput> InitDept()
    {
        var token = (await GetDingTalkToken()).AccessToken;

        var dingDeptList = await FetchDeptTreeAsync(token, 1);

        var localDeptList = await _sysOrgRep.AsQueryable().Where(t => t.Remark == _dingTalkOptions.DingTalkInfoRemark).ToListAsync();
        var comparer = new SysOrgIdPidComparer();

        var toInsert = dingDeptList.Except(localDeptList, comparer).ToList();
        var toUpdate = dingDeptList.Intersect(localDeptList, comparer).ToList();
        var toDelete = localDeptList.Except(dingDeptList, comparer).ToList();

        if (toDelete.Count > 0)
            await _sysOrgRep.DeleteAsync(toDelete);

        var upsert = new List<SysOrg>();
        upsert.AddRange(toInsert);
        upsert.AddRange(toUpdate);
        if (upsert.Count > 0)
            await _sysOrgRep.InsertOrUpdateAsync(upsert);

        Log.Information($"[钉钉初始化] 部门初始化完成，新增{toInsert.Count}，更新{toUpdate.Count}，删除{toDelete.Count}");
        return new DingTalkInitCountOutput
        {
            Inserted = toInsert.Count,
            Updated = toUpdate.Count,
            Deleted = toDelete.Count,
        };
    }

    /// <summary>
    /// 递归拉取指定部门及其所有子部门（含自身），映射为 SysOrg。
    /// </summary>
    private async Task<List<SysOrg>> FetchDeptTreeAsync(string token, long deptId)
    {
        var result = new List<SysOrg>();
        var res = await _dingTalkApi.GetDingTalkDept(token, new GetDingTalkDeptInput { dept_id = deptId });
        if (res.ErrCode != 0 || res.Result == null)
        {
            if (res.ErrCode != 0)
                Log.Warning($"[钉钉初始化] 拉取子部门失败[{deptId}]：{res.ErrMsg}");
            return result;
        }

        result.AddRange(res.Result.Select(MapDept));
        foreach (var item in res.Result)
            result.AddRange(await FetchDeptTreeAsync(token, item.dept_id));
        return result;
    }

    /// <summary>钉钉部门映射为本地 SysOrg（根部门 parent_id=1 时映射到本地根机构）。</summary>
    private SysOrg MapDept(DingTalkDeptOutput d) =>
        new()
        {
            Id = d.dept_id,
            Name = d.name,
            Code = d.name,
            Pid = d.parent_id == 1 ? _dingTalkOptions.RootOrgPid : d.parent_id,
            TenantId = _dingTalkOptions.DefaultTenantId,
            Remark = _dingTalkOptions.DingTalkInfoRemark,
        };

    /// <summary>
    /// 初始化钉钉用户到本地 DingTalkUser 并回填 SysUser.OrgId 🔖
    /// </summary>
    /// <remarks>参考 SyncDingTalkUserJob：分页拉取在职员工花名册，按 DingTalkUserId 增/改，按工号关联 SysUser 并更新 OrgId。</remarks>
    /// <returns>新增/更新数量</returns>
    [HttpPost, DisplayName("初始化钉钉人员")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<DingTalkInitCountOutput> InitUser()
    {
        var token = (await GetDingTalkToken()).AccessToken;

        // 分页拉取所有在职员工花名册
        var allEmps = new List<DingTalkEmpRosterFieldVo>();
        var offset = 0;
        while (offset >= 0)
        {
            var idsRes = await _dingTalkApi.GetDingTalkCurrentEmployeesList(
                token,
                new GetDingTalkCurrentEmployeesListInput
                {
                    StatusList = "2,3,5,-1",
                    Size = 50,
                    Offset = offset,
                }
            );
            if (idsRes.ErrCode != 0)
            {
                Log.Error($"[钉钉初始化] 获取员工ID列表失败：{idsRes.ErrMsg}");
                break;
            }
            if (idsRes.Result?.DataList == null || idsRes.Result.DataList.Count == 0)
                break;

            var rosterRes = await _dingTalkApi.GetDingTalkCurrentEmployeesRosterList(
                token,
                new GetDingTalkCurrentEmployeesRosterListInput
                {
                    UserIdList = string.Join(",", idsRes.Result.DataList),
                    FieldFilterList =
                        $"{DingTalkConst.NameField},{DingTalkConst.JobNumberField},{DingTalkConst.MobileField},{DingTalkConst.DeptId},{DingTalkConst.Dept},{DingTalkConst.Position}",
                    AgentId = _dingTalkOptions.AgentId,
                }
            );
            if (rosterRes.ErrCode != 0 || rosterRes.Result == null)
            {
                Log.Error($"[钉钉初始化] 获取花名册失败：{rosterRes.ErrMsg}");
                break;
            }

            allEmps.AddRange(rosterRes.Result);
            offset = idsRes.Result.NextCursor.HasValue ? (int)idsRes.Result.NextCursor : -1;
        }

        if (allEmps.Count == 0)
        {
            Log.Warning("[钉钉初始化] 未获取到任何用户数据");
            return new DingTalkInitCountOutput();
        }

        // 现有钉钉用户：DingTalkUserId -> Id
        var existingMap = (
            await _dingTalkUserRep.AsQueryable().Select(u => new { u.Id, u.DingTalkUserId }).ToListAsync()
        ).ToDictionary(x => x.DingTalkUserId, x => x.Id);

        // 系统用户账号 -> Id
        var sysUserAccountMap = (
            await _sysUserRep.AsQueryable().Select(u => new { u.Id, u.Account }).ToListAsync()
        )
            .Where(x => !string.IsNullOrWhiteSpace(x.Account))
            .ToDictionary(x => x.Account, x => x.Id);

        // 先把钉钉侧有工号、但本地 SysUser 无对应账号的用户补建为 SysUser（Account=工号，随机8位密码）
        var mappedUsers = allEmps
            .Select(emp =>
            {
                var user = MapRosterUser(emp);
                user.DingTalkUserId = emp.UserId;
                return user;
            })
            .ToList();

        var missingAccounts = mappedUsers
            .Where(u => !string.IsNullOrWhiteSpace(u.JobNumber) && !sysUserAccountMap.ContainsKey(u.JobNumber))
            .GroupBy(u => u.JobNumber)
            .Select(g => g.First())
            .ToList();

        var createdSysUserCount = 0;
        if (missingAccounts.Count > 0)
        {
            var newSysUsers = missingAccounts
                .Select(u => new SysUser
                {
                    Account = u.JobNumber,
                    Password = CryptogramUtil.Encrypt(GenRandomPassword(8)),
                    RealName = u.Name,
                    NickName = u.Name,
                    Phone = u.Mobile,
                    JobNum = u.JobNumber,
                    OrgId = u.DeptId is > 0 ? u.DeptId.Value : 0,
                    TenantId = _dingTalkOptions.DefaultTenantId,
                    Status = StatusEnum.Enable,
                    AccountType = AccountTypeEnum.NormalUser,
                    Remark = _dingTalkOptions.DingTalkInfoRemark,
                })
                .ToList();

            await _sysUserRep.CopyNew().AsInsertable(newSysUsers).ExecuteCommandAsync();
            createdSysUserCount = newSysUsers.Count;

            // 回填账号->Id 映射，供后续 DingTalkUser.SysUserId 关联使用
            foreach (var su in newSysUsers)
                sysUserAccountMap[su.Account] = su.Id;

            Log.Information($"[钉钉初始化] 为缺失账号新建 SysUser {createdSysUserCount} 条");
        }

        var toInsert = new List<DingTalkUser>();
        var toUpdate = new List<DingTalkUser>();
        var sysUserOrgIdUpdates = new Dictionary<long, long>();

        foreach (var user in mappedUsers)
        {
            if (existingMap.TryGetValue(user.DingTalkUserId, out var existingId))
            {
                user.Id = existingId;
                toUpdate.Add(user);
            }
            else
            {
                toInsert.Add(user);
            }

            if (!string.IsNullOrWhiteSpace(user.JobNumber)
                && sysUserAccountMap.TryGetValue(user.JobNumber, out var sysUserId))
            {
                user.SysUserId = sysUserId;
                if (user.DeptId is > 0)
                    sysUserOrgIdUpdates[sysUserId] = user.DeptId.Value;
            }
        }

        if (toInsert.Count > 0)
            await _dingTalkUserRep.CopyNew().AsInsertable(toInsert).ExecuteCommandAsync();
        if (toUpdate.Count > 0)
        {
            await _dingTalkUserRep
                .CopyNew()
                .AsUpdateable(toUpdate)
                .UpdateColumns(u => new
                {
                    u.DingTalkUserId,
                    u.Name,
                    u.Mobile,
                    u.JobNumber,
                    u.DeptId,
                    u.Dept,
                    u.Position,
                    u.SysUserId,
                    u.UpdateTime,
                })
                .ExecuteCommandAsync();
        }

        // 回填 SysUser.OrgId
        if (sysUserOrgIdUpdates.Count > 0)
        {
            var sysUsers = await _sysUserRep
                .AsQueryable()
                .Where(u => sysUserOrgIdUpdates.Keys.Contains(u.Id))
                .ToListAsync();
            foreach (var su in sysUsers)
                if (sysUserOrgIdUpdates.TryGetValue(su.Id, out var newOrgId))
                    su.OrgId = newOrgId;
            if (sysUsers.Count > 0)
                await _sysUserRep.AsUpdateable(sysUsers).UpdateColumns(u => new { u.OrgId }).ExecuteCommandAsync();
        }

        Log.Information(
            $"[钉钉初始化] 人员初始化完成，共{allEmps.Count}，新增{toInsert.Count}，更新{toUpdate.Count}，补建SysUser {createdSysUserCount}"
        );
        return new DingTalkInitCountOutput { Inserted = toInsert.Count, Updated = toUpdate.Count };
    }

    /// <summary>生成指定长度的随机密码（大小写字母+数字，与 SysUserService.ResetPwd 一致的字符集）。</summary>
    private static string GenRandomPassword(int length) =>
        new(
            Enumerable
                .Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", length)
                .Select(s => s[Random.Shared.Next(s.Length)])
                .ToArray()
        );

    /// <summary>花名册字段映射为 DingTalkUser（DingTalkUserId / Id 由调用方设置）。</summary>
    private static DingTalkUser MapRosterUser(DingTalkEmpRosterFieldVo emp)
    {
        string GetValue(string fieldCode) =>
            emp.FieldDataList?.FirstOrDefault(f => f.FieldCode == fieldCode)
                ?.FieldValueList?.FirstOrDefault()
                ?.Value;

        var deptIdStr = GetValue(DingTalkConst.DeptId);
        long.TryParse(deptIdStr, out var deptId);

        return new DingTalkUser
        {
            Name = GetValue(DingTalkConst.NameField),
            Mobile = GetValue(DingTalkConst.MobileField),
            JobNumber = GetValue(DingTalkConst.JobNumberField),
            DeptId = deptId,
            Dept = GetValue(DingTalkConst.Dept),
            Position = GetValue(DingTalkConst.Position),
        };
    }

    /// <summary>
    /// 初始化钉钉角色到本地 SysRole，并重建 DingTalkRoleUser / SysUserRole 🔖
    /// </summary>
    /// <remarks>参考 SyncDingTalkRoleJob：拉取角色列表与各角色成员，与本地“钉钉同步角色”做全量对账（增/改/删）。</remarks>
    /// <returns>角色新增/更新/删除数量</returns>
    [HttpPost, DisplayName("初始化钉钉角色")]
    [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
                + ","
                + SignatureAuthenticationDefaults.AuthenticationScheme
        )]
    public async Task<DingTalkInitCountOutput> InitRole()
    {
        var token = (await GetDingTalkToken()).AccessToken;

        var roleListTemp = new List<DingTalkRoleResult>();
        var roleUserListTemp = new List<DingTalkRoleUser>();

        // 获取角色列表（成功语义以 ErrCode==0 为准）
        var roleIdsRes = await _dingTalkApi.GetDingTalkRoleList(
            token,
            new GetDingTalkCurrentRoleListInput { }
        );
        if (roleIdsRes.ErrCode != 0)
        {
            Log.Error($"[钉钉初始化] 获取角色列表失败：{roleIdsRes.ErrMsg}");
            throw Oops.Oh(roleIdsRes.ErrMsg);
        }

        foreach (var group in roleIdsRes.Result.list)
        {
            roleListTemp.AddRange(group.roles);
            foreach (var role in group.roles)
            {
                var memberRes = await _dingTalkApi.GetDingTalkRoleSimplelist(
                    token,
                    new GetDingTalkCurrentRoleSimplelistInput { role_id = role.id }
                );
                if (memberRes.ErrCode != 0)
                {
                    Log.Warning($"[钉钉初始化] 获取角色成员失败[{role.id}]：{memberRes.ErrMsg}");
                    continue;
                }
                var members = memberRes.Result.list?.Select(u => new DingTalkRoleUser
                {
                    DingTalkUserId = u.userid,
                    groupId = group.groupId,
                    groupName = group.name,
                    roleId = role.id,
                    roleName = role.name,
                });
                if (members != null)
                    roleUserListTemp.AddRange(members);
            }
        }

        // ---- 角色（SysRole）对账 ----
        var localRoles = await _sysRoleRep.AsQueryable().Where(t => t.Remark == _dingTalkOptions.DingTalkInfoRemark).ToListAsync();

        var roleToInsert = roleListTemp
            .ExceptBy(localRoles.Select(r => r.Id), r => r.id)
            .Select(t => new SysRole
            {
                Id = t.id,
                Name = t.name,
                Code = t.name,
                OrderNo = 100,
                DataScope = DataScopeEnum.DeptChild,
                Remark = _dingTalkOptions.DingTalkInfoRemark,
                TenantId = _dingTalkOptions.DefaultTenantId,
            })
            .ToList();
        var roleToUpdate = localRoles
            .IntersectBy(roleListTemp.Select(r => r.id), r => r.Id)
            .Select(r =>
            {
                var d = roleListTemp.First(x => x.id == r.Id);
                r.Name = d.name;
                r.Code = d.name;
                return r;
            })
            .ToList();
        var roleToDelete = localRoles.ExceptBy(roleListTemp.Select(r => r.id), r => r.Id).ToList();

        if (roleToDelete.Count > 0)
            await _sysRoleRep.DeleteAsync(roleToDelete);
        var roleUpsert = new List<SysRole>();
        roleUpsert.AddRange(roleToInsert);
        roleUpsert.AddRange(roleToUpdate);
        if (roleUpsert.Count > 0)
            await _sysRoleRep.InsertOrUpdateAsync(roleUpsert);

        // ---- 钉钉用户角色（DingTalkRoleUser）对账 ----
        var localRoleUsers = await _dingTalkRoleUserRep.AsQueryable().ToListAsync();
        var ruComparer = new DingTalkRoleUserKeyComparer();

        var ruToInsert = roleUserListTemp.Except(localRoleUsers, ruComparer).ToList();
        var ruToUpdate = localRoleUsers
            .Intersect(roleUserListTemp, ruComparer)
            .Select(item =>
            {
                var d = roleUserListTemp.First(r => r.DingTalkUserId == item.DingTalkUserId && r.roleId == item.roleId);
                item.groupId = d.groupId;
                item.groupName = d.groupName;
                item.roleName = d.roleName;
                item.IsDelete = false;
                return item;
            })
            .ToList();
        // 钉钉侧已无的关系：软删
        var ruToDelete = localRoleUsers.Except(roleUserListTemp, ruComparer).Where(r => !r.IsDelete).ToList();
        foreach (var r in ruToDelete)
            r.IsDelete = true;

        var ruUpsert = new List<DingTalkRoleUser>();
        ruUpsert.AddRange(ruToInsert);
        ruUpsert.AddRange(ruToUpdate);
        ruUpsert.AddRange(ruToDelete);
        if (ruUpsert.Count > 0)
            await _dingTalkRoleUserRep.InsertOrUpdateAsync(ruUpsert);

        // ---- 系统用户角色（SysUserRole）对账 ----
        var dingUsers = await _dingTalkUserRep.AsQueryable().Where(t => t.SysUserId > 0).ToListAsync();
        var sysUserRoleExpected = (
            from ru in roleUserListTemp
            join du in dingUsers on ru.DingTalkUserId equals du.DingTalkUserId
            select new SysUserRole { UserId = du.SysUserId, RoleId = ru.roleId }
        ).Distinct(new SysUserRoleKeyComparer()).ToList();

        var sysUserRoleCurrent = await _sysUserRoleRep.AsQueryable().ToListAsync();
        var surComparer = new SysUserRoleKeyComparer();
        var surToInsert = sysUserRoleExpected.Except(sysUserRoleCurrent, surComparer).ToList();
        var surToDelete = sysUserRoleCurrent.Except(sysUserRoleExpected, surComparer).ToList();
        if (surToInsert.Count > 0)
            await _sysUserRoleRep.InsertRangeAsync(surToInsert);
        if (surToDelete.Count > 0)
            await _sysUserRoleRep.DeleteAsync(surToDelete);

        Log.Information(
            $"[钉钉初始化] 角色初始化完成，SysRole 新增{roleToInsert.Count}/更新{roleToUpdate.Count}/删除{roleToDelete.Count}，"
                + $"DingTalkRoleUser 新增{ruToInsert.Count}/更新{ruToUpdate.Count}/软删{ruToDelete.Count}，"
                + $"SysUserRole 新增{surToInsert.Count}/删除{surToDelete.Count}"
        );
        return new DingTalkInitCountOutput
        {
            Inserted = roleToInsert.Count,
            Updated = roleToUpdate.Count,
            Deleted = roleToDelete.Count,
        };
    }

    #endregion 初始化（全量拉取落库）

    #region 初始化辅助（比较器）

    /// <summary>SysOrg 按 Id + Pid 比较（与 SyncDingTalkDeptJob 的 SysOrgComparer 一致）。</summary>
    private sealed class SysOrgIdPidComparer : IEqualityComparer<SysOrg>
    {
        public bool Equals(SysOrg x, SysOrg y) =>
            x == null || y == null ? x == y : x.Id == y.Id && x.Pid == y.Pid;

        public int GetHashCode(SysOrg obj) => obj == null ? 0 : HashCode.Combine(obj.Id, obj.Pid);
    }

    /// <summary>DingTalkRoleUser 按 DingTalkUserId + roleId 比较。</summary>
    private sealed class DingTalkRoleUserKeyComparer : IEqualityComparer<DingTalkRoleUser>
    {
        public bool Equals(DingTalkRoleUser x, DingTalkRoleUser y) =>
            x == null || y == null ? x == y : x.DingTalkUserId == y.DingTalkUserId && x.roleId == y.roleId;

        public int GetHashCode(DingTalkRoleUser obj) =>
            obj == null ? 0 : HashCode.Combine(obj.DingTalkUserId, obj.roleId);
    }

    /// <summary>SysUserRole 按 UserId + RoleId 比较。</summary>
    private sealed class SysUserRoleKeyComparer : IEqualityComparer<SysUserRole>
    {
        public bool Equals(SysUserRole x, SysUserRole y) =>
            x == null || y == null ? x == y : x.UserId == y.UserId && x.RoleId == y.RoleId;

        public int GetHashCode(SysUserRole obj) => obj == null ? 0 : HashCode.Combine(obj.UserId, obj.RoleId);
    }

    #endregion 初始化辅助（比较器）
}