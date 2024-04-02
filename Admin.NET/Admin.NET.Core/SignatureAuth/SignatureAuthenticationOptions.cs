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
/// Signature 身份验证选项
/// </summary>
public class SignatureAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// 请求时间允许的偏差范围
    /// </summary>
    public TimeSpan AllowedDateDrift { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Signature 身份验证事件
    /// </summary>
    public new SignatureAuthenticationEvent Events
    {
        get => (SignatureAuthenticationEvent)base.Events;
        set => base.Events = value;
    }
}