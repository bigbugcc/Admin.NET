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

    public async Task WriteAsync(LogMessage logMsg, bool flush)
    {
        // 是否启用操作日志
        var sysOpLogEnabled = await _sysConfigService.GetConfigValue<bool>(ConfigConst.SysOpLog);
        if (!sysOpLogEnabled) return;

        var jsonStr = logMsg.Context?.Get("loggingMonitor")?.ToString();
        if (string.IsNullOrWhiteSpace(jsonStr)) return;

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
        (string ipLocation, double? longitude, double? latitude) = CommonUtil.GetIpAddress(remoteIPv4);

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
        await _esClient.IndexAsync(sysLogOp);
    }

    /// <summary>
    /// 释放服务作用域
    /// </summary>
    public void Dispose()
    {
        _serviceScope.Dispose();
    }
}