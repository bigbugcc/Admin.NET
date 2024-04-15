// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// ES配置选项
/// </summary>
public class ElasticSearchOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// ES认证类型，可选 Basic、ApiKey、Base64ApiKey
    /// </summary>
    public ElasticSearchAuthTypeEnum AuthType { get; set; }

    /// <summary>
    /// Basic认证的用户名
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Basic认证的密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// ApiKey认证的ApiId
    /// </summary>
    public string ApiId { get; set; }

    /// <summary>
    /// ApiKey认证的ApiKey
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Base64ApiKey认证时加密的加密字符串
    /// </summary>
    public string Base64ApiKey { get; set; }

    /// <summary>
    /// ES使用Https时的证书指纹，使用证书请自行实现
    /// <para>https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/connecting.html</para>
    /// </summary>
    public string Fingerprint { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public List<string> ServerUris { get; set; } = new List<string>();

    /// <summary>
    /// 索引
    /// </summary>
    public string DefaultIndex { get; set; }
}