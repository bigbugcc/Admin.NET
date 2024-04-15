// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统配置表种子数据
/// </summary>
public class SysConfigSeedData : ISqlSugarEntitySeedData<SysConfig>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysConfig> HasData()
    {
        return new[]
        {
            new SysConfig{ Id=1300000000101, Name="演示环境", Code="sys_demo", Value="False", SysFlag=YesNoEnum.Y, Remark="演示环境", OrderNo=1, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000102, Name="默认密码", Code="sys_password", Value="123456", SysFlag=YesNoEnum.Y, Remark="默认密码", OrderNo=2, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000103, Name="记录操作日志", Code="sys_oplog", Value="True", SysFlag=YesNoEnum.Y, Remark="是否记录操作日志", OrderNo=3, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000104, Name="开启单设备登录", Code="sys_single_login", Value="False", SysFlag=YesNoEnum.Y, Remark="是否开启单设备登录", OrderNo=4, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000105, Name="开启登录二次验证", Code="sys_second_ver", Value="False", SysFlag=YesNoEnum.Y, Remark="是否开启登录二次验证", OrderNo=5, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000106, Name="开启图形验证码", Code="sys_captcha", Value="True", SysFlag=YesNoEnum.Y, Remark="是否开启图形验证码", OrderNo=6, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000107, Name="开启水印", Code="sys_watermark", Value="False", SysFlag=YesNoEnum.Y, Remark="是否开启水印", OrderNo=7, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000108, Name="Token过期时间", Code="sys_token_expire", Value="10080", SysFlag=YesNoEnum.Y, Remark="Token过期时间（分钟）", OrderNo=8, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000109, Name="刷新Token过期时间", Code="sys_refresh_token_expire", Value="20160", SysFlag=YesNoEnum.Y, Remark="刷新Token过期时间（分钟）（一般 refresh_token 的有效时间 > 2 * access_token 的有效时间）", OrderNo=9, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000110, Name="发送异常日志邮件", Code="sys_error_mail", Value="True", SysFlag=YesNoEnum.Y, Remark="是否发送异常日志邮件", OrderNo=10, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000111, Name="开启域登录验证", Code="sys_domain_login", Value="False", SysFlag=YesNoEnum.Y, Remark="是否开启域登录验证", OrderNo=11, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
        };
    }
}