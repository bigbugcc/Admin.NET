// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 登录类型枚举
/// </summary>
[Description("登录类型枚举")]
public enum LoginTypeEnum
{
    /// <summary>
    /// PC登录
    /// </summary>
    [Description("PC登录")]
    Login = 1,

    /// <summary>
    /// PC退出
    /// </summary>
    [Description("PC退出")]
    Logout = 2,

    /// <summary>
    /// PC注册
    /// </summary>
    [Description("PC注册")]
    Register = 3
}