// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.DingTalk.Service;

/// <summary>
/// 钉钉服务 🧩
/// </summary>
[ApiDescriptionSettings(DingTalkConst.GroupName, Order = 100)]
public class DingTalkService : IDynamicApiController, IScoped
{
    private readonly IDingTalkApi _dingTalkApi;
    private readonly DingTalkOptions _dingTalkOptions;

    public DingTalkService(IDingTalkApi dingTalkApi,
        IOptions<DingTalkOptions> dingTalkOptions)
    {
        _dingTalkApi = dingTalkApi;
        _dingTalkOptions = dingTalkOptions.Value;
    }

    /// <summary>
    /// 获取企业内部应用的access_token
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取企业内部应用的access_token")]
    public async Task<GetDingTalkTokenOutput> GetDingTalkToken()
    {
        var tokenRes = await _dingTalkApi.GetDingTalkToken(_dingTalkOptions.ClientId, _dingTalkOptions.ClientSecret);
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
    [DisplayName("获取在职员工列表")]
    public async Task<DingTalkBaseResponse<GetDingTalkCurrentEmployeesListOutput>> GetDingTalkCurrentEmployeesList(string access_token, [Required] GetDingTalkCurrentEmployeesListInput input)
    {
        return await _dingTalkApi.GetDingTalkCurrentEmployeesList(access_token, input);
    }

    /// <summary>
    /// 获取员工花名册字段信息 🔖
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
    /// 发送钉钉互动卡片 🔖
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