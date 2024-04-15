// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统用户域配置表
/// </summary>
[SugarTable(null, "系统用户域配置表")]
[SysTable]
[SugarIndex("index_{table}_A", nameof(Account), OrderByType.Asc)]
[SugarIndex("index_{table}_U", nameof(UserId), OrderByType.Asc)]
public class SysUserLdap : EntityTenant
{
    /// <summary>
    /// 用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "用户Id")]
    public long UserId { get; set; }

    /// <summary>
    /// 域账号
    /// AD域对应sAMAccountName
    /// Ldap对应uid
    /// </summary>
    [SugarColumn(ColumnDescription = "域账号", Length = 32)]
    [Required]
    public string Account { get; set; }

    /// <summary>
    /// 对应EmployeeId(用于数据导入对照)
    /// </summary>
    [SugarColumn(ColumnDescription = "对应EmployeeId", Length = 32)]
    public string? EmployeeId { get; set; }

    /// <summary>
    /// 组织代码
    /// </summary>
    [SugarColumn(ColumnDescription = "组织代码", Length = 64)]
    public string? DeptCode { get; set; }
}