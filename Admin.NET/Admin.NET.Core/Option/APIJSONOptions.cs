// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// APIJSON配置选项
/// </summary>
public sealed class APIJSONOptions : IConfigurableOptions
{
    /// <summary>
    /// 角色集合
    /// </summary>
    public List<APIJSON_Role> Roles { get; set; }
}

/// <summary>
/// APIJSON角色权限
/// </summary>
public class APIJSON_Role
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; }

    /// <summary>
    /// 查询
    /// </summary>
    public APIJSON_RoleItem Select { get; set; }

    /// <summary>
    /// 增加
    /// </summary>
    public APIJSON_RoleItem Insert { get; set; }

    /// <summary>
    /// 更新
    /// </summary>
    public APIJSON_RoleItem Update { get; set; }

    /// <summary>
    /// 删除
    /// </summary>
    public APIJSON_RoleItem Delete { get; set; }
}

/// <summary>
/// APIJSON角色权限内容
/// </summary>
public class APIJSON_RoleItem
{
    /// <summary>
    /// 表集合
    /// </summary>
    public string[] Table { get; set; }

    /// <summary>
    /// 列集合
    /// </summary>
    public string[] Column { get; set; }

    /// <summary>
    /// 过滤器
    /// </summary>
    public string[] Filter { get; set; }
}