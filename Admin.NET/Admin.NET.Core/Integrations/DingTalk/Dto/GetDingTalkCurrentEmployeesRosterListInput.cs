// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Integrations;
public class GetDingTalkCurrentEmployeesRosterListInput
{
    /// <summary>
    /// 员工的userid列表，多个userid之间使用逗号分隔，一次最多支持传100个值。
    /// </summary>
    [JsonPropertyName("userid_list")]
    public string UserIdList { get; set; }
    /// <summary>
    /// 需要获取的花名册字段field_code值列表，多个字段之间使用逗号分隔，一次最多支持传100个值。
    /// </summary>
    [JsonPropertyName("field_filter_list")]
    public string FieldFilterList { get; set; }
    /// <summary>
    /// 应用的AgentId
    /// </summary>
    [JsonPropertyName("agentid")]
    public string AgentId { get; set; }
}
