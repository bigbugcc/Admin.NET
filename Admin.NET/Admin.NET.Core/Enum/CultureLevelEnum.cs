// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 文化程度枚举
/// </summary>
[Description("文化程度枚举")]
public enum CultureLevelEnum
{
    /// <summary>
    /// 其他
    /// </summary>
    [Description("其他")]
    Level0 = 0,

    /// <summary>
    /// 文盲
    /// </summary>
    [Description("文盲")]
    Level1 = 1,

    /// <summary>
    /// 小学
    /// </summary>
    [Description("小学")]
    Level2 = 2,

    /// <summary>
    /// 初中
    /// </summary>
    [Description("初中")]
    Level3 = 3,

    /// <summary>
    /// 普通高中
    /// </summary>
    [Description("普通高中")]
    Level4 = 4,

    /// <summary>
    /// 技工学校
    /// </summary>
    [Description("技工学校")]
    Level5 = 5,

    /// <summary>
    /// 职业教育
    /// </summary>
    [Description("职业教育")]
    Level6 = 6,

    /// <summary>
    /// 职业高中
    /// </summary>
    [Description("职业高中")]
    Level7 = 7,

    /// <summary>
    /// 中等专科
    /// </summary>
    [Description("中等专科")]
    Level8 = 8,

    /// <summary>
    /// 大学专科
    /// </summary>
    [Description("大学专科")]
    Level9 = 9,

    /// <summary>
    /// 大学本科
    /// </summary>
    [Description("大学本科")]
    Level10 = 10,

    /// <summary>
    /// 硕士研究生
    /// </summary>
    [Description("硕士研究生")]
    Level11 = 11,

    /// <summary>
    /// 博士研究生
    /// </summary>
    [Description("博士研究生")]
    Level12 = 12,
}