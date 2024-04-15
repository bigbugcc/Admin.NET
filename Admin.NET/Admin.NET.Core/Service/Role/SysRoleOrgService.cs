// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统角色机构服务
/// </summary>
public class SysRoleOrgService : ITransient
{
    private readonly SqlSugarRepository<SysRoleOrg> _sysRoleOrgRep;

    public SysRoleOrgService(SqlSugarRepository<SysRoleOrg> sysRoleOrgRep)
    {
        _sysRoleOrgRep = sysRoleOrgRep;
    }

    /// <summary>
    /// 授权角色机构
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task GrantRoleOrg(RoleOrgInput input)
    {
        await _sysRoleOrgRep.DeleteAsync(u => u.RoleId == input.Id);
        if (input.DataScope == (int)DataScopeEnum.Define)
        {
            var roleOrgs = input.OrgIdList.Select(u => new SysRoleOrg
            {
                RoleId = input.Id,
                OrgId = u
            }).ToList();
            await _sysRoleOrgRep.InsertRangeAsync(roleOrgs);
        }
    }

    /// <summary>
    /// 根据角色Id集合获取角色机构Id集合
    /// </summary>
    /// <param name="roleIdList"></param>
    /// <returns></returns>
    public async Task<List<long>> GetRoleOrgIdList(List<long> roleIdList)
    {
        return await _sysRoleOrgRep.AsQueryable()
            .Where(u => roleIdList.Contains(u.RoleId))
            .Select(u => u.OrgId).ToListAsync();
    }

    /// <summary>
    /// 根据机构Id集合删除角色机构
    /// </summary>
    /// <param name="orgIdList"></param>
    /// <returns></returns>
    public async Task DeleteRoleOrgByOrgIdList(List<long> orgIdList)
    {
        await _sysRoleOrgRep.DeleteAsync(u => orgIdList.Contains(u.OrgId));
    }

    /// <summary>
    /// 根据角色Id删除角色机构
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task DeleteRoleOrgByRoleId(long roleId)
    {
        await _sysRoleOrgRep.DeleteAsync(u => u.RoleId == roleId);
    }
}