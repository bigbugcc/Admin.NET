// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 密码配置选项
/// </summary>
public sealed class CryptogramOptions : IConfigurableOptions
{
    /// <summary>
    /// 是否开启密码强度验证
    /// </summary>
    public bool StrongPassword { get; set; }

    /// <summary>
    /// 密码强度验证正则表达式
    /// </summary>
    public string PasswordStrengthValidation { get; set; }

    /// <summary>
    /// 密码强度验证提示
    /// </summary>
    public string PasswordStrengthValidationMsg { get; set; }

    /// <summary>
    /// 密码类型
    /// </summary>
    public string CryptoType { get; set; }

    /// <summary>
    /// 公钥
    /// </summary>
    public string PublicKey { get; set; }

    /// <summary>
    /// 私钥
    /// </summary>
    public string PrivateKey { get; set; }
}