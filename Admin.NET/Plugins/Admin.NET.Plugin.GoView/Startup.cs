// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

using Admin.NET.Plugin.GoView.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Admin.NET.Plugin.GoView;

[AppStartup(100)]
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // 注册 GoView 规范化处理提供器
        services.AddUnifyProvider<GoViewResultProvider>("GoView");
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}