﻿// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core;

/// <summary>
/// 清理日志作业任务
/// </summary>
[JobDetail("job_log", Description = "清理操作日志", GroupName = "default", Concurrent = false)]
[Daily(TriggerId = "trigger_log", Description = "清理操作日志")]
public class LogJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;

    public LogJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();

        var logVisRep = serviceScope.ServiceProvider.GetRequiredService<SqlSugarRepository<SysLogVis>>();
        var logOpRep = serviceScope.ServiceProvider.GetRequiredService<SqlSugarRepository<SysLogOp>>();
        var logDiffRep = serviceScope.ServiceProvider.GetRequiredService<SqlSugarRepository<SysLogDiff>>();

        var daysAgo = 30; // 删除30天以前
        await logVisRep.CopyNew().AsDeleteable().Where(u => (DateTime)u.CreateTime < DateTime.Now.AddDays(-daysAgo)).ExecuteCommandAsync(stoppingToken); // 删除访问日志
        await logOpRep.CopyNew().AsDeleteable().Where(u => (DateTime)u.CreateTime < DateTime.Now.AddDays(-daysAgo)).ExecuteCommandAsync(stoppingToken); // 删除操作日志
        await logDiffRep.CopyNew().AsDeleteable().Where(u => (DateTime)u.CreateTime < DateTime.Now.AddDays(-daysAgo)).ExecuteCommandAsync(stoppingToken); // 删除差异日志

        var originColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("【" + DateTime.Now + "】清空系统日志（30天前）");
        Console.ForegroundColor = originColor;
    }
}