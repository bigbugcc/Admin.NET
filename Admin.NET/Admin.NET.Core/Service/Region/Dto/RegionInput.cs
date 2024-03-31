// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

public class PageRegionInput : BasePageInput
{
    /// <summary>
    /// 父节点Id
    /// </summary>
    public long Pid { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    public string Code { get; set; }
}

public class RegionInput : BaseIdInput
{
}

public class AddRegionInput : SysRegion
{
    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "名称不能为空")]
    public override string Name { get; set; }
}

public class UpdateRegionInput : AddRegionInput
{
}

public class DeleteRegionInput : BaseIdInput
{
}