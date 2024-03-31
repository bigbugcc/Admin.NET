// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// HTTP请求类型
/// </summary>
[Description("HTTP请求类型")]
public enum RequestTypeEnum
{
    /// <summary>
    /// 执行内部方法
    /// </summary>
    Run = 0,

    /// <summary>
    /// GET
    /// </summary>
    Get = 1,

    /// <summary>
    /// POST
    /// </summary>
    Post = 2,

    /// <summary>
    /// PUT
    /// </summary>
    Put = 3,

    /// <summary>
    /// DELETE
    /// </summary>
    Delete = 4
}