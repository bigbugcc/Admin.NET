// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

using Nest;

namespace Admin.NET.Core;

/// <summary>
/// ES日志写入器
/// </summary>
public class ElasticSearchLoggingWriter : IDatabaseLoggingWriter, IDisposable
{
    private readonly IServiceScope _serviceScope;
    private readonly ElasticClient _esClient;
    private readonly SysConfigService _sysConfigService;

    public ElasticSearchLoggingWriter(IServiceScopeFactory scopeFactory)
    {
        _serviceScope = scopeFactory.CreateScope();
        _esClient = _serviceScope.ServiceProvider.GetRequiredService<ElasticClient>();
        _sysConfigService = _serviceScope.ServiceProvider.GetRequiredService<SysConfigService>();
    }

    public async Task WriteAsync(LogMessage logMsg, bool flush)
    {
        // 是否启用操作日志
        var sysOpLogEnabled = await _sysConfigService.GetConfigValue<bool>(CommonConst.SysOpLog);
        if (!sysOpLogEnabled) return;

        var jsonStr = logMsg.Context.Get("loggingMonitor").ToString();
        var loggingMonitor = JSON.Deserialize<dynamic>(jsonStr);

        // 不记录登录退出日志
        if (loggingMonitor.actionName == "userInfo" || loggingMonitor.actionName == "logout")
            return;

        // 获取当前操作者
        string account = "", realName = "", userId = "", tenantId = "";
        if (loggingMonitor.authorizationClaims != null)
        {
            foreach (var item in loggingMonitor.authorizationClaims)
            {
                if (item.type == ClaimConst.Account)
                    account = item.value;
                if (item.type == ClaimConst.RealName)
                    realName = item.value;
                if (item.type == ClaimConst.TenantId)
                    tenantId = item.value;
                if (item.type == ClaimConst.UserId)
                    userId = item.value;
            }
        }

        string remoteIPv4 = loggingMonitor.remoteIPv4;
        (string ipLocation, double? longitude, double? latitude) = DatabaseLoggingWriter.GetIpAddress(remoteIPv4);

        var sysLogOp = new SysLogOp
        {
            Id = DateTime.Now.Ticks,
            ControllerName = loggingMonitor.controllerName,
            ActionName = loggingMonitor.actionTypeName,
            DisplayTitle = loggingMonitor.displayTitle,
            Status = loggingMonitor.returnInformation.httpStatusCode,
            RemoteIp = remoteIPv4,
            Location = ipLocation,
            Longitude = longitude,
            Latitude = latitude,
            Browser = loggingMonitor.userAgent,
            Os = loggingMonitor.osDescription + " " + loggingMonitor.osArchitecture,
            Elapsed = loggingMonitor.timeOperationElapsedMilliseconds,
            LogDateTime = logMsg.LogDateTime,
            Account = account,
            RealName = realName,
            HttpMethod = loggingMonitor.httpMethod,
            RequestUrl = loggingMonitor.requestUrl,
            RequestParam = (loggingMonitor.parameters == null || loggingMonitor.parameters.Count == 0) ? null : JSON.Serialize(loggingMonitor.parameters[0].value),
            ReturnResult = JSON.Serialize(loggingMonitor.returnInformation),
            EventId = logMsg.EventId.Id,
            ThreadId = logMsg.ThreadId,
            TraceId = logMsg.TraceId,
            Exception = (loggingMonitor.exception == null) ? null : JSON.Serialize(loggingMonitor.exception),
            Message = logMsg.Message,
            CreateUserId = string.IsNullOrWhiteSpace(userId) ? 0 : long.Parse(userId),
            TenantId = string.IsNullOrWhiteSpace(tenantId) ? 0 : long.Parse(tenantId)
        };
        await _esClient.IndexDocumentAsync(sysLogOp);
    }

    /// <summary>
    /// 释放服务作用域
    /// </summary>
    public void Dispose()
    {
        _serviceScope.Dispose();
    }
}