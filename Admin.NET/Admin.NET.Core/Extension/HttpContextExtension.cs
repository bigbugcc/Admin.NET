// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Microsoft.AspNetCore.Authentication;

namespace Admin.NET.Core;

public static class HttpContextExtension
{
    public static async Task<AuthenticationScheme[]> GetExternalProvidersAsync(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

        return (from scheme in await schemes.GetAllSchemesAsync()
                where !string.IsNullOrEmpty(scheme.DisplayName)
                select scheme).ToArray();
    }

    public static async Task<bool> IsProviderSupportedAsync(this HttpContext context, string provider)
    {
        ArgumentNullException.ThrowIfNull(context);

        return (from scheme in await context.GetExternalProvidersAsync()
                where string.Equals(scheme.Name, provider, StringComparison.OrdinalIgnoreCase)
                select scheme).Any();
    }

    /// <summary>
    /// 获取设备信息
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetClientDeviceInfo(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return CommonUtil.GetClientDeviceInfo(context.Request.Headers.UserAgent);
    }

    /// <summary>
    /// 获取浏览器信息
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetClientBrowser(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        string userAgent = context.Request.Headers.UserAgent;
        try
        {
            if (userAgent != null)
            {
                var client = Parser.GetDefault().Parse(userAgent);
                if (client.Device.IsSpider)
                    return "爬虫";
                return $"{client.UA.Family} {client.UA.Major}.{client.UA.Minor} / {client.Device.Family}";
            }
        }
        catch
        { }
        return "未知";
    }

    /// <summary>
    /// 获取操作系统信息
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetClientOs(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        string userAgent = context.Request.Headers.UserAgent;
        try
        {
            if (userAgent != null)
            {
                var client = Parser.GetDefault().Parse(userAgent);
                if (client.Device.IsSpider)
                    return "爬虫";
                return $"{client.OS.Family} {client.OS.Major} {client.OS.Minor}";
            }
        }
        catch
        { }
        return "未知";
    }
}