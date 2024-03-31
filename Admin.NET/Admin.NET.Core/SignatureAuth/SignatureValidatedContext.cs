// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

using Microsoft.AspNetCore.Authentication;

namespace Admin.NET.Core;

/// <summary>
/// Signature 身份验证已验证上下文
/// </summary>
public class SignatureValidatedContext : ResultContext<SignatureAuthenticationOptions>
{
    public SignatureValidatedContext(HttpContext context,
        AuthenticationScheme scheme,
        SignatureAuthenticationOptions options)
        : base(context, scheme, options)
    {
    }

    /// <summary>
    /// 身份标识
    /// </summary>
    public string AccessKey { get; set; }

    /// <summary>
    /// 密钥
    /// </summary>
    public string AccessSecret { get; set; }
}