// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

public class ConfigInput : BaseIdInput
{
}

public class PageConfigInput : BasePageInput
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
    /// 分组编码
    /// </summary>
    public string GroupCode { get; set; }
}

public class AddConfigInput : SysConfig
{
}

public class UpdateConfigInput : AddConfigInput
{
}

public class DeleteConfigInput : BaseIdInput
{
}