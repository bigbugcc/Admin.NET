﻿// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Admin.NET.Plugin.K3Cloud;

[AppStartup(100)]
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddConfigurableOptions<K3CloudOptions>();
        services.AddRemoteRequest(options =>
        {
            options.AddHttpClient("K3Cloud", u =>
            {
                u.BaseAddress = new Uri(App.GetConfig<K3CloudOptions>("K3Cloud", true).Url);
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}