// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

using MailKit.Net.Smtp;
using MimeKit;

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统邮件发送服务 💥
/// </summary>
[ApiDescriptionSettings(Order = 370)]
public class SysEmailService : IDynamicApiController, ITransient
{
    private readonly EmailOptions _emailOptions;

    public SysEmailService(IOptions<EmailOptions> emailOptions)
    {
        _emailOptions = emailOptions.Value;
    }

    /// <summary>
    /// 发送邮件 📧
    /// </summary>
    /// <param name="content"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    [DisplayName("发送邮件")]
    public async Task SendEmail([Required] string content, string title = "Admin.NET 系统邮件")
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailOptions.DefaultFromEmail, _emailOptions.DefaultFromEmail));
        message.To.Add(new MailboxAddress(_emailOptions.DefaultToEmail, _emailOptions.DefaultToEmail));
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