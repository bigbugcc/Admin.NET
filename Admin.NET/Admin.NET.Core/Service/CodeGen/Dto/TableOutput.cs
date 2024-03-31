// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

/// <summary>
/// 数据库表
/// </summary>
public class TableOutput
{
    /// <summary>
    /// 库定位器名
    /// </summary>
    public string ConfigId { get; set; }

    /// <summary>
    /// 表名（字母形式的）
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// 实体名称
    /// </summary>
    public string EntityName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public string UpdateTime { get; set; }

    /// <summary>
    /// 表名称描述（功能名）
    /// </summary>
    public string TableComment { get; set; }
}