using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Admin.NET.Plugin.Flow;

[AppStartup(100)]
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // 使用审批流
        app.UseApprovalFlow(); 
    }
}

