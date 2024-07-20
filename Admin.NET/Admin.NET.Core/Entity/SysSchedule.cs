// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统日程表
/// </summary>
[SugarTable(null, "系统日程表")]
[SysTable]
public class SysSchedule : EntityTenant
{
    /// <summary>
    /// 用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "用户Id")]
    public long UserId { get; set; }

    /// <summary>
    /// 日程日期
    /// </summary>
    [SugarColumn(ColumnDescription = "日程日期")]
    public DateTime? ScheduleTime { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    [SugarColumn(ColumnDescription = "开始时间", Length = 10)]
    public string? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [SugarColumn(ColumnDescription = "结束时间", Length = 10)]
    public string? EndTime { get; set; }

    /// <summary>
    /// 日程内容
    /// </summary>
    [SugarColumn(ColumnDescription = "日程内容", Length = 256)]
    [Required, MaxLength(256)]
    public virtual string Content { get; set; }

    /// <summary>
    /// 完成状态
    /// </summary>
    [SugarColumn(ColumnDescription = "完成状态")]
    public FinishStatusEnum Status { get; set; } = FinishStatusEnum.UnFinish;
}