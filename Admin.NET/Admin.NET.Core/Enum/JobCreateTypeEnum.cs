// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 作业创建类型枚举
/// </summary>
[Description("作业创建类型枚举")]
public enum JobCreateTypeEnum
{
    /// <summary>
    /// 内置
    /// </summary>
    [Description("内置")]
    BuiltIn = 0,

    /// <summary>
    /// 脚本
    /// </summary>
    [Description("脚本")]
    Script = 1,

    /// <summary>
    /// HTTP请求
    /// </summary>
    [Description("HTTP请求")]
    Http = 2,
}