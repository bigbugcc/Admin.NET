// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using AlibabaCloud.SDK.Dysmsapi20170525.Models;

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统短信服务 🧩
/// </summary>
[AllowAnonymous]
[ApiDescriptionSettings(Order = 150)]
public class SysSmsService : IDynamicApiController, ITransient
{
    private readonly SMSOptions _smsOptions;
    private readonly SysCacheService _sysCacheService;

    public SysSmsService(IOptions<SMSOptions> smsOptions,
        SysCacheService sysCacheService)
    {
        _smsOptions = smsOptions.Value;
        _sysCacheService = sysCacheService;
    }

    /// <summary>
    /// 发送短信 📨
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("发送短信")]
    public async Task SendSms([Required] string phoneNumber)
    {
        if (!phoneNumber.TryValidate(ValidationTypes.PhoneNumber).IsValid)
            throw Oops.Oh("请正确填写手机号码");

        // 生成随机验证码
        var random = new Random();
        var verifyCode = random.Next(100000, 999999);

        var templateParam = Clay.Object(new
        {
            code = verifyCode
        });

        var client = CreateClient();
        var sendSmsRequest = new SendSmsRequest
        {
            PhoneNumbers = phoneNumber, // 待发送手机号, 多个以逗号分隔
            SignName = _smsOptions.Aliyun.SignName, // 短信签名
            TemplateCode = _smsOptions.Aliyun.TemplateCode, // 短信模板
            TemplateParam = templateParam.ToString(), // 模板中的变量替换JSON串
            OutId = YitIdHelper.NextId().ToString()
        };
        var sendSmsResponse = client.SendSms(sendSmsRequest);
        if (sendSmsResponse.Body.Code == "OK" && sendSmsResponse.Body.Message == "OK")
        {
            // var bizId = sendSmsResponse.Body.BizId;
            _sysCacheService.Set($"{CacheConst.KeyPhoneVerCode}{phoneNumber}", verifyCode, TimeSpan.FromSeconds(60));
        }
        else
        {
            throw Oops.Oh($"短信发送失败：{sendSmsResponse.Body.Code}-{sendSmsResponse.Body.Message}");
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// 阿里云短信配置
    /// </summary>
    /// <returns></returns>
    private AlibabaCloud.SDK.Dysmsapi20170525.Client CreateClient()
    {
        var config = new AlibabaCloud.OpenApiClient.Models.Config
        {
            AccessKeyId = _smsOptions.Aliyun.AccessKeyId,
            AccessKeySecret = _smsOptions.Aliyun.AccessKeySecret,
            Endpoint = "dysmsapi.aliyuncs.com"
        };
        return new AlibabaCloud.SDK.Dysmsapi20170525.Client(config);
    }
}