// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

/// <summary>
/// 生成网页授权Url
/// </summary>
public class GenAuthUrlInput
{
    /// <summary>
    /// RedirectUrl
    /// </summary>
    public string RedirectUrl { get; set; }

    /// <summary>
    /// Scope
    /// </summary>
    public string Scope { get; set; }
    
    /// <summary>
    /// State
    /// </summary>
    public string State { get; set; }
}

/// <summary>
/// 获取微信用户OpenId
/// </summary>
public class WechatOAuth2Input
{
    /// <summary>
    /// Code
    /// </summary>
    [Required(ErrorMessage = "Code不能为空"), MinLength(10, ErrorMessage = "Code错误")]
    public string Code { get; set; }
}

/// <summary>
/// 微信用户登录
/// </summary>
public class WechatUserLogin
{
    /// <summary>
    /// OpenId
    /// </summary>
    [Required(ErrorMessage = "微信标识不能为空"), MinLength(10, ErrorMessage = "微信标识长错误")]
    public string OpenId { get; set; }
}

/// <summary>
/// 获取配置签名
/// </summary>
public class SignatureInput
{
    /// <summary>
    /// Url
    /// </summary>
    public string Url { get; set; }
}

/// <summary>
/// 获取消息模板列表
/// </summary>
public class MessageTemplateSendInput
{
    /// <summary>
    /// 订阅模板Id
    /// </summary>
    [Required(ErrorMessage = "订阅模板Id不能为空")]
    public string TemplateId { get; set; }

    /// <summary>
    /// 接收者的OpenId
    /// </summary>
    [Required(ErrorMessage = "接收者的OpenId不能为空")]
    public string ToUserOpenId { get; set; }

    /// <summary>
    /// 模板数据，格式形如 { "key1": { "value": any }, "key2": { "value": any } }
    /// </summary>
    [Required(ErrorMessage = "模板数据不能为空")]
    public Dictionary<string, CgibinMessageSubscribeSendRequest.Types.DataItem> Data { get; set; }

    /// <summary>
    /// 模板跳转链接
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 所需跳转到小程序的具体页面路径，支持带参数,（示例index?foo=bar）
    /// </summary>
    public string MiniProgramPagePath { get; set; }
}

/// <summary>
/// 删除消息模板
/// </summary>
public class DeleteMessageTemplateInput
{
    /// <summary>
    /// 订阅模板Id
    /// </summary>
    [Required(ErrorMessage = "订阅模板Id不能为空")]
    public string TemplateId { get; set; }
}