// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

/// <summary>
/// 开放接口身份输入参数
/// </summary>
public class OpenAccessInput : BasePageInput
{
    /// <summary>
    /// 身份标识
    /// </summary>
    public string AccessKey { get; set; }
}

public class AddOpenAccessInput : SysOpenAccess
{
    /// <summary>
    /// 身份标识
    /// </summary>
    [Required(ErrorMessage = "身份标识不能为空")]
    public override string AccessKey { get; set; }

    /// <summary>
    /// 密钥
    /// </summary>
    [Required(ErrorMessage = "密钥不能为空")]
    public override string AccessSecret { get; set; }

    /// <summary>
    /// 绑定用户Id
    /// </summary>
    [Required(ErrorMessage = "绑定用户不能为空")]
    public override long BindUserId { get; set; }
}

public class UpdateOpenAccessInput : AddOpenAccessInput
{
}

public class DeleteOpenAccessInput : BaseIdInput
{
}