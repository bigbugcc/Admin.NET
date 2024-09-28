// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统异常日志服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 350)]
public class SysLogExService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysLogEx> _sysLogExRep;

    public SysLogExService(SqlSugarRepository<SysLogEx> sysLogExRep)
    {
        _sysLogExRep = sysLogExRep;
    }

    /// <summary>
    /// 获取异常日志分页列表 🔖
    /// </summary>
    /// <returns></returns>
    [SuppressMonitor]
    [DisplayName("获取异常日志分页列表")]
    public async Task<SqlSugarPagedList<SysLogEx>> Page(PageExLogInput input)
    {
        return await _sysLogExRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()), u => u.CreateTime >= input.StartTime)
            .WhereIF(!string.IsNullOrWhiteSpace(input.EndTime.ToString()), u => u.CreateTime <= input.EndTime)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Account), u => u.Account == input.Account)
            .WhereIF(!string.IsNullOrWhiteSpace(input.ControllerName), u => u.ControllerName == input.ControllerName)
            .WhereIF(!string.IsNullOrWhiteSpace(input.ActionName), u => u.ActionName == input.ActionName)
            .WhereIF(!string.IsNullOrWhiteSpace(input.RemoteIp), u => u.RemoteIp == input.RemoteIp)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Elapsed.ToString()), u => u.Elapsed >= input.Elapsed)
            .WhereIF(!string.IsNullOrWhiteSpace(input.Status) && input.Status == "200", u => u.Status == "200")
            .WhereIF(!string.IsNullOrWhiteSpace(input.Status) && input.Status != "200", u => u.Status != "200")
            //.OrderBy(u => u.CreateTime, OrderByType.Desc)
            .IgnoreColumns(u => new { u.RequestParam, u.ReturnResult, u.Message })
            .OrderBuilder(input)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 获取异常日志详情 🔖
    /// </summary>
    /// <returns></returns>
    [SuppressMonitor]
    [DisplayName("获取异常日志详情")]
    public async Task<SysLogEx> GetDetail(long id)
    {
        return await _sysLogExRep.GetFirstAsync(u => u.Id == id);
    }

    /// <summary>
    /// 清空异常日志 🔖
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Clear"), HttpPost]
    [DisplayName("清空异常日志")]
    public void Clear()
    {
        _sysLogExRep.AsSugarClient().DbMaintenance.TruncateTable<SysLogEx>();
    }

    /// <summary>
    /// 导出异常日志 🔖
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Export"), NonUnify]
    [DisplayName("导出异常日志")]
    public async Task<IActionResult> ExportLogEx(LogInput input)
    {
        var logExList = await _sysLogExRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()) && !string.IsNullOrWhiteSpace(input.EndTime.ToString()),
                    u => u.CreateTime >= input.StartTime && u.CreateTime <= input.EndTime)
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .Select<ExportLogDto>().ToListAsync();

        IExcelExporter excelExporter = new ExcelExporter();
        var res = await excelExporter.ExportAsByteArray(logExList);
        return new FileStreamResult(new MemoryStream(res), "application/octet-stream") { FileDownloadName = DateTime.Now.ToString("yyyyMMddHHmm") + "异常日志.xlsx" };
    }
}