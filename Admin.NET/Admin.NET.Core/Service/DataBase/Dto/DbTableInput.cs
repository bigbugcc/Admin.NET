// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

public class DbTableInput
{
    public string ConfigId { get; set; }

    public string TableName { get; set; }

    public string Description { get; set; }

    public List<DbColumnInput> DbColumnInfoList { get; set; }
}

public class UpdateDbTableInput
{
    public string ConfigId { get; set; }

    public string TableName { get; set; }

    public string OldTableName { get; set; }

    public string Description { get; set; }
}

public class DeleteDbTableInput
{
    public string ConfigId { get; set; }

    public string TableName { get; set; }
}