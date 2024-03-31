// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 雪花Id配置选项
/// </summary>
public sealed class SnowIdOptions : IdGeneratorOptions, IConfigurableOptions
{
    /// <summary>
    /// 缓存前缀
    /// </summary>
    public string WorkerPrefix { get; set; }
}