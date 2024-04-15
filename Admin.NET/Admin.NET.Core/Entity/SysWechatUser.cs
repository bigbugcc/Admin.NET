// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统微信用户表
/// </summary>
[SugarTable(null, "系统微信用户表")]
[SysTable]
[SugarIndex("index_{table}_N", nameof(NickName), OrderByType.Asc)]
[SugarIndex("index_{table}_M", nameof(Mobile), OrderByType.Asc)]
public partial class SysWechatUser : EntityBase
{
    /// <summary>
    /// 系统用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "系统用户Id")]
    public long UserId { get; set; }

    /// <summary>
    /// 系统用户
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(UserId))]
    public SysUser SysUser { get; set; }

    /// <summary>
    /// 平台类型
    /// </summary>
    [SugarColumn(ColumnDescription = "平台类型")]
    public PlatformTypeEnum PlatformType { get; set; } = PlatformTypeEnum.微信公众号;

    /// <summary>
    /// OpenId
    /// </summary>
    [SugarColumn(ColumnDescription = "OpenId", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string OpenId { get; set; }

    /// <summary>
    /// 会话密钥
    /// </summary>
    [SugarColumn(ColumnDescription = "会话密钥", Length = 256)]
    [MaxLength(256)]
    public string? SessionKey { get; set; }

    /// <summary>
    /// UnionId
    /// </summary>
    [SugarColumn(ColumnDescription = "UnionId", Length = 64)]
    [MaxLength(64)]
    public string? UnionId { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [SugarColumn(ColumnDescription = "昵称", Length = 64)]
    [MaxLength(64)]
    public string? NickName { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    [SugarColumn(ColumnDescription = "头像", Length = 256)]
    [MaxLength(256)]
    public string? Avatar { get; set; }

    /// <summary>
    /// 手机号码
    /// </summary>
    [SugarColumn(ColumnDescription = "手机号码", Length = 16)]
    [MaxLength(16)]
    public string? Mobile { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [SugarColumn(ColumnDescription = "性别")]
    public int? Sex { get; set; }

    /// <summary>
    /// 语言
    /// </summary>
    [SugarColumn(ColumnDescription = "语言", Length = 64)]
    [MaxLength(64)]
    public string? Language { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [SugarColumn(ColumnDescription = "城市", Length = 64)]
    [MaxLength(64)]
    public string? City { get; set; }

    /// <summary>
    /// 省
    /// </summary>
    [SugarColumn(ColumnDescription = "省", Length = 64)]
    [MaxLength(64)]
    public string? Province { get; set; }

    /// <summary>
    /// 国家
    /// </summary>
    [SugarColumn(ColumnDescription = "国家", Length = 64)]
    [MaxLength(64)]
    public string? Country { get; set; }

    /// <summary>
    /// AccessToken
    /// </summary>
    [SugarColumn(ColumnDescription = "AccessToken", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? AccessToken { get; set; }

    /// <summary>
    /// RefreshToken
    /// </summary>
    [SugarColumn(ColumnDescription = "RefreshToken", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    [SugarColumn(ColumnDescription = "ExpiresIn")]
    public int? ExpiresIn { get; set; }

    /// <summary>
    /// 用户授权的作用域，使用逗号分隔
    /// </summary>
    [SugarColumn(ColumnDescription = "授权作用域", Length = 64)]
    [MaxLength(64)]
    public string? Scope { get; set; }
}