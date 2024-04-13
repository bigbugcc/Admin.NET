// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

using Furion.RemoteRequest;

namespace Admin.NET.Core.Integrations;
public interface IDingTalkApi : IHttpDispatchProxy
{
    /// <summary>
    /// 获取企业内部应用的access_token
    /// </summary>
    /// <returns></returns>
    [Get("https://oapi.dingtalk.com/gettoken")]
    Task<GetDingTalkTokenOutput> GetDingTalkToken([QueryString] GetDingTalkTokenInput input);
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