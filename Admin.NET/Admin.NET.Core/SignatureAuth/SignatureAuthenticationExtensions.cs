// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

using Microsoft.AspNetCore.Authentication;

namespace Admin.NET.Core;

/// <summary>
/// Signature 身份验证扩展
/// </summary>
public static class SignatureAuthenticationExtensions
{
    /// <summary>
    /// 注册 Signature 身份验证处理模块
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddSignatureAuthentication(this AuthenticationBuilder builder)
    {
        return builder.AddSignatureAuthentication(options => { });
    }

    /// <summary>
    /// 注册 Signature 身份验证处理模块
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddSignatureAuthentication(this AuthenticationBuilder builder, Action<SignatureAuthenticationOptions> options)
    {
        return builder.AddScheme<SignatureAuthenticationOptions, SignatureAuthenticationHandler>(SignatureAuthenticationDefaults.AuthenticationScheme, options);
    }
}