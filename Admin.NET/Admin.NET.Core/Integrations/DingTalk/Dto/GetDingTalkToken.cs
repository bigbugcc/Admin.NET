// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Integrations;
public class GetDingTalkTokenInput
{
    /// <summary>
    /// 应用的唯一标识key 
    /// </summary>
    [JsonPropertyName("appkey")]
    public string AppKey { get; set; }
    /// <summary>
    /// 应用的密钥。AppKey和AppSecret可在钉钉开发者后台的应用详情页面获取。
    /// </summary>
    [JsonPropertyName("appsecret")]
    public string AppSecret { get; set; }
}

public class GetDingTalkTokenOutput
{
    /// <summary>
    /// 生成的access_token 
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    /// <summary>
    /// access_token的过期时间，单位秒
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    /// <summary>
    /// 返回码描述。
    /// </summary>
    [JsonPropertyName("errmsg")]
    public string ErrMsg { get; set; }
    /// <summary>
    /// 返回码。
    /// </summary>
    [JsonPropertyName("errcode")]
    public int ErrCode { get; set; }
}

