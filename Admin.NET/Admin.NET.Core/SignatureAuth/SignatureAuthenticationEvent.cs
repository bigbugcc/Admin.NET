// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// Signature 身份验证事件
/// </summary>
public class SignatureAuthenticationEvent
{
    public SignatureAuthenticationEvent()
    {
    }

    /// <summary>
    /// 获取或设置获取 AccessKey 的 AccessSecret 的逻辑处理
    /// </summary>
    public Func<GetAccessSecretContext, Task<string>> OnGetAccessSecret { get; set; }

    /// <summary>
    /// 获取或设置质询的逻辑处理
    /// </summary>
    public Func<SignatureChallengeContext, Task> OnChallenge { get; set; } = _ => Task.CompletedTask;

    /// <summary>
    /// 获取或设置已验证的逻辑处理
    /// </summary>
    public Func<SignatureValidatedContext, Task> OnValidated { get; set; } = _ => Task.CompletedTask;

    /// <summary>
    /// 获取 AccessKey 的 AccessSecret
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual Task<string> GetAccessSecret(GetAccessSecretContext context) => OnGetAccessSecret?.Invoke(context) ?? throw new NotImplementedException($"需要提供 {nameof(OnGetAccessSecret)} 实现");

    /// <summary>
    /// 质询
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual Task Challenge(SignatureChallengeContext context) => OnChallenge?.Invoke(context) ?? Task.CompletedTask;

    /// <summary>
    /// 已验证成功
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual Task Validated(SignatureValidatedContext context) => OnValidated?.Invoke(context) ?? Task.CompletedTask;
}