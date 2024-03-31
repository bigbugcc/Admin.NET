// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

public class DbColumnInput
{
    public string ConfigId { get; set; }

    public string TableName { get; set; }

    public string DbColumnName { get; set; }

    public string DataType { get; set; }

    public int Length { get; set; }

    public string ColumnDescription { get; set; }

    public int IsNullable { get; set; }

    public int IsIdentity { get; set; }

    public int IsPrimarykey { get; set; }

    public int DecimalDigits { get; set; }
}

public class UpdateDbColumnInput
{
    public string ConfigId { get; set; }

    public string TableName { get; set; }

    public string ColumnName { get; set; }

    public string OldColumnName { get; set; }

    public string Description { get; set; }
}

public class DeleteDbColumnInput
{
    public string ConfigId { get; set; }

    public string TableName { get; set; }

    public string DbColumnName { get; set; }
}