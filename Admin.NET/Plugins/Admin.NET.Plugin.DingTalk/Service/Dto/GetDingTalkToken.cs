// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Plugin.DingTalk;


public class GetDingTalkTokenOutput
{
    /// <summary>
    /// 生成的access_token
    /// </summary>
    [Newtonsoft.Json.JsonProperty("access_token")]
    [System.Text.Json.Serialization.JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// access_token的过期时间，单位秒
    /// </summary>
    [Newtonsoft.Json.JsonProperty("expires_in")]
    [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// 返回码描述
    /// </summary>
    public string ErrMsg { get; set; }

    /// <summary>
    /// 返回码
    /// </summary>
    public int ErrCode { get; set; }
}