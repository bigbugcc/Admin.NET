// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 通知公告状类型枚举
/// </summary>
[Description("通知公告状类型枚举")]
public enum NoticeTypeEnum
{
    /// <summary>
    /// 通知
    /// </summary>
    [Description("通知")]
    NOTICE = 1,

    /// <summary>
    /// 公告
    /// </summary>
    [Description("公告")]
    ANNOUNCEMENT = 2,
}