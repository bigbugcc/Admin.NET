// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Elastic.Clients.Elasticsearch;

namespace Admin.NET.Core;

/// <summary>
/// ES客户端容器
/// </summary>
public class ElasticSearchClientContainer
{
    private readonly Dictionary<EsClientTypeEnum, ElasticsearchClient> _clients;

    /// <summary>
    /// 初始化容器（通过字典注入所有客户端）
    /// </summary>
    public ElasticSearchClientContainer(Dictionary<EsClientTypeEnum, ElasticsearchClient> clients)
    {
        _clients = clients ?? throw new ArgumentNullException(nameof(clients));
    }

    /// <summary>
    /// 日志专用客户端
    /// </summary>
    public ElasticsearchClient Logging => GetClient(EsClientTypeEnum.Logging);

    /// <summary>
    /// 业务数据同步客户端
    /// </summary>
    public ElasticsearchClient Business => GetClient(EsClientTypeEnum.Business);

    /// <summary>
    /// 根据类型获取客户端（内部校验，避免未注册的类型）
    /// </summary>
    private ElasticsearchClient GetClient(EsClientTypeEnum type)
    {
        if (_clients.TryGetValue(type, out var client))
        {
            return client;
        }
        throw new KeyNotFoundException($"未注册的ES客户端类型：{type}，请检查注册配置");
    }
}