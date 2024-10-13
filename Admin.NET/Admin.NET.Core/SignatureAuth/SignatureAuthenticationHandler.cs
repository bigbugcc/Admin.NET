// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Encodings.Web;

namespace Admin.NET.Core;

/// <summary>
/// Signature 身份验证处理
/// </summary>
public sealed class SignatureAuthenticationHandler : AuthenticationHandler<SignatureAuthenticationOptions>
{
#if NET6_0

    public SignatureAuthenticationHandler(IOptionsMonitor<SignatureAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

#else
    public SignatureAuthenticationHandler(IOptionsMonitor<SignatureAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }
#endif

    private new SignatureAuthenticationEvent Events
    {
        get => (SignatureAuthenticationEvent)base.Events;
        set => base.Events = value;
    }

    /// <summary>
    /// 确保创建的 Event 类型是 DigestEvents
    /// </summary>
    /// <returns></returns>
    protected override Task<object> CreateEventsAsync() => throw new NotImplementedException($"{nameof(SignatureAuthenticationOptions)}.{nameof(SignatureAuthenticationOptions.Events)} 需要提供一个实例");

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var accessKey = Request.Headers["accessKey"].FirstOrDefault();
        var timestampStr = Request.Headers["timestamp"].FirstOrDefault(); // 精确到秒
        var nonce = Request.Headers["nonce"].FirstOrDefault();
        var sign = Request.Headers["sign"].FirstOrDefault();

        if (string.IsNullOrEmpty(accessKey))
            return await AuthenticateResultFailAsync("accessKey 不能为空");
        if (string.IsNullOrEmpty(timestampStr))
            return await AuthenticateResultFailAsync("timestamp 不能为空");
        if (string.IsNullOrEmpty(nonce))
            return await AuthenticateResultFailAsync("nonce 不能为空");
        if (string.IsNullOrEmpty(sign))
            return await AuthenticateResultFailAsync("sign 不能为空");

        // 验证请求数据是否在可接受的时间内
        if (!long.TryParse(timestampStr, out var timestamp))
            return await AuthenticateResultFailAsync("timestamp 值不合法");

        var requestDate = DateTimeUtil.ConvertUnixTime(timestamp);

#if NET6_0
        var utcNow = Clock.UtcNow;
#else
        var utcNow = TimeProvider.GetUtcNow();
#endif
        if (requestDate > utcNow.Add(Options.AllowedDateDrift).LocalDateTime || requestDate < utcNow.Subtract(Options.AllowedDateDrift).LocalDateTime)
            return await AuthenticateResultFailAsync("timestamp 值已超过允许的偏差范围");

        // 获取 accessSecret
        var getAccessSecretContext = new GetAccessSecretContext(Context, Scheme, Options) { AccessKey = accessKey };
        var accessSecret = await Events.GetAccessSecret(getAccessSecretContext);
        if (string.IsNullOrEmpty(accessSecret))
            return await AuthenticateResultFailAsync("accessKey 无效");

        // 校验签名
        var appSecretByte = Encoding.UTF8.GetBytes(accessSecret);
        string serverSign = SignData(appSecretByte, GetMessageForSign(Context));

        if (serverSign != sign)
            return await AuthenticateResultFailAsync("sign 无效的签名");

        // 重放检测
        var cache = App.GetRequiredService<SysCacheService>();
        var cacheKey = $"{CacheConst.KeyOpenAccessNonce}{accessKey}|{nonce}";
        if (cache.ExistKey(cacheKey))
            return await AuthenticateResultFailAsync("重复的请求");
        cache.Set(cacheKey, null, Options.AllowedDateDrift * 2); // 缓存过期时间为偏差范围时间的2倍

        // 已验证成功
        var signatureValidatedContext = new SignatureValidatedContext(Context, Scheme, Options)
        {
            Principal = new ClaimsPrincipal(new ClaimsIdentity(SignatureAuthenticationDefaults.AuthenticationScheme)),
            AccessKey = accessKey
        };
        await Events.Validated(signatureValidatedContext);
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (signatureValidatedContext.Result != null)
            return signatureValidatedContext.Result;

        // ReSharper disable once HeuristicUnreachableCode
        signatureValidatedContext.Success();
        return signatureValidatedContext.Result;
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var authResult = await HandleAuthenticateOnceSafeAsync();
        var challengeContext = new SignatureChallengeContext(Context, Scheme, Options, properties)
        {
            AuthenticateFailure = authResult.Failure,
        };
        await Events.Challenge(challengeContext);
        // 质询已处理
        if (challengeContext.Handled) return;

        await base.HandleChallengeAsync(properties);
    }

    /// <summary>
    /// 获取用于签名的消息
    /// </summary>
    /// <returns></returns>
    private static string GetMessageForSign(HttpContext context)
    {
        var method = context.Request.Method; // 请求方法（大写）
        var url = context.Request.Path; // 请求 url，去除协议、域名、参数，以 / 开头
        var accessKey = context.Request.Headers["accessKey"].FirstOrDefault(); // 身份标识
        var timestamp = context.Request.Headers["timestamp"].FirstOrDefault(); // 时间戳，精确到秒
        var nonce = context.Request.Headers["nonce"].FirstOrDefault(); // 唯一随机数

        return $"{method}&{url}&{accessKey}&{timestamp}&{nonce}";
    }

    /// <summary>
    /// 对数据进行签名
    /// </summary>
    /// <param name="secret"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private static string SignData(byte[] secret, string data)
    {
        if (secret == null)
            throw new ArgumentNullException(nameof(secret));

        if (data == null)
            throw new ArgumentNullException(nameof(data));

        using HMAC hmac = new HMACSHA256();
        hmac.Key = secret;
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(data)));
    }

    /// <summary>
    /// 返回验证失败结果，并在 Items 中增加 <see cref="SignatureAuthenticationDefaults.AuthenticateFailMsgKey"/>，记录身份验证失败消息
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private Task<AuthenticateResult> AuthenticateResultFailAsync(string message)
    {
        // 写入身份验证失败消息
        Context.Items[SignatureAuthenticationDefaults.AuthenticateFailMsgKey] = message;
        return Task.FromResult(AuthenticateResult.Fail(message));
    }
}