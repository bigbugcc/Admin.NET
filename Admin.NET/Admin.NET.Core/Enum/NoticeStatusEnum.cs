// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 通知公告状态枚举
/// </summary>
[Description("通知公告状态枚举")]
public enum NoticeStatusEnum
{
    /// <summary>
    /// 草稿
    /// </summary>
    [Description("草稿")]
    DRAFT = 0,

    /// <summary>
    /// 发布
    /// </summary>
    [Description("发布")]
    PUBLIC = 1,

    /// <summary>
    /// 撤回
    /// </summary>
    [Description("撤回")]
    CANCEL = 2,

    /// <summary>
    /// 删除
    /// </summary>
    [Description("删除")]
    DELETED = 3
}