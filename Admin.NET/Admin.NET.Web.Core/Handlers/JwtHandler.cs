// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Core;
using Admin.NET.Core.Service;
using Furion;
using Furion.Authorization;
using Furion.DataEncryption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Admin.NET.Web.Core
{
    public class JwtHandler : AppAuthorizeHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public JwtHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 自动刷新Token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task HandleAsync(AuthorizationHandlerContext context)
        {
            // var serviceProvider = context.GetCurrentHttpContext().RequestServices;
            using var serviceScope = _serviceProvider.CreateScope();

            // 若当前账号存在黑名单中则授权失败
            var sysCacheService = serviceScope.ServiceProvider.GetRequiredService<SysCacheService>();
            if (sysCacheService.ExistKey($"{CacheConst.KeyBlacklist}{context.User.FindFirst(ClaimConst.UserId)?.Value}"))
            {
                context.Fail();
                context.GetCurrentHttpContext().SignoutToSwagger();
                return;
            }

            var sysConfigService = serviceScope.ServiceProvider.GetRequiredService<SysConfigService>();
            var tokenExpire = await sysConfigService.GetTokenExpire();
            var refreshTokenExpire = await sysConfigService.GetRefreshTokenExpire();
            if (JWTEncryption.AutoRefreshToken(context, context.GetCurrentHttpContext(), tokenExpire, refreshTokenExpire))
            {
                await AuthorizeHandleAsync(context);
            }
            else
            {
                context.Fail(); // 授权失败
                var currentHttpContext = context.GetCurrentHttpContext();
                if (currentHttpContext == null)
                    return;
                // 跳过由于 SignatureAuthentication 引发的失败
                if (currentHttpContext.Items.ContainsKey(SignatureAuthenticationDefaults.AuthenticateFailMsgKey))
                    return;
                currentHttpContext.SignoutToSwagger();
            }
        }

        public override async Task<bool> PipelineAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
        {
            // 已自动验证 Jwt Token 有效性
            return await CheckAuthorizeAsync(httpContext);
        }

        /// <summary>
        /// 权限校验核心逻辑
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private static async Task<bool> CheckAuthorizeAsync(DefaultHttpContext httpContext)
        {
            // 登录模式判断PC、APP
            if (App.User.FindFirst(ClaimConst.LoginMode)?.Value == ((int)LoginModeEnum.APP).ToString())
                return true;

            // 排除超管
            if (App.User.FindFirst(ClaimConst.AccountType)?.Value == ((int)AccountTypeEnum.SuperAdmin).ToString())
                return true;

            // 路由名称
            var routeName = httpContext.Request.Path.StartsWithSegments("/api")
                ? httpContext.Request.Path.Value[5..].Replace("/", ":")
                : httpContext.Request.Path.Value[1..].Replace("/", ":");

            var sysMenuService = App.GetRequiredService<SysMenuService>();
            // 获取用户拥有按钮权限集合
            var ownBtnPermList = await sysMenuService.GetOwnBtnPermList();
            // 获取系统所有按钮权限集合
            var allBtnPermList = await sysMenuService.GetAllBtnPermList();

            // 已拥有该按钮权限或者所有按钮集合里面不存在
            var exist1 = ownBtnPermList.Exists(u => routeName.Equals(u, StringComparison.CurrentCultureIgnoreCase));
            var exist2 = allBtnPermList.TrueForAll(u => !routeName.Equals(u, StringComparison.CurrentCultureIgnoreCase));
            return exist1 || exist2;
        }
    }
}