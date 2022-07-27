﻿namespace Admin.NET.Core;

/// <summary>
/// 系统参数配置表
/// </summary>
[SugarTable("sys_config", "系统参数配置表")]
public class SysConfig : EntityBase
{
    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(ColumnDescription = "名称", Length = 64)]
    [Required, MaxLength(64)]
    public string Name { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    [SugarColumn(ColumnDescription = "编码", Length = 64)]
    [MaxLength(64)]
    public string Code { get; set; }

    /// <summary>
    /// 属性值
    /// </summary>
    [SugarColumn(ColumnDescription = "属性值", Length = 64)]
    [MaxLength(64)]
    public string Value { get; set; }

    /// <summary>
    /// 是否是系统参数（Y-是，N-否）
    /// </summary>
    [SugarColumn(ColumnDescription = "是否是系统参数")]
    public YesNoEnum SysFlag { get; set; }

    /// <summary>
    /// 常量所属分类的编码
    /// </summary>
    [SugarColumn(ColumnDescription = "常量所属分类的编码", Length = 64)]
    [MaxLength(64)]
    public string GroupCode { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int Order { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 256)]
    [MaxLength(256)]
    public string Remark { get; set; }
}