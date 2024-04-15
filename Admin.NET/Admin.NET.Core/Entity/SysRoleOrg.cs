// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统角色机构表
/// </summary>
[SugarTable(null, "系统角色机构表")]
[SysTable]
public class SysRoleOrg : EntityBaseId
{
    /// <summary>
    /// 角色Id
    /// </summary>
    [SugarColumn(ColumnDescription = "角色Id")]
    public long RoleId { get; set; }

    /// <summary>
    /// 机构Id
    /// </summary>
    [SugarColumn(ColumnDescription = "机构Id")]
    public long OrgId { get; set; }

    /// <summary>
    /// 机构
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(OrgId))]
    public SysOrg SysOrg { get; set; }
}