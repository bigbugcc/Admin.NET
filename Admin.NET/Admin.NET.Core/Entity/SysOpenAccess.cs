// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 开放接口身份表
/// </summary>
[SugarTable(null, "开放接口身份表")]
[SysTable]
[SugarIndex("index_{table}_A", nameof(AccessKey), OrderByType.Asc)]
public partial class SysOpenAccess : EntityBase
{
    /// <summary>
    /// 身份标识
    /// </summary>
    [SugarColumn(ColumnDescription = "身份标识", Length = 128)]
    [Required, MaxLength(128)]
    public virtual string AccessKey { get; set; }

    /// <summary>
    /// 密钥
    /// </summary>
    [SugarColumn(ColumnDescription = "密钥", Length = 256)]
    [Required, MaxLength(256)]
    public virtual string AccessSecret { get; set; }

    /// <summary>
    /// 绑定租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "绑定租户Id")]
    public long BindTenantId { get; set; }

    /// <summary>
    /// 绑定租户
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(BindTenantId))]
    public SysTenant BindTenant { get; set; }

    /// <summary>
    /// 绑定用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "绑定用户Id")]
    public virtual long BindUserId { get; set; }

    /// <summary>
    /// 绑定用户
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(BindUserId))]
    public SysUser BindUser { get; set; }
}