// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

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