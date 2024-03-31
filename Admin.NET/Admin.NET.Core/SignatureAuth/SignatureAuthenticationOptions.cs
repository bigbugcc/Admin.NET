// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

using Microsoft.AspNetCore.Authentication;

namespace Admin.NET.Core;

/// <summary>
/// Signature 身份验证选项
/// </summary>
public class SignatureAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// 请求时间允许的偏差范围
    /// </summary>
    public TimeSpan AllowedDateDrift { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Signature 身份验证事件
    /// </summary>
    public new SignatureAuthenticationEvent Events
    {
        get => (SignatureAuthenticationEvent)base.Events;
        set => base.Events = value;
    }
}