// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统操作日志表
/// </summary>
[SugarTable(null, "系统操作日志表")]
[SysTable]
[LogTable]
public partial class SysLogOp : SysLogVis
{
    /// <summary>
    /// 请求方式
    /// </summary>
    [SugarColumn(ColumnDescription = "请求方式", Length = 32)]
    [MaxLength(32)]
    public string? HttpMethod { get; set; }

    /// <summary>
    /// 请求地址
    /// </summary>
    [SugarColumn(ColumnDescription = "请求地址", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? RequestUrl { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    [SugarColumn(ColumnDescription = "请求参数", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? RequestParam { get; set; }

    /// <summary>
    /// 返回结果
    /// </summary>
    [SugarColumn(ColumnDescription = "返回结果", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? ReturnResult { get; set; }

    /// <summary>
    /// 事件Id
    /// </summary>
    [SugarColumn(ColumnDescription = "事件Id")]
    public int? EventId { get; set; }

    /// <summary>
    /// 线程Id
    /// </summary>
    [SugarColumn(ColumnDescription = "线程Id")]
    public int? ThreadId { get; set; }

    /// <summary>
    /// 请求跟踪Id
    /// </summary>
    [SugarColumn(ColumnDescription = "请求跟踪Id", Length = 128)]
    [MaxLength(128)]
    public string? TraceId { get; set; }

    /// <summary>
    /// 异常信息
    /// </summary>
    [SugarColumn(ColumnDescription = "异常信息", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? Exception { get; set; }

    /// <summary>
    /// 日志消息Json
    /// </summary>
    [SugarColumn(ColumnDescription = "日志消息Json", ColumnDataType = StaticConfig.CodeFirst_BigString)]
    public string? Message { get; set; }
}