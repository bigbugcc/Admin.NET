// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

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