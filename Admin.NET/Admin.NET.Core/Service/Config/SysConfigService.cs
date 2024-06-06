// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统参数配置服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 440)]
public class SysConfigService : IDynamicApiController, ITransient
{
    private readonly ISqlSugarClient _db;
    private SimpleClient<SysConfig> sysConfigRep_ = null;
    private readonly SysCacheService _sysCacheService;

    public SysConfigService(ISqlSugarClient db,
        SysCacheService sysCacheService)
    {
        _db = db;
        _sysCacheService = sysCacheService;
    }
    public SimpleClient<SysConfig> _sysConfigRep
    {
        get
        {
            if (sysConfigRep_ == null)
                sysConfigRep_ = _db.GetSimpleClient<SysConfig>();
            return sysConfigRep_;
        }
    }
    /// <summary>
    /// 获取参数配置分页列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取参数配置分页列表")]
    public async Task<SqlSugarPagedList<SysConfig>> Page(PageConfigInput input)
    {
        return await _sysConfigRep.AsQueryable()
            .Where(u => u.GroupCode != "WebConfig") // 不显示 WebConfig 分组
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name?.Trim()), u => u.Name.Contains(input.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code?.Trim()), u => u.Code.Contains(input.Code))
            .WhereIF(!string.IsNullOrWhiteSpace(input.GroupCode?.Trim()), u => u.GroupCode.Equals(input.GroupCode))
            .OrderBuilder(input)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取参数配置列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取参数配置列表")]
    public async Task<List<SysConfig>> GetList()
    {
        return await _sysConfigRep.GetListAsync();
    }

    /// <summary>
    /// 增加参数配置 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加参数配置")]
    public async Task AddConfig(AddConfigInput input)
    {
        var isExist = await _sysConfigRep.IsAnyAsync(u => u.Name == input.Name || u.Code == input.Code);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D9000);

        await _sysConfigRep.InsertAsync(input.Adapt<SysConfig>());
    }

    /// <summary>
    /// 更新参数配置 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新参数配置")]
    public async Task UpdateConfig(UpdateConfigInput input)
    {
        var isExist = await _sysConfigRep.IsAnyAsync(u => (u.Name == input.Name || u.Code == input.Code) && u.Id != input.Id);
        if (isExist)
            throw Oops.Oh(ErrorCodeEnum.D9000);

        var config = input.Adapt<SysConfig>();
        await _sysConfigRep.AsUpdateable(config).IgnoreColumns(true).ExecuteCommandAsync();

        _sysCacheService.Remove(config.Code);
    }

    /// <summary>
    /// 删除参数配置 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除参数配置")]
    public async Task DeleteConfig(DeleteConfigInput input)
    {
        var config = await _sysConfigRep.GetFirstAsync(u => u.Id == input.Id);
        if (config.SysFlag == YesNoEnum.Y) // 禁止删除系统参数
            throw Oops.Oh(ErrorCodeEnum.D9001);

        await _sysConfigRep.DeleteAsync(config);

        _sysCacheService.Remove(config.Code);
    }

    /// <summary>
    /// 批量删除参数配置 🔖
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "BatchDelete"), HttpPost]
    [DisplayName("批量删除参数配置")]
    public async Task BatchDeleteConfig(List<long> ids)
    {
        foreach (var id in ids)
        {
            var config = await _sysConfigRep.GetFirstAsync(u => u.Id == id);
            if (config.SysFlag == YesNoEnum.Y) // 禁止删除系统参数
                continue;

            await _sysConfigRep.DeleteAsync(config);

            _sysCacheService.Remove(config.Code);
        }
    }

    /// <summary>
    /// 获取参数配置详情 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取参数配置详情")]
    public async Task<SysConfig> GetDetail([FromQuery] ConfigInput input)
    {
        return await _sysConfigRep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 获取参数配置值
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<T> GetConfigValue<T>(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return default;

        var value = _sysCacheService.Get<string>(code);
        if (string.IsNullOrEmpty(value))
        {
            var config = await _sysConfigRep.GetFirstAsync(u => u.Code == code);
            value = config != null ? config.Value : default;
            _sysCacheService.Set(code, value);
        }
        if (string.IsNullOrWhiteSpace(value)) return default;
        return (T)Convert.ChangeType(value, typeof(T));
    }

    /// <summary>
    /// 更新参数配置值
    /// </summary>
    /// <param name="code"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [NonAction]
    public async Task UpdateConfigValue(string code, string value)
    {
        var config = await _sysConfigRep.GetFirstAsync(u => u.Code == code);
        if (config == null) return;

        config.Value = value;
        await _sysConfigRep.AsUpdateable(config).ExecuteCommandAsync();

        _sysCacheService.Remove(config.Code);
    }

    /// <summary>
    /// 获取分组列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取分组列表")]
    public async Task<List<string>> GetGroupList()
    {
        return await _sysConfigRep.AsQueryable()
            .Where(u => u.GroupCode != "WebConfig") // 不显示 WebConfig 分组
            .GroupBy(u => u.GroupCode)
            .Select(u => u.GroupCode).ToListAsync();
    }

    /// <summary>
    /// 获取 Token 过期时间
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<int> GetTokenExpire()
    {
        var tokenExpireStr = await GetConfigValue<string>(CommonConst.SysTokenExpire);
        _ = int.TryParse(tokenExpireStr, out var tokenExpire);
        return tokenExpire == 0 ? 20 : tokenExpire;
    }

    /// <summary>
    /// 获取 RefreshToken 过期时间
    /// </summary>
    /// <returns></returns>
    [NonAction]
    public async Task<int> GetRefreshTokenExpire()
    {
        var refreshTokenExpireStr = await GetConfigValue<string>(CommonConst.SysRefreshTokenExpire);
        _ = int.TryParse(refreshTokenExpireStr, out var refreshTokenExpire);
        return refreshTokenExpire == 0 ? 40 : refreshTokenExpire;
    }

    /// <summary>
    /// 获取系统信息 🔖
    /// </summary>
    /// <returns></returns>
    [SuppressMonitor]
    [AllowAnonymous]
    [DisplayName("获取系统信息")]
    public async Task<dynamic> GetSysInfo()
    {
        var sysLogo = await GetConfigValue<string>("sys_web_logo");
        var sysTitle = await GetConfigValue<string>("sys_web_title");
        var sysViceTitle = await GetConfigValue<string>("sys_web_viceTitle");
        var sysViceDesc = await GetConfigValue<string>("sys_web_viceDesc");
        var sysWatermark = await GetConfigValue<string>("sys_web_watermark");
        var sysCopyright = await GetConfigValue<string>("sys_web_copyright");
        var sysIcp = await GetConfigValue<string>("sys_web_icp");
        var sysIcpUrl = await GetConfigValue<string>("sys_web_icpUrl");
        return new
        {
            SysLogo = sysLogo,
            SysTitle = sysTitle,
            SysViceTitle = sysViceTitle,
            SysViceDesc = sysViceDesc,
            SysWatermark = sysWatermark,
            SysCopyright = sysCopyright,
            SysIcp = sysIcp,
            SysIcpUrl = sysIcpUrl
        };
    }

    /// <summary>
    /// 保存系统信息 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("保存系统信息")]
    public async Task SaveSysInfo(InfoSaveInput input)
    {
        // logo 不为空才保存
        if (!string.IsNullOrEmpty(input.SysLogoBase64))
        {
            // 旧图标文件相对路径
            var oldSysLogoRelativeFilePath = await GetConfigValue<string>("sys_web_logo") ?? "";
            var oldSysLogoAbsoluteFilePath = Path.Combine(App.WebHostEnvironment.WebRootPath, oldSysLogoRelativeFilePath.TrimStart('/'));

            var groups = Regex.Match(input.SysLogoBase64, @"data:image/(?<type>.+?);base64,(?<data>.+)").Groups;
            var type = groups["type"].Value;
            var base64Data = groups["data"].Value;
            var binData = Convert.FromBase64String(base64Data);

            // 本地图标保存路径
            var path = "Upload";
            var absoluteFilePath = Path.Combine(App.WebHostEnvironment.WebRootPath, path, $"logo.{type}");

            // 删除已存在文件
            if (File.Exists(oldSysLogoAbsoluteFilePath))
                File.Delete(oldSysLogoAbsoluteFilePath);

            // 创建文件夹
            var absoluteFileDir = Path.GetDirectoryName(absoluteFilePath);
            if (!Directory.Exists(absoluteFileDir))
                Directory.CreateDirectory(absoluteFileDir);

            // 保存图标文件
            await File.WriteAllBytesAsync(absoluteFilePath, binData);

            // 保存图标配置
            var relativeUrl = $"/{path}/logo.{type}";
            await UpdateConfigValue("sys_web_logo", relativeUrl);
        }

        await UpdateConfigValue("sys_web_title", input.SysTitle);
        await UpdateConfigValue("sys_web_viceTitle", input.SysViceTitle);
        await UpdateConfigValue("sys_web_viceDesc", input.SysViceDesc);
        await UpdateConfigValue("sys_web_watermark", input.SysWatermark);
        await UpdateConfigValue("sys_web_copyright", input.SysCopyright);
        await UpdateConfigValue("sys_web_icp", input.SysIcp);
        await UpdateConfigValue("sys_web_icpUrl", input.SysIcpUrl);
    }
}