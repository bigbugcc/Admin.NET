// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

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