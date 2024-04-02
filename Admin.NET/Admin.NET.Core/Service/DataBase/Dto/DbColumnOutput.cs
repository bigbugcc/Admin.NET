// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

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