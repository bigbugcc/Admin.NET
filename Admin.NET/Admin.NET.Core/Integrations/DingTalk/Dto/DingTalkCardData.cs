// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Integrations;

/// <summary>
/// 卡片公有数据
/// </summary>
public class DingTalkCardData
{
    /// <summary>
    /// 卡片模板内容替换参数，普通文本类型。
    /// </summary>
    [JsonPropertyName("cardParamMap")]
    public DingTalkCardParamMap CardParamMap { get; set; }
    /// <summary>
    /// 卡片模板内容替换参数，多媒体类型。
    /// </summary>
    [JsonPropertyName("cardMediaIdParamMap")]
    public string CardMediaIdParamMap { get; set; }
}

/// <summary>
/// 卡片模板内容替换参数
/// </summary>
public class DingTalkCardParamMap
{
    /// <summary>
    /// 片模板内容替换参数
    /// </summary>
    [JsonPropertyName("sys_full_json_obj")]
    public string SysFullJsonObj { get; set; }
}