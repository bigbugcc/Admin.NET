// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

using Admin.NET.Plugin.Flow.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Admin.NET.Plugin.Flow;

/// <summary>
/// 扩展审批流中间件
/// </summary>
public static class ApprovalFlowMiddlewareExtensions
{
    /// <summary>
    /// 使用审批流
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseApprovalFlow(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApprovalFlowMiddleware>();
    }
}

/// <summary>
/// 审批流中间件
/// </summary>
public class ApprovalFlowMiddleware
{
    private readonly RequestDelegate _next;

    public ApprovalFlowMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        await App.GetService<SysApprovalService>().MatchApproval(context);

        await _next.Invoke(context);
    }
}
