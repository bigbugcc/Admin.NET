// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统用户服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 490)]
public class SysUserService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SysOrgService _sysOrgService;
    private readonly SysUserExtOrgService _sysUserExtOrgService;
    private readonly SysUserRoleService _sysUserRoleService;
    private readonly SysConfigService _sysConfigService;
    private readonly SysOnlineUserService _sysOnlineUserService;
    private readonly SysCacheService _sysCacheService;
    private readonly SysUserLdapService _sysUserLdapService;
    private readonly SqlSugarRepository<SysUser> _sysUserRep;

    public SysUserService(UserManager userManager,
        SysOrgService sysOrgService,
        SysUserExtOrgService sysUserExtOrgService,
        SysUserRoleService sysUserRoleService,
        SysConfigService sysConfigService,
        SysOnlineUserService sysOnlineUserService,
        SysCacheService sysCacheService,
        SysUserLdapService sysUserLdapService,
        SqlSugarRepository<SysUser> sysUserRep)
    {
        _userManager = userManager;
        _sysOrgService = sysOrgService;
        _sysUserExtOrgService = sysUserExtOrgService;
        _sysUserRoleService = sysUserRoleService;
        _sysConfigService = sysConfigService;
        _sysOnlineUserService = sysOnlineUserService;
        _sysCacheService = sysCacheService;
        _sysUserLdapService = sysUserLdapService;
        _sysUserRep = sysUserRep;
    }

    /// <summary>
    /// 获取用户分页列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取用户分页列表")]
    public virtual async Task<SqlSugarPagedList<UserOutput>> Page(PageUserInput input)
    {
        // 获取用户拥有的机构集合
        var userOrgIdList = await _sysOrgService.GetUserOrgIdList();
        List<long> orgList = null;
        if (input.OrgId > 0) // 指定机构查询时
        {
            orgList = await _sysOrgService.GetChildIdListWithSelfById(input.OrgId);
            orgList = _userManager.SuperAdmin ? orgList : orgList.Where(u => userOrgIdList.Contains(u)).ToList();
        }
        else // 各管理员只能看到自己机构下的用户列表
        {
            orgList = _userManager.SuperAdmin ? null : userOrgIdList;
        }

        return await _sysUserRep.AsQueryable()
            .LeftJoin<SysOrg>((u, a) => u.OrgId == a.Id)
            .LeftJoin<SysPos>((u, a, b) => u.PosId == b.Id)
            .Where(u => u.AccountType != AccountTypeEnum.SuperAdmin)
            .WhereIF(orgList != null, u => orgList.Contains(u.OrgId))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Account), u => u.Account.Contains(input.Account))
            .WhereIF(!string.IsNullOrWhiteSpace(input.RealName), u => u.RealName.Contains(input.RealName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.PosName), (u, a, b) => b.Name.Contains(input.PosName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Phone), u => u.Phone.Contains(input.Phone))
            .OrderBy(u => new { u.OrderNo, u.Id })
            .Select((u, a, b) => new UserOutput
            {
                OrgName = a.Name,
                PosName = b.Name,
                RoleName = SqlFunc.Subqueryable<SysUserRole>().LeftJoin<SysRole>((m, n) => m.RoleId == n.Id).Where(m => m.UserId == u.Id).SelectStringJoin((m, n) => n.Name, ","),
                DomainAccount = SqlFunc.Subqueryable<SysUserLdap>().Where(m => m.UserId == u.Id).Select(m => m.Account)
            }, true)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 增加用户 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加用户")]
    public virtual async Task<long> AddUser(AddUserInput input)
    {
        var isExist = await _sysUserRep.AsQueryable().ClearFilter().AnyAsync(u => u.Account == input.Account);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D1003);

        var password = await _sysConfigService.GetConfigValue<string>(ConfigConst.SysPassword);

        var user = input.Adapt<SysUser>();
        user.Password = CryptogramUtil.Encrypt(password);
        var newUser = await _sysUserRep.AsInsertable(user).ExecuteReturnEntityAsync();

        input.Id = newUser.Id;
        await UpdateRoleAndExtOrg(input);

        // 增加域账号
        if (!string.IsNullOrWhiteSpace(input.DomainAccount))
            await _sysUserLdapService.AddUserLdap(newUser.TenantId.Value, newUser.Id, newUser.Account, input.DomainAccount);

        return newUser.Id;
    }

    /// <summary>
    /// 更新用户 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新用户")]
    public virtual async Task UpdateUser(UpdateUserInput input)
    {
        if (await _sysUserRep.AsQueryable().ClearFilter().AnyAsync(u => u.Account == input.Account && u.Id != input.Id))
            throw Oops.Oh(ErrorCodeEnum.D1003);

        await _sysUserRep.AsUpdateable(input.Adapt<SysUser>()).IgnoreColumns(true)
            .IgnoreColumns(u => new { u.Password, u.Status }).ExecuteCommandAsync();

        await UpdateRoleAndExtOrg(input);

        // 删除用户机构缓存
        SqlSugarFilter.DeleteUserOrgCache(input.Id, _sysUserRep.Context.CurrentConnectionConfig.ConfigId.ToString());

        // 若账号的角色和组织架构发生变化,则强制下线账号进行权限更新
        var user = await _sysUserRep.AsQueryable().ClearFilter().FirstAsync(u => u.Id == input.Id);
        var roleIds = await GetOwnRoleList(input.Id);
        if (input.OrgId != user.OrgId || !input.RoleIdList.OrderBy(u => u).SequenceEqual(roleIds.OrderBy(u => u)))
            await _sysOnlineUserService.ForceOffline(input.Id);
        // 更新域账号
        await _sysUserLdapService.AddUserLdap(user.TenantId.Value, user.Id, user.Account, input.DomainAccount);
    }

    /// <summary>
    /// 更新角色和扩展机构
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private async Task UpdateRoleAndExtOrg(AddUserInput input)
    {
        await GrantRole(new UserRoleInput { UserId = input.Id, RoleIdList = input.RoleIdList });

        await _sysUserExtOrgService.UpdateUserExtOrg(input.Id, input.ExtOrgIdList);
    }

    /// <summary>
    /// 删除用户 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除用户")]
    public virtual async Task DeleteUser(DeleteUserInput input)
    {
        var user = await _sysUserRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D0009);
        user.ValidateIsSuperAdminAccountType();
        user.ValidateIsUserId(_userManager.UserId);

        // 若账号为租户默认账号则禁止删除
        var isTenantUser = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysTenant>>().IsAnyAsync(u => u.UserId == input.Id);
        if (isTenantUser)
            throw Oops.Oh(ErrorCodeEnum.D1029);

        // 若账号为开放接口绑定账号则禁止删除
        var isOpenAccessUser = await _sysUserRep.ChangeRepository<SqlSugarRepository<SysOpenAccess>>().IsAnyAsync(u => u.BindUserId == input.Id);
        if (isOpenAccessUser)
            throw Oops.Oh(ErrorCodeEnum.D1030);

        // 强制下线
        await _sysOnlineUserService.ForceOffline(user.Id);

        await _sysUserRep.DeleteAsync(user);

        // 删除用户角色
        await _sysUserRoleService.DeleteUserRoleByUserId(input.Id);

        // 删除用户扩展机构
        await _sysUserExtOrgService.DeleteUserExtOrgByUserId(input.Id);

        // 删除域账号
        await _sysUserLdapService.DeleteUserLdapByUserId(input.Id);
    }

    /// <summary>
    /// 查看用户基本信息 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("查看用户基本信息")]
    public virtual async Task<SysUser> GetBaseInfo()
    {
        return await _sysUserRep.GetByIdAsync(_userManager.UserId);
    }

    /// <summary>
    /// 更新用户基本信息 🔖
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "BaseInfo"), HttpPost]
    [DisplayName("更新用户基本信息")]
    public virtual async Task<int> UpdateBaseInfo(SysUser user)
    {
        return await _sysUserRep.AsUpdateable(user)
            .IgnoreColumns(u => new { u.CreateTime, u.Account, u.Password, u.AccountType, u.OrgId, u.PosId }).ExecuteCommandAsync();
    }

    /// <summary>
    /// 设置用户状态 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("设置用户状态")]
    public virtual async Task<int> SetStatus(UserInput input)
    {
        if (_userManager.UserId == input.Id)
            throw Oops.Oh(ErrorCodeEnum.D1026);

        var user = await _sysUserRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D0009);
        user.ValidateIsSuperAdminAccountType(ErrorCodeEnum.D1015);
        if (!Enum.IsDefined(typeof(StatusEnum), input.Status))
            throw Oops.Oh(ErrorCodeEnum.D3005);

        // 账号禁用则增加黑名单，账号启用则移除黑名单
        var sysCacheService = App.GetRequiredService<SysCacheService>();
        if (input.Status == StatusEnum.Disable)
        {
            sysCacheService.Set($"{CacheConst.KeyBlacklist}{user.Id}", $"{user.RealName}-{user.Phone}");

            // 强制下线
            await _sysOnlineUserService.ForceOffline(user.Id);
        }
        else
        {
            sysCacheService.Remove($"{CacheConst.KeyBlacklist}{user.Id}");
        }

        user.Status = input.Status;
        return await _sysUserRep.AsUpdateable(user).UpdateColumns(u => new { u.Status }).ExecuteCommandAsync();
    }

    /// <summary>
    /// 授权用户角色 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("授权用户角色")]
    public async Task GrantRole(UserRoleInput input)
    {
        //var user = await _sysUserRep.GetFirstAsync(u => u.Id == input.UserId) ?? throw Oops.Oh(ErrorCodeEnum.D0009);
        //if (user.AccountType == AccountTypeEnum.SuperAdmin)
        //    throw Oops.Oh(ErrorCodeEnum.D1022);

        await _sysUserRoleService.GrantUserRole(input);
    }

    /// <summary>
    /// 修改用户密码 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("修改用户密码")]
    public virtual async Task<int> ChangePwd(ChangePwdInput input)
    {
        // 国密SM2解密（前端密码传输SM2加密后的）
        input.PasswordOld = CryptogramUtil.SM2Decrypt(input.PasswordOld);
        input.PasswordNew = CryptogramUtil.SM2Decrypt(input.PasswordNew);

        var user = await _sysUserRep.GetByIdAsync(_userManager.UserId) ?? throw Oops.Oh(ErrorCodeEnum.D0009);
        if (CryptogramUtil.CryptoType == CryptogramEnum.MD5.ToString())
        {
            if (user.Password != MD5Encryption.Encrypt(input.PasswordOld))
                throw Oops.Oh(ErrorCodeEnum.D1004);
        }
        else
        {
            if (CryptogramUtil.Decrypt(user.Password) != input.PasswordOld)
                throw Oops.Oh(ErrorCodeEnum.D1004);
        }

        if (input.PasswordOld == input.PasswordNew)
            throw Oops.Oh(ErrorCodeEnum.D1028);

        // 验证密码强度
        if (CryptogramUtil.StrongPassword)
        {
            user.Password = input.PasswordNew.TryValidate(CryptogramUtil.PasswordStrengthValidation)
                ? CryptogramUtil.Encrypt(input.PasswordNew)
                : throw Oops.Oh(CryptogramUtil.PasswordStrengthValidationMsg);
        }
        else
        {
            user.Password = CryptogramUtil.Encrypt(input.PasswordNew);
        }

        return await _sysUserRep.AsUpdateable(user).UpdateColumns(u => u.Password).ExecuteCommandAsync();
    }

    /// <summary>
    /// 重置用户密码 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("重置用户密码")]
    public virtual async Task<string> ResetPwd(ResetPwdUserInput input)
    {
        var user = await _sysUserRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D0009);
        var password = await _sysConfigService.GetConfigValue<string>(ConfigConst.SysPassword);
        user.Password = CryptogramUtil.Encrypt(password);
        await _sysUserRep.AsUpdateable(user).UpdateColumns(u => u.Password).ExecuteCommandAsync();

        // 清空密码错误次数
        var keyErrorPasswordCount = $"{CacheConst.KeyPasswordErrorTimes}{user.Account}";
        _sysCacheService.Remove(keyErrorPasswordCount);

        return password;
    }

    /// <summary>
    /// 解除登录锁定 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("解除登录锁定")]
    public virtual async Task UnlockLogin(UnlockLoginInput input)
    {
        var user = await _sysUserRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D0009);

        // 清空密码错误次数
        var keyPasswordErrorTimes = $"{CacheConst.KeyPasswordErrorTimes}{user.Account}";
        _sysCacheService.Remove(keyPasswordErrorTimes);
    }

    /// <summary>
    /// 获取用户拥有角色集合 🔖
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [DisplayName("获取用户拥有角色集合")]
    public async Task<List<long>> GetOwnRoleList(long userId)
    {
        return await _sysUserRoleService.GetUserRoleIdList(userId);
    }

    /// <summary>
    /// 获取用户扩展机构集合 🔖
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [DisplayName("获取用户扩展机构集合")]
    public async Task<List<SysUserExtOrg>> GetOwnExtOrgList(long userId)
    {
        return await _sysUserExtOrgService.GetUserExtOrgList(userId);
    }
}