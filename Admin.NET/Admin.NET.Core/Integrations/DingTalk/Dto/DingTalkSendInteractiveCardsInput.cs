// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Integrations;
public class DingTalkSendInteractiveCardsInput
{
    /// <summary>
    /// 互动卡片的消息模板ID
    /// </summary>
    [Required(ErrorMessage = "互动卡片的消息模板Id必填!")]
    [JsonPropertyName("cardTemplateId")]
    public string? CardTemplateId { get; set; }
    /// <summary>
    /// 群ID
    /// </summary>
    /// <remarks>
    /// 1、基于群模板创建的群。
    /// 企业内部应用，调用创建群接口获取open_conversation_id参数值。
    /// 2、安装群聊酷应用的群。
    /// 企业内部应用，通过群内安装酷应用事件获取回调参数OpenConversationId参数值。
    /// </remarks>
    [JsonPropertyName("openConversationId")]
    public string OpenConversationId { get; set; }
    /// <summary>
    /// 接收人userId列表。
    /// </summary>
    /// <remarks>
    /// 单聊：receiverUserIdList填写用户ID，最大值20。
    /// 群聊：receiverUserIdList填写用户ID，表示当前对应ID的群内用户可见
    /// receiverUserIdList参数不填写，表示当前群内所有用户可见
    /// </remarks>
    [Required(ErrorMessage = "接收人userId列表必填!")]
    [JsonPropertyName("receiverUserIdList")]
    public List<string>? ReceiverUserIdList { get; set; }
    /// <summary>
    /// 唯一标示卡片的外部编码
    /// </summary>
    [Required(ErrorMessage = "唯一标示卡片的外部编码必填!")]
    [JsonPropertyName("outTrackId")]
    public string? OutTrackId { get; set; }
    /// <summary>
    /// 机器人的编码
    /// </summary>
    [JsonPropertyName("robotCode")]
    public string RobotCode { get; set; }
    /// <summary>
    /// 发送的会话类型：
    /// </summary>
    [Required(ErrorMessage = "会话类型必填!")]
    [JsonPropertyName("conversationType")]
    public DingTalkConversationTypeEnum? ConversationType { get; set; }
    /// <summary>
    /// 卡片回调时的路由Key，用于查询注册的callbackUrl
    /// </summary>
    [JsonPropertyName("callbackRouteKey")]
    public string CallbackRouteKey { get; set; }
    /// <summary>
    /// 卡片公有数据。
    /// </summary>
    [Required(ErrorMessage = "卡片公有数据必填!")]
    [JsonPropertyName("cardData")]
    public DingTalkCardData CardData { get; set; }
}


public class GetDingTalkCardMessageReadStatusInput
{
    /// <summary>
    /// 机器人的编码
    /// </summary>
    [JsonPropertyName("robotCode")]
    public string robotCode { set; get; }
    /// <summary>
    /// 消息唯一标识，可通过批量发送人与机器人会话中机器人消息接口返回参数中processQueryKey字段获取。
    /// </summary>
    [JsonPropertyName("processQueryKey")]
    public string processQueryKey { set; get; }
}

public class GetDingTalkCardMessageReadStatusOutput
{
    /// <summary>
    /// 消息发送状态
    /// </summary>
    /// <remarks>
    /// SUCCESS：成功
    /// RECALLED：已撤回
    /// PROCESSING： 处理中
    /// </remarks>
    [JsonPropertyName("sendStatus")]
    public string sendStatus { get; set; }
    [JsonPropertyName("messageReadInfoList")]
    public DingTalkCardMessageReadInfoList messageReadInfoList { get; set; }
}
/// <summary>
/// 钉钉卡片消息已读情况
/// </summary>
public class DingTalkCardMessageReadInfoList
{
    /// <summary>
    /// 消息接收者名称
    /// </summary>
    [JsonPropertyName("name")]
    public string name { set; get; }
    /// <summary>
    /// 消息接收者的userId
    /// </summary>
    [JsonPropertyName("userId")]
    public string userId { set; get; }
    /// <summary>
    /// 已读状态：
    /// </summary>
    /// <remarks>
    /// READ：已读
    /// UNREAD：未读
    /// </remarks>
    [JsonPropertyName("readStatus")]
    public string readStatus { set; get; }
}