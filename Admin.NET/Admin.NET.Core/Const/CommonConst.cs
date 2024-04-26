// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 通用常量
/// </summary>
[Const("平台配置")]
public class CommonConst
{
    /// <summary>
    /// 演示环境开关
    /// </summary>
    public const string SysDemoEnv = "sys_demo";

    /// <summary>
    /// 默认密码
    /// </summary>
    public const string SysPassword = "sys_password";

    /// <summary>
    /// 登录二次验证
    /// </summary>
    public const string SysSecondVer = "sys_second_ver";

    /// <summary>
    /// 开启图形验证码
    /// </summary>
    public const string SysCaptcha = "sys_captcha";

    /// <summary>
    /// 开启水印
    /// </summary>
    public const string SysWatermark = "sys_watermark";

    /// <summary>
    /// 开启操作日志
    /// </summary>
    public const string SysOpLog = "sys_oplog";

    /// <summary>
    /// Token过期时间
    /// </summary>
    public const string SysTokenExpire = "sys_token_expire";

    /// <summary>
    /// RefreshToken过期时间
    /// </summary>
    public const string SysRefreshTokenExpire = "sys_refresh_token_expire";

    /// <summary>
    /// 开启发送异常日志邮件
    /// </summary>
    public const string SysErrorMail = "sys_error_mail";

    /// <summary>
    /// 单用户登录
    /// </summary>
    public const string SysSingleLogin = "sys_single_login";

    /// <summary>
    /// 系统管理员角色编码
    /// </summary>
    public const string SysAdminRole = "sys_admin";

    ///// <summary>
    ///// 开启全局脱敏处理（默认不开启）
    ///// </summary>
    //public static bool SysSensitiveDetection = false;

    /// <summary>
    /// 开启域登录验证
    /// </summary>
    public const string SysDomainLogin = "sys_domain_login";
}