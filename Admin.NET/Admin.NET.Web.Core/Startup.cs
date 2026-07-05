// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Core;
using Admin.NET.Core.Service;
using AspNetCoreRateLimit;
using Furion;
using Furion.Logging;
using Furion.SpecificationDocument;
using Furion.VirtualFileServer;
using IPTools.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OnceMi.AspNetCore.OSS;
using Scalar.AspNetCore;
using SixLabors.ImageSharp.Web.DependencyInjection;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

#if NET10_0_OR_GREATER
using Admin.NET.Core.Update;
#endif

namespace Admin.NET.Web.Core;

[AppStartup(int.MaxValue)]
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // 配置选项
        services.AddProjectOptions();

        // 缓存注册
        services.AddCache();
        // SqlSugar
        services.AddSqlSugar();
        // JWT
        services.AddJwt<JwtHandler>(enableGlobalAuthorize: true, jwtBearerConfigure: options =>
        {
            // 实现 JWT 身份验证过程控制
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var httpContext = context.HttpContext;
                    // 若请求 Url 包含 token 参数，则设置 Token 值
                    if (httpContext.Request.Query.ContainsKey("token"))
                        context.Token = httpContext.Request.Query["token"];
                    return Task.CompletedTask;
                }
            };
        }).AddSignatureAuthentication(options =>  // 添加 Signature 身份验证
        {
            options.Events = SysOpenAccessService.GetSignatureAuthenticationEventImpl();
        });

        // 允许跨域
        services.AddCorsAccessor();
        // 远程请求
        services.AddHttpRemote();
        // 任务队列
        services.AddTaskQueue();
        // 任务调度
        services.AddSchedule(options =>
        {
            options.AddPersistence<DbJobPersistence>(); // 添加作业持久化器
            options.AddMonitor<JobMonitor>(); // 添加作业执行监视器
        });
        // 脱敏检测
        services.AddSensitiveDetection();

        // Json序列化设置
        static void SetNewtonsoftJsonSetting(JsonSerializerSettings setting)
        {
            setting.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            setting.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            //setting.Converters.AddDateTimeTypeConverters(localized: false); // 时间本地化
            setting.DateFormatString = "yyyy-MM-dd HH:mm:ss"; // 时间格式化
            setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; // 忽略循环引用
            // setting.ContractResolver = new CamelCasePropertyNamesContractResolver(); // 解决动态对象属性名大写
            // setting.NullValueHandling = NullValueHandling.Ignore; // 忽略空值
            setting.Converters.AddLongTypeConverters(); // long转string（防止js精度溢出） 超过17位开启
            // setting.MetadataPropertyHandling = MetadataPropertyHandling.Ignore; // 解决DateTimeOffset异常
            // setting.DateParseHandling = DateParseHandling.None; // 解决DateTimeOffset异常
            // setting.Converters.Add(new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }); // 解决DateTimeOffset异常
        }
        ;

        services.AddControllersWithViews()
            .AddAppLocalization()
            .AddNewtonsoftJson(options => SetNewtonsoftJsonSetting(options.SerializerSettings))
            //.AddXmlSerializerFormatters()
            //.AddXmlDataContractSerializerFormatters()
            .AddInjectWithUnifyResult<AdminResultProvider>()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All); // 禁止Unicode转码
                options.JsonSerializerOptions.Converters.AddDateTimeTypeConverters("yyyy-MM-dd HH:mm:ss"); // 时间格式化
            });

        // 三方授权登录OAuth
        services.AddOAuth();

        // ElasticSearch
        services.AddElasticSearch();

        // 配置Nginx转发获取客户端真实IP
        // 注1：如果负载均衡不是在本机通过 Loopback 地址转发请求的，一定要加上options.KnownNetworks.Clear()和options.KnownProxies.Clear()
        // 注2：如果设置环境变量 ASPNETCORE_FORWARDEDHEADERS_ENABLED 为 True，则不需要下面的配置代码
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
#if NET8_0
            options.KnownNetworks.Clear();
#elif NET10_0
            options.KnownIPNetworks.Clear();
