// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 婚姻状况枚举
/// </summary>
[Description("婚姻状况枚举")]
public enum MaritalStatusEnum
{
    /// <summary>
    /// 未婚
    /// </summary>
    [Description("未婚")]
    UnMarried = 1,

    /// <summary>
    /// 已婚
    /// </summary>
    [Description("已婚")]
    Married = 2,

    /// <summary>
    /// 离异
    /// </summary>
    [Description("离异")]
    Divorce = 3,

    /// <summary>
    /// 再婚
    /// </summary>
    [Description("再婚")]
    Remarry = 4,

    /// <summary>
    /// 丧偶
    /// </summary>
    [Description("丧偶")]
    Widowed = 5,

    /// <summary>
    /// 未知
    /// </summary>
    [Description("未知")]
    None = 6,
}