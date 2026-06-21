// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统角色菜单表种子数据
/// </summary>
public class SysRoleMenuSeedData : ISqlSugarEntitySeedData<SysRoleMenu>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysRoleMenu> HasData()
    {
        var roleMenuList = new List<SysRoleMenu>();

        var roleList = new SysRoleSeedData().HasData().ToList();
        var menuList = new SysMenuSeedData().HasData().ToList();
        var defaultMenuList = new SysTenantMenuSeedData().HasData().ToList();

        // 第一个角色拥有全部默认租户菜单
        //roleMenuList.AddRange(defaultMenuList.Select(u => new SysRoleMenu { Id = u.MenuId + (roleList[0].Id % 1300000000000), RoleId = roleList[0].Id, MenuId = u.MenuId }));
        roleMenuList.AddRange(defaultMenuList.Select(u => new SysRoleMenu { Id = CommonUtil.GetFixedHashCode($"{roleList[0].Id}{u.MenuId}", 1300000000000), RoleId = roleList[0].Id, MenuId = u.MenuId }));

        // 其他角色权限：工作台、系统管理、个人中心、帮助文档、关于项目
        var otherRoleMenuList = menuList.ToChildList(u => u.Id, u => u.Pid, u => new[] { "工作台", "帮助文档", "关于项目", "个人中心" }.Contains(u.Title)).ToList();
        otherRoleMenuList.Add(menuList.First(u => u.Type == MenuTypeEnum.Dir && u.Title == "系统管理"));
        //foreach (var role in roleList.Skip(1)) roleMenuList.AddRange(otherRoleMenuList.Select(u => new SysRoleMenu { Id = u.Id + (role.Id % 1300000000000), RoleId = role.Id, MenuId = u.Id }));
        foreach (var role in roleList.Skip(1)) roleMenuList.AddRange(otherRoleMenuList.Select(u => new SysRoleMenu { Id = CommonUtil.GetFixedHashCode($"{role.Id}{u.Id}", 1300000000000), RoleId = role.Id, MenuId = u.Id }));

        return roleMenuList;
    }
}