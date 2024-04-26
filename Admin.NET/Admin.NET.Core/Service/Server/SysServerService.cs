// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using AngleSharp.Html.Parser;
using AspNetCoreRateLimit;
using Elastic.Clients.Elasticsearch;
using Lazy.Captcha.Core;
using Magicodes.ExporterAndImporter.Pdf;
using MailKit.Net.Smtp;
using OnceMi.AspNetCore.OSS;

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统服务器监控服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 290)]
public class SysServerService : IDynamicApiController, ITransient
{
    public SysServerService()
    {
    }

    /// <summary>
    /// 获取服务器配置信息 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取服务器配置信息")]
    public dynamic GetServerBase()
    {
        return new
        {
            HostName = Environment.MachineName, // 主机名称
            SystemOs = ComputerUtil.GetOSInfo(),//RuntimeInformation.OSDescription, // 操作系统
            OsArchitecture = Environment.OSVersion.Platform.ToString() + " " + RuntimeInformation.OSArchitecture.ToString(), // 系统架构
            ProcessorCount = Environment.ProcessorCount + " 核", // CPU核心数
            SysRunTime = ComputerUtil.GetRunTime(), // 系统运行时间
            RemoteIp = ComputerUtil.GetIpFromOnline(), // 外网地址
            LocalIp = App.HttpContext?.Connection?.LocalIpAddress.ToString(), // 本地地址
            RuntimeInformation.FrameworkDescription, // NET框架
            Environment = App.HostEnvironment.IsDevelopment() ? "Development" : "Production",
            Wwwroot = App.WebHostEnvironment.WebRootPath, // 网站根目录
            Stage = App.HostEnvironment.IsStaging() ? "Stage环境" : "非Stage环境", // 是否Stage环境
        };
    }

    /// <summary>
    /// 获取服务器使用信息 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取服务器使用信息")]
    public dynamic GetServerUsed()
    {
        var programStartTime = Process.GetCurrentProcess().StartTime;
        var totalMilliseconds = (DateTime.Now - programStartTime).TotalMilliseconds.ToString();
        var ts = totalMilliseconds.Contains('.') ? totalMilliseconds.Split('.')[0] : totalMilliseconds;
        var programRunTime = DateTimeUtil.FormatTime(ts.ParseToLong());

        var memoryMetrics = ComputerUtil.GetComputerInfo();
        return new
        {
            memoryMetrics.FreeRam, // 空闲内存
            memoryMetrics.UsedRam, // 已用内存
            memoryMetrics.TotalRam, // 总内存
            memoryMetrics.RamRate, // 内存使用率
            memoryMetrics.CpuRate, // Cpu使用率
            StartTime = programStartTime.ToString("yyyy-MM-dd HH:mm:ss"), // 服务启动时间
            RunTime = programRunTime, // 服务运行时间
        };
    }

    /// <summary>
    /// 获取服务器磁盘信息 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取服务器磁盘信息")]
    public dynamic GetServerDisk()
    {
        return ComputerUtil.GetDiskInfos();
    }

    /// <summary>
    /// 获取框架主要程序集 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取框架主要程序集")]
    public dynamic GetAssemblyList()
    {
        var furionAssembly = typeof(App).Assembly.GetName();
        var sqlSugarAssembly = typeof(ISqlSugarClient).Assembly.GetName();
        var yitIdAssembly = typeof(YitIdHelper).Assembly.GetName();
        var redisAssembly = typeof(Redis).Assembly.GetName();
        var jsonAssembly = typeof(NewtonsoftJsonMvcCoreBuilderExtensions).Assembly.GetName();
        var excelAssembly = typeof(IExcelImporter).Assembly.GetName();
        var pdfAssembly = typeof(IPdfExporter).Assembly.GetName();
        var captchaAssembly = typeof(ICaptcha).Assembly.GetName();
        var wechatApiAssembly = typeof(WechatApiClient).Assembly.GetName();
        var wechatTenpayAssembly = typeof(WechatTenpayClient).Assembly.GetName();
        var ossAssembly = typeof(IOSSServiceFactory).Assembly.GetName();
        var parserAssembly = typeof(Parser).Assembly.GetName();
        var nestAssembly = typeof(ElasticsearchClient).Assembly.GetName();
        var limitAssembly = typeof(IpRateLimitMiddleware).Assembly.GetName();
        var htmlParserAssembly = typeof(HtmlParser).Assembly.GetName();
        var fluentEmailAssembly = typeof(SmtpClient).Assembly.GetName();

        return new[]
        {
            new { furionAssembly.Name, furionAssembly.Version },
            new { sqlSugarAssembly.Name, sqlSugarAssembly.Version },
            new { yitIdAssembly.Name, yitIdAssembly.Version },
            new { redisAssembly.Name, redisAssembly.Version },
            new { jsonAssembly.Name, jsonAssembly.Version },
            new { excelAssembly.Name, excelAssembly.Version },
            new { pdfAssembly.Name, pdfAssembly.Version },
            new { captchaAssembly.Name, captchaAssembly.Version },
            new { wechatApiAssembly.Name, wechatApiAssembly.Version },
            new { wechatTenpayAssembly.Name, wechatTenpayAssembly.Version },
            new { ossAssembly.Name, ossAssembly.Version },
            new { parserAssembly.Name, parserAssembly.Version },
            new { nestAssembly.Name, nestAssembly.Version },
            new { limitAssembly.Name, limitAssembly.Version },
            new { htmlParserAssembly.Name, htmlParserAssembly.Version },
            new { fluentEmailAssembly.Name, fluentEmailAssembly.Version },
        };
    }
}