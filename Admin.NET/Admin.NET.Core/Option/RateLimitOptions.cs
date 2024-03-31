// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

using AspNetCoreRateLimit;

namespace Admin.NET.Core;

/// <summary>
/// IP限流配置选项
/// </summary>
public sealed class IpRateLimitingOptions : IpRateLimitOptions
{
}

/// <summary>
/// IP限流策略配置选项
/// </summary>
public sealed class IpRateLimitPoliciesOptions : IpRateLimitPolicies, IConfigurableOptions
{
}

/// <summary>
/// 客户端限流配置选项
/// </summary>
public sealed class ClientRateLimitingOptions : ClientRateLimitOptions, IConfigurableOptions
{
}

/// <summary>
/// 客户端限流策略配置选项
/// </summary>
public sealed class ClientRateLimitPoliciesOptions : ClientRateLimitPolicies, IConfigurableOptions
{
}