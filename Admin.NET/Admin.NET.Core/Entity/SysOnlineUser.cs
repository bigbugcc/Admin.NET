// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 系统在线用户表
/// </summary>
[SugarTable(null, "系统在线用户表")]
[SysTable]
public partial class SysOnlineUser : EntityTenantId
{
    /// <summary>
    /// 连接Id
    /// </summary>
    [SugarColumn(ColumnDescription = "连接Id")]
    public string? ConnectionId { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "用户Id")]
    public long UserId { get; set; }

    /// <summary>
    /// 账号
    /// </summary>
    [SugarColumn(ColumnDescription = "账号", Length = 32)]
    [Required, MaxLength(32)]
    public virtual string UserName { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "真实姓名", Length = 32)]
    [MaxLength(32)]
    public string? RealName { get; set; }

    /// <summary>
    /// 连接时间
    /// </summary>
    [SugarColumn(ColumnDescription = "连接时间")]
    public DateTime? Time { get; set; }

    /// <summary>
    /// 连接IP
    /// </summary>
    [SugarColumn(ColumnDescription = "连接IP", Length = 256)]
    [MaxLength(256)]
    public string? Ip { get; set; }

    /// <summary>
    /// 浏览器
    /// </summary>
    [SugarColumn(ColumnDescription = "浏览器", Length = 128)]
    [MaxLength(128)]
    public string? Browser { get; set; }

    /// <summary>
    /// 操作系统
    /// </summary>
    [SugarColumn(ColumnDescription = "操作系统", Length = 128)]
    [MaxLength(128)]
    public string? Os { get; set; }
}