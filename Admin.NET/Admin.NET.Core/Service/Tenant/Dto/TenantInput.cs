// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

public class TenantInput : BaseIdInput
{
    /// <summary>
    /// 状态
    /// </summary>
    public StatusEnum Status { get; set; }
}

public class PageTenantInput : BasePageInput
{
    /// <summary>
    /// 名称
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    public virtual string Phone { get; set; }
}

public class AddTenantInput : TenantOutput
{
    /// <summary>
    /// 租户名称
    /// </summary>
    [Required(ErrorMessage = "租户名称不能为空"), MinLength(2, ErrorMessage = "租户名称不能少于2个字符")]
    public override string Name { get; set; }

    /// <summary>
    /// 租管账号
    /// </summary>
    [Required(ErrorMessage = "租管账号不能为空"), MinLength(3, ErrorMessage = "租管账号不能少于3个字符")]
    public override string AdminAccount { get; set; }
}

public class UpdateTenantInput : AddTenantInput
{
}

public class DeleteTenantInput : BaseIdInput
{
}

public class TenantUserInput
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public long UserId { get; set; }
}

public class TenantIdInput
{
    /// <summary>
    /// 租户Id
    /// </summary>
    public long TenantId { get; set; }
}