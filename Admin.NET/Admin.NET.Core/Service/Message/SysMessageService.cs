// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Microsoft.AspNetCore.SignalR;

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统消息发送服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 370)]
public class SysMessageService : IDynamicApiController, ITransient
{
    private readonly SysCacheService _sysCacheService;
    private readonly IHubContext<OnlineUserHub, IOnlineUserHub> _chatHubContext;
    private readonly SysConfigService _sysConfigService;

    public SysMessageService(SysCacheService sysCacheService,
        IHubContext<OnlineUserHub, IOnlineUserHub> chatHubContext,
        SysConfigService sysConfigService)
    {
        _sysCacheService = sysCacheService;
        _chatHubContext = chatHubContext;
        _sysConfigService = sysConfigService;
    }

    /// <summary>
    /// 发送消息给所有人 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("发送消息给所有人")]
    public async Task SendAllUser(MessageInput input)
    {
        await _chatHubContext.Clients.All.ReceiveMessage(input);
    }

    /// <summary>
    /// 发送消息给除了发送人的其他人 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("发送消息给除了发送人的其他人")]
    public async Task SendOtherUser(MessageInput input)
    {
        var cacheKey = CacheConst.KeyUserOnline + input.UserId;
        // 是否开启单用户登录
        if (await _sysConfigService.GetConfigValue<bool>(CommonConst.SysSingleLogin))
        {
            var user = _sysCacheService.Get<SysOnlineUser>(cacheKey);
            if (user == null) return;
            await _chatHubContext.Clients.AllExcept(user.ConnectionId).ReceiveMessage(input);
        }
        else
        {
            var _cacheKeys = _sysCacheService.GetKeyList().Where(u => u.StartsWith(cacheKey)).ToArray();
            foreach (var _cacheKey in _cacheKeys)
            {
                var user = _sysCacheService.Get<SysOnlineUser>(_cacheKey);
                if (user == null) return;
                await _chatHubContext.Clients.AllExcept(user.ConnectionId).ReceiveMessage(input);
            }
        }
    }

    /// <summary>
    /// 发送消息给某个人 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("发送消息给某个人")]
    public async Task SendUser(MessageInput input)
    {
        var cacheKey = CacheConst.KeyUserOnline + input.UserId;
        // 是否开启单用户登录
        if (await _sysConfigService.GetConfigValue<bool>(CommonConst.SysSingleLogin))
        {
            var user = _sysCacheService.Get<SysOnlineUser>(cacheKey);
            if (user == null) return;
            await _chatHubContext.Clients.Client(user.ConnectionId).ReceiveMessage(input);
        }
        else
        {
            var _cacheKeys = _sysCacheService.GetKeyList().Where(u => u.StartsWith(cacheKey)).ToArray();
            foreach (var _cacheKey in _cacheKeys)
            {
                var user = _sysCacheService.Get<SysOnlineUser>(_cacheKey);
                if (user == null) return;
                await _chatHubContext.Clients.Client(user.ConnectionId).ReceiveMessage(input);
            }
        }
    }

    /// <summary>
    /// 发送消息给某些人 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("发送消息给某些人")]
    public async Task SendUsers(MessageInput input)
    {
        var userList = new List<string>();
        foreach (var userId in input.UserIds)
        {
            var cacheKey = CacheConst.KeyUserOnline + userId;
            // 是否开启单用户登录
            if (await _sysConfigService.GetConfigValue<bool>(CommonConst.SysSingleLogin))
            {
                var user = _sysCacheService.Get<SysOnlineUser>(cacheKey);
                if (user != null) userList.Add(user.ConnectionId);
            }
            else
            {
                var _cacheKeys = _sysCacheService.GetKeyList().Where(u => u.StartsWith(cacheKey)).ToArray();
                foreach (var _cacheKey in _cacheKeys)
                {
                    var user = _sysCacheService.Get<SysOnlineUser>(_cacheKey);
                    if (user != null) userList.Add(user.ConnectionId);
                }
            }
        }
        await _chatHubContext.Clients.Clients(userList).ReceiveMessage(input);
    }
}