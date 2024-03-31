// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

public class TenantOutput : SysTenant
{
    /// <summary>
    /// 租户名称
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// 管理员账号
    /// </summary>
    public virtual string AdminAccount { get; set; }

    /// <summary>
    /// 电子邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    public string Phone { get; set; }
}