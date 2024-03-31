// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 系统通知公告用户表
/// </summary>
[SugarTable(null, "系统通知公告用户表")]
[SysTable]
public partial class SysNoticeUser : EntityBaseId
{
    /// <summary>
    /// 通知公告Id
    /// </summary>
    [SugarColumn(ColumnDescription = "通知公告Id")]
    public long NoticeId { get; set; }

    /// <summary>
    /// 通知公告
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(NoticeId))]
    public SysNotice SysNotice { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "用户Id")]
    public long UserId { get; set; }

    /// <summary>
    /// 阅读时间
    /// </summary>
    [SugarColumn(ColumnDescription = "阅读时间")]
    public DateTime? ReadTime { get; set; }

    /// <summary>
    /// 状态（0未读 1已读）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态（0未读 1已读）")]
    public NoticeUserStatusEnum ReadStatus { get; set; } = NoticeUserStatusEnum.UNREAD;
}