// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

public static class LoggingSetup
{
    /// <summary>
    /// 日志注册
    /// </summary>
    /// <param name="services"></param>
    public static void AddLoggingSetup(this IServiceCollection services)
    {
        // 日志监听
        services.AddMonitorLogging(options =>
        {
            options.IgnorePropertyNames = new[] { "Byte" };
            options.IgnorePropertyTypes = new[] { typeof(byte[]) };
        });

        // 控制台日志
        var consoleLog = App.GetConfig<bool>("Logging:Monitor:ConsoleLog", true);
        services.AddConsoleFormatter(options =>
        {
            options.DateFormat = "yyyy-MM-dd HH:mm:ss(zzz) dddd";
            //options.WithTraceId = true; // 显示线程Id
            //options.WithStackFrame = true; // 显示程序集
            options.WriteFilter = (logMsg) =>
            {
                return consoleLog;
            };
        });

        // 日志写入文件
        if (App.GetConfig<bool>("Logging:File:Enabled", true))
        {
            var loggingMonitorSettings = App.GetConfig<LoggingMonitorSettings>("Logging:Monitor", true);
            Array.ForEach(new[] { LogLevel.Information, LogLevel.Warning, LogLevel.Error }, logLevel =>
            {
                services.AddFileLogging(options =>
                {
                    options.WithTraceId = true; // 显示线程Id
                    options.WithStackFrame = true; // 显示程序集
                    options.FileNameRule = fileName => string.Format(fileName, DateTime.Now, logLevel.ToString()); // 每天创建一个文件
                    options.WriteFilter = logMsg => logMsg.LogLevel == logLevel; // 日志级别
                    options.HandleWriteError = (writeError) => // 写入失败时启用备用文件
                    {
                        writeError.UseRollbackFileName(Path.GetFileNameWithoutExtension(writeError.CurrentFileName) + "-oops" + Path.GetExtension(writeError.CurrentFileName));
                    };
                    if (loggingMonitorSettings.JsonBehavior == JsonBehavior.OnlyJson)
                    {
                        options.MessageFormat = LoggerFormatter.Json;
                        // options.MessageFormat = LoggerFormatter.JsonIndented;
                        options.MessageFormat = (logMsg) =>
                        {
                            var jsonString = logMsg.Context.Get("loggingMonitor");
                            return jsonString?.ToString();
                        };
                    }
                });
            });
        }

        // 日志写入ElasticSearch
        if (App.GetConfig<bool>("Logging:ElasticSearch:Enabled", true))
        {
            services.AddDatabaseLogging<ElasticSearchLoggingWriter>(options =>
            {
                options.WithTraceId = true; // 显示线程Id
                options.WithStackFrame = true; // 显示程序集
                options.IgnoreReferenceLoop = false; // 忽略循环检测
                options.MessageFormat = LoggerFormatter.Json;
                options.WriteFilter = (logMsg) =>
                {
                    return logMsg.LogName == "System.Logging.LoggingMonitor"; // 只写LoggingMonitor日志
                };
            });
        }

        // 日志写入数据库
        if (App.GetConfig<bool>("Logging:Database:Enabled", true))
        {
            services.AddDatabaseLogging<DatabaseLoggingWriter>(options =>
            {
                options.WithTraceId = true; // 显示线程Id
                options.WithStackFrame = true; // 显示程序集
                options.IgnoreReferenceLoop = false; // 忽略循环检测
                options.WriteFilter = (logMsg) =>
                {
                    return logMsg.LogName == "System.Logging.LoggingMonitor"; // 只写LoggingMonitor日志
                };
            });
        }
    }
}