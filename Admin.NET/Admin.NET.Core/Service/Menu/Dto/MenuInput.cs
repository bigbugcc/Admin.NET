// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Service;

public class MenuInput
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 菜单类型（1目录 2菜单 3按钮）
    /// </summary>
    public MenuTypeEnum? Type { get; set; }
}

public class AddMenuInput : SysMenu
{
    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "菜单名称不能为空")]
    public override string Title { get; set; }
}

public class UpdateMenuInput : AddMenuInput
{
}

public class DeleteMenuInput : BaseIdInput
{
}