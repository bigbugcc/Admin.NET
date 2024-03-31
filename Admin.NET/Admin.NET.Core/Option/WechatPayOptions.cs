// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 微信支付配置选项
/// </summary>
public sealed class WechatPayOptions : WechatTenpayClientOptions, IConfigurableOptions
{
    /// <summary>
    /// 微信公众平台AppId、开放平台AppId、小程序AppId、企业微信CorpId
    /// </summary>
    public string AppId { get; set; }
}