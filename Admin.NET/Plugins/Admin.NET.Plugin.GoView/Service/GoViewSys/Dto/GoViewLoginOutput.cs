// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Plugin.GoView.Service;

/// <summary>
/// 登录输出
/// </summary>
public class GoViewLoginOutput
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public GoViewLoginUserInfo Userinfo { get; set; }

    /// <summary>
    /// Token
    /// </summary>
    public GoViewLoginToken Token { get; set; }
}

/// <summary>
/// 登录 Token
/// </summary>
public class GoViewLoginToken
{
    /// <summary>
    /// Token 名
    /// </summary>
    public string TokenName { get; set; } = "Authorization";

    /// <summary>
    /// Token 值
    /// </summary>
    public string TokenValue { get; set; }
}

/// <summary>
/// 用户信息
/// </summary>
public class GoViewLoginUserInfo
{
    /// <summary>
    /// 用户 Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public string Nickname { get; set; }
}