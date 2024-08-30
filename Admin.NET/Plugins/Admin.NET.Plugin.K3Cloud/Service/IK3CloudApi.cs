using Furion.DependencyInjection;
using Furion.RemoteRequest;

namespace Admin.NET.Plugin.K3Cloud.Service;

/// <summary>
/// 金蝶云星空ERP接口
/// </summary>
[Client("K3Cloud")]
public interface IK3CloudApi : IHttpDispatchProxy
{
    /// <summary>
    /// 验证用户
    /// </summary>
    /// <param name="input"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    [Post("Kingdee.BOS.WebApi.ServicesStub.AuthService.ValidateUser.common.kdsvc")]
    Task<K3CloudLoginOutput> ValidateUser([Body] K3CloudLoginInput input, [Interceptor(InterceptorTypes.Response)] Action<HttpClient, HttpResponseMessage> action = default);
    /// <summary>
    /// 保存表单
    /// </summary>
    /// <param name="input"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    [Post("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Save.common.kdsvc")]
    Task<K3CloudPushResultOutput> Save<T>([Body] K3CloudBaeInput<T> input, [Interceptor(InterceptorTypes.Request)] Action<HttpClient, HttpRequestMessage> action = default);
    /// <summary>
    /// 提交表单
    /// </summary>
    /// <param name="input"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    [Post("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Submit.common.kdsvc")]
    Task<K3CloudPushResultOutput> Submit<T>([Body] K3CloudBaeInput<T> input, [Interceptor(InterceptorTypes.Request)] Action<HttpClient, HttpRequestMessage> action = default);
    /// <summary>
    /// 审核表单
    /// </summary>
    /// <param name="input"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    [Post("Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.Audit.common.kdsvc")]
    Task<K3CloudPushResultOutput> Audit<T>([Body] K3CloudBaeInput<T> input, [Interceptor(InterceptorTypes.Request)] Action<HttpClient, HttpRequestMessage> action = default);


}