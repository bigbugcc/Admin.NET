﻿// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 事件执行器-超时控制、失败重试熔断等等
/// </summary>
public class RetryEventHandlerExecutor : IEventHandlerExecutor
{
    public async Task ExecuteAsync(EventHandlerExecutingContext context, Func<EventHandlerExecutingContext, Task> handler)
    {
        var eventSubscribeAttribute = context.Attribute;
        // 判断是否自定义了重试失败回调服务
        var fallbackPolicyService = eventSubscribeAttribute?.FallbackPolicy == null
            ? null
            : App.GetRequiredService(eventSubscribeAttribute.FallbackPolicy) as IEventFallbackPolicy;

        await Retry.InvokeAsync(async () =>
        {
            try
            {
                await handler(context);
            }
            catch (Exception ex)
            {
                Log.Error($"Invoke EventHandler {context.Source.EventId} Error", ex);
                throw;
            }
        }
        , eventSubscribeAttribute?.NumRetries ?? 0
        , eventSubscribeAttribute?.RetryTimeout ?? 1000
        , exceptionTypes: eventSubscribeAttribute?.ExceptionTypes
        , fallbackPolicy: fallbackPolicyService == null ? null : async (Exception ex) => { await fallbackPolicyService.CallbackAsync(context, ex); }
        , retryAction: (total, times) =>
        {
            // 输出重试日志
            Log.Warning($"Retrying {times}/{total} times for  EventHandler {context.Source.EventId}");
        });
    }
}