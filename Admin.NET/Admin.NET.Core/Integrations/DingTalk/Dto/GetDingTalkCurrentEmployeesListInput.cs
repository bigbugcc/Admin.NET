// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Integrations;
/// <summary>
/// 获取在职员工列表参数
/// </summary>
public class GetDingTalkCurrentEmployeesListInput
{
    /// <summary>
    /// 在职员工状态筛选，可以查询多个状态。不同状态之间使用英文逗号分隔。
    /// </summary>
    /// <remarks>
    /// 2：试用期
    /// 3：正式
    /// 5：待离职
    /// -1：无状态
    /// </remarks>
    [JsonPropertyName("status_list")]
    public string StatusList { get; set; }
    /// <summary>
    /// 分页游标，从0开始。根据返回结果里的next_cursor是否为空来判断是否还有下一页，且再次调用时offset设置成next_cursor的值。
    /// </summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }
    /// <summary>
    /// 分页大小，最大50。
    /// </summary>
    [JsonPropertyName("size")]
    public int Size { get; set; }
}
