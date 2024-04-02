// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Service;

/// <summary>
/// 用户登录参数
/// </summary>
public class LoginInput
{
    /// <summary>
    /// 账号
    /// </summary>
    /// <example>admin</example>
    [Required(ErrorMessage = "账号不能为空"), MinLength(2, ErrorMessage = "账号不能少于2个字符")]
    public string Account { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    /// <example>123456</example>
    [Required(ErrorMessage = "密码不能为空"), MinLength(3, ErrorMessage = "密码不能少于3个字符")]
    public string Password { get; set; }

    /// <summary>
    /// 验证码Id
    /// </summary>
    public long CodeId { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    public string Code { get; set; }
}

public class LoginPhoneInput
{
    /// <summary>
    /// 手机号码
    /// </summary>
    /// <example>admin</example>
    [Required(ErrorMessage = "手机号码不能为空")]
    [DataValidation(ValidationTypes.PhoneNumber, ErrorMessage = "手机号码不正确")]
    public string Phone { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    /// <example>123456</example>
    [Required(ErrorMessage = "验证码不能为空"), MinLength(4, ErrorMessage = "验证码不能少于4个字符")]
    public string Code { get; set; }
}