// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

public class OrgInput : BaseIdInput
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 机构类型
    /// </summary>
    public string Type { get; set; }
}

public class AddOrgInput : SysOrg
{
    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "机构名称不能为空")]
    public override string Name { get; set; }
}

public class UpdateOrgInput : AddOrgInput
{
}

public class DeleteOrgInput : BaseIdInput
{
}