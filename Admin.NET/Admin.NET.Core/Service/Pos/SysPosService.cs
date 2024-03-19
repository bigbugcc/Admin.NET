// 大名科技（天津）有限公司版权所有  电话：18020030720  QQ：515096995
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统职位服务 💥
/// </summary>
[ApiDescriptionSettings(Order = 460)]
public class SysPosService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysPos> _sysPosRep;
    private readonly SysUserExtOrgService _sysUserExtOrgService;

    public SysPosService(UserManager userManager,
        SqlSugarRepository<SysPos> sysPosRep,
        SysUserExtOrgService sysUserExtOrgService)
    {
        _userManager = userManager;
        _sysPosRep = sysPosRep;
        _sysUserExtOrgService = sysUserExtOrgService;
    }

    /// <summary>
    /// 获取职位列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取职位列表")]
    public async Task<List<SysPos>> GetList([FromQuery] PosInput input)
    {
        return await _sysPosRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code))
            .OrderBy(u => u.OrderNo).ToListAsync();
    }

    /// <summary>
    /// 增加职位 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加职位")]
    public async Task AddPos(AddPosInput input)
    {
        if (await _sysPosRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code))
            throw Oops.Oh(ErrorCodeEnum.D6000);

        await _sysPosRep.InsertAsync(input.Adapt<SysPos>());
    }

    /// <summary>
    /// 更新职位 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新职位")]
    public async Task UpdatePos(UpdatePosInput input)
    {
        if (await _sysPosRep.IsAnyAsync(u => u.Name == input.Name && u.Code == input.Code && u.Id != input.Id))
            throw Oops.Oh(ErrorCodeEnum.D6000);

        var sysPos = await _sysPosRep.GetByIdAsync(input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D6003);
        if (!_userManager.SuperAdmin && sysPos.CreateUserId != _userManager.UserId)
            throw Oops.Oh(ErrorCodeEnum.D6002);

        await _sysPosRep.AsUpdateable(input.Adapt<SysPos>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除职位 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除职位")]
    public async Task DeletePos(DeletePosInput input)
    {
        var sysPos = await _sysPosRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D6003);
        if (!_userManager.SuperAdmin && sysPos.CreateUserId != _userManager.UserId)
            throw Oops.Oh(ErrorCodeEnum.D6002);

        // 若职位有用户则禁止删除
        var hasPosEmp = await _sysPosRep.ChangeRepository<SqlSugarRepository<SysUser>>()
            .IsAnyAsync(u => u.PosId == input.Id);
        if (hasPosEmp)
            throw Oops.Oh(ErrorCodeEnum.D6001);

        // 若附属职位有用户则禁止删除
        var hasExtPosEmp = await _sysUserExtOrgService.HasUserPos(input.Id);
        if (hasExtPosEmp)
            throw Oops.Oh(ErrorCodeEnum.D6001);

        await _sysPosRep.DeleteAsync(u => u.Id == input.Id);
    }
}