// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

using System.IO;
using System.Linq;
using System.Text;
using Admin.NET.Plugin.Flow.Entity;
using Furion.Logging.Extensions;
using Microsoft.AspNetCore.Http;
using NewLife;
using RazorEngine.Compilation.ImpromptuInterface.InvokeExt;

namespace Admin.NET.Plugin.Flow.Service;

[ApiDescriptionSettings(Order = 300)]
public class SysApprovalService : IDynamicApiController, ITransient
{
    public SysApprovalService()
    {

    }

    /// <summary>
    /// 匹配审批流程
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [NonAction]
    public async Task MatchApproval(HttpContext context)
    {
        var request = context.Request;
        var response = context.Response;

        var path = request.Path.ToString().Split("/");

        var method = request.Method;
        var qs = request.QueryString;
        var h = request.Headers;
        var b = request.Body;

        var requestHeaders = request.Headers;
        var responseHeaders = response.Headers;

        path.Join(",").LogTrace();
        
        await Task.CompletedTask;
    }
}