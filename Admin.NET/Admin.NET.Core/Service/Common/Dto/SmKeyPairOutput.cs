// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

/// <summary>
/// 国密公钥私钥对输出
/// </summary>
public class SmKeyPairOutput
{
    /// <summary>
    /// 私匙
    /// </summary>
    public string PrivateKey { get; set; }

    /// <summary>
    /// 公匙
    /// </summary>
    public string PublicKey { get; set; }
}