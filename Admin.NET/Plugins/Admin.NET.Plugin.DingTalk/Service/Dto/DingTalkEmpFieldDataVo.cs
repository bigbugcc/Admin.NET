// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.DingTalk;

public class DingTalkEmpFieldDataVo
{
    /// <summary>
    /// 字段名称
    /// </summary>
    [Newtonsoft.Json.JsonProperty("field_name")]
    [System.Text.Json.Serialization.JsonPropertyName("field_name")]
    public string FieldName { get; set; }

    /// <summary>
    /// 字段标识
    /// </summary>
    [Newtonsoft.Json.JsonProperty("field_code")]
    [System.Text.Json.Serialization.JsonPropertyName("field_code")]
    public string FieldCode { get; set; }

    /// <summary>
    /// 分组标识
    /// </summary>
    [Newtonsoft.Json.JsonProperty("group_id")]
    [System.Text.Json.Serialization.JsonPropertyName("group_id")]
    public string GroupId { get; set; }

    /// <summary>
    ///
    /// </summary>
    [Newtonsoft.Json.JsonProperty("field_value_list")]
    [System.Text.Json.Serialization.JsonPropertyName("field_value_list")]
    public List<DingTalkFieldValueVo> FieldValueList { get; set; }
}