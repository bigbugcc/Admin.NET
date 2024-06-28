// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Microsoft.Extensions.DependencyInjection.Extensions;
using NewLife.Caching.Services;

namespace Admin.NET.Core;

public static class CacheSetup
{
    /// <summary>
    /// 缓存注册（新生命Redis组件）
    /// </summary>
    /// <param name="services"></param>
    public static void AddCache(this IServiceCollection services)
    {
        var cacheOptions = App.GetConfig<CacheOptions>("Cache", true);
        if (cacheOptions.CacheType == CacheTypeEnum.Redis.ToString())
        {
            var redis = new FullRedis(new RedisOptions
            {
                Configuration = cacheOptions.Redis.Configuration,
                Prefix = cacheOptions.Redis.Prefix
            });
            if (cacheOptions.Redis.MaxMessageSize > 0)
                redis.MaxMessageSize = cacheOptions.Redis.MaxMessageSize;

            // 注入 Redis 缓存提供者
            services.AddSingleton<ICacheProvider>(p => new RedisCacheProvider(p) { Cache = redis });
        }

        // 内存缓存兜底。在没有配置Redis时，使用内存缓存，逻辑代码无需修改
        services.TryAddSingleton<ICacheProvider, CacheProvider>();
    }
}