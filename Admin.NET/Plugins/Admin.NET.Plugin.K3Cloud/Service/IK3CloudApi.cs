// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

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