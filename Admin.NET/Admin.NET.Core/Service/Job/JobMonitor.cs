// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 作业执行监视器
/// </summary>
public class JobMonitor : IJobMonitor
{
    private readonly IEventPublisher _eventPublisher;
    private readonly IServiceScope _serviceScope;
    private readonly SysConfigService _sysConfigService;

    public JobMonitor(IServiceScopeFactory scopeFactory)
    {
        _serviceScope = scopeFactory.CreateScope();
        _sysConfigService = _serviceScope.ServiceProvider.GetRequiredService<SysConfigService>();
        _eventPublisher = _serviceScope.ServiceProvider.GetRequiredService<IEventPublisher>(); ;
    }

    public Task OnExecutingAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public async Task OnExecutedAsync(JobExecutedContext context, CancellationToken stoppingToken)
    {
        // 将异常作业发送到邮件
        if (await _sysConfigService.GetConfigValue<bool>(CommonConst.SysErrorMail) && context.Exception != null)
        {
            var errorInfo = $"【{context.Trigger.Description}】定时任务错误：{context.Exception}";
            await _eventPublisher.PublishAsync(CommonConst.SendErrorMail, errorInfo);
        }
    }
}