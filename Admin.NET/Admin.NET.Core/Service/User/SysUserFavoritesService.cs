// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 用户菜单快捷导航服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 500)]
public class SysUserFavoritesService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysUserFavorites> _sysUserFavoritesRep;

    public SysUserFavoritesService(SqlSugarRepository<SysUserFavorites> sysUserFavoritesRep)
    {
        _sysUserFavoritesRep = sysUserFavoritesRep;
    }

    /// <summary>
    /// 收藏菜单 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    public async Task GrantUserFavorites(UserFavoritesInput input)
    {
        await _sysUserFavoritesRep.DeleteAsync(u => u.UserId == input.UserId);

        if (input.MenuIdList == null || input.MenuIdList.Count < 1) return;
        var menus = input.MenuIdList.Select(u => new SysUserFavorites
        {
            UserId = input.UserId,
            MenuId = u
        }).ToList();
        await _sysUserFavoritesRep.InsertRangeAsync(menus);
    }

    /// <summary>
    /// 取消收藏菜单 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task DeleteUserFavorites(UserFavoritesInput input)
    {
        await _sysUserFavoritesRep.DeleteAsync(u => u.UserId == input.UserId && input.MenuIdList.Contains(u.MenuId));
    }

    /// <summary>
    /// 根据用户Id删除收藏菜单 🔖
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task DeleteUserRoleByUserId(long userId)
    {
        await _sysUserFavoritesRep.DeleteAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// 根据用户Id获取收藏菜单集合 🔖
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<MenuOutput>> GetUserRoleList(long userId)
    {
        var sysUserRoleList = await _sysUserFavoritesRep.AsQueryable()
            .Includes(u => u.SysMenu)
            .Where(u => u.UserId == userId).ToListAsync();
        return sysUserRoleList.Where(u => u.SysMenu != null).Select(u => u.SysMenu).ToList().Adapt<List<MenuOutput>>();
    }

    /// <summary>
    /// 根据用户Id获取收藏菜单Id集合 🔖
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<long>> GetUserRoleIdList(long userId)
    {
        return await _sysUserFavoritesRep.AsQueryable()
            .Where(u => u.UserId == userId).Select(u => u.MenuId).ToListAsync();
    }
}