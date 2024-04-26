// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

//using Microsoft.AspNetCore.Mvc.Controllers;
//using System.Security.Claims;

//namespace Admin.NET.Core.Logging;

///// <summary>
///// 全局异常处理
///// </summary>
//public class LogExceptionHandler : IGlobalExceptionHandler, ISingleton
//{
//    private readonly IEventPublisher _eventPublisher;

//    public LogExceptionHandler(IEventPublisher eventPublisher)
//    {
//        _eventPublisher = eventPublisher;
//    }

//    public async Task OnExceptionAsync(ExceptionContext context)
//    {
//        var actionMethod = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo;
//        var displayNameAttribute = actionMethod.IsDefined(typeof(DisplayNameAttribute), true) ? actionMethod.GetCustomAttribute<DisplayNameAttribute>(true) : default;

//        var sysLogEx = new SysLogEx
//        {
//            Account = App.User?.FindFirstValue(ClaimConst.Account),
//            RealName = App.User?.FindFirstValue(ClaimConst.RealName),
//            ControllerName = actionMethod.DeclaringType.FullName,
//            ActionName = actionMethod.Name,
//            DisplayTitle = displayNameAttribute?.DisplayName,
//            Exception = $"异常信息:{context.Exception.Message} 异常来源：{context.Exception.Source} 堆栈信息：{context.Exception.StackTrace}",
//            Message = "全局异常",
//            RequestParam = context.Exception.TargetSite.GetParameters().ToString(),
//            HttpMethod = context.HttpContext.Request.Method,
//            RequestUrl = context.HttpContext.Request.GetRequestUrlAddress(),
//            RemoteIp = context.HttpContext.GetRemoteIpAddressToIPv4(),
//            Browser = context.HttpContext.Request.Headers["User-Agent"],
//            TraceId = App.GetTraceId(),
//            ThreadId = App.GetThreadId(),
//            LogDateTime = DateTime.Now,
//            LogLevel = LogLevel.Error
//        };

//        await _eventPublisher.PublishAsync(new ChannelEventSource("Add:ExLog", sysLogEx));

//        await _eventPublisher.PublishAsync("Send:ErrorMail", sysLogEx);
//    }
//}