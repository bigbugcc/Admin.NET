// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 微信相关配置选项
/// </summary>
public sealed class WechatOptions : IConfigurableOptions
{
    // 公众号
    public string WechatAppId { get; set; }

    public string WechatAppSecret { get; set; }

    /// <summary>
    /// 微信公众号服务器配置中的令牌(Token)
    /// </summary>
    public string WechatToken { get; set; }

    /// <summary>
    /// 微信公众号服务器配置中的消息加解密密钥(EncodingAESKey)
    /// </summary>
    public string WechatEncodingAESKey { get; set; }

    // 小程序
    public string WxOpenAppId { get; set; }

    public string WxOpenAppSecret { get; set; }

    /// <summary>
    /// 小程序消息推送中的令牌(Token)
    /// </summary>
    public string WxToken { get; set; }

    /// <summary>
    /// 小程序消息推送中的消息加解密密钥(EncodingAESKey)
    /// </summary>
    public string WxEncodingAESKey { get; set; }
}