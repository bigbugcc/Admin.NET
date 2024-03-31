// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 导出日志数据
/// </summary>
[ExcelExporter(Name = "日志数据", TableStyle = OfficeOpenXml.Table.TableStyles.None, AutoFitAllColumn = true)]
public class ExportLogDto
{
    /// <summary>
    /// 记录器类别名称
    /// </summary>
    [ExporterHeader(DisplayName = "记录器类别名称", IsBold = true)]
    public string LogName { get; set; }

    /// <summary>
    /// 日志级别
    /// </summary>
    [ExporterHeader(DisplayName = "日志级别", IsBold = true)]
    public string LogLevel { get; set; }

    /// <summary>
    /// 事件Id
    /// </summary>
    [ExporterHeader(DisplayName = "事件Id", IsBold = true)]
    public string EventId { get; set; }

    /// <summary>
    /// 日志消息
    /// </summary>
    [ExporterHeader(DisplayName = "日志消息", IsBold = true)]
    public string Message { get; set; }

    /// <summary>
    /// 异常对象
    /// </summary>
    [ExporterHeader(DisplayName = "异常对象", IsBold = true)]
    public string Exception { get; set; }

    /// <summary>
    /// 当前状态值
    /// </summary>
    [ExporterHeader(DisplayName = "当前状态值", IsBold = true)]
    public string State { get; set; }

    /// <summary>
    /// 日志记录时间
    /// </summary>
    [ExporterHeader(DisplayName = "日志记录时间", IsBold = true)]
    public DateTime LogDateTime { get; set; }

    /// <summary>
    /// 线程Id
    /// </summary>
    [ExporterHeader(DisplayName = "线程Id", IsBold = true)]
    public int ThreadId { get; set; }

    /// <summary>
    /// 请求跟踪Id
    /// </summary>
    [ExporterHeader(DisplayName = "请求跟踪Id", IsBold = true)]
    public string TraceId { get; set; }
}