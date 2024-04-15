// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 证件类型枚举
/// </summary>
[Description("证件类型枚举")]
public enum CardTypeEnum
{
    /// <summary>
    /// 身份证
    /// </summary>
    [Description("身份证")]
    IdCard = 0,

    /// <summary>
    /// 护照
    /// </summary>
    [Description("护照")]
    PassportCard = 1,

    /// <summary>
    /// 出生证
    /// </summary>
    [Description("出生证")]
    BirthCard = 2,

    /// <summary>
    /// 港澳台通行证
    /// </summary>
    [Description("港澳台通行证")]
    GatCard = 3,

    /// <summary>
    /// 外国人居留证
    /// </summary>
    [Description("外国人居留证")]
    ForeignCard = 4,

    /// <summary>
    /// 营业执照
    /// </summary>
    [Description("营业执照")]
    License = 5,
}