// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.DingTalk;

/// <summary>
/// 获取在职员工列表参数
/// </summary>
public class GetDingTalkCurrentEmployeesListInput
{
    /// <summary>
    /// 在职员工状态筛选，可以查询多个状态。不同状态之间使用英文逗号分隔。2：试用期、3：正式、5：待离职、-1：无状态
    /// </summary>
    [Newtonsoft.Json.JsonProperty("status_list")]
    [System.Text.Json.Serialization.JsonPropertyName("status_list")]
    public string StatusList { get; set; }

    /// <summary>
    /// 分页游标，从0开始。根据返回结果里的next_cursor是否为空来判断是否还有下一页，且再次调用时offset设置成next_cursor的值。
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// 分页大小，最大50。
    /// </summary>
    public int Size { get; set; }
}