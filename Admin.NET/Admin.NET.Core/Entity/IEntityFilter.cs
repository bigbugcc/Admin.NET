// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 假删除接口过滤器
/// </summary>
internal interface IDeletedFilter
{
    /// <summary>
    /// 软删除
    /// </summary>
    bool IsDelete { get; set; }
}

/// <summary>
/// 租户Id接口过滤器
/// </summary>
internal interface ITenantIdFilter
{
    /// <summary>
    /// 租户Id
    /// </summary>
    long? TenantId { get; set; }
}

/// <summary>
/// 机构Id接口过滤器
/// </summary>
internal interface IOrgIdFilter
{
    /// <summary>
    /// 创建者部门Id
    /// </summary>
    long? CreateOrgId { get; set; }
}