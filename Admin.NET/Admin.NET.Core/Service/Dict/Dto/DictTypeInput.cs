// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Service;

public class DictTypeInput : BaseIdInput
{
    /// <summary>
    /// 状态
    /// </summary>
    public StatusEnum Status { get; set; }
}

public class PageDictTypeInput : BasePageInput
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    public string Code { get; set; }
}

public class AddDictTypeInput : SysDictType
{
}

public class UpdateDictTypeInput : AddDictTypeInput
{
}

public class DeleteDictTypeInput : BaseIdInput
{
}

public class GetDataDictTypeInput
{
    /// <summary>
    /// 编码
    /// </summary>
    [Required(ErrorMessage = "字典类型编码不能为空")]
    public string Code { get; set; }
}