// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Security.Claims;

namespace Admin.NET.Core;

/// <summary>
/// 防止重复请求过滤器特性
/// </summary>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public class IdempotentAttribute : Attribute, IAsyncActionFilter
{
    /// <summary>
    /// 请求间隔时间/秒
    /// </summary>
    public int IntervalTime { get; set; } = 5;

    /// <summary>
    /// 错误提示内容
    /// </summary>
    public string Message { get; set; } = "你操作频率过快，请稍后重试！";

    /// <summary>
    /// 缓存前缀: Key+请求路由+用户Id+请求参数
    /// </summary>
    public string CacheKey { get; set; }

    /// <summary>
    /// 是否直接抛出异常：Ture是，False返回上次请求结果
    /// </summary>
    public bool ThrowBah { get; set; }

    public IdempotentAttribute()
    {
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var path = httpContext.Request.Path.Value.ToString();
        var userId = httpContext.User?.FindFirstValue(ClaimConst.UserId);
        var cacheExpireTime = TimeSpan.FromSeconds(IntervalTime);

        var parameters = "";
        foreach (var parameter in context.ActionDescriptor.Parameters)
        {
            parameters += parameter.Name;
            parameters += context.ActionArguments[parameter.Name].ToJson();
        }

        var cacheKey = MD5Encryption.Encrypt($"{CacheKey}{path}{userId}{parameters}");
        var sysCacheService = App.GetRequiredService<SysCacheService>();
        if (sysCacheService.ExistKey(cacheKey))
        {
            if (ThrowBah) throw Oops.Oh(Message);

            try
            {
                var cachedResult = sysCacheService.Get<ResponseData>(cacheKey);
                context.Result = new ObjectResult(cachedResult.Value);
            }
            catch (Exception ex)
            {
                throw Oops.Oh($"{Message}-{ex}");
            }
        }
        else
        {
            // 先加入一个空缓存，防止第一次请求结果没回来导致连续请求
            sysCacheService.Set(cacheKey, "", cacheExpireTime);
            var resultContext = await next();
            if (resultContext.Result is ObjectResult objectResult)
            {
                var valueType = objectResult.Value.GetType();
                var responseData = new ResponseData
                {
                    Type = valueType.Name,
                    Value = objectResult.Value
                };
                sysCacheService.Set(cacheKey, responseData, cacheExpireTime);
            }
        }
    }

    /// <summary>
    /// 请求结果数据
    /// </summary>
    private class ResponseData
    {
        /// <summary>
        /// 结果类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 请求结果
        /// </summary>
        public dynamic Value { get; set; }
    }
}