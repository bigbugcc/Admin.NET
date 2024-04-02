// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

using Elsa;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Admin.NET.Plugin.Elsa;

[AppStartup(100)]
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        //var elsaOptions = App.GetOptions<ElsaOptions>();
        services
            .AddElsa(options => options
                .AddActivitiesFrom<Startup>()
                .AddWorkflowsFrom<Startup>()
                // .AddFeatures(startups, Configuration)
                // .ConfigureWorkflowChannels(options => elsaSection.GetSection("WorkflowChannels").Bind(options))
                .AddHttpActivities(App.Configuration.GetSection("Elsa").GetSection("Server").Bind)
            );

        services
            .AddNotificationHandlersFrom<Startup>()
            .AddElsaApiEndpoints()
            .AddElsaSwagger(options =>
            {
                //options.SwaggerDoc("Elsa", new OpenApiInfo() { Title = "Elsa", Description = "https://v2.elsaworkflows.io/" });
                //options.TagActionsBy(api => new[] { new OpenApiTag { Name = "Elsa", Description = "Elsa Core API Endpoints" } });
                options.TagActionsBy(api => new[] { "Elsa" });
                options.DocInclusionPredicate((docName, description) => true);
            });

        services.AddApiVersioning(options =>
        {
            options.UseApiBehavior = false;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseApiVersioning();
        app.UseHttpActivities();
        app.UseElsaFeatures();
    }
}