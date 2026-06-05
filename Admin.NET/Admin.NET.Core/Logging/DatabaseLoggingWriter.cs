// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 数据库日志写入器
/// </summary>
public class DatabaseLoggingWriter : IDatabaseLoggingWriter, IDisposable
{
    private readonly ILogger<DatabaseLoggingWriter> _logger;
    private readonly SysConfigService _sysConfigService;
    private readonly IEventPublisher _eventPublisher;
    private readonly IServiceScope _serviceScope;
    private readonly SqlSugarScopeProvider _db;

    public DatabaseLoggingWriter(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<DatabaseLoggingWriter> logger,
        IEventPublisher eventPublisher)
    {
        _serviceScope = serviceScopeFactory.CreateScope();
        _sysConfigService = _serviceScope.ServiceProvider.GetRequiredService<SysConfigService>();
        _eventPublisher = eventPublisher;
        _logger = logger;

        // 切换日志独立数据库
        _db = SqlSugarSetup.LogDb;
    }

    /// <summary>
    /// 批量写入日志
    /// </summary>
    /// <param name="batchLogMsgs">批量日志消息列表</param>
    /// <param name="flush">是否立即刷新</param>
    public async Task WriteAsync(IReadOnlyList<LogMessage> batchLogMsgs, bool flush)
    {
        if (batchLogMsgs == null || batchLogMsgs.Count == 0) return;

        // 分类存储日志集合
        List<SysLogOp> opLogs = [];      // 操作日志
        List<SysLogEx> exLogs = [];      // 异常日志
        List<SysLogVis> visLogs = [];    // 访问日志（登录/退出）
        List<Exception> exceptions = []; // 收集异常用户发送邮件

        // 是否启用操作日志记录
        var opLogEnabled = await _sysConfigService.GetConfigValueByCode<bool>(ConfigConst.SysOpLog);

        // 遍历处理日志
        foreach (var logMsg in batchLogMsgs)
        {
            try
            {
                var (logType, logEntity) = await ProcessLogMessageAsync(logMsg);
                switch (logType)
                {
                    case LogTypeEnum.Visit:
                        visLogs.Add((SysLogVis)logEntity);
                        break;

                    case LogTypeEnum.Operation when opLogEnabled:
                        opLogs.Add((SysLogOp)logEntity);
                        break;

                    case LogTypeEnum.Exception:
                        exLogs.Add((SysLogEx)logEntity);

                        if (logMsg.Exception != null) exceptions.Add(logMsg.Exception); // 收集异常日志发送邮件
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理单条日志失败");
                exceptions.Add(ex);
            }
        }

        // 并行执行批量插入日志
        await Task.WhenAll
        (
            opLogs.Count != 0 ? _db.Insertable(opLogs).ExecuteCommandAsync() : Task.CompletedTask,   // 操作日志
            exLogs.Count != 0 ? _db.Insertable(exLogs).ExecuteCommandAsync() : Task.CompletedTask,   // 异常日志
            visLogs.Count != 0 ? _db.Insertable(visLogs).ExecuteCommandAsync() : Task.CompletedTask  // 访问日志
        );

        //await Task.Delay(50); // 延迟 0.05 秒写入数据库，有效减少高频写入数据库导致死锁问题

        // 批量发送异常邮件
        if (exceptions.Count != 0) await _eventPublisher.PublishAsync(nameof(CommonConst.SendErrorMail), exceptions);
    }

    /// <summary>
    /// 处理单条日志消息
    /// </summary>
    /// <param name="logMsg">日志消息对象</param>
    /// <returns>(日志类型, 日志实体对象)</returns>
    private async Task<(LogTypeEnum, object)> ProcessLogMessageAsync(LogMessage logMsg)
    {
        // 从上下文中获取日志监控信息
        var jsonStr = logMsg.Context?.Get("loggingMonitor")?.ToString();

        // 若没有日志监控信息则是自定义操作日志
        if (string.IsNullOrWhiteSpace(jsonStr))
        {
            var simpleLog = BuildSimpleLog(logMsg);
            return (LogTypeEnum.Operation, simpleLog);
        }

        // 若有日志监控信息则是框架监控日志
        var loggingMonitor = JSON.Deserialize<LoggingMonitorDto>(jsonStr);
        var userInfo = CommonHelper.GetUserInfo(loggingMonitor);
        var detailedLog = await BuildDetailedLogAsync(logMsg, loggingMonitor, userInfo);

        // 异常日志
        if (logMsg.Exception != null || loggingMonitor.Exception != null)
            return (LogTypeEnum.Exception, detailedLog.Adapt<SysLogEx>());

        // 访问日志（登录/退出）
        if (loggingMonitor.ActionName is "login" or "loginPhone" or "logout")
            return (LogTypeEnum.Visit, detailedLog.Adapt<SysLogVis>());

        // 操作日志
        return (LogTypeEnum.Operation, detailedLog.Adapt<SysLogOp>());
    }

    /// <summary>
    /// 构建简单日志实体（无监控信息的自定义日志）
    /// </summary>
    /// <param name="logMsg">日志消息对象</param>
    /// <returns>操作日志实体</returns>
    private SysLogOp BuildSimpleLog(LogMessage logMsg)
    {
        var (actionName, controllerName) = ExtractActionAndController(logMsg.Message);

        return new SysLogOp
        {
            DisplayTitle = logMsg.Context?.Get("Title")?.ToString() ?? "自定义操作日志",     // 显示标题
            LogDateTime = logMsg.LogDateTime,                                                // 日志时间
            ActionName = logMsg.Context?.Get("Action")?.ToString() ?? actionName,            // 操作名称
            ControllerName = logMsg.Context?.Get("Controller")?.ToString() ?? controllerName,// 控制器名称
            EventId = logMsg.EventId.Id,                                                     // 事件ID
            ThreadId = logMsg.ThreadId,                                                      // 线程ID
            TraceId = logMsg.TraceId,                                                        // 追踪ID（用于链路追踪）
            Exception = logMsg.Exception == null ? null : JSON.Serialize(logMsg.Exception),  // 异常信息
            Message = logMsg.Message,                                                        // 日志消息内容
            LogLevel = logMsg.LogLevel,                                                      // 日志级别
            HttpMethod = logMsg.Context?.Get("Method")?.ToString() ?? "",                    // HTTP方法
            RequestUrl = logMsg.Context?.Get("Url")?.ToString() ?? "",                       // 请求URL
            Status = "200"                                                                   // 默认状态码
        };
    }

    /// <summary>
    /// 构建详细日志实体（含监控信息的完整日志）
    /// </summary>
    /// <param name="logMsg">日志消息对象</param>
    /// <param name="loggingMonitor">监控数据传输对象</param>
    /// <param name="userInfo">用户信息</param>
    /// <returns>操作日志实体</returns>
    private async Task<SysLogOp> BuildDetailedLogAsync(LogMessage logMsg, LoggingMonitorDto loggingMonitor, LoggingUserInfo userInfo)
    {
        // 获取客户端真实IP（优先获取 X-Forwarded-For 头，Nginx等代理）
        var remoteIPv4 = loggingMonitor.RequestHeaders?.FirstOrDefault(u => u.Key == "X-Forwarded-For").Value?.ToString() ?? loggingMonitor.RemoteIPv4;

        // 获取IP地理位置信息（省市、经纬度）
        var (ipLocation, longitude, latitude) = CommonHelper.GetIpAddress(remoteIPv4);

        // 解析用户代理（获取操作系统和浏览器信息）
        var (os, browser) = CommonHelper.GetClientDeviceInfo(loggingMonitor.UserAgent);

        return new SysLogOp
        {
            // 基础信息
            ControllerName = loggingMonitor.DisplayName,                              // 控制器名称
            ActionName = loggingMonitor.ActionTypeName,                               // 操作方法名称
            DisplayTitle = loggingMonitor.DisplayTitle,                               // 显示标题
            Status = loggingMonitor.ReturnInformation?.HttpStatusCode?.ToString(),    // HTTP状态码

            // 网络信息
            RemoteIp = remoteIPv4,                                                    // 客户端IP
            Location = ipLocation,                                                    // IP地理位置
            Longitude = (decimal)longitude,                                           // 经度
            Latitude = (decimal)latitude,                                             // 纬度

            // 设备信息
            Browser = browser,                                                        // 浏览器信息
            Os = os,                                                                  // 操作系统信息

            // 性能信息
            Elapsed = loggingMonitor.TimeOperationElapsedMilliseconds,                // 接口执行耗时（毫秒）

            // 请求信息
            Message = logMsg.Message,                                                 // 日志消息
            HttpMethod = loggingMonitor.HttpMethod,                                   // HTTP方法（GET/POST等）
            RequestUrl = loggingMonitor.RequestUrl,                                   // 请求URL
            RequestParam = loggingMonitor.Parameters?.FirstOrDefault()?.Value != null ? JSON.Serialize(loggingMonitor.Parameters.First().Value) : null,  // 请求参数
            ReturnResult = loggingMonitor.ReturnInformation?.Value != null ? JSON.Serialize(loggingMonitor.ReturnInformation.Value) : null,              // 返回结果

            // 异常信息
            Exception = JSON.Serialize(logMsg.Exception ?? loggingMonitor.Exception), // 异常详情

            // 日志元数据
            LogDateTime = logMsg.LogDateTime,                        // 日志时间
            EventId = logMsg.EventId.Id,                             // 事件ID
            ThreadId = logMsg.ThreadId,                              // 线程ID
            TraceId = logMsg.TraceId,                                // 链路追踪ID

            // 用户信息
            Account = userInfo.Account,                              // 用户账号
            RealName = userInfo.RealName,                            // 真实姓名
            CreateUserId = userInfo.UserId,                          // 创建用户ID
            CreateUserName = userInfo.RealName,                      // 创建用户名称
            TenantId = userInfo.TenantId,                            // 租户ID

            // 日志级别
            LogLevel = logMsg.LogLevel                               // 日志级别
        };
    }

    /// <summary>
    /// 根据操作日志内容解析出控制器名称和函数名称
    /// </summary>
    /// <param name="logText">日志文本内容，格式示例："2024-01-01 10:00:00.123 +08:00 [INF] Admin.NET.Core.SomeController.SomeMethod..."</param>
    /// <returns>(操作方法名称, 控制器名称)</returns>
    private static (string actionName, string controllerName) ExtractActionAndController(string logText)
    {
        try
        {
            // 按换行符分割日志内容，并移除空条目（避免空行干扰） 示例日志通常第一行为时间戳和日志级别，第二行为完整的类名.方法名
            var lines = logText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            // 确保至少有两行内容（第一行时间信息，第二行类名.方法名信息）
            if (lines.Length < 2) return (string.Empty, string.Empty);

            // 获取第二行内容，格式如："Admin.NET.Core.SomeController.SomeMethod(...)"
            var secondLine = lines[1];
            // 按点号分割，获取命名空间、类名、方法名等部分 示例：["Admin.NET.Core", "SomeController", "SomeMethod(...)"]
            var parts = secondLine.Split('.');
            // 确保至少有两个部分（控制器和方法）
            if (parts.Length < 2) return (string.Empty, string.Empty);

            // 获取方法名（最后一个部分） 示例：parts[^1] = "SomeMethod(...)"，需要移除参数部分
            var fullActionName = parts[^1];
            var openParenIndex = fullActionName.IndexOf('(');
            var actionName = openParenIndex > 0
                ? fullActionName.Substring(0, openParenIndex).Trim()   // 提取方法名，移除参数
                : fullActionName.Trim();                               // 无参数时直接使用

            // 获取控制器名称（倒数第二个部分） 示例：parts[^2] = "SomeController"
            var controllerName = parts[^2].Trim();
            return (actionName, controllerName);
        }
        catch
        {
            // 解析失败时返回空字符串，避免影响主流程 可能的原因：日志格式异常、空引用、索引越界等
            return (string.Empty, string.Empty);
        }
    }

    /// <summary>
    /// 日志类型枚举
    /// </summary>
    private enum LogTypeEnum
    {
        Operation,  // 操作日志
        Exception,  // 异常日志
        Visit       // 访问日志（登录/退出）
    }

    /// <summary>
    /// 释放服务作用域
    /// </summary>
    public void Dispose()
    {
        _serviceScope.Dispose();
    }
}