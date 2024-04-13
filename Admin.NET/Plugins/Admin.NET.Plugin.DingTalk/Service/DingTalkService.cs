// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Plugin.DingTalk;

/// <summary>
/// 钉钉服务
/// </summary>
[ApiDescriptionSettings(DingTalkConst.GroupName, Module = "DingTalk", Order = 100)]
public class DingTalkService : IDynamicApiController, IScoped
{
    private readonly IDingTalkApi _dingTalkApi;
    private readonly DingTalkOptions _dingTalkOptions;
    private readonly SqlSugarRepository<DingTalkUser> _dingTalkUserRepo;
    private readonly SqlSugarRepository<SysUser> _sysUserRep;

    public DingTalkService(IDingTalkApi dingTalkApi,
        IOptions<DingTalkOptions> dingTalkOptions,
        SqlSugarRepository<DingTalkUser> dingTalkUserRepo,
        SqlSugarRepository<SysUser> sysUserRep)
    {
        _dingTalkApi = dingTalkApi;
        _dingTalkOptions = dingTalkOptions.Value;
        _dingTalkUserRepo = dingTalkUserRepo;
        _sysUserRep = sysUserRep;
    }

    /// <summary>
    /// 同步钉钉用户
    /// </summary>
    /// <returns></returns>
    [DisplayName("同步钉钉用户")]
    public async Task SyncDingTalkUser()
    {
        var param = new GetDingTalkTokenInput()
        {
            AppKey = _dingTalkOptions.ClientId,
            AppSecret = _dingTalkOptions.ClientSecret
        };
        var tokenRes = await _dingTalkApi.GetDingTalkToken(param);
        if (tokenRes.ErrCode != 0)
            throw Oops.Oh(tokenRes.ErrMsg);

        var offset = 0;
        while (offset >= 0)
        {
            // 获取用户Id列表
            var userIdsRes = await _dingTalkApi.GetDingTalkCurrentEmployeesList(tokenRes.AccessToken, new GetDingTalkCurrentEmployeesListInput
            {
                StatusList = "2,3,5,-1",
                Size = 50,
                Offset = offset
            });
            if (!userIdsRes.Success)
                throw Oops.Oh(userIdsRes.ErrMsg);

            // 根据用户Id获取花名册
            var rosterRes = await _dingTalkApi.GetDingTalkCurrentEmployeesRosterList(tokenRes.AccessToken, new GetDingTalkCurrentEmployeesRosterListInput()
            {
                UserIdList = string.Join(",", userIdsRes.Result.DataList),
                FieldFilterList = $"{DingTalkConst.NameField},{DingTalkConst.JobNumberField},{DingTalkConst.MobileField}",
                AgentId = _dingTalkOptions.AgentId
            });
            if (!rosterRes.Success)
                throw Oops.Oh(rosterRes.ErrMsg);

            // 判断新增还是更新
            var userIds = rosterRes.Result.Select(u => u.UserId).ToList();
            var uDingTalkUser = await _dingTalkUserRepo.AsQueryable()
                .Where(u => userIds.Contains(u.DingTalkUserId))
                .ToListAsync();

            var uUserIds = uDingTalkUser.Select(u => u.DingTalkUserId); // 需要更新的用户Id
            var iUserIds = userIds.Where(u => !uUserIds.Contains(u)); // 需要新增的用户Id

            // 保存钉钉用户
            var iUsers = rosterRes.Result
                .Where(u => iUserIds.Contains(u.UserId))
                .Select(u => new DingTalkUser
                {
                    DingTalkUserId = u.UserId,
                    Name = u.FieldDataList.Where(m => m.FieldCode == DingTalkConst.NameField).Select(m => m.FieldValueList.Select(n => n.Value).FirstOrDefault()).FirstOrDefault(),
                    Mobile = u.FieldDataList.Where(m => m.FieldCode == DingTalkConst.MobileField).Select(m => m.FieldValueList.Select(n => n.Value).FirstOrDefault()).FirstOrDefault(),
                    JobNumber = u.FieldDataList.Where(m => m.FieldCode == DingTalkConst.JobNumberField).Select(m => m.FieldValueList.Select(n => n.Value).FirstOrDefault()).FirstOrDefault(),
                }).ToList();
            if (iUsers.Count > 0)
            {
                await _dingTalkUserRepo.AsInsertable(iUsers).ExecuteCommandAsync();
            }

            // 更新钉钉用户
            var uUsers = rosterRes.Result
                .Where(u => uUserIds.Contains(u.UserId))
                .Select(u => new DingTalkUser
                {
                    Id = uDingTalkUser.Where(m => m.DingTalkUserId == u.UserId).Select(m => m.Id).FirstOrDefault(),
                    DingTalkUserId = u.UserId,
                    Name = u.FieldDataList.Where(m => m.FieldCode == DingTalkConst.NameField).Select(m => m.FieldValueList.Select(n => n.Value).FirstOrDefault()).FirstOrDefault(),
                    Mobile = u.FieldDataList.Where(m => m.FieldCode == DingTalkConst.MobileField).Select(m => m.FieldValueList.Select(n => n.Value).FirstOrDefault()).FirstOrDefault(),
                    JobNumber = u.FieldDataList.Where(m => m.FieldCode == DingTalkConst.JobNumberField).Select(m => m.FieldValueList.Select(n => n.Value).FirstOrDefault()).FirstOrDefault(),
                }).ToList();
            if (uUsers.Count > 0)
            {
                await _dingTalkUserRepo.AsUpdateable(uUsers).UpdateColumns(u => new
                {
                    u.DingTalkUserId,
                    u.Name,
                    u.Mobile,
                    u.JobNumber,
                    u.UpdateTime,
                    u.UpdateUserName,
                    u.UpdateUserId,
                }).ExecuteCommandAsync();
            }

            // 保存分页游标
            if (userIdsRes.Result.NextCursor == null)
                break;
            offset = (int)userIdsRes.Result.NextCursor;
        }

        var sysUser = await _sysUserRep.AsQueryable()
            .Select(u => new
            {
                u.Id,
                u.Account,
                u.Phone
            }).ToListAsync();
        var dingTalkUser = await _dingTalkUserRepo.AsQueryable()
            .Where(u => sysUser.Any(m => m.Account == u.JobNumber))
            .Select(u => new
            {
                u.Id,
                u.JobNumber,
                u.Mobile
            }).ToListAsync();

        // 更新钉钉用户中系统用户Id
        var uDingTalkUsers = dingTalkUser.Select(u => new DingTalkUser
        {
            Id = u.Id,
            SysUserId = sysUser.Where(m => m.Account == u.JobNumber).Select(m => m.Id).FirstOrDefault(),
        }).ToList();
        if (uDingTalkUsers.Count > 0)
        {
            await _dingTalkUserRepo.AsUpdateable(uDingTalkUsers).UpdateColumns(u => new
            {
                u.SysUserId,
                u.UpdateTime,
                u.UpdateUserName,
                u.UpdateUserId,
            }).ExecuteCommandAsync();
        }

        return;
    }

