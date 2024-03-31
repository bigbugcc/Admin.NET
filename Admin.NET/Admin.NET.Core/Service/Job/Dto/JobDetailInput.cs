// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

public class JobDetailInput
{
    /// <summary>
    /// 作业Id
    /// </summary>
    public string JobId { get; set; }
}

public class PageJobDetailInput : BasePageInput
{
    /// <summary>
    /// 作业Id
    /// </summary>
    public string JobId { get; set; }

    /// <summary>
    /// 描述信息
    /// </summary>
    public string Description { get; set; }
}

public class AddJobDetailInput : SysJobDetail
{
    /// <summary>
    /// 作业Id
    /// </summary>
    [Required(ErrorMessage = "作业Id不能为空"), MinLength(2, ErrorMessage = "作业Id不能少于2个字符")]
    public override string JobId { get; set; }
}

public class UpdateJobDetailInput : AddJobDetailInput
{
}

public class DeleteJobDetailInput : JobDetailInput
{
}