// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Text.Json.Serialization;

namespace Admin.NET.Core;

/// <summary>
/// HTTP 作业消息
/// </summary>
public sealed class HttpJobMessage
{
    /// <summary>
    /// 请求地址
    /// </summary>
    public string RequestUri { get; set; }

    /// <summary>
    /// 请求方法
    /// </summary>
    public HttpMethod HttpMethod { get; set; } = HttpMethod.Get;

    /// <summary>
    /// 请求头
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 请求报文体
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// 请求客户端名称
    /// </summary>
    public string ClientName { get; set; } = nameof(HttpJob);

    /// <summary>
    /// 确保请求成功，否则抛异常
    /// </summary>
    public bool EnsureSuccessStatusCode { get; set; } = true;

    /// <summary>
    /// 超时时间（毫秒）
    /// </summary>
    public int? Timeout { get; set; }

    /// <summary>
    /// 是否打印 HTTP 响应内容
    /// </summary>
    /// <remarks>默认 true（打印）</remarks>
    public bool PrintResponseContent { get; set; } = true;

    /// <summary>
    /// 作业组名称
    /// </summary>
    [JsonIgnore]
    public string GroupName { get; set; }

    /// <summary>
    /// 描述信息
    /// </summary>
    [JsonIgnore]
    public string Description { get; set; }
}