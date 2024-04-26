// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Core;
using AspNetCoreRateLimit;
using Furion;
using Microsoft.Extensions.DependencyInjection;

namespace Admin.NET.Web.Core;

public static class ProjectOptions
{
    /// <summary>
    /// 注册项目配置选项
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddProjectOptions(this IServiceCollection services)
    {
        services.AddConfigurableOptions<DbConnectionOptions>();
        services.AddConfigurableOptions<SnowIdOptions>();
        services.AddConfigurableOptions<CacheOptions>();
        services.AddConfigurableOptions<ClusterOptions>();
        services.AddConfigurableOptions<OSSProviderOptions>();
        services.AddConfigurableOptions<UploadOptions>();
        services.AddConfigurableOptions<WechatOptions>();
        services.AddConfigurableOptions<WechatPayOptions>();
        services.AddConfigurableOptions<PayCallBackOptions>();
        services.AddConfigurableOptions<CodeGenOptions>();
        services.AddConfigurableOptions<EnumOptions>();
        services.AddConfigurableOptions<APIJSONOptions>();
        services.AddConfigurableOptions<EmailOptions>();
        services.AddConfigurableOptions<OAuthOptions>();
        services.AddConfigurableOptions<CryptogramOptions>();
        services.AddConfigurableOptions<SMSOptions>();
        services.AddConfigurableOptions<EventBusOptions>();
        services.Configure<IpRateLimitOptions>(App.Configuration.GetSection("IpRateLimiting"));
        services.Configure<IpRateLimitPolicies>(App.Configuration.GetSection("IpRateLimitPolicies"));
        services.Configure<ClientRateLimitOptions>(App.Configuration.GetSection("ClientRateLimiting"));
        services.Configure<ClientRateLimitPolicies>(App.Configuration.GetSection("ClientRateLimitPolicies"));

        return services;
    }
}