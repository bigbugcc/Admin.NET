// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.DingTalk;

public class DingTalkFieldValueVo
{
    /// <summary>
    /// 第几条的明细标识，下标从0开始
    /// </summary>
    [Newtonsoft.Json.JsonProperty("item_index")]
    [System.Text.Json.Serialization.JsonPropertyName("item_index")]
    public int ItemIndex { get; set; }

    /// <summary>
    /// 字段展示值，选项类型字段对应选项的value
    /// </summary>
    [Newtonsoft.Json.JsonProperty("label")]
    [System.Text.Json.Serialization.JsonPropertyName("label")]
    public string Label { get; set; }

    /// <summary>
    /// 字段取值，选项类型字段对应选项的key
    /// </summary>
    [Newtonsoft.Json.JsonProperty("value")]
    [System.Text.Json.Serialization.JsonPropertyName("value")]
    public string Value { get; set; }
}