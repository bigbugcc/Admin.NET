﻿// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using MailKit.Net.Smtp;
using MimeKit;

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统邮件发送服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 370)]
public class SysEmailService : IDynamicApiController, ITransient
{
    private readonly EmailOptions _emailOptions;
    private readonly SysConfigService _sysConfigService;

    public SysEmailService(IOptions<EmailOptions> emailOptions, SysConfigService sysConfigService)
    {
        _emailOptions = emailOptions.Value;
        _sysConfigService = sysConfigService;
    }

    /// <summary>
    /// 发送邮件 📧
    /// </summary>
    /// <param name="content"></param>
    /// <param name="title"></param>
    /// <param name="toEmail"></param>
    /// <returns></returns>
    [DisplayName("发送邮件")]
    public async Task SendEmail([Required] string content, string title = "", string toEmail = "")
    {
        var webTitle = await _sysConfigService.GetConfigValue<string>(ConfigConst.SysWebTitle);
        title = string.IsNullOrWhiteSpace(title) ? $"{webTitle} 系统邮件" : title;
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailOptions.DefaultFromEmail, _emailOptions.DefaultFromEmail));
        if (string.IsNullOrWhiteSpace(toEmail))
            message.To.Add(new MailboxAddress(_emailOptions.DefaultToEmail, _emailOptions.DefaultToEmail));
        else
            message.To.Add(new MailboxAddress(toEmail, toEmail));
        message.Subject = title;
        message.Body = new TextPart("html")
        {
            Text = content
        };

        using var client = new SmtpClient();
        client.Connect(_emailOptions.Host, _emailOptions.Port, _emailOptions.EnableSsl);
        client.Authenticate(_emailOptions.UserName, _emailOptions.Password);
        client.Send(message);
        client.Disconnect(true);

        await Task.CompletedTask;
    }
}