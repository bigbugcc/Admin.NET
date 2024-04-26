// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.DingTalk;

public class DingTalkSendInteractiveCardsInput
{
    /// <summary>
    /// 互动卡片的消息模板Id
    /// </summary>
    [Required(ErrorMessage = "互动卡片的消息模板Id必填!")]
    public string? CardTemplateId { get; set; }

    /// <summary>
    /// 群Id
    /// </summary>
    /// <remarks>
    /// 1、基于群模板创建的群。
    /// 企业内部应用，调用创建群接口获取open_conversation_id参数值。
    /// 2、安装群聊酷应用的群。
    /// 企业内部应用，通过群内安装酷应用事件获取回调参数OpenConversationId参数值。
    /// </remarks>
    public string OpenConversationId { get; set; }

    /// <summary>
    /// 接收人userId列表
    /// </summary>
    /// <remarks>
    /// 单聊：receiverUserIdList填写用户ID，最大值20。
    /// 群聊：receiverUserIdList填写用户ID，表示当前对应ID的群内用户可见
    /// receiverUserIdList参数不填写，表示当前群内所有用户可见
    /// </remarks>
    [Required(ErrorMessage = "接收人userId列表必填!")]
    public List<string>? ReceiverUserIdList { get; set; }

    /// <summary>
    /// 唯一标示卡片的外部编码
    /// </summary>
    [Required(ErrorMessage = "唯一标示卡片的外部编码必填!")]
    public string? OutTrackId { get; set; }

    /// <summary>
    /// 机器人的编码
    /// </summary>
    public string RobotCode { get; set; }

    /// <summary>
    /// 发送的会话类型
    /// </summary>
    [Required(ErrorMessage = "会话类型必填!")]
    public DingTalkConversationTypeEnum? ConversationType { get; set; }

    /// <summary>
    /// 卡片回调时的路由Key，用于查询注册的callbackUrl
    /// </summary>
    public string CallbackRouteKey { get; set; }

    /// <summary>
    /// 卡片公有数据
    /// </summary>
    [Required(ErrorMessage = "卡片公有数据必填!")]
    public DingTalkCardData CardData { get; set; }
}

public class GetDingTalkCardMessageReadStatusInput
{
    /// <summary>
    /// 机器人的编码
    /// </summary>
    public string RobotCode { set; get; }

    /// <summary>
    /// 消息唯一标识，可通过批量发送人与机器人会话中机器人消息接口返回参数中processQueryKey字段获取。
    /// </summary>
    public string ProcessQueryKey { set; get; }
}

public class GetDingTalkCardMessageReadStatusOutput
{
    /// <summary>
    /// 消息发送状态，SUCCESS：成功、RECALLED：已撤回、PROCESSING： 处理中
    /// </summary>
    public string SendStatus { get; set; }

    /// <summary>
    ///
    /// </summary>
    public DingTalkCardMessageReadInfoList MessageReadInfoList { get; set; }
}

/// <summary>
/// 钉钉卡片消息已读情况
/// </summary>
public class DingTalkCardMessageReadInfoList
{
    /// <summary>
    /// 消息接收者名称
    /// </summary>
    public string Name { set; get; }

    /// <summary>
    /// 消息接收者的userId
    /// </summary>
    public string UserId { set; get; }

    /// <summary>
    /// 已读状态，READ：已读、UNREAD：未读
    /// </summary>
    public string ReadStatus { set; get; }
}