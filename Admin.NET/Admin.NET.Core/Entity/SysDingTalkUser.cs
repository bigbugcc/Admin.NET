// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core;

/// <summary>
/// 钉钉用户表
/// </summary>
[SugarTable(null, "钉钉用户表")]
[SysTable]
public class SysDingTalkUser : EntityBase
{
    /// <summary>
    /// 系统用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "系统用户Id")]
    public long SysUserId { get; set; }

    /// <summary>
    /// 系统用户
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(SysUserId))]
    [JsonIgnore]
    public SysUser SysUser { get; set; }

    /// <summary>
    /// 钉钉用户id
    /// </summary>
    [SugarColumn(ColumnDescription = "钉钉用户id", Length = 64)]
    [Required, MaxLength(64)]
    public virtual string? DingTalkUserId { get; set; }

    /// <summary>
    /// UnionId
    /// </summary>
    [SugarColumn(ColumnDescription = "UnionId", Length = 64)]
    [MaxLength(64)]
    public string? UnionId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(ColumnDescription = "用户名", Length = 64)]
    [MaxLength(64)]
    public string? Name { get; set; }

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
    /// 工号
    /// </summary>
    [SugarColumn(ColumnDescription = "工号", Length = 16)]
    [MaxLength(16)]
    public string? JobNumber { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [SugarColumn(ColumnDescription = "性别")]
    public int? Sex { get; set; }


}
