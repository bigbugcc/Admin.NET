// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

public class DbColumnOutput
{
    public string TableName { get; set; }

    public int TableId { get; set; }

    public string DbColumnName { get; set; }

    public string PropertyName { get; set; }

    public string DataType { get; set; }

    public object PropertyType { get; set; }

    public int Length { get; set; }

    public string ColumnDescription { get; set; }

    public string DefaultValue { get; set; }

    public bool IsNullable { get; set; }

    public bool IsIdentity { get; set; }

    public bool IsPrimarykey { get; set; }

    public object Value { get; set; }

    public int DecimalDigits { get; set; }

    public int Scale { get; set; }

    public bool IsArray { get; set; }

    public bool IsJson { get; set; }

    public bool? IsUnsigned { get; set; }

    public int CreateTableFieldSort { get; set; }

    internal object SqlParameterDbType { get; set; }
}