    /// <summary>
    /// 获取企业内部应用的access_token
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取企业内部应用的access_token")]
    public async Task<GetDingTalkTokenOutput> GetDingTalkToken([FromQuery] GetDingTalkTokenInput input)
    {
        return await _dingTalkApi.GetDingTalkToken(input);
    }

    /// <summary>
    /// 获取在职员工列表
    /// </summary>
    /// <param name="access_token"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取在职员工列表")]
    public async Task<DingTalkBaseResponse<GetDingTalkCurrentEmployeesListOutput>> GetDingTalkCurrentEmployeesList(string access_token, [Required] GetDingTalkCurrentEmployeesListInput input)
    {
        return await _dingTalkApi.GetDingTalkCurrentEmployeesList(access_token, input);
    }

    /// <summary>
    /// 获取员工花名册字段信息
    /// </summary>
    /// <param name="access_token"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取员工花名册字段信息")]
    public async Task<DingTalkBaseResponse<List<DingTalkEmpRosterFieldVo>>> GetDingTalkCurrentEmployeesRosterList(string access_token, [Required] GetDingTalkCurrentEmployeesRosterListInput input)
    {
        return await _dingTalkApi.GetDingTalkCurrentEmployeesRosterList(access_token, input);
    }

    /// <summary>
    /// 发送钉钉互动卡片
    /// </summary>
    /// <param name="token"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("给指定用户发送钉钉互动卡片")]
    public async Task<DingTalkSendInteractiveCardsOutput> DingTalkSendInteractiveCards(string token, DingTalkSendInteractiveCardsInput input)
    {
        return await _dingTalkApi.DingTalkSendInteractiveCards(token, input);
    }
}