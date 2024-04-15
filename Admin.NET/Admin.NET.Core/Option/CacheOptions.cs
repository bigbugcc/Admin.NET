// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 缓存配置选项
/// </summary>
public sealed class CacheOptions : IConfigurableOptions<CacheOptions>
{
    /// <summary>
    /// 缓存前缀
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    /// 缓存类型
    /// </summary>
    public string CacheType { get; set; }

    /// <summary>
    /// Redis缓存
    /// </summary>
    public RedisOption Redis { get; set; }

    public void PostConfigure(CacheOptions options, IConfiguration configuration)
    {
        options.Prefix = string.IsNullOrWhiteSpace(options.Prefix) ? "" : options.Prefix.Trim();
    }
}

/// <summary>
/// Redis缓存
/// </summary>
public sealed class RedisOption : RedisOptions
{
    /// <summary>
    /// 最大消息大小
    /// </summary>
    public int MaxMessageSize { get; set; }
}

/// <summary>
/// 集群配置选项
/// </summary>
public sealed class ClusterOptions : IConfigurableOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 服务器标识
    /// </summary>
    public string ServerId { get; set; }

    /// <summary>
    /// 服务器IP
    /// </summary>
    public string ServerIp { get; set; }

    /// <summary>
    /// SignalR配置
    /// </summary>
    public ClusterSignalR SignalR { get; set; }

    /// <summary>
    /// 数据保护key
    /// </summary>
    public string DataProtecteKey { get; set; }

    /// <summary>
    /// 是否哨兵模式
    /// </summary>
    public bool IsSentinel { get; set; }

    /// <summary>
    /// 哨兵配置
    /// </summary>
    public StackExchangeSentinelConfig SentinelConfig { get; set; }
}

/// <summary>
/// 集群SignalR配置
/// </summary>
public sealed class ClusterSignalR
{
    /// <summary>
    /// Redis连接字符串
    /// </summary>
    public string RedisConfiguration { get; set; }

    /// <summary>
    /// 缓存前缀
    /// </summary>
    public string ChannelPrefix { get; set; }
}

/// <summary>
/// 哨兵配置
/// </summary>
public sealed class StackExchangeSentinelConfig
{
    /// <summary>
    /// master名称
    /// </summary>
    public string ServiceName { get; set; }

    /// <summary>
    /// master访问密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 哨兵访问密码
    /// </summary>
    public string SentinelPassword { get; set; }

    /// <summary>
    /// 哨兵端口
    /// </summary>
    public List<string> EndPoints { get; set; }

    /// <summary>
    /// 默认库
    /// </summary>
    public int DefaultDb { get; set; }

    /// <summary>
    /// 主前缀
    /// </summary>
    public string MainPrefix { get; set; }

    /// <summary>
    /// SignalR前缀
    /// </summary>
    public string SignalRChannelPrefix { get; set; }
}