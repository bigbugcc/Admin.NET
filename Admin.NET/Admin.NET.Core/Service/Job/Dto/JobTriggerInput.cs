// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

public class JobTriggerInput
{
    /// <summary>
    /// 作业Id
    /// </summary>
    public string JobId { get; set; }

    /// <summary>
    /// 触发器Id
    /// </summary>
    public string TriggerId { get; set; }
}

public class AddJobTriggerInput : SysJobTrigger
{
    /// <summary>
    /// 作业Id
    /// </summary>
    [Required(ErrorMessage = "作业Id不能为空"), MinLength(2, ErrorMessage = "作业Id不能少于2个字符")]
    public override string JobId { get; set; }

    /// <summary>
    /// 触发器Id
    /// </summary>
    [Required(ErrorMessage = "触发器Id不能为空"), MinLength(2, ErrorMessage = "触发器Id不能少于2个字符")]
    public override string TriggerId { get; set; }
}

public class UpdateJobTriggerInput : AddJobTriggerInput
{
}

public class DeleteJobTriggerInput : JobTriggerInput
{
}