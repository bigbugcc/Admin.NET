// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

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