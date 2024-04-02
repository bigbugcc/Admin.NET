// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Service;

public class RoleInput : BaseIdInput
{
    /// <summary>
    /// 状态
    /// </summary>
    public virtual StatusEnum Status { get; set; }
}

public class PageRoleInput : BasePageInput
{
    /// <summary>
    /// 名称
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    public virtual string Code { get; set; }
}

public class AddRoleInput : SysRole
{
    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "角色名称不能为空")]
    public override string Name { get; set; }

    /// <summary>
    /// 菜单Id集合
    /// </summary>
    public List<long> MenuIdList { get; set; }
}

public class UpdateRoleInput : AddRoleInput
{
}

public class DeleteRoleInput : BaseIdInput
{
}