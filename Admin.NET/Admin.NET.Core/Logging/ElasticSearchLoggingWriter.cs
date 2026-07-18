// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Elastic.Clients.Elasticsearch;

namespace Admin.NET.Core;

/// <summary>
/// ES日志写入器
/// </summary>
public class ElasticSearchLoggingWriter : IDatabaseLoggingWriter, IDisposable
{
    private readonly IServiceScope _serviceScope;
    private readonly ElasticsearchClient _esClient;
    private readonly SysConfigService _sysConfigService;

    public ElasticSearchLoggingWriter(IServiceScopeFactory scopeFactory)
    {
        _serviceScope = scopeFactory.CreateScope();
        _esClient = _serviceScope.ServiceProvider.GetRequiredService<ElasticsearchClient>();
        _sysConfigService = _serviceScope.ServiceProvider.GetRequiredService<SysConfigService>();
    }

    /// <summary>
    /// 批量写入日志到 Elasticsearch
    /// </summary>
    /// <param name="batchLogMsgs">批量日志消息列表</param>
    /// <param name="flush">是否立即刷新（保留参数，便于扩展）</param>
    public async Task WriteAsync(IReadOnlyList<LogMessage> batchLogMsgs, bool flush)
    {
        if (batchLogMsgs == null || batchLogMsgs.Count == 0) return;

        // 是否启用操作日志
        var opLogEnabled = await _sysConfigService.GetConfigValueByCode<bool>(ConfigConst.SysOpLog);
        if (!opLogEnabled) return;

        // 操作日志集合
        List<SysLogOp> opLogs = [];

        // 遍历处理日志
        foreach (var logMsg in batchLogMsgs)
        {
            var opLog = await ProcessLogMessageAsync(logMsg);
            if (opLog != null)
                opLogs.Add(opLog);
        }

        // 批量写入 Elasticsearch
        await _esClient.IndexManyAsync(opLogs);
    }

    /// <summary>
    /// 处理单条日志消息
    /// </summary>
    /// <param name="logMsg">日志消息对象</param>
    /// <returns>日志实体</returns>
    private async Task<SysLogOp> ProcessLogMessageAsync(LogMessage logMsg)
    {
        // 从上下文中获取日志监控信息
        var jsonStr = logMsg.Context?.Get("loggingMonitor")?.ToString();
        if (string.IsNullOrWhiteSpace(jsonStr)) return null;

        // 反序列化为动态对象（便于处理）
        var loggingMonitor = JSON.Deserialize<LoggingMonitorDto>(jsonStr);

        // 不记录 userInfo 和 logout 操作的日志
        if (loggingMonitor.ActionName is "userInfo" or "logout") return null;

        // 解析用户信息（从授权声明中获取）
        var userInfo = CommonHelper.GetUserInfo(loggingMonitor);

        // 获取客户端真实IP（考虑代理转发场景）
        string remoteIPv4 = loggingMonitor.RequestHeaders?.FirstOrDefault(u => u.Key == "X-Forwarded-For").Value?.ToString() ?? loggingMonitor.RemoteIPv4;

        // 获取IP地理位置信息（省市、经纬度）
        var (ipLocation, longitude, latitude) = CommonHelper.GetIpAddress(remoteIPv4);

        // 序列化请求参数（避免空引用异常）
        var requestParam = loggingMonitor.Parameters?.FirstOrDefault()?.Value != null ? JSON.Serialize(loggingMonitor.Parameters.First().Value) : null;

        // 序列化返回结果
        var returnResult = loggingMonitor.ReturnInformation?.Value != null ? JSON.Serialize(loggingMonitor.ReturnInformation.Value) : null;

        // 序列化异常信息
        var exception = loggingMonitor.Exception != null ? JSON.Serialize(loggingMonitor.Exception) : null;

        // 构建日志实体
        return new SysLogOp
        {
            Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),                     // 日志ID（使用时间戳）
            ControllerName = loggingMonitor.ControllerName,                          // 控制器名称
            ActionName = loggingMonitor.ActionTypeName,                              // 操作方法名称
            DisplayTitle = loggingMonitor.DisplayTitle,                              // 显示标题
            Status = loggingMonitor.ReturnInformation?.HttpStatusCode.ToString() ?? "200",    // HTTP状态码
            RemoteIp = remoteIPv4,                                                   // 客户端IP
            Location = ipLocation,                                                   // IP地理位置
            Longitude = (decimal)longitude,                                                   // 经度
            Latitude = (decimal)latitude,                                                     // 纬度
            Browser = loggingMonitor.UserAgent,                                      // 浏览器信息（原始UserAgent）
            Os = $"{loggingMonitor.OsDescription} {loggingMonitor.OsArchitecture}",  // 操作系统信息
            Elapsed = loggingMonitor.TimeOperationElapsedMilliseconds,               // 接口执行耗时（毫秒）
            LogDateTime = logMsg.LogDateTime,                                        // 日志时间
            Account = userInfo.Account,                                              // 用户账号
            RealName = userInfo.RealName,                                            // 用户真实姓名
            HttpMethod = loggingMonitor.HttpMethod,                                  // HTTP方法（GET/POST等）
            RequestUrl = loggingMonitor.RequestUrl,                                  // 请求URL
            RequestParam = requestParam,                                             // 请求参数
            ReturnResult = returnResult,                                             // 返回结果
            EventId = logMsg.EventId.Id,                                             // 事件ID
            ThreadId = logMsg.ThreadId,                                              // 线程ID
            TraceId = logMsg.TraceId,                                                // 链路追踪ID
            Exception = exception,                                                   // 异常详情
            Message = logMsg.Message,                                                // 日志消息内容
            CreateUserId = userInfo.UserId,                                          // 创建用户ID
            TenantId = userInfo.TenantId                                             // 租户ID
        };
    }

    /// <summary>
    /// 释放服务作用域
    /// </summary>
    public void Dispose()
    {
        _serviceScope.Dispose();
    }
}