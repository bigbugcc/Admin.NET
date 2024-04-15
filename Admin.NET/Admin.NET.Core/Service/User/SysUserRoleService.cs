// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统用户角色服务
/// </summary>
public class SysUserRoleService : ITransient
{
    private readonly SqlSugarRepository<SysUserRole> _sysUserRoleRep;
    private readonly SysCacheService _sysCacheService;

    public SysUserRoleService(SqlSugarRepository<SysUserRole> sysUserRoleRep,
        SysCacheService sysCacheService)
    {
        _sysUserRoleRep = sysUserRoleRep;
        _sysCacheService = sysCacheService;
    }

    /// <summary>
    /// 授权用户角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task GrantUserRole(UserRoleInput input)
    {
        await _sysUserRoleRep.DeleteAsync(u => u.UserId == input.UserId);

        if (input.RoleIdList == null || input.RoleIdList.Count < 1) return;
        var roles = input.RoleIdList.Select(u => new SysUserRole
        {
            UserId = input.UserId,
            RoleId = u
        }).ToList();
        await _sysUserRoleRep.InsertRangeAsync(roles);
        _sysCacheService.Remove(CacheConst.KeyUserButton + input.UserId);
    }

    /// <summary>
    /// 根据角色Id删除用户角色
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task DeleteUserRoleByRoleId(long roleId)
    {
        await _sysUserRoleRep.AsQueryable()
             .Where(u => u.RoleId == roleId)
             .Select(u => u.UserId)
             .ForEachAsync(userId =>
             {
                 _sysCacheService.Remove(CacheConst.KeyUserButton + userId);
             });

        await _sysUserRoleRep.DeleteAsync(u => u.RoleId == roleId);
    }

    /// <summary>
    /// 根据用户Id删除用户角色
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task DeleteUserRoleByUserId(long userId)
    {
        await _sysUserRoleRep.DeleteAsync(u => u.UserId == userId);
        _sysCacheService.Remove(CacheConst.KeyUserButton + userId);
    }

    /// <summary>
    /// 根据用户Id获取角色集合
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<SysRole>> GetUserRoleList(long userId)
    {
        var sysUserRoleList = await _sysUserRoleRep.AsQueryable()
            .Includes(u => u.SysRole)
            .Where(u => u.UserId == userId).ToListAsync();
        return sysUserRoleList.Where(u => u.SysRole != null).Select(u => u.SysRole).ToList();
    }

    /// <summary>
    /// 根据用户Id获取角色Id集合
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<long>> GetUserRoleIdList(long userId)
    {
        return await _sysUserRoleRep.AsQueryable()
            .Where(u => u.UserId == userId).Select(u => u.RoleId).ToListAsync();
    }

    /// <summary>
    /// 根据角色Id获取用户Id集合
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task<List<long>> GetUserIdList(long roleId)
    {
        return await _sysUserRoleRep.AsQueryable()
            .Where(u => u.RoleId == roleId).Select(u => u.UserId).ToListAsync();
    }
}