#endif
            options.KnownProxies.Clear();
        });

        // 限流服务
        services.AddInMemoryRateLimiting();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        // 事件总线
        services.AddEventBus(options =>
        {
            options.UseUtcTimestamp = false;
            // 不启用事件日志
            options.LogEnabled = false;
            // 事件执行器（失败重试）
            options.AddExecutor<RetryEventHandlerExecutor>();
            // 事件执行器（重试后依然处理未处理异常的处理器）
            options.UnobservedTaskExceptionHandler = (obj, args) =>
            {
                if (args.Exception?.Message != null)
                    Log.Error($"EeventBus 有未处理异常 ：{args.Exception?.Message} ", args.Exception);
            };
            // 事件执行器-监视器（每一次处理都会进入）
            options.AddMonitor<EventHandlerMonitor>();

            #region Redis消息队列

            // 替换事件源存储器为Redis
            var cacheOptions = App.GetConfig<CacheOptions>("Cache", true);
            if (cacheOptions.CacheType == CacheTypeEnum.Redis.ToString())
            {
                options.ReplaceStorer(serviceProvider =>
                {
                    var cacheProvider = serviceProvider.GetRequiredService<NewLife.Caching.ICacheProvider>();
                    // 创建默认内存通道事件源对象，可自定义队列路由key，如：adminnet_eventsource_queue
                    return new RedisEventSourceStorer(cacheProvider, "adminnet_eventsource_queue", 3000);
                });
            }

            #endregion Redis消息队列

            #region RabbitMQ消息队列

            //// 创建默认内存通道事件源对象，可自定义队列路由key，如：adminnet
            //var eventBusOpt = App.GetConfig<EventBusOptions>("EventBus", true);
            //var rbmqEventSourceStorer = new RabbitMQEventSourceStore(new ConnectionFactory
            //{
            //    UserName = eventBusOpt.RabbitMQ.UserName,
            //    Password = eventBusOpt.RabbitMQ.Password,
            //    HostName = eventBusOpt.RabbitMQ.HostName,
            //    Port = eventBusOpt.RabbitMQ.Port
            //}, "adminnet", 3000);

            //// 替换默认事件总线存储器
            //options.ReplaceStorer(serviceProvider =>
            //{
            //    return rbmqEventSourceStorer;
            //});

            #endregion RabbitMQ消息队列
        });

        // 图像处理
        services.AddImageSharp();

        // OSS对象存储
        var ossOpt = App.GetConfig<OSSProviderOptions>("OSSProvider", true);
        services.AddOSSService(Enum.GetName(ossOpt.Provider), "OSSProvider");

        // 文件存储服务
        services.AddTransient<SysFileProviderService>();
        services.AddSingleton<IOSSServiceManager, OSSServiceManager>(); // 改为单例以保持缓存
        services.AddTransient<MultiOSSFileProvider>();

        // 模板引擎
        services.AddViewEngine();

        // 即时通讯
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
            options.KeepAliveInterval = TimeSpan.FromSeconds(15); // 服务器端向客户端ping的间隔
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30); // 客户端向服务器端ping的间隔
            options.MaximumReceiveMessageSize = 1024 * 1014 * 10; // 数据包大小10M，默认最大为32K
        }).AddNewtonsoftJsonProtocol(options => SetNewtonsoftJsonSetting(options.PayloadSerializerSettings));

        // 系统日志
        services.AddLoggingSetup();

        // 验证码
        services.AddCaptcha();

        // 控制台logo
        services.AddConsoleLogo();

        //// Swagger 时间格式化
        //services.AddSwaggerGen(c =>
        //{
        //    c.MapType<DateTime>(() => new Microsoft.OpenApi.Models.OpenApiSchema
        //    {
        //        Type = "string",
        //        Format = "date-time",
        //        Example = new Microsoft.OpenApi.Any.OpenApiString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")) // 示例值
        //    });

        //    // 确保生成的文档包含 OpenAPI 版本字段
        //    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        //    {
        //        Version = "v1",
        //        Title = "Admin.NET API",
        //        Description = "Admin.NET 通用权限开发平台"
        //    });
        //    c.OperationFilter<TenantHeaderOperationFilter>();
        //});

        // 将IP地址数据库文件完全加载到内存，提升查询速度（以空间换时间，内存将会增加60-70M）
        IpToolSettings.LoadInternationalDbToMemory = true;
        // 设置默认查询器China和International
        //IpToolSettings.DefalutSearcherType = IpSearcherType.China;
        IpToolSettings.DefalutSearcherType = IpSearcherType.International;

        // 配置gzip与br的压缩等级为最优
        //services.Configure<BrotliCompressionProviderOptions>(options =>
        //{
        //    options.Level = CompressionLevel.Optimal;
        //});
        //services.Configure<GzipCompressionProviderOptions>(options =>
        //{
        //    options.Level = CompressionLevel.Optimal;
        //});
        // 注册压缩响应
        services.AddResponseCompression((options) =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            [
                    "text/html; charset=utf-8",
                    "application/xhtml+xml",
                    "application/atom+xml",
                    "image/svg+xml"
             ]);
        });

        // 注册虚拟文件系统服务
        services.AddVirtualFileServer();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // 响应压缩
        app.UseResponseCompression();

        app.UseForwardedHeaders();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Admin.NET", "Admin.NET");
            await next();
        });

        // 图像处理
        app.UseImageSharp();

        // 特定文件类型（文件后缀）处理
        var contentTypeProvider = FS.GetFileExtensionContentTypeProvider();
        // contentTypeProvider.Mappings[".文件后缀"] = "MIME 类型";
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = contentTypeProvider
        });
        // 二级目录文件路径解析
        if (!string.IsNullOrEmpty(App.Settings.VirtualPath))
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = App.Settings.VirtualPath,
                FileProvider = App.WebHostEnvironment.WebRootFileProvider
            });
        //// 启用HTTPS
        //app.UseHttpsRedirection();

        // 启用OAuth
        app.UseOAuth();

        // 添加状态码拦截中间件
        app.UseUnifyResultStatusCodes();

        // 启用多语言，必须在 UseRouting 之前
        app.UseAppLocalization();

        // 路由注册
        app.UseRouting();

        // 启用跨域，必须在 UseRouting 和 UseAuthentication 之间注册
        app.UseCorsAccessor();

        // 启用鉴权授权
        app.UseAuthentication();
        app.UseAuthorization();

        // 限流组件（在跨域之后）
        app.UseIpRateLimiting();
        app.UseClientRateLimiting();
        app.UsePolicyRateLimit();

        // 任务调度看板
        app.UseScheduleUI(options =>
        {
            options.RequestPath = "/schedule"; // 必须以 / 开头且不以 / 结尾
            options.DisableOnProduction = false; // 是否在生产环境中关闭
            options.DisplayEmptyTriggerJobs = true; // 是否显示空作业触发器的作业
            options.DisplayHead = false; // 是否显示页头
            options.DefaultExpandAllJobs = false; // 是否默认展开所有作业
            options.EnableDirectoryBrowsing = false; // 是否启用目录浏览
            options.Title = "定时任务看板"; // 自定义看板标题

            options.LoginConfig.OnLoging = async (username, password, httpContext) =>
            {
                var res = await httpContext.RequestServices.GetRequiredService<SysAuthService>().SwaggerSubmitUrl(new SpecificationAuth { UserName = username, Password = password });
                return res == 200;
            };
            options.LoginConfig.DefaultUsername = "";
            options.LoginConfig.DefaultPassword = "";
            options.LoginConfig.AppSecret = "0D7174503141C11DC1A79A7C72B44E0B"; // 登录客户端的授权密钥，必须要求不少于16位的密钥
        });

        app.UseInject(string.Empty, options =>
        {
            foreach (var groupInfo in SpecificationDocumentBuilder.GetOpenApiGroups())
            {
                var warning = "本项目遵循技术中立原则，使用者须严格遵守国家相关法律法规，严禁用于任何危害国家安全、破坏社会稳定或侵犯他人合法权益的行为。任何基于本项目的二次开发、修改、分发或实际部署所引发的法律纠纷、行政处罚及侵权赔偿，均由相关行为人自行承担全部法律责任，本团队不承担任何形式的连带责任。";
                groupInfo.Description += $"<br/><u><b><font color='FF0000'> 👮{warning}</font></b></u>";
			}
            options.ConfigureSwagger(m =>
            {
                m.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
            });
        });

#if NET10_0_OR_GREATER
        app.UseAutoVersionUpdate();
#endif

        app.UseEndpoints(endpoints =>
        {
            // 配置 Scalar 第三方 UI 集成（路由前缀一致代表独立，不同则代表共存）
            if (App.GetConfig<bool>("AppSettings:InjectSpecificationDocument", true))
            {
                endpoints.MapScalarApiReference("sapi", options =>
                {
                    options.WithTitle("Admin.NET");

                    // 配置 OpenAPI 文档
                    foreach (var groupInfo in SpecificationDocumentBuilder.GetOpenApiGroups())
                    {
                        options.AddDocument(groupInfo.Group, groupInfo.Title, groupInfo.RouteTemplate);
                    }
                });
            }
            // 注册集线器
            endpoints.MapHubs();

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}