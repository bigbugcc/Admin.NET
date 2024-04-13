// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Integrations;

/// <summary>
/// 钉钉基础响应结果
/// </summary>
/// <typeparam name="T">Data</typeparam>
public class DingTalkBaseResponse<T>
{
    /// <summary>
    /// 返回结果
    /// </summary>
    [JsonPropertyName("result")]
    public T Result { get; set; }
    /// <summary>
    /// 返回码
    /// </summary>
    [JsonPropertyName("errcode")]
    public int ErrCode { get; set; }
    /// <summary>
    /// 返回码描述。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string ErrMsg { get; set; }
    /// <summary>
    /// 是否调用成功
    /// </summary>
    /// <remarks>
    /// true：成功；
    /// false：失败；
    /// </remarks>
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    /// <summary>
    /// 请求ID。
    /// </summary>
    [JsonPropertyName("request_id")]
    public string RequestId { get; set; }

}
