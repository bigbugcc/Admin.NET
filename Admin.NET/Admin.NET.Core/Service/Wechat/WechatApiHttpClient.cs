// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Newtonsoft.Json;

namespace Admin.NET.Core.Service;

/// <summary>
/// 微信API客户端
/// </summary>
public partial class WechatApiClientFactory : ISingleton
{
    private readonly IHttpClientFactory _httpClientFactory;
    public readonly WechatOptions _wechatOptions;

    public WechatApiClientFactory(IHttpClientFactory httpClientFactory, IOptions<WechatOptions> wechatOptions)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _wechatOptions = wechatOptions.Value ?? throw new ArgumentNullException(nameof(wechatOptions));
    }

    /// <summary>
    /// 微信公众号
    /// </summary>
    /// <returns></returns>
    public WechatApiClient CreateWechatClient()
    {
        if (string.IsNullOrEmpty(_wechatOptions.WechatAppId) || string.IsNullOrEmpty(_wechatOptions.WechatAppSecret))
            throw Oops.Oh("微信公众号配置错误");

        var client = WechatApiClientBuilder.Create(new WechatApiClientOptions()
        {
            AppId = _wechatOptions.WechatAppId,
            AppSecret = _wechatOptions.WechatAppSecret,
            PushToken = _wechatOptions.WechatToken,
            PushEncodingAESKey = _wechatOptions.WechatEncodingAESKey,
        })
        .UseHttpClient(_httpClientFactory.CreateClient(), disposeClient: false) // 设置 HttpClient 不随客户端一同销毁
        .Build();

        client.Configure(config =>
        {
            JsonSerializerSettings jsonSerializerSettings = NewtonsoftJsonSerializer.GetDefaultSerializerSettings();
            jsonSerializerSettings.Formatting = Formatting.Indented;
            config.JsonSerializer = new NewtonsoftJsonSerializer(jsonSerializerSettings); // 指定 System.Text.Json JSON序列化
            // config.JsonSerializer = new SystemTextJsonSerializer(jsonSerializerOptions); // 指定 Newtonsoft.Json  JSON序列化
        });

        return client;
    }

    /// <summary>
    /// 微信小程序
    /// </summary>
    /// <returns></returns>
    public WechatApiClient CreateWxOpenClient()
    {
        if (string.IsNullOrEmpty(_wechatOptions.WxOpenAppId) || string.IsNullOrEmpty(_wechatOptions.WxOpenAppSecret))
            throw Oops.Oh("微信小程序配置错误");

        var client = WechatApiClientBuilder.Create(new WechatApiClientOptions()
        {
            AppId = _wechatOptions.WxOpenAppId,
            AppSecret = _wechatOptions.WxOpenAppSecret,
            PushToken = _wechatOptions.WxToken,
            PushEncodingAESKey = _wechatOptions.WxEncodingAESKey,
        })
        .UseHttpClient(_httpClientFactory.CreateClient(), disposeClient: false) // 设置 HttpClient 不随客户端一同销毁
        .Build();

        client.Configure(config =>
        {
            JsonSerializerSettings jsonSerializerSettings = NewtonsoftJsonSerializer.GetDefaultSerializerSettings();
            jsonSerializerSettings.Formatting = Formatting.Indented;
            config.JsonSerializer = new NewtonsoftJsonSerializer(jsonSerializerSettings); // 指定 System.Text.Json JSON序列化
            // config.JsonSerializer = new SystemTextJsonSerializer(jsonSerializerOptions); // 指定 Newtonsoft.Json  JSON序列化
        });

        return client;
    }
}