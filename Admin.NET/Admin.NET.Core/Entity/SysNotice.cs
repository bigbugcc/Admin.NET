// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统通知公告表
/// </summary>
[SugarTable(null, "系统通知公告表")]
[SysTable]
[SugarIndex("index_{table}_T", nameof(Type), OrderByType.Asc)]
public partial class SysNotice : EntityBase
{
    /// <summary>
    /// 标题
    /// </summary>
    [SugarColumn(ColumnDescription = "标题", Length = 32)]
    [Required, MaxLength(32)]
    [SensitiveDetection('*')]
    public virtual string Title { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    [SugarColumn(ColumnDescription = "内容", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    [Required]
    [SensitiveDetection('*')]
    public virtual string Content { get; set; }

    /// <summary>
    /// 类型（1通知 2公告）
    /// </summary>
    [SugarColumn(ColumnDescription = "类型（1通知 2公告）")]
    public NoticeTypeEnum Type { get; set; }

    /// <summary>
    /// 发布人Id
    /// </summary>
    [SugarColumn(ColumnDescription = "发布人Id")]
    public long PublicUserId { get; set; }

    /// <summary>
    /// 发布人姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "发布人姓名", Length = 32)]
    [MaxLength(32)]
    public string? PublicUserName { get; set; }

    /// <summary>
    /// 发布机构Id
    /// </summary>
    [SugarColumn(ColumnDescription = "发布机构Id")]
    public long PublicOrgId { get; set; }

    /// <summary>
    /// 发布机构名称
    /// </summary>
    [SugarColumn(ColumnDescription = "发布机构名称", Length = 64)]
    [MaxLength(64)]
    public string? PublicOrgName { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    [SugarColumn(ColumnDescription = "发布时间")]
    public DateTime? PublicTime { get; set; }

    /// <summary>
    /// 撤回时间
    /// </summary>
    [SugarColumn(ColumnDescription = "撤回时间")]
    public DateTime? CancelTime { get; set; }

    /// <summary>
    /// 状态（0草稿 1发布 2撤回 3删除）
    /// </summary>
    [SugarColumn(ColumnDescription = "状态（0草稿 1发布 2撤回 3删除）")]
    public NoticeStatusEnum Status { get; set; }
}