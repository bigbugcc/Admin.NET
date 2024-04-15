// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统机构服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 470)]
public class SysOrgService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysOrg> _sysOrgRep;
    private readonly SysCacheService _sysCacheService;
    private readonly SysUserExtOrgService _sysUserExtOrgService;
    private readonly SysUserRoleService _sysUserRoleService;
    private readonly SysRoleOrgService _sysRoleOrgService;

    public SysOrgService(UserManager userManager,
        SqlSugarRepository<SysOrg> sysOrgRep,
        SysCacheService sysCacheService,
        SysUserExtOrgService sysUserExtOrgService,
        SysUserRoleService sysUserRoleService,
        SysRoleOrgService sysRoleOrgService)
    {
        _sysOrgRep = sysOrgRep;
        _userManager = userManager;
        _sysCacheService = sysCacheService;
        _sysUserExtOrgService = sysUserExtOrgService;
        _sysUserRoleService = sysUserRoleService;
        _sysRoleOrgService = sysRoleOrgService;
    }

    /// <summary>
    /// 获取机构列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取机构列表")]
    public async Task<List<SysOrg>> GetList([FromQuery] OrgInput input)
    {
        // 获取拥有的机构Id集合
        var userOrgIdList = await GetUserOrgIdList();

        var iSugarQueryable = _sysOrgRep.AsQueryable().OrderBy(u => u.OrderNo);

        // 带条件筛选时返回列表数据
        if (!string.IsNullOrWhiteSpace(input.Name) || !string.IsNullOrWhiteSpace(input.Code) || !string.IsNullOrWhiteSpace(input.Type))
        {
            return await iSugarQueryable.WhereIF(userOrgIdList.Count > 0, u => userOrgIdList.Contains(u.Id))
                .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
                .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code == input.Code)
                .WhereIF(!string.IsNullOrWhiteSpace(input.Type), u => u.Type == input.Type)
                .ToListAsync();
        }

        var orgTree = new List<SysOrg>();
        if (_userManager.SuperAdmin)
        {
            orgTree = await iSugarQueryable.ToTreeAsync(u => u.Children, u => u.Pid, input.Id);
        }
        else
        {
            orgTree = await iSugarQueryable.ToTreeAsync(u => u.Children, u => u.Pid, input.Id, userOrgIdList.Select(d => (object)d).ToArray());
            // 递归禁用没权限的机构（防止用户修改或创建无权的机构和用户）
            HandlerOrgTree(orgTree, userOrgIdList);
        }

        var sysOrg = await _sysOrgRep.GetSingleAsync(u => u.Id == input.Id);
        if (sysOrg != null)
        {
            sysOrg.Children = orgTree;
            orgTree = new List<SysOrg> { sysOrg };
        }
        return orgTree;
    }

    /// <summary>
    /// 递归禁用没权限的机构
    /// </summary>
    /// <param name="orgTree"></param>
    /// <param name="userOrgIdList"></param>
    private static void HandlerOrgTree(List<SysOrg> orgTree, List<long> userOrgIdList)
    {
        foreach (var org in orgTree)
        {
            org.Disabled = !userOrgIdList.Contains(org.Id); // 设置禁用/不可选择
            if (org.Children != null)
                HandlerOrgTree(org.Children, userOrgIdList);
        }
    }

    /// <summary>
    /// 增加机构 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加机构")]
    public async Task<long> AddOrg(AddOrgInput input)
    {
        if (!_userManager.SuperAdmin && input.Pid == 0)
            throw Oops.Oh(ErrorCodeEnum.D2009);

        if (await _sysOrgRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code))
            throw Oops.Oh(ErrorCodeEnum.D2002);

        if (!_userManager.SuperAdmin && input.Pid != 0)
        {
            // 新增机构父Id不是0，则进行权限校验
            var orgIdList = await GetUserOrgIdList();
            // 新增机构的父机构不在自己的数据范围内
            if (orgIdList.Count < 1 || !orgIdList.Contains(input.Pid))
                throw Oops.Oh(ErrorCodeEnum.D2003);
        }

        // 删除与此父机构有关的用户机构缓存
        var pOrg = await _sysOrgRep.GetFirstAsync(u => u.Id == input.Pid);
        if (pOrg != null)
            DeleteAllUserOrgCache(pOrg.Id, pOrg.Pid);
        else if (input.Pid == 0)
            DeleteAllUserOrgCache(0, 0);

        var newOrg = await _sysOrgRep.AsInsertable(input.Adapt<SysOrg>()).ExecuteReturnEntityAsync();
        return newOrg.Id;
    }

    /// <summary>
    /// 批量增加机构
    /// </summary>
    /// <param name="orgs"></param>
    /// <returns></returns>
    [NonAction]
    public async Task BatchAddOrgs(List<SysOrg> orgs)
    {
        DeleteAllUserOrgCache(0, 0);
        await _sysOrgRep.AsDeleteable().ExecuteCommandAsync();
        await _sysOrgRep.AsInsertable(orgs).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新机构 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新机构")]
    public async Task UpdateOrg(UpdateOrgInput input)
    {
        if (!_userManager.SuperAdmin && input.Pid == 0)
            throw Oops.Oh(ErrorCodeEnum.D2009);

        if (input.Pid != 0)
        {
            //var pOrg = await _sysOrgRep.GetFirstAsync(u => u.Id == input.Pid);
            //_ = pOrg ?? throw Oops.Oh(ErrorCodeEnum.D2000);

            // 若父机构发生变化则清空用户机构缓存
            var sysOrg = await _sysOrgRep.GetFirstAsync(u => u.Id == input.Id);
            if (sysOrg != null && sysOrg.Pid != input.Pid)
            {
                // 删除与此机构、新父机构有关的用户机构缓存
                DeleteAllUserOrgCache(sysOrg.Id, input.Pid);
            }
        }
        if (input.Id == input.Pid)
            throw Oops.Oh(ErrorCodeEnum.D2001);

        if (await _sysOrgRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code && u.Id != input.Id))
            throw Oops.Oh(ErrorCodeEnum.D2002);

        // 父Id不能为自己的子节点
        var childIdList = await GetChildIdListWithSelfById(input.Id);
        if (childIdList.Contains(input.Pid))
            throw Oops.Oh(ErrorCodeEnum.D2001);

        // 是否有权限操作此机构
        if (!_userManager.SuperAdmin)
        {
            var orgIdList = await GetUserOrgIdList();
            if (orgIdList.Count < 1 || !orgIdList.Contains(input.Id))
                throw Oops.Oh(ErrorCodeEnum.D2003);
        }

        await _sysOrgRep.AsUpdateable(input.Adapt<SysOrg>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除机构 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除机构")]
    public async Task DeleteOrg(DeleteOrgInput input)
    {
        var sysOrg = await _sysOrgRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);

        // 是否有权限操作此机构
        if (!_userManager.SuperAdmin)
        {
            var orgIdList = await GetUserOrgIdList();
            if (orgIdList.Count < 1 || !orgIdList.Contains(sysOrg.Id))
                throw Oops.Oh(ErrorCodeEnum.D2003);
        }

        // 若机构为租户默认机构禁止删除
        var isTenantOrg = await _sysOrgRep.ChangeRepository<SqlSugarRepository<SysTenant>>()
            .IsAnyAsync(u => u.OrgId == input.Id);
        if (isTenantOrg)
            throw Oops.Oh(ErrorCodeEnum.D2008);

        // 若机构有用户则禁止删除
        var orgHasEmp = await _sysOrgRep.ChangeRepository<SqlSugarRepository<SysUser>>()
            .IsAnyAsync(u => u.OrgId == input.Id);
        if (orgHasEmp)
            throw Oops.Oh(ErrorCodeEnum.D2004);

        // 若扩展机构有用户则禁止删除
        var hasExtOrgEmp = await _sysUserExtOrgService.HasUserOrg(sysOrg.Id);
        if (hasExtOrgEmp)
            throw Oops.Oh(ErrorCodeEnum.D2005);

        // 若子机构有用户则禁止删除
        var childOrgTreeList = await _sysOrgRep.AsQueryable().ToChildListAsync(u => u.Pid, input.Id, true);
        var childOrgIdList = childOrgTreeList.Select(u => u.Id).ToList();

        // 若子机构有用户则禁止删除
        var cOrgHasEmp = await _sysOrgRep.ChangeRepository<SqlSugarRepository<SysUser>>()
            .IsAnyAsync(u => childOrgIdList.Contains(u.OrgId));
        if (cOrgHasEmp)
            throw Oops.Oh(ErrorCodeEnum.D2007);

        // 删除与此机构、父机构有关的用户机构缓存
        DeleteAllUserOrgCache(sysOrg.Id, sysOrg.Pid);

        // 级联删除机构子节点
        await _sysOrgRep.DeleteAsync(u => childOrgIdList.Contains(u.Id));

        // 级联删除角色机构数据
        await _sysRoleOrgService.DeleteRoleOrgByOrgIdList(childOrgIdList);

        // 级联删除用户机构数据
        await _sysUserExtOrgService.DeleteUserExtOrgByOrgIdList(childOrgIdList);
    }

    /// <summary>
    /// 删除与此机构、父机构有关的用户机构缓存
    /// </summary>
    /// <param name="orgId"></param>
    /// <param name="orgPid"></param>
    private void DeleteAllUserOrgCache(long orgId, long orgPid)
    {
        var userOrgKeyList = _sysCacheService.GetKeysByPrefixKey(CacheConst.KeyUserOrg);
        if (userOrgKeyList != null && userOrgKeyList.Count > 0)
        {
            foreach (var userOrgKey in userOrgKeyList)
            {
                var userOrgs = _sysCacheService.Get<List<long>>(userOrgKey);
                var userId = long.Parse(userOrgKey.Substring(CacheConst.KeyUserOrg));
                if (userOrgs.Contains(orgId) || userOrgs.Contains(orgPid))
                {
                    SqlSugarFilter.DeleteUserOrgCache(userId, _sysOrgRep.Context.CurrentConnectionConfig.ConfigId.ToString());
                }
                if (orgPid == 0)
                {
                    var dataScope = _sysCacheService.Get<int>($"{CacheConst.KeyRoleMaxDataScope}{userId}");
                    if (dataScope == (int)DataScopeEnum.All)
                    {
                        SqlSugarFilter.DeleteUserOrgCache(userId, _sysOrgRep.Context.CurrentConnectionConfig.ConfigId.ToString());
                    }
                }
            }
        }
    }

    /// <summary>
    /// 根据用户Id获取机构Id集合
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<long>> GetUserOrgIdList()
    {
        if (_userManager.SuperAdmin)
            return new List<long>();

        var userId = _userManager.UserId;
        var orgIdList = _sysCacheService.Get<List<long>>($"{CacheConst.KeyUserOrg}{userId}"); // 取缓存
        if (orgIdList == null || orgIdList.Count < 1)
        {
            // 本人创建机构集合
            var orgList0 = await _sysOrgRep.AsQueryable().Where(u => u.CreateUserId == userId).Select(u => u.Id).ToListAsync();
            // 扩展机构集合
            var orgList1 = await _sysUserExtOrgService.GetUserExtOrgList(userId);
            // 角色机构集合
            var orgList2 = await GetUserRoleOrgIdList(userId);
            // 机构并集
            orgIdList = orgList1.Select(u => u.OrgId).Union(orgList2).Union(orgList0).ToList();
            // 当前所属机构
            if (!orgIdList.Contains(_userManager.OrgId))
                orgIdList.Add(_userManager.OrgId);
            _sysCacheService.Set($"{CacheConst.KeyUserOrg}{userId}", orgIdList, TimeSpan.FromDays(7)); // 存缓存
        }
        return orgIdList;
    }

    /// <summary>
    /// 获取用户角色机构Id集合
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<List<long>> GetUserRoleOrgIdList(long userId)
    {
        var roleList = await _sysUserRoleService.GetUserRoleList(userId);
        if (roleList.Count < 1)
            return new List<long>(); // 空机构Id集合

        return await GetUserOrgIdList(roleList);
    }

    /// <summary>
    /// 根据角色Id集合获取机构Id集合
    /// </summary>
    /// <param name="roleList"></param>
    /// <returns></returns>
    private async Task<List<long>> GetUserOrgIdList(List<SysRole> roleList)
    {
        // 按最大范围策略设定(若同时拥有ALL和SELF权限，则结果ALL)
        int strongerDataScopeType = (int)DataScopeEnum.Self;

        // 自定义数据范围的角色集合
        var customDataScopeRoleIdList = new List<long>();

        // 数据范围的机构集合
        var dataScopeOrgIdList = new List<long>();

        if (roleList != null && roleList.Count > 0)
        {
            roleList.ForEach(u =>
            {
                if (u.DataScope == DataScopeEnum.Define)
                {
                    customDataScopeRoleIdList.Add(u.Id);
                    strongerDataScopeType = (int)u.DataScope; // 自定义数据权限时也要更新最大范围
                }
                else if ((int)u.DataScope <= strongerDataScopeType)
                {
                    strongerDataScopeType = (int)u.DataScope;
                    // 根据数据范围获取机构集合
                    var orgIds = GetOrgIdListByDataScope(strongerDataScopeType).GetAwaiter().GetResult();
                    dataScopeOrgIdList = dataScopeOrgIdList.Union(orgIds).ToList();
                }
            });
        }

        // 缓存当前用户最大角色数据范围
        _sysCacheService.Set(CacheConst.KeyRoleMaxDataScope + _userManager.UserId, strongerDataScopeType, TimeSpan.FromDays(7));

        // 根据角色集合获取机构集合
        var roleOrgIdList = await _sysRoleOrgService.GetRoleOrgIdList(customDataScopeRoleIdList);

        // 并集机构集合
        return roleOrgIdList.Union(dataScopeOrgIdList).ToList();
    }

    /// <summary>
    /// 根据数据范围获取机构Id集合
    /// </summary>
    /// <param name="dataScope"></param>
    /// <returns></returns>
    private async Task<List<long>> GetOrgIdListByDataScope(int dataScope)
    {
        var orgId = _userManager.OrgId;
        var orgIdList = new List<long>();
        // 若数据范围是全部，则获取所有机构Id集合
        if (dataScope == (int)DataScopeEnum.All)
        {
            orgIdList = await _sysOrgRep.AsQueryable().Select(u => u.Id).ToListAsync();
        }
        // 若数据范围是本部门及以下，则获取本节点和子节点集合
        else if (dataScope == (int)DataScopeEnum.DeptChild)
        {
            orgIdList = await GetChildIdListWithSelfById(orgId);
        }
        // 若数据范围是本部门不含子节点，则直接返回本部门
        else if (dataScope == (int)DataScopeEnum.Dept)
        {
            orgIdList.Add(orgId);
        }
        return orgIdList;
    }

    /// <summary>
    /// 根据节点Id获取子节点Id集合(包含自己)
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<List<long>> GetChildIdListWithSelfById(long pid)
    {
        var orgTreeList = await _sysOrgRep.AsQueryable().ToChildListAsync(u => u.Pid, pid, true);
        return orgTreeList.Select(u => u.Id).ToList();
    }
}