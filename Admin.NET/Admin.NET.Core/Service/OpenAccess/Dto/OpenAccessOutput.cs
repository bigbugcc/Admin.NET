// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

public class OpenAccessOutput : SysOpenAccess
{
    /// <summary>
    /// 绑定用户账号
    /// </summary>
    public string BindUserAccount { get; set; }

    /// <summary>
    /// 绑定租户名称
    /// </summary>
    public string BindTenantName { get; set; }
}