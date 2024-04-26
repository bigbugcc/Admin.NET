// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统行政地区表
/// </summary>
[SugarTable(null, "系统行政地区表")]
[SysTable]
[SugarIndex("index_{table}_N", nameof(Name), OrderByType.Asc)]
[SugarIndex("index_{table}_C", nameof(Code), OrderByType.Asc)]
public partial class SysRegion : EntityBaseId
{
    /// <summary>
    /// 父Id
    /// </summary>
    [SugarColumn(ColumnDescription = "父Id")]
    public long Pid { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(ColumnDescription = "名称", Length = 128)]
    [Required, MaxLength(128)]
    public virtual string Name { get; set; }

    /// <summary>
    /// 简称
    /// </summary>
    [SugarColumn(ColumnDescription = "简称", Length = 32)]
    [MaxLength(32)]
    public string? ShortName { get; set; }

    /// <summary>
    /// 组合名
    /// </summary>
    [SugarColumn(ColumnDescription = "组合名", Length = 64)]
    [MaxLength(64)]
    public string? MergerName { get; set; }

    /// <summary>
    /// 行政代码
    /// </summary>
    [SugarColumn(ColumnDescription = "行政代码", Length = 32)]
    [MaxLength(32)]
    public string? Code { get; set; }

    /// <summary>
    /// 邮政编码
    /// </summary>
    [SugarColumn(ColumnDescription = "邮政编码", Length = 6)]
    [MaxLength(6)]
    public string? ZipCode { get; set; }

    /// <summary>
    /// 区号
    /// </summary>
    [SugarColumn(ColumnDescription = "区号", Length = 6)]
    [MaxLength(6)]
    public string? CityCode { get; set; }

    /// <summary>
    /// 层级
    /// </summary>
    [SugarColumn(ColumnDescription = "层级")]
    public int Level { get; set; }

    /// <summary>
    /// 拼音
    /// </summary>
    [SugarColumn(ColumnDescription = "拼音", Length = 128)]
    [MaxLength(128)]
    public string? PinYin { get; set; }

    /// <summary>
    /// 经度
    /// </summary>
    [SugarColumn(ColumnDescription = "经度")]
    public float Lng { get; set; }

    /// <summary>
    /// 维度
    /// </summary>
    [SugarColumn(ColumnDescription = "维度")]
    public float Lat { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int OrderNo { get; set; } = 100;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 128)]
    [MaxLength(128)]
    public string? Remark { get; set; }

    /// <summary>
    /// 机构子项
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<SysRegion> Children { get; set; }
}