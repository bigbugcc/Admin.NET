// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 岗位状态枚举
/// </summary>
[Description("岗位状态枚举")]
public enum JobStatusEnum
{
    /// <summary>
    /// 在职
    /// </summary>
    [Description("在职")]
    On = 1,

    /// <summary>
    /// 离职
    /// </summary>
    [Description("离职")]
    Off = 2,

    /// <summary>
    /// 请假
    /// </summary>
    [Description("请假")]
    Leave = 3,

    /// <summary>
    /// 其他
    /// </summary>
    [Description("其他")]
    Other = 4,
}