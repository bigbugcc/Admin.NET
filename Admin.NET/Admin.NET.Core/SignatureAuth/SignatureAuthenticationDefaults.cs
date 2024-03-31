// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// Signature 身份验证处理程序相关的默认值
/// </summary>
public static class SignatureAuthenticationDefaults
{
    /// <summary>
    /// SignatureAuthenticationOptions.AuthenticationScheme 使用的默认值
    /// </summary>
    public const string AuthenticationScheme = "Signature";

    /// <summary>
    /// 附加在 HttpContext Item 中验证失败消息的 Key
    /// </summary>
    public const string AuthenticateFailMsgKey = "SignatureAuthenticateFailMsg";
}