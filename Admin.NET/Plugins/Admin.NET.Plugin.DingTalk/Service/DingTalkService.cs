// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Plugin.DingTalk.Service;

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