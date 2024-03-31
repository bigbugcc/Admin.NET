// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

Serve.Run(RunOptions.Default.AddWebComponent<WebComponent>());

public class WebComponent : IWebComponent
{
    public void Load(WebApplicationBuilder builder, ComponentContext componentContext)
    {
        // 设置日志过滤
        builder.Logging.AddFilter((provider, category, logLevel) =>
        {
            return !new[] { "Microsoft.Hosting", "Microsoft.AspNetCore" }.Any(u => category.StartsWith(u)) && logLevel >= LogLevel.Information;
        });

        // 设置接口超时时间和上传大小
        builder.Configuration.Get<WebHostBuilder>().ConfigureKestrel(u =>
        {
            u.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
            u.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(30);
            u.Limits.MaxRequestBodySize = null;
        });
    }
}