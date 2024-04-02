// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统用户扩展机构服务
/// </summary>
public class SysUserExtOrgService : ITransient
{
    private readonly SqlSugarRepository<SysUserExtOrg> _sysUserExtOrgRep;

    public SysUserExtOrgService(SqlSugarRepository<SysUserExtOrg> sysUserExtOrgRep)
    {
        _sysUserExtOrgRep = sysUserExtOrgRep;
    }

    /// <summary>
    /// 获取用户扩展机构集合
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<SysUserExtOrg>> GetUserExtOrgList(long userId)
    {
        return await _sysUserExtOrgRep.GetListAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// 更新用户扩展机构
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="extOrgList"></param>
    /// <returns></returns>
    public async Task UpdateUserExtOrg(long userId, List<SysUserExtOrg> extOrgList)
    {
        await _sysUserExtOrgRep.DeleteAsync(u => u.UserId == userId);

        if (extOrgList == null || extOrgList.Count < 1) return;
        extOrgList.ForEach(u =>
        {
            u.UserId = userId;
        });
        await _sysUserExtOrgRep.InsertRangeAsync(extOrgList);
    }

    /// <summary>
    /// 根据机构Id集合删除扩展机构
    /// </summary>
    /// <param name="orgIdList"></param>
    /// <returns></returns>
    public async Task DeleteUserExtOrgByOrgIdList(List<long> orgIdList)
    {
        await _sysUserExtOrgRep.DeleteAsync(u => orgIdList.Contains(u.OrgId));
    }

    /// <summary>
    /// 根据用户Id删除扩展机构
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task DeleteUserExtOrgByUserId(long userId)
    {
        await _sysUserExtOrgRep.DeleteAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// 根据机构Id判断是否有用户
    /// </summary>
    /// <param name="orgId"></param>
    /// <returns></returns>
    public async Task<bool> HasUserOrg(long orgId)
    {
        return await _sysUserExtOrgRep.IsAnyAsync(u => u.OrgId == orgId);
    }

    /// <summary>
    /// 根据职位Id判断是否有用户
    /// </summary>
    /// <param name="posId"></param>
    /// <returns></returns>
    public async Task<bool> HasUserPos(long posId)
    {
        return await _sysUserExtOrgRep.IsAnyAsync(u => u.PosId == posId);
    }
}