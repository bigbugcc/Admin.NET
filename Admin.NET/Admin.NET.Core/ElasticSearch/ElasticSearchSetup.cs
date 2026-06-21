// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace Admin.NET.Core;

/// <summary>
/// ES服务注册
/// </summary>
public static class ElasticSearchSetup
{
    public static void AddElasticSearch(this IServiceCollection services)
    {
        var option = App.GetConfig<ElasticSearchOptions>("Logging:ElasticSearch");
        if (!option.Enabled) return;

        var uris = option.ServerUris.Select(u => new Uri(u));
        // 集群
        var connectionPool = new StaticNodePool(uris);
        var connectionSettings = new ElasticsearchClientSettings(connectionPool).DefaultIndex(option.DefaultIndex);
        // 单连接
        //var connectionSettings = new ElasticsearchClientSettings(new StaticNodePool(new List<Uri> { uris.FirstOrDefault() })).DefaultIndex(option.DefaultIndex);

        // 认证类型
        if (option.AuthType == ElasticSearchAuthTypeEnum.Basic) // Basic 认证
        {
            connectionSettings.Authentication(new BasicAuthentication(option.User, option.Password));
        }
        else if (option.AuthType == ElasticSearchAuthTypeEnum.ApiKey) // ApiKey 认证
        {
            connectionSettings.Authentication(new ApiKey(option.ApiKey));
        }
        else if (option.AuthType == ElasticSearchAuthTypeEnum.Base64ApiKey) // Base64ApiKey 认证
        {
            connectionSettings.Authentication(new Base64ApiKey(option.Base64ApiKey));
        }
        else return;

        // ES使用Https时的证书指纹
        if (!string.IsNullOrEmpty(option.Fingerprint))
        {
            connectionSettings.CertificateFingerprint(option.Fingerprint);
        }

        var client = new ElasticsearchClient(connectionSettings);
        services.AddSingleton(client); // 单例注册
    }
}