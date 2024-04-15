// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.GoView.Service;

/// <summary>
/// 系统登录服务 🧩
/// </summary>
[UnifyProvider("GoView")]
[ApiDescriptionSettings(GoViewConst.GroupName, Module = "goview", Name = "sys", Order = 100)]
public class GoViewSysService : IDynamicApiController
{
    private readonly SysAuthService _sysAuthService;
    private readonly SqlSugarRepository<SysUser> _sysUserRep;
    private readonly SysCacheService _sysCacheService;

    public GoViewSysService(SysAuthService sysAuthService,
        SqlSugarRepository<SysUser> sysUserRep,
        SysCacheService sysCacheService)
    {
        _sysAuthService = sysAuthService;
        _sysUserRep = sysUserRep;
        _sysCacheService = sysCacheService;
    }

    /// <summary>
    /// GoView 登录 🔖
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("GoView 登录")]
    public async Task<GoViewLoginOutput> Login(GoViewLoginInput input)
    {
        _sysCacheService.Set(CommonConst.SysCaptcha, false);

        input.Password = CryptogramUtil.SM2Encrypt(input.Password);
        var loginResult = await _sysAuthService.Login(new LoginInput()
        {
            Account = input.Username,
            Password = input.Password,
        });

        _sysCacheService.Remove(CommonConst.SysCaptcha);

        var sysUser = await _sysUserRep.AsQueryable().ClearFilter().FirstAsync(u => u.Account.Equals(input.Username));
        return new GoViewLoginOutput()
        {
            Userinfo = new GoViewLoginUserInfo
            {
                Id = sysUser.Id.ToString(),
                Username = sysUser.Account,
                Nickname = sysUser.NickName,
            },
            Token = new GoViewLoginToken
            {
                TokenValue = $"Bearer {loginResult.AccessToken}"
            }
        };
    }

    /// <summary>
    /// GoView 退出 🔖
    /// </summary>
    [DisplayName("GoView 退出")]
    public void GetLogout()
    {
        _sysAuthService.Logout();
    }

    /// <summary>
    /// 获取 OSS 上传接口 🔖
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [ApiDescriptionSettings(Name = "GetOssInfo")]
    [DisplayName("获取 OSS 上传接口")]
    public static Task<GoViewOssUrlOutput> GetOssInfo()
    {
        return Task.FromResult(new GoViewOssUrlOutput { BucketURL = "" });
    }
}