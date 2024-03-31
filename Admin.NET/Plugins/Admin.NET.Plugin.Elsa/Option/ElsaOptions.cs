// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Plugin.Elsa;

/// <summary>
/// Elsa 配置选项
/// </summary>
public sealed class ElsaOptions : IConfigurableOptions
{
    /// <summary>
    /// 服务地址
    /// </summary>
    public Elsa_Server Server { get; set; }
}

public sealed class Elsa_Server
{
    /// <summary>
    /// 地址
    /// </summary>
    public string BaseUrl { get; set; }
}