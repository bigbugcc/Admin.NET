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

    /// <summary>
    /// 系统主标题
    /// </summary>
    [CommonValidation("!string.IsNullOrWhiteSpace(Host) && string.IsNullOrWhiteSpace(Title)", "系统主标题不能为空")]
    public override string Title { get; set; }

    /// <summary>
    /// 系统副标题
    /// </summary>
    [CommonValidation("!string.IsNullOrWhiteSpace(Host) && string.IsNullOrWhiteSpace(ViceTitle)", "系统副标题不能为空")]
    public override string ViceTitle { get; set; }

    /// <summary>
    /// 系统描述
    /// </summary>
    [CommonValidation("!string.IsNullOrWhiteSpace(Host) && string.IsNullOrWhiteSpace(ViceDesc)", "系统描述不能为空")]
    public override string ViceDesc { get; set; }

    /// <summary>
    /// 版权说明
    /// </summary>
    [CommonValidation("!string.IsNullOrWhiteSpace(Host) && string.IsNullOrWhiteSpace(Copyright)", "版权说明不能为空")]
    public override string Copyright { get; set; }

    /// <summary>
    /// ICP备案号
    /// </summary>
    public override string Icp { get; set; }

    /// <summary>
    /// ICP地址
    /// </summary>
    [CommonValidation("!string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(Icp) && string.IsNullOrWhiteSpace(IcpUrl)", "ICP地址不能为空")]
    public override string IcpUrl { get; set; }

    /// <summary>
    /// Logo图片Base64码
    /// </summary>
    [CommonValidation("!string.IsNullOrWhiteSpace(Host) && string.IsNullOrWhiteSpace(Logo) && string.IsNullOrWhiteSpace(LogoBase64)", "图标不能为空")]
    public virtual string LogoBase64 { get; set; }

    /// <summary>
    /// Logo文件名
    /// </summary>
    public virtual string LogoFileName { get; set; }
}

public class UpdateTenantInput : AddTenantInput
{
}

public class DeleteTenantInput : BaseIdInput
{
}

/// <summary>
/// 租户菜单
/// </summary>
public class TenantMenuInput : BaseIdInput
{
    /// <summary>
    /// 同步租户Id集合
    /// </summary>
    public List<long> TenantIdList { get; set; }

    /// <summary>
    /// 菜单Id集合
    /// </summary>
    public List<long> MenuIdList { get; set; }
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