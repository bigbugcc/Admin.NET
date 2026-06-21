// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace Admin.NET.Core;

public class ElasticSearchClientFactory
{
    /// <summary>
    /// 创建 ES 客户端（通用方法）
    /// </summary>
    /// <typeparam name="TOptions">配置类型（支持通用或场景专用）</typeparam>
    /// <param name="configPath">配置文件路径（如 "ElasticSearch:Logging"）</param>
    /// <returns>ES 客户端实例（或 null  if 未启用）</returns>
    public static ElasticsearchClient? CreateClient<TOptions>(string configPath) where TOptions : ElasticSearchOptions, new()
    {
        // 从配置文件读取当前场景的配置
        var options = App.GetConfig<TOptions>(configPath);
        if (options == null)
            throw Oops.Oh($"未找到{configPath}配置项");

        if (!options.Enabled)
            return null;

        // 验证服务地址
        if (options.ServerUris == null || !options.ServerUris.Any())
            throw new ArgumentException($"ES 配置 {configPath} 未设置 ServerUris");

        // 构建连接池（支持集群）
        var uris = options.ServerUris.Select(uri => new Uri(uri)).ToList();
        var connectionPool = new StaticNodePool(uris);
        var connectionSettings = new ElasticsearchClientSettings(connectionPool)
            .DefaultIndex(options.DefaultIndex) // 设置默认索引
            .DisableDirectStreaming()  // 开启请求/响应日志，方便排查问题
            .OnRequestCompleted(response =>
            {
                if (response.HttpStatusCode == 401)
                {
                    Console.WriteLine("ES 请求被拒绝：未提供有效认证信息");
                }
            });

        // 配置认证
        ConfigureAuthentication(connectionSettings, options);

        // 配置 HTTPS 证书指纹
        if (!string.IsNullOrEmpty(options.Fingerprint))
            connectionSettings.CertificateFingerprint(options.Fingerprint);

        return new ElasticsearchClient(connectionSettings);
    }

    /// <summary>
    /// 配置认证（通用逻辑）
    /// </summary>
    private static void ConfigureAuthentication(ElasticsearchClientSettings settings, ElasticSearchOptions options)
    {
        switch (options.AuthType)
        {
            case ElasticSearchAuthTypeEnum.Basic:
                settings.Authentication(new BasicAuthentication(options.User, options.Password));
                break;

            case ElasticSearchAuthTypeEnum.ApiKey:
                settings.Authentication(new ApiKey(options.ApiKey));
                break;

            case ElasticSearchAuthTypeEnum.Base64ApiKey:
                settings.Authentication(new Base64ApiKey(options.Base64ApiKey));
                break;

            case ElasticSearchAuthTypeEnum.None:
                // 无需认证
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(options.AuthType));
        }
    }
}