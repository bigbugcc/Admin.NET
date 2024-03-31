// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

public interface IOnlineUserHub
{
    /// <summary>
    /// 在线用户列表
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task OnlineUserList(OnlineUserList context);

    /// <summary>
    /// 强制下线
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task ForceOffline(object context);

    /// <summary>
    /// 发布站内消息
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task PublicNotice(SysNotice context);

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task ReceiveMessage(object context);
}