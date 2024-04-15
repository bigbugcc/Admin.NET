// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.DingTalk;

public interface IDingTalkApi : IHttpDispatchProxy
{
    /// <summary>
    /// 获取企业内部应用的access_token
    /// </summary>
    /// <param name="appkey">应用的唯一标识key</param>
    /// <param name="appsecret"> 应用的密钥。AppKey和AppSecret可在钉钉开发者后台的应用详情页面获取。</param>
    /// <returns></returns>
    [Get("https://oapi.dingtalk.com/gettoken")]
    Task<GetDingTalkTokenOutput> GetDingTalkToken([QueryString] string appkey, [QueryString] string appsecret);

    /// <summary>
    /// 获取在职员工列表
    /// </summary>
    /// <param name="access_token">调用该接口的应用凭证</param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Post("https://oapi.dingtalk.com/topapi/smartwork/hrm/employee/queryonjob")]
    Task<DingTalkBaseResponse<GetDingTalkCurrentEmployeesListOutput>> GetDingTalkCurrentEmployeesList([QueryString] string access_token,
        [Body, Required] GetDingTalkCurrentEmployeesListInput input);

    /// <summary>
    /// 获取员工花名册字段信息
    /// </summary>
    /// <param name="access_token">调用该接口的应用凭证</param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Post("https://oapi.dingtalk.com/topapi/smartwork/hrm/employee/v2/list")]
    Task<DingTalkBaseResponse<List<DingTalkEmpRosterFieldVo>>> GetDingTalkCurrentEmployeesRosterList([QueryString] string access_token,
        [Body, Required] GetDingTalkCurrentEmployeesRosterListInput input);

    /// <summary>
    /// 发送钉钉互动卡片
    /// </summary>
    /// <param name="token">调用该接口的访问凭证</param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Post("https://api.dingtalk.com/v1.0/im/interactiveCards/send")]
    Task<DingTalkSendInteractiveCardsOutput> DingTalkSendInteractiveCards(
        [Headers("x-acs-dingtalk-access-token")] string token,
        [Body] DingTalkSendInteractiveCardsInput input);

    /// <summary>
    /// 获取钉钉卡片消息读取状态
    /// </summary>
    /// <param name="token"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Get("https://api.dingtalk.com/v1.0/robot/oToMessages/readStatus")]
    Task<GetDingTalkCardMessageReadStatusOutput> GetDingTalkCardMessageReadStatus(
    [Headers("x-acs-dingtalk-access-token")] string token,
    [QueryString] GetDingTalkCardMessageReadStatusInput input);
}