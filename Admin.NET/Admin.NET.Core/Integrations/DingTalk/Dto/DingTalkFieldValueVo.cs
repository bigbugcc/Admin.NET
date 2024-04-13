// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Integrations;

public class DingTalkFieldValueVo
{

    /// <summary>
    /// 第几条的明细标识，下标从0开始
    /// </summary>
    [JsonPropertyName("item_index")]
    public int ItemIndex { get; set; }
    /// <summary>
    /// 字段展示值，选项类型字段对应选项的value。
    /// </summary>
    [JsonPropertyName("label")]
    public string Label { get; set; }
    /// <summary>
    /// 字段取值，选项类型字段对应选项的key。
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; }
}