// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

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