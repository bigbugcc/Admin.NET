// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 系统菜单类型枚举
/// </summary>
[Description("系统菜单类型枚举")]
public enum MenuTypeEnum
{
    /// <summary>
    /// 目录
    /// </summary>
    [Description("目录")]
    Dir = 1,

    /// <summary>
    /// 菜单
    /// </summary>
    [Description("菜单")]
    Menu = 2,

    /// <summary>
    /// 按钮
    /// </summary>
    [Description("按钮")]
    Btn = 3
}