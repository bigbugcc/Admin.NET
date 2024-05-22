namespace Admin.NET.Core.Service;

/// <summary>
/// 用户菜单快捷导航服务
/// </summary>
[ApiDescriptionSettings(Order = 500)]
public class SysUserFavoritesService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysUserFavorites> _sysUserFavoritesRep;
    private readonly SysCacheService _sysCacheService;

    public SysUserFavoritesService(SqlSugarRepository<SysUserFavorites> sysUserFavoritesRep,
        SysCacheService sysCacheService)
    {
        _sysUserFavoritesRep = sysUserFavoritesRep;
        _sysCacheService = sysCacheService;
    }

    /// <summary>
    /// 收藏菜单
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
    /// 取消收藏菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task DeleteUserFavorites(UserFavoritesInput input)
    {
        await _sysUserFavoritesRep.DeleteAsync(u => u.UserId == input.UserId && input.MenuIdList.Contains(u.MenuId));
    }

    /// <summary>
    /// 根据用户Id删除收藏菜单
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task DeleteUserRoleByUserId(long userId)
    {
        await _sysUserFavoritesRep.DeleteAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// 根据用户Id获取收藏菜单集合
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
    /// 根据用户Id获取收藏菜单Id集合
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<long>> GetUserRoleIdList(long userId)
    {
        return await _sysUserFavoritesRep.AsQueryable()
            .Where(u => u.UserId == userId).Select(u => u.MenuId).ToListAsync();
    }
}