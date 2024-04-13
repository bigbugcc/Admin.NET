// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

using Admin.NET.Core.Integrations;

namespace Admin.NET.Core.DingTalk;
/// <summary>
/// 钉钉服务
/// </summary>
[ApiDescriptionSettings(Order = 999)]
public class DingTalkService : IDynamicApiController, IScoped
{
    private readonly UserManager _userManager;
    private readonly IDingTalkApi _dingTalkApi;
    private readonly DingTalkOptions _dingTalkOptions;
    private readonly SqlSugarRepository<SysDingTalkUser> _sysDingTalkUserRepo;
    private readonly SqlSugarRepository<SysUser> _sysUserRep;
    private readonly IJsonSerializerProvider _jsonSerializer;
    public DingTalkService(
        UserManager userManager,
        IDingTalkApi dingTalkApi,
        IOptions<DingTalkOptions> dingTalkOptions,
        SqlSugarRepository<SysDingTalkUser> sysDingTalkUserRepo,
        SqlSugarRepository<SysUser> sysUserRep,
        IJsonSerializerProvider jsonSerializer
        )
    {
        _userManager = userManager;
        _dingTalkApi = dingTalkApi;
        _dingTalkOptions = dingTalkOptions.Value;
        _sysDingTalkUserRepo = sysDingTalkUserRepo;
        _sysUserRep = sysUserRep;
        _jsonSerializer = jsonSerializer;

    }

