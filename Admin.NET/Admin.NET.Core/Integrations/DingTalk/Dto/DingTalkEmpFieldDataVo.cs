// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Integrations;
public class DingTalkEmpFieldDataVo
{
    /// <summary>
    /// 字段名称
    /// </summary>
    [JsonPropertyName("field_name")]
    public string FieldName { get; set; }
    /// <summary>
    /// 字段标识
    /// </summary>
    [JsonPropertyName("field_code")]
    public string FieldCode { get; set; }
    /// <summary>
    /// 分组标识
    /// </summary>
    [JsonPropertyName("group_id")]
    public string GroupId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("field_value_list")]
    public List<DingTalkFieldValueVo> FieldValueList { get; set; }

}
