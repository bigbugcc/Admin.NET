// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统菜单服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 450)]
public class SysMenuService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysTenantMenu> _sysTenantMenuRep;
    private readonly SqlSugarRepository<SysMenu> _sysMenuRep;
    private readonly SysRoleMenuService _sysRoleMenuService;
    private readonly SysUserRoleService _sysUserRoleService;
    private readonly SysUserMenuService _sysUserMenuService;
    private readonly SysCacheService _sysCacheService;
    private readonly UserManager _userManager;
    private readonly SysLangTextCacheService _sysLangTextCacheService;
    private readonly SysLangTextService _sysLangTextService;

    public SysMenuService(
        SqlSugarRepository<SysTenantMenu> sysTenantMenuRep,
        SqlSugarRepository<SysMenu> sysMenuRep,
        SysRoleMenuService sysRoleMenuService,
        SysUserRoleService sysUserRoleService,
        SysUserMenuService sysUserMenuService,
        SysCacheService sysCacheService,
        UserManager userManager,
        SysLangTextCacheService sysLangTextCacheService,
        SysLangTextService sysLangTextService)
    {
        _userManager = userManager;
        _sysMenuRep = sysMenuRep;
        _sysRoleMenuService = sysRoleMenuService;
        _sysUserRoleService = sysUserRoleService;
        _sysUserMenuService = sysUserMenuService;
        _sysTenantMenuRep = sysTenantMenuRep;
        _sysCacheService = sysCacheService;
        _sysLangTextCacheService = sysLangTextCacheService;
        _sysLangTextService = sysLangTextService;
    }

    /// <summary>
    /// 获取登录菜单树 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取登录菜单树")]
    public async Task<List<MenuOutput>> GetLoginMenuTree()
    {
        var sysDefaultLang = App.GetOptions<LocalizationSettingsOptions>().DefaultCulture;
        var langCode = _userManager.LangCode;
        var (query, _) = GetSugarQueryableAndTenantId(_userManager.TenantId);

        // 查询菜单主表（过滤非按钮和禁用）
        var menuQuery = query.Where(u => u.Type != MenuTypeEnum.Btn && u.Status == StatusEnum.Enable);

        if (!(_userManager.SuperAdmin || _userManager.SysAdmin))
        {
            var menuIdList = await GetMenuIdList();
            menuQuery = menuQuery.Where(u => menuIdList.Contains(u.Id));
        }

        // 查询主表（不再 LEFT JOIN）
        var menuList = await menuQuery
            .OrderBy(u => new { u.OrderNo, u.Id })
            .ToListAsync();

        // 仅当用户语言和系统默认语言不同时，才进行翻译，避免不必要的性能开销
        if (langCode != sysDefaultLang)
        {
            // 调用缓存翻译：翻译 Title 字段
            var fields = new List<LangFieldMap<SysMenu>>
            {
                new LangFieldMap<SysMenu>
                {
                    EntityName = "SysMenu",
                    FieldName = "Title",
                    IdSelector = m => m.Id,
                    SetTranslatedValue = (m, val) => m.Title = val
                }
            };
            await _sysLangTextCacheService.TranslateMultiFields(menuList, fields, langCode);
        }

        // 构造树
        var menuTree = menuList.ToTree(
            it => it.Children, it => it.Pid, 0
        );

        // 转换为输出 DTO
        return menuTree.Adapt<List<MenuOutput>>();
    }

    /// <summary>
    /// 获取菜单列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取菜单列表")]
    public async Task<List<SysMenu>> GetList([FromQuery] MenuInput input)
    {
        var langCode = _userManager.LangCode;
        var menuIdList = _userManager.SuperAdmin || _userManager.SysAdmin ? new List<long>() : await GetMenuIdList();
        var (query, _) = GetSugarQueryableAndTenantId(input.TenantId);

        // 有条件直接查询菜单列表（带 Title、Type 过滤）
        if (!string.IsNullOrWhiteSpace(input.Title) || input.Type is > 0)
        {
            var menuList = await query
                .WhereIF(!string.IsNullOrWhiteSpace(input.Title), u => u.Title.Contains(input.Title))
                .WhereIF(input.Type is > 0, u => u.Type == input.Type)
                .WhereIF(menuIdList.Count > 0, u => menuIdList.Contains(u.Id))
                .OrderBy(u => new { u.OrderNo, u.Id })
                .ToListAsync();

            // 走缓存批量翻译
            var fields = new List<LangFieldMap<SysMenu>>
            {
                new LangFieldMap<SysMenu>
                {
                    EntityName = "SysMenu",
                    FieldName = "Title",
                    IdSelector = m => m.Id,
                    SetTranslatedValue = (m, val) => m.Title = val
                }
            };
            await _sysLangTextCacheService.TranslateMultiFields(menuList, fields, langCode);

            return menuList.Distinct().ToList();
        }

        // 无筛选条件则走全量树形结构（带权限）
        if (!(_userManager.SuperAdmin || _userManager.SysAdmin))
        {
            query = query.Where(u => menuIdList.Contains(u.Id));
        }

        var menuFullList = await query
            .OrderBy(u => new { u.OrderNo, u.Id })
            .ToListAsync();

        // 走缓存批量翻译
        var treeFields = new List<LangFieldMap<SysMenu>>
        {
            new LangFieldMap<SysMenu>
            {
                    EntityName = "SysMenu",
                    FieldName = "Title",
                    IdSelector = m => m.Id,
                    SetTranslatedValue = (m, val) => m.Title = val
            }
        };
        await _sysLangTextCacheService.TranslateMultiFields(menuFullList, treeFields, langCode);

        // 组装树
        var menuTree = menuFullList.ToTree(it => it.Children, it => it.Pid, 0);
        return menuTree.ToList();
    }

    /// <summary>
    /// 增加菜单 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加菜单")]
    public async Task<long> AddMenu(AddMenuInput input)
    {
        var (query, tenantId) = GetSugarQueryableAndTenantId(input.TenantId);

        var isExist = input.Type != MenuTypeEnum.Btn
            ? await query.AnyAsync(u => u.Title == input.Title && u.Pid == input.Pid)
            : await query.AnyAsync(u => u.Permission == input.Permission);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D4000);

        if (!string.IsNullOrWhiteSpace(input.Name) && await query.AnyAsync(u => u.Name == input.Name)) throw Oops.Oh(ErrorCodeEnum.D4009);

        if (input.Pid != 0 && await query.AnyAsync(u => u.Id == input.Pid && u.Type == MenuTypeEnum.Btn)) throw Oops.Oh(ErrorCodeEnum.D4010);

        // 校验菜单参数
        var sysMenu = input.Adapt<SysMenu>();
        CheckMenuParam(sysMenu);

        // 保存租户菜单权限
        await _sysMenuRep.InsertAsync(sysMenu);
        await _sysTenantMenuRep.InsertAsync(new SysTenantMenu { TenantId = tenantId, MenuId = sysMenu.Id });

        // 清除缓存
        DeleteMenuCache();

        return sysMenu.Id;
    }

    /// <summary>
    /// 更新菜单 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新菜单")]
    public async Task UpdateMenu(UpdateMenuInput input)
    {
        if (!_userManager.SuperAdmin && new SysMenuSeedData().HasData().Any(u => u.Id == input.Id)) throw Oops.Oh(ErrorCodeEnum.D4012);

        if (input.Id == input.Pid) throw Oops.Oh(ErrorCodeEnum.D4008);
        var (query, _) = GetSugarQueryableAndTenantId(input.TenantId);

        var isExist = input.Type != MenuTypeEnum.Btn
            ? await query.AnyAsync(u => u.Title == input.Title && u.Type == input.Type && u.Pid == input.Pid && u.Id != input.Id)
            : await query.AnyAsync(u => u.Permission == input.Permission && u.Id != input.Id);
        if (isExist) throw Oops.Oh(ErrorCodeEnum.D4000);

        if (!string.IsNullOrWhiteSpace(input.Name) && await query.AnyAsync(u => u.Id != input.Id && u.Name == input.Name)) throw Oops.Oh(ErrorCodeEnum.D4009);

        if (input.Pid != 0 && await query.AnyAsync(u => u.Id == input.Pid && u.Type == MenuTypeEnum.Btn)) throw Oops.Oh(ErrorCodeEnum.D4010);

        // 校验菜单参数
        var sysMenu = input.Adapt<SysMenu>();
        CheckMenuParam(sysMenu);

        await _sysMenuRep.AsTenant().UseTranAsync(async () =>
        {
            // 更新菜单
            await _sysMenuRep.AsUpdateable(sysMenu).ExecuteCommandAsync();

            // 同步更新翻译表
            var menuTranslation = await _sysLangTextCacheService.GetTranslationEntity("SysMenu", "Title", sysMenu.Id, _userManager.LangCode);
            if (!menuTranslation.IsNullOrEmpty())
            {
                await _sysLangTextService.Update(new UpdateSysLangTextInput
                {
                    Id = menuTranslation.Id,
                    EntityName = "SysMenu",
                    EntityId = sysMenu.Id,
                    FieldName = "Title",
                    LangCode = _userManager.LangCode,
                    Content = sysMenu.Title
                });
            }
        }, err =>
        {
            Oops.Oh("更新数据时发生错误", err.Message);
        });

        // 清除缓存
        DeleteMenuCache();
    }

    /// <summary>
    /// 删除菜单 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除菜单")]
    public async Task DeleteMenu(DeleteMenuInput input)
    {
        if (!_userManager.SuperAdmin && new SysMenuSeedData().HasData().Any(u => u.Id == input.Id)) throw Oops.Oh(ErrorCodeEnum.D4013);

        var menuTreeList = await _sysMenuRep.AsQueryable().ToChildListAsync(u => u.Pid, input.Id);
        var menuIdList = menuTreeList.Select(u => u.Id).ToList();

        await _sysMenuRep.DeleteAsync(u => menuIdList.Contains(u.Id));

        // 级联删除租户菜单数据
        await _sysTenantMenuRep.AsDeleteable().Where(u => menuIdList.Contains(u.MenuId)).ExecuteCommandAsync();

        // 级联删除角色菜单数据
        await _sysRoleMenuService.DeleteRoleMenuByMenuIdList(menuIdList);

        // 级联删除用户收藏菜单
        await _sysUserMenuService.DeleteMenuList(menuIdList);

        // 清除缓存
        DeleteMenuCache();
    }

    /// <summary>
    /// 设置菜单状态 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork]
    [DisplayName("设置菜单状态")]
    public virtual async Task<int> SetStatus(MenuStatusInput input)
    {
        if (_userManager.UserId == input.Id)
            throw Oops.Oh(ErrorCodeEnum.D1026);

        var menu = await _sysMenuRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        menu.Status = input.Status;
        var rows = await _sysMenuRep.AsUpdateable(menu).UpdateColumns(u => new { u.Status }).ExecuteCommandAsync();
        return rows;
    }

    /// <summary>
    /// 增加和编辑时检查菜单数据
    /// </summary>
    /// <param name="menu"></param>
    private static void CheckMenuParam(SysMenu menu)
    {
        var permission = menu.Permission;
        if (menu.Type == MenuTypeEnum.Btn)
        {
            menu.Name = null;
            menu.Path = null;
            menu.Component = null;
            menu.Icon = null;
            menu.Redirect = null;
            menu.OutLink = null;
            menu.IsHide = false;
            menu.IsKeepAlive = true;
            menu.IsAffix = false;
            menu.IsIframe = false;

            if (string.IsNullOrEmpty(permission)) throw Oops.Oh(ErrorCodeEnum.D4003);
            if (!permission.Contains(':')) throw Oops.Oh(ErrorCodeEnum.D4004);
        }
        else
        {
            menu.Permission = null;
        }
    }

    /// <summary>
    /// 获取用户拥有按钮权限集合（缓存） 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取按钮权限集合")]
    public async Task<List<string>> GetOwnBtnPermList()
    {
        var userId = _userManager.UserId;
        var permissions = _sysCacheService.Get<List<string>>(CacheConst.KeyUserButton + userId);
        if (permissions != null) return permissions;

        var menuIdList = _userManager.SuperAdmin ? new() : await GetMenuIdList();
        if (menuIdList.Count <= 0 && !_userManager.SuperAdmin && !_userManager.SysAdmin)
        {
            //_sysCacheService.Set(CacheConst.KeyUserButton + userId, new List<string>(), TimeSpan.FromDays(7));
            return new List<string>();
        }

        permissions = await _sysMenuRep.AsQueryable()
            .InnerJoinIF<SysTenantMenu>(!_userManager.SuperAdmin, (u, t) => t.TenantId == _userManager.TenantId && u.Id == t.MenuId)
            .Where(u => u.Type == MenuTypeEnum.Btn)
            .WhereIF(menuIdList.Count > 0, u => menuIdList.Contains(u.Id))
            .Select(u => u.Permission).ToListAsync();

        _sysCacheService.Set(CacheConst.KeyUserButton + userId, permissions, TimeSpan.FromDays(7));

        return permissions;
    }

    /// <summary>
    /// 获取系统所有按钮权限集合（缓存）
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<string>> GetAllBtnPermList()
    {
        var permissions = _sysCacheService.Get<List<string>>(CacheConst.KeyUserButton + 0);
        if (permissions != null && permissions.Count != 0) return permissions;

        permissions = await _sysMenuRep.AsQueryable()
            .Where(u => u.Type == MenuTypeEnum.Btn)
            .Select(u => u.Permission).ToListAsync();
        _sysCacheService.Set(CacheConst.KeyUserButton + 0, permissions);

        return permissions;
    }

    /// <summary>
    /// 根据租户id获取构建菜单联表查询实例
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    [NonAction]
    public (ISugarQueryable<SysMenu, SysTenantMenu> query, long tenantId) GetSugarQueryableAndTenantId(long tenantId)
    {
        if (!_userManager.SuperAdmin) tenantId = _userManager.TenantId;

        // 超管用户菜单范围：种子菜单 + 租户id菜单
        ISugarQueryable<SysMenu, SysTenantMenu> query;
        if (_userManager.SuperAdmin)
        {
            if (tenantId <= 0)
            {
                query = _sysMenuRep.AsQueryable().InnerJoinIF<SysTenantMenu>(false, (u, t) => true);
            }
            else
            {
                // 指定租户的菜单
                var menuIds = _sysTenantMenuRep.AsQueryable().Where(u => u.TenantId == tenantId).ToList(u => u.MenuId) ?? new();

                // 种子菜单
                //menuIds.AddRange(new SysMenuSeedData().HasData().Select(u => u.Id).ToList());

                menuIds = menuIds.Distinct().ToList();
                query = _sysMenuRep.AsQueryable().InnerJoinIF<SysTenantMenu>(false, (u, t) => true).Where(u => menuIds.Contains(u.Id));
            }
        }
        else
        {
            query = _sysMenuRep.AsQueryable().InnerJoinIF<SysTenantMenu>(tenantId > 0, (u, t) => t.TenantId == tenantId && u.Id == t.MenuId);
        }

        return (query, tenantId);
    }

    /// <summary>
    /// 清除菜单和按钮缓存
    /// </summary>
    [NonAction]
    public void DeleteMenuCache()
    {
        // _sysCacheService.RemoveByPrefixKey(CacheConst.KeyUserMenu);
        _sysCacheService.RemoveByPrefixKey(CacheConst.KeyUserButton);
    }

    /// <summary>
    /// 获取当前用户菜单Id集合
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<long>> GetMenuIdList()
    {
        var roleIdList = await _sysUserRoleService.GetUserRoleIdList(_userManager.UserId);
        return await _sysRoleMenuService.GetRoleMenuIdList(roleIdList);
    }

    /// <summary>
    /// 排除前端存在全选的父级菜单
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<List<long>> ExcludeParentMenuOfFullySelected(List<long> menuIds)
    {
        // 获取当前用户菜单
        var (query, _) = GetSugarQueryableAndTenantId(0);
        var menuList = await query.ToListAsync();

        // 排除列表，防止前端全选问题
        var exceptList = new List<long>();
        foreach (var id in menuIds)
        {
            // 排除按钮菜单
            if (menuList.Any(u => u.Id == id && u.Type == MenuTypeEnum.Btn)) continue;

            // 如果没有子集或有全部子集权限
            var children = menuList.ToChildList(u => u.Id, u => u.Pid, id, false).ToList();
            if (children.Count == 0 || children.All(u => menuIds.Contains(u.Id))) continue;

            // 排除没有全部子集权限的菜单
            exceptList.Add(id);
        }
        return menuIds.Except(exceptList).ToList();
    }
}