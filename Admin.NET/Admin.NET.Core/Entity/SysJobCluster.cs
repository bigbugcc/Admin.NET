// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 系统作业集群表
/// </summary>
[SugarTable(null, "系统作业集群表")]
[SysTable]
public partial class SysJobCluster : EntityBaseId
{
    /// <summary>
    /// 作业集群Id
    /// </summary>
    [SugarColumn(ColumnDescription = "作业集群Id", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string ClusterId { get; set; }

    /// <summary>
    /// 描述信息
    /// </summary>
    [SugarColumn(ColumnDescription = "描述信息", Length = 128)]
    [MaxLength(128)]
    public string? Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public ClusterStatus Status { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public DateTime? UpdatedTime { get; set; }
}