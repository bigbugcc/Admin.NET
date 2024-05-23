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
            new SysConfig{ Id=1300000000101, Name="演示环境", Code="sys_demo", Value="False", SysFlag=YesNoEnum.Y, Remark="演示环境", OrderNo=10, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000111, Name="默认密码", Code="sys_password", Value="123456", SysFlag=YesNoEnum.Y, Remark="默认密码", OrderNo=20, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000121, Name="记录操作日志", Code="sys_oplog", Value="True", SysFlag=YesNoEnum.Y, Remark="是否记录操作日志", OrderNo=30, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000131, Name="开启单设备登录", Code="sys_single_login", Value="False", SysFlag=YesNoEnum.Y, Remark="是否开启单设备登录", OrderNo=40, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000141, Name="开启登录二次验证", Code="sys_second_ver", Value="False", SysFlag=YesNoEnum.Y, Remark="是否开启登录二次验证", OrderNo=50, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000151, Name="开启图形验证码", Code="sys_captcha", Value="True", SysFlag=YesNoEnum.Y, Remark="是否开启图形验证码", OrderNo=60, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000161, Name="Token过期时间", Code="sys_token_expire", Value="10080", SysFlag=YesNoEnum.Y, Remark="Token过期时间（分钟）", OrderNo=70, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000171, Name="刷新Token过期时间", Code="sys_refresh_token_expire", Value="20160", SysFlag=YesNoEnum.Y, Remark="刷新Token过期时间（分钟）（一般 refresh_token 的有效时间 > 2 * access_token 的有效时间）", OrderNo=80, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000181, Name="发送异常日志邮件", Code="sys_error_mail", Value="True", SysFlag=YesNoEnum.Y, Remark="是否发送异常日志邮件", OrderNo=90, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000191, Name="开启域登录验证", Code="sys_domain_login", Value="False", SysFlag=YesNoEnum.Y, Remark="是否开启域登录验证", OrderNo=100, GroupCode="Default", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000211, Name="系统主标题", Code="sys_web_title", Value="Admin.NET", SysFlag=YesNoEnum.Y, Remark="系统主标题", OrderNo=110, GroupCode="WebConfig", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000221, Name="系统副标题", Code="sys_web_viceTitle", Value="Admin.NET", SysFlag=YesNoEnum.Y, Remark="系统副标题", OrderNo=120, GroupCode="WebConfig", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000231, Name="系统描述", Code="sys_web_viceDesc", Value="站在巨人肩膀上的 .NET 通用权限开发框架", SysFlag=YesNoEnum.Y, Remark="系统描述", OrderNo=130, GroupCode="WebConfig", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000241, Name="水印内容", Code="sys_web_watermark", Value="Admin.NET", SysFlag=YesNoEnum.Y, Remark="水印内容", OrderNo=140, GroupCode="WebConfig", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysConfig{ Id=1300000000251, Name="版权说明", Code="sys_web_copyright", Value="Copyright © 2021-2014 Admin.NET All rights reserved.", SysFlag=YesNoEnum.Y, Remark="版权说明", OrderNo=150, GroupCode="WebConfig", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
    };
    }
}