    /// <summary>
    /// 同步钉钉用户(后面可以做到job中)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [DisplayName("同步钉钉用户")]
    public async Task SyncDingTalkUser()
    {
        // 获取token
        var param = new GetDingTalkTokenInput()
        {
            AppKey = _dingTalkOptions.ClientId,
            AppSecret = _dingTalkOptions.ClientSecret
        };
        var tokenRes = await _dingTalkApi.GetDingTalkToken(param);
        if (tokenRes.ErrCode != 0)
        {
            throw Oops.Oh(tokenRes.ErrMsg);
        }
        var offset = 0;
        while (offset >= 0)
        {
            // 获取用户id列表
            var userIdsRes = await _dingTalkApi.GetDingTalkCurrentEmployeesList(tokenRes.AccessToken, new GetDingTalkCurrentEmployeesListInput
            {
                StatusList = "2,3,5,-1",
                Size = 50,
                Offset = offset
            });
            if (!userIdsRes.Success)
            {
                throw Oops.Oh(userIdsRes.ErrMsg);
            }
            // 根据用户id获取花名册
            var rosterRes = await _dingTalkApi.GetDingTalkCurrentEmployeesRosterList(tokenRes.AccessToken, new GetDingTalkCurrentEmployeesRosterListInput()
            {
                UserIdList = string.Join(",", userIdsRes.Result.DataList),
                FieldFilterList = $"{DingTalkFieldConst.NameField},{DingTalkFieldConst.JobNumberField},{DingTalkFieldConst.MobileField}",
                AgentId = _dingTalkOptions.AgentId
            });
            if (!rosterRes.Success)
            {
                throw Oops.Oh(rosterRes.ErrMsg);
            }
            // 判断新增还是更新
            var userIds = rosterRes.Result.Select(res => res.UserId).ToList();
            var uDingTalkUser = await _sysDingTalkUserRepo.AsQueryable()
                .Where(u => userIds.Contains(u.DingTalkUserId))
                .ToListAsync();
            // 需要更新的用户id
            var uUserIds = uDingTalkUser.Select(u => u.DingTalkUserId);
            // 需要新增的用户id
            var iUserIds = userIds.Where(u => !uUserIds.Contains(u));

            #region 保存到钉钉用户表
            var iUser = rosterRes.Result
                .Where(res => iUserIds.Contains(res.UserId))
                .Select(res => new SysDingTalkUser
                {
                    DingTalkUserId = res.UserId,
                    Name = res.FieldDataList
                    .Where(f => f.FieldCode == DingTalkFieldConst.NameField)
                    .Select(f => f.FieldValueList.Select(v => v.Value).FirstOrDefault())
                    .FirstOrDefault(),
                    Mobile = res.FieldDataList
                    .Where(f => f.FieldCode == DingTalkFieldConst.MobileField)
                    .Select(f => f.FieldValueList.Select(v => v.Value).FirstOrDefault())
                    .FirstOrDefault(),
                    JobNumber = res.FieldDataList
                    .Where(f => f.FieldCode == DingTalkFieldConst.JobNumberField)
                    .Select(f => f.FieldValueList.Select(v => v.Value).FirstOrDefault())
                    .FirstOrDefault(),
                }).ToList();
            if (iUser.Count > 0)
            {
                var iUserRes = await _sysDingTalkUserRepo.AsInsertable(iUser).ExecuteCommandAsync();
                if (iUserRes <= 0)
                {
                    throw Oops.Oh("保存钉钉用户错误");
                }
            }
            #endregion

            #region 更新钉钉用户
            var uUser = rosterRes.Result
            .Where(res => uUserIds.Contains(res.UserId))
            .Select(res => new SysDingTalkUser
            {
                Id = uDingTalkUser.Where(d => d.DingTalkUserId == res.UserId).Select(d => d.Id).FirstOrDefault(),
                DingTalkUserId = res.UserId,
                Name = res.FieldDataList
                    .Where(f => f.FieldCode == DingTalkFieldConst.NameField)
                    .Select(f => f.FieldValueList.Select(v => v.Value).FirstOrDefault())
                    .FirstOrDefault(),
                Mobile = res.FieldDataList
                    .Where(f => f.FieldCode == DingTalkFieldConst.MobileField)
                    .Select(f => f.FieldValueList.Select(v => v.Value).FirstOrDefault())
                    .FirstOrDefault(),
                JobNumber = res.FieldDataList
                    .Where(f => f.FieldCode == DingTalkFieldConst.JobNumberField)
                    .Select(f => f.FieldValueList.Select(v => v.Value).FirstOrDefault())
                    .FirstOrDefault(),
            }).ToList();
            if (uUser.Count > 0)
            {
                var uUserRes = await _sysDingTalkUserRepo.AsUpdateable(uUser)
                .UpdateColumns(d => new
                {
                    d.DingTalkUserId,
                    d.Name,
                    d.Mobile,
                    d.JobNumber,
                    d.UpdateTime,
                    d.UpdateUserName,
                    d.UpdateUserId,
                }).ExecuteCommandAsync();
                if (uUserRes <= 0)
                {
                    throw Oops.Oh("更新钉钉用户错误");
                }
            }
            #endregion

            // 保存分页游标
            if (userIdsRes.Result.NextCursor == null)
            {
                break;
            }
            offset = (int)userIdsRes.Result.NextCursor;
        }


        var sysUser = await _sysUserRep.AsQueryable().Select(x => new
        {
            x.Id,
            x.Account,
            x.Phone
        }).ToListAsync();

        var sysDingTalkUser = await _sysDingTalkUserRepo.AsQueryable()
            .Where(d => sysUser.Any(u => u.Account == d.JobNumber))
            .Select(x => new
            {
                x.Id,
                x.JobNumber,
                x.Mobile
            }).ToListAsync();

        // 更新钉钉用户中系统用户id
        var uSysDingTalkUser = sysDingTalkUser.Select(d => new SysDingTalkUser
        {
            Id = d.Id,
            SysUserId = sysUser.Where(u => u.Account == d.JobNumber).Select(u => u.Id).FirstOrDefault(),
        }).ToList();
        var uSysDingTalkUserRes = await _sysDingTalkUserRepo.AsUpdateable(uSysDingTalkUser)
        .UpdateColumns(d => new
        {
            d.SysUserId,
            d.UpdateTime,
            d.UpdateUserName,
            d.UpdateUserId,
        }).ExecuteCommandAsync();
        if (uSysDingTalkUserRes <= 0)
        {
            throw Oops.Oh("更新钉钉用户错误");
        }
        return;




    }


    /// <summary>
    /// 获取企业内部应用的access_token
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
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
    [HttpPost]
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
    [HttpPost]
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
    [HttpPost]
    [DisplayName("给指定用户发送钉钉互动卡片")]
    public async Task<DingTalkSendInteractiveCardsOutput> DingTalkSendInteractiveCards(string token, DingTalkSendInteractiveCardsInput input)
    {
        return await _dingTalkApi.DingTalkSendInteractiveCards(token, input);
    }



}


