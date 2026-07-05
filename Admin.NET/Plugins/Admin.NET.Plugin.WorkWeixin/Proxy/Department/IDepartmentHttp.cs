// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin.Proxy.AppChat;

/// <summary>
/// 部门远程调用服务
/// </summary>
public interface IDepartmentHttp : IHttpDeclarative
{
    /// <summary>
    /// 创建部门
    /// https://developer.work.weixin.qq.com/document/path/90205
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/department/create")]
    Task<BaseWorkIdOutput> Create([QueryParam("access_token")] string accessToken, [Body] DepartmentHttpInput body);

    /// <summary>
    /// 修改部门
    /// https://developer.work.weixin.qq.com/document/path/90206
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/department/update")]
    Task<BaseWorkOutput> Update([QueryParam("access_token")] string accessToken, [Body] DepartmentHttpInput body);

    /// <summary>
    /// 删除部门
    /// https://developer.work.weixin.qq.com/document/path/90207
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [Get("https://qyapi.weixin.qq.com/cgi-bin/department/delete")]
    Task<BaseWorkOutput> Delete([QueryParam("access_token")] string accessToken, [QueryParam] long id);

    /// <summary>
    /// 获取部门Id列表
    /// https://developer.work.weixin.qq.com/document/path/90208
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [Get("https://qyapi.weixin.qq.com/cgi-bin/department/simplelist")]
    Task<DepartmentIdOutput> SimpleList([QueryParam("access_token")] string accessToken, [QueryParam] long id);

    /// <summary>
    /// 获取部门详情
    /// https://developer.work.weixin.qq.com/document/path/90208
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [Get("https://qyapi.weixin.qq.com/cgi-bin/department/get")]
    Task<DepartmentOutput> Get([QueryParam("access_token")] string accessToken, [QueryParam] long id);
}