// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core;

/// <summary>
/// 事件订阅
/// </summary>
public class AppEventSubscriber : IEventSubscriber, ISingleton, IDisposable
{
    private readonly IServiceScope _serviceScope;

    public AppEventSubscriber(IServiceScopeFactory scopeFactory)
    {
        _serviceScope = scopeFactory.CreateScope();
    }

    /// <summary>
    /// 增加异常日志
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe("Add:ExLog")]
    public async Task CreateExLog(EventHandlerExecutingContext context)
    {
        var rep = _serviceScope.ServiceProvider.GetRequiredService<SqlSugarRepository<SysLogEx>>();
        await rep.InsertAsync((SysLogEx)context.Source.Payload);
    }

    /// <summary>
    /// 发送异常邮件
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe("Send:ErrorMail")]
    public async Task SendOrderErrorMail(EventHandlerExecutingContext context)
    {
        //var mailTempPath = Path.Combine(App.WebHostEnvironment.WebRootPath, "Temp\\ErrorMail.tp");
        //var mailTemp = File.ReadAllText(mailTempPath);
        //var mail = await _serviceScope.ServiceProvider.GetRequiredService<IViewEngine>().RunCompileFromCachedAsync(mailTemp, );

        var title = "Admin.NET 系统异常";
        await _serviceScope.ServiceProvider.GetRequiredService<SysEmailService>().SendEmail(JSON.Serialize(context.Source.Payload), title);
    }

    /// <summary>
    /// 释放服务作用域
    /// </summary>
    public void Dispose()
    {
        _serviceScope.Dispose();
    }
}