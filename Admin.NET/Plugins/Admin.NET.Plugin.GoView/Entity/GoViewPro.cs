﻿// 大名科技（天津）有限公司版权所有  电话：18020030720  QQ：515096995
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证

namespace Admin.NET.Plugin.GoView.Entity;

/// <summary>
/// GoView 项目表
/// </summary>
[SugarTable(null, "GoView 项目表")]
public class GoViewPro : EntityTenant
{
    /// <summary>
    /// 项目名称
    /// </summary>
    [SugarColumn(ColumnDescription = "项目名称", Length = 64)]
    [Required, MaxLength(64)]
    public string ProjectName { get; set; }

    /// <summary>
    /// 项目状态
    /// </summary>
    [SugarColumn(ColumnDescription = "项目状态")]
    public GoViewProState State { get; set; } = GoViewProState.UnPublish;

    /// <summary>
    /// 预览图片Url
    /// </summary>
    [SugarColumn(ColumnDescription = "预览图片Url", Length = 1024)]
    [MaxLength(1024)]
    public string? IndexImage { get; set; }

    /// <summary>
    /// 项目备注
    /// </summary>
    [SugarColumn(ColumnDescription = "项目备注", Length = 512)]
    [MaxLength(512)]
    public string? Remarks { get; set; }

    ///// <summary>
    ///// 项目数据
    ///// </summary>
    //[Newtonsoft.Json.JsonIgnore]
    //[System.Text.Json.Serialization.JsonIgnore]
    //[Navigate(NavigateType.OneToOne, nameof(Id))]
    //public GoViewProData GoViewProData { get; set; }
}