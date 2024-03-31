// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 微信相关配置选项
/// </summary>
public sealed class WechatOptions : IConfigurableOptions
{
    //公众号
    public string WechatAppId { get; set; }

    public string WechatAppSecret { get; set; }

    //小程序
    public string WxOpenAppId { get; set; }

    public string WxOpenAppSecret { get; set; }
}