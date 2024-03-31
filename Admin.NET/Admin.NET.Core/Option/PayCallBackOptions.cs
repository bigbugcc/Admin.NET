// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 支付回调配置选项
/// </summary>
public sealed class PayCallBackOptions : IConfigurableOptions
{
    /// <summary>
    /// 微信支付回调
    /// </summary>
    public string WechatPayUrl { get; set; }

    /// <summary>
    /// 微信退款回调
    /// </summary>
    public string WechatRefundUrl { get; set; }

    /// <summary>
    /// 支付宝支付回调
    /// </summary>
    public string AlipayUrl { get; set; }

    /// <summary>
    /// 支付宝退款回调
    /// </summary>
    public string AlipayRefundUrl { get; set; }
}