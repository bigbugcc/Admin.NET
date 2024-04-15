// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统作业信息表
/// </summary>
[SugarTable(null, "系统作业信息表")]
[SysTable]
[SugarIndex("index_{table}_J", nameof(JobId), OrderByType.Asc)]
public partial class SysJobDetail : EntityBaseId
{
    /// <summary>
    /// 作业Id
    /// </summary>
    [SugarColumn(ColumnDescription = "作业Id", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string JobId { get; set; }

    /// <summary>
    /// 组名称
    /// </summary>
    [SugarColumn(ColumnDescription = "组名称", Length = 128)]
    [MaxLength(128)]
    public string? GroupName { get; set; } = "default";

    /// <summary>
    /// 作业类型FullName
    /// </summary>
    [SugarColumn(ColumnDescription = "作业类型", Length = 128)]
    [MaxLength(128)]
    public string? JobType { get; set; }

    /// <summary>
    /// 程序集Name
    /// </summary>
    [SugarColumn(ColumnDescription = "程序集", Length = 128)]
    [MaxLength(128)]
    public string? AssemblyName { get; set; }

    /// <summary>
    /// 描述信息
    /// </summary>
    [SugarColumn(ColumnDescription = "描述信息", Length = 128)]
    [MaxLength(128)]
    public string? Description { get; set; }

    /// <summary>
    /// 是否并行执行
    /// </summary>
    [SugarColumn(ColumnDescription = "是否并行执行")]
    public bool Concurrent { get; set; } = true;

    /// <summary>
    /// 是否扫描特性触发器
    /// </summary>
    [SugarColumn(ColumnDescription = "是否扫描特性触发器", ColumnName = "annotation")]
    public bool IncludeAnnotation { get; set; } = false;

    /// <summary>
    /// 额外数据
    /// </summary>
    [SugarColumn(ColumnDescription = "额外数据", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? Properties { get; set; } = "{}";

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public DateTime? UpdatedTime { get; set; }

    /// <summary>
    /// 作业创建类型
    /// </summary>
    [SugarColumn(ColumnDescription = "作业创建类型")]
    public JobCreateTypeEnum CreateType { get; set; } = JobCreateTypeEnum.BuiltIn;

    /// <summary>
    /// 脚本代码
    /// </summary>
    [SugarColumn(ColumnDescription = "脚本代码", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? ScriptCode { get; set; }
}