// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;

namespace Admin.NET.Core;

public static class OAuthSetup
{
    /// <summary>
    /// 三方授权登录OAuth注册
    /// </summary>
    /// <param name="services"></param>
    public static void AddOAuth(this IServiceCollection services)
    {
        var authOpt = App.GetConfig<OAuthOptions>("OAuth", true);
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
            })
            .AddWeixin(options =>
            {
                options.ClientId = authOpt.Weixin?.ClientId;
                options.ClientSecret = authOpt.Weixin?.ClientSecret;
            })
            .AddGitee(options =>
            {
                options.ClientId = authOpt.Gitee?.ClientId;
                options.ClientSecret = authOpt.Gitee?.ClientSecret;

                options.ClaimActions.MapJsonKey(OAuthClaim.GiteeAvatarUrl, "avatar_url");
            });
    }

    public static void UseOAuth(this IApplicationBuilder app)
    {
        app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
    }
}