// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 文件存储提供者管理控制器 🧩
/// </summary>
[ApiDescriptionSettings(Order = 412, Description = "文件存储提供者管理")]
[SysAdminOnly]
public class SysFileProviderController : IDynamicApiController, ITransient
{
    private readonly SysFileProviderService _fileProviderService;

    public SysFileProviderController(SysFileProviderService fileProviderService)
    {
        _fileProviderService = fileProviderService;
    }

    /// <summary>
    /// 获取存储提供者列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取存储提供者列表")]
    public async Task<List<SysFileProvider>> GetProviderList()
    {
        return await _fileProviderService.GetFileProviderList();
    }

    /// <summary>
    /// 获取存储提供者分页列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取存储提供者分页列表")]
    public async Task<SqlSugarPagedList<SysFileProvider>> GetProviderPage(PageFileProviderInput input)
    {
        return await _fileProviderService.GetFileProviderPage(input);
    }

    /// <summary>
    /// 获取存储提供者详情 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取存储提供者详情")]
    public async Task<SysFileProvider> GetProvider([FromQuery] QueryFileProviderInput input)
    {
        return await _fileProviderService.GetFileProvider(input);
    }

    /// <summary>
    /// 添加存储提供者 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("添加存储提供者")]
    public async Task AddProvider(AddFileProviderInput input)
    {
        await _fileProviderService.AddFileProvider(input);
    }

    /// <summary>
    /// 更新存储提供者 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新存储提供者")]
    public async Task UpdateProvider(UpdateFileProviderInput input)
    {
        await _fileProviderService.UpdateFileProvider(input);
    }

    /// <summary>
    /// 删除存储提供者 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除存储提供者")]
    public async Task DeleteProvider(DeleteFileProviderInput input)
    {
        await _fileProviderService.DeleteFileProvider(input);
    }

    /// <summary>
    /// 根据存储桶名称获取存储提供者 🔖
    /// </summary>
    /// <param name="bucketName">存储桶名称</param>
    /// <returns></returns>
    [DisplayName("根据存储桶名称获取存储提供者")]
    public async Task<SysFileProvider?> GetProviderByBucketName(string bucketName)
    {
        return await _fileProviderService.GetProviderByBucketName(bucketName);
    }

    /// <summary>
    /// 清除存储提供者缓存 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("清除存储提供者缓存")]
    public async Task ClearCache()
    {
        await _fileProviderService.ClearCache();
    }

    /// <summary>
    /// 批量启用/禁用存储提供者 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "BatchEnable"), HttpPost]
    [DisplayName("批量启用/禁用存储提供者")]
    public async Task BatchEnableProvider(BatchEnableProviderInput input)
    {
        foreach (var id in input.Ids)
        {
            var provider = await _fileProviderService.GetFileProviderById(id);
            if (provider != null)
            {
                var updateInput = new UpdateFileProviderInput
                {
                    Id = id,
                    Provider = provider.Provider,
                    BucketName = provider.BucketName,
                    IsEnable = input.IsEnable
                };
                await _fileProviderService.UpdateFileProvider(updateInput);
            }
        }
    }

    /// <summary>
    /// 获取存储提供者统计信息 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取存储提供者统计信息")]
    public async Task<object> GetProviderStatistics()
    {
        var providers = await _fileProviderService.GetCachedFileProviders();

        var statistics = new
        {
            Total = providers.Count,
            Enabled = providers.Count(p => p.IsEnable == true),
            Disabled = providers.Count(p => p.IsEnable != true),
            ByProvider = providers.GroupBy(p => p.Provider)
                .Select(g => new { Provider = g.Key, Count = g.Count() })
                .ToList(),
            ByRegion = providers.Where(p => !string.IsNullOrEmpty(p.Region))
                .GroupBy(p => p.Region)
                .Select(g => new { Region = g.Key, Count = g.Count() })
                .ToList()
        };

        return statistics;
    }

    /// <summary>
    /// 获取所有可用的存储桶列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取所有可用的存储桶列表")]
    public async Task<List<string>> GetAvailableBuckets()
    {
        return await _fileProviderService.GetAvailableBuckets();
    }

    /// <summary>
    /// 获取存储桶和提供者的映射关系 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取存储桶和提供者的映射关系")]
    public async Task<Dictionary<string, List<SysFileProvider>>> GetBucketProviderMapping()
    {
        return await _fileProviderService.GetBucketProviderMapping();
    }
}

/// <summary>
/// 批量启用/禁用存储提供者输入参数
/// </summary>
public class BatchEnableProviderInput
{
    /// <summary>
    /// 存储提供者ID列表
    /// </summary>
    [Required]
    public List<long> Ids { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnable { get; set; }
}