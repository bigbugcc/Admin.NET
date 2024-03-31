// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

/// <summary>
/// 数据库表列
/// </summary>
public class ColumnOuput
{
    /// <summary>
    /// 字段名
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    /// 实体的Property名
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// 字段数据长度
    /// </summary>
    public int ColumnLength { get; set; }

    /// <summary>
    /// 数据库中类型
    /// </summary>
    public string DataType { get; set; }

    /// <summary>
    /// 是否为主键
    /// </summary>
    public bool IsPrimarykey { get; set; }

    /// <summary>
    /// 是否允许为空
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    /// .NET字段类型
    /// </summary>
    public string NetType { get; set; }

    /// <summary>
    /// 字段描述
    /// </summary>
    public string ColumnComment { get; set; }

    /// <summary>
    /// 主外键
    /// </summary>
    public string ColumnKey { get; set; }
}