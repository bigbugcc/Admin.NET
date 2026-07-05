// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.WorkWeixin.Proxy;

/// <summary>
/// 标签远程调用服务
/// </summary>
public interface ITagHttp : IHttpDeclarative
{
    /// <summary>
    /// 创建标签
    /// https://developer.work.weixin.qq.com/document/path/90210
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/tag/create")]
    Task<BaseWorkIdOutput> Create([QueryParam("access_token")] string accessToken, [Body] TagHttpInput body);

    /// <summary>
    /// 更新标签名字
    /// https://developer.work.weixin.qq.com/document/path/90211
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/tag/update")]
    Task<TagIdHttpOutput> Update([QueryParam("access_token")] string accessToken, [Body] TagHttpInput body);

    /// <summary>
    /// 删除标签
    /// https://developer.work.weixin.qq.com/document/path/90212
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="tagId"></param>
    /// <returns></returns>
    [Get("https://qyapi.weixin.qq.com/cgi-bin/tag/delete")]
    Task<BaseWorkOutput> Delete([QueryParam("access_token")] string accessToken, [QueryParam("tagid")] long tagId);

    /// <summary>
    /// 获取标签详情
    /// https://developer.work.weixin.qq.com/document/path/90213
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="tagId"></param>
    /// <returns></returns>
    [Get("https://qyapi.weixin.qq.com/cgi-bin/tag/get")]
    Task<DepartmentOutput> Get([QueryParam("access_token")] string accessToken, [QueryParam("tagid")] long tagId);

    /// <summary>
    /// 增加标签成员
    /// https://developer.work.weixin.qq.com/document/path/90214
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/tag/addtagusers")]
    Task<DepartmentOutput> AddTagUsers([QueryParam("access_token")] string accessToken, [Body] TagUsersTagInput body);

    /// <summary>
    /// 删除标签成员
    /// https://developer.work.weixin.qq.com/document/path/90215
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [Post("https://qyapi.weixin.qq.com/cgi-bin/tag/deltagusers")]
    Task<DepartmentOutput> DelTagUsers([QueryParam("access_token")] string accessToken, [Body] TagUsersTagInput body);

    /// <summary>
    /// 获取标签列表
    /// https://developer.work.weixin.qq.com/document/path/90216
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    [Get("https://qyapi.weixin.qq.com/cgi-bin/tag/list")]
    Task<TagListHttpOutput> List([QueryParam("access_token")] string accessToken);
}