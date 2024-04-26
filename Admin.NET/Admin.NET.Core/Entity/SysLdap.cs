// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统域登录信息配置表
/// </summary>
[SugarTable(null, "系统域登录信息配置表")]
[SysTable]
public class SysLdap : EntityTenant
{
    /// <summary>
    /// 主机
    /// </summary>
    [SugarColumn(ColumnDescription = "主机", Length = 128)]
    [Required]
    public virtual string Host { get; set; }

    /// <summary>
    /// 端口
    /// </summary>
    [SugarColumn(ColumnDescription = "端口")]
    public virtual int Port { get; set; }

    /// <summary>
    /// 用户搜索基准
    /// </summary>
    [SugarColumn(ColumnDescription = "用户搜索基准", Length = 128)]
    [Required]
    public virtual string BaseDn { get; set; }

    /// <summary>
    /// 绑定DN(有管理权限制的用户)
    /// </summary>
    [SugarColumn(ColumnDescription = "绑定DN", Length = 32)]
    [Required]
    public virtual string BindDn { get; set; }

    /// <summary>
    /// 绑定密码(有管理权限制的用户密码)
    /// </summary>
    [SugarColumn(ColumnDescription = "绑定密码", Length = 512)]
    [Required]
    public virtual string BindPass { get; set; }

    /// <summary>
    /// 用户过滤规则
    /// </summary>
    [SugarColumn(ColumnDescription = "用户过滤规则", Length = 128)]
    [Required]
    public virtual string AuthFilter { get; set; } = "sAMAccountName=%s";

    /// <summary>
    /// Ldap版本
    /// </summary>
    [SugarColumn(ColumnDescription = "Ldap版本")]
    public int Version { get; set; }

    /// <summary>
    /// 绑定域账号字段属性值
    /// </summary>
    [SugarColumn(ColumnDescription = "绑定域账号字段属性值", Length = 32)]
    [Required]
    public virtual string BindAttrAccount { get; set; } = "sAMAccountName";

    /// <summary>
    /// 绑定用户EmployeeId属性值
    /// </summary>
    [SugarColumn(ColumnDescription = "绑定用户EmployeeId属性值", Length = 32)]
    [Required]
    public virtual string BindAttrEmployeeId { get; set; } = "EmployeeId";

    /// <summary>
    /// 绑定Code属性值
    /// </summary>
    [SugarColumn(ColumnDescription = "绑定对象Code属性值", Length = 64)]
    [Required]
    public virtual string BindAttrCode { get; set; } = "objectGUID";

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public StatusEnum Status { get; set; } = StatusEnum.Enable;
}