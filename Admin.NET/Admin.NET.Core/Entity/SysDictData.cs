// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统字典值表
/// </summary>
[SugarTable(null, "系统字典值表")]
[SysTable]
[SugarIndex("index_{table}_C", nameof(Code), OrderByType.Asc)]
public partial class SysDictData : EntityBase
{
    /// <summary>
    /// 字典类型Id
    /// </summary>
    [SugarColumn(ColumnDescription = "字典类型Id")]
    public long DictTypeId { get; set; }

    /// <summary>
    /// 字典类型
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(DictTypeId))]
    public SysDictType DictType { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    [SugarColumn(ColumnDescription = "值", Length = 128)]
    [Required, MaxLength(128)]
    public virtual string Value { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    [SugarColumn(ColumnDescription = "编码", Length = 256)]
    [Required, MaxLength(256)]
    public virtual string Code { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(ColumnDescription = "名称", Length = 256)]
    [MaxLength(256)]
    public virtual string? Name { get; set; }

    /// <summary>
    /// 显示样式-标签颜色
    /// </summary>
    [SugarColumn(ColumnDescription = "显示样式-标签颜色", Length = 16)]
    [MaxLength(16)]
    public string? TagType { get; set; }

    /// <summary>
    /// 显示样式-Style(控制显示样式)
    /// </summary>
    [SugarColumn(ColumnDescription = "显示样式-Style", Length = 512)]
    [MaxLength(512)]
    public string? StyleSetting { get; set; }

    /// <summary>
    /// 显示样式-Class(控制显示样式)
    /// </summary>
    [SugarColumn(ColumnDescription = "显示样式-Class", Length = 512)]
    [MaxLength(512)]
    public string? ClassSetting { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int OrderNo { get; set; } = 100;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 2048)]
    [MaxLength(2048)]
    public string? Remark { get; set; }

    /// <summary>
    /// 拓展数据(保存业务功能的配置项)
    /// </summary>
    [SugarColumn(ColumnDescription = "拓展数据(保存业务功能的配置项)", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? ExtData { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public StatusEnum Status { get; set; } = StatusEnum.Enable;
}