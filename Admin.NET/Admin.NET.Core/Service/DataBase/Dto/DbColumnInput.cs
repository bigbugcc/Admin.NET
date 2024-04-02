// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

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