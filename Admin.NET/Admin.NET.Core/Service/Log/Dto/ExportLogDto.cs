// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

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