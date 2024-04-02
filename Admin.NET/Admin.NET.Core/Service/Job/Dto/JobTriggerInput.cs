// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

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