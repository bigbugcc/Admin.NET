// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Integrations;
public class GetDingTalkCurrentEmployeesListOutput
{
    /// <summary>
    /// 查询到的员工userid列表
    /// </summary>
    [JsonPropertyName("data_list")]
    public List<string> DataList { get; set; }
    /// <summary>
    /// 下一次分页调用的offset值，当返回结果里没有next_cursor时，表示分页结束。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public int? NextCursor { get; set; }

}
