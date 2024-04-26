// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.DingTalk;

/// <summary>
/// 钉钉基础响应结果
/// </summary>
/// <typeparam name="T">Data</typeparam>
public class DingTalkBaseResponse<T>
{
    /// <summary>
    /// 返回结果
    /// </summary>
    public T Result { get; set; }

    /// <summary>
    /// 返回码
    /// </summary>
    public int ErrCode { get; set; }

    /// <summary>
    /// 返回码描述。
    /// </summary>
    public string ErrMsg { get; set; }

    /// <summary>
    /// 是否调用成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 请求Id
    /// </summary>
    [Newtonsoft.Json.JsonProperty("request_id")]
    [System.Text.Json.Serialization.JsonPropertyName("request_id")]
    public string RequestId { get; set; }
}