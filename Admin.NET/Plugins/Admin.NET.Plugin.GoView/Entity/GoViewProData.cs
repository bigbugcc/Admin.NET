// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Plugin.GoView.Entity;

/// <summary>
/// GoView 项目数据表
/// </summary>
[SugarTable(null, "GoView 项目数据表")]
public class GoViewProData : EntityTenant
{
    /// <summary>
    /// 项目内容
    /// </summary>
    [SugarColumn(ColumnDescription = "项目内容", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? Content { get; set; }

    /// <summary>
    /// 预览图片
    /// </summary>
    [SugarColumn(ColumnDescription = "预览图片", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? IndexImageData { get; set; }
}