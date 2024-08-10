// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统存储过程服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 102)]
public class SysProcService : IDynamicApiController, ITransient
{
    private readonly ISqlSugarClient _db;

    public SysProcService(ISqlSugarClient db)
    {
        _db = db;
    }

    /// <summary>
    /// 导出存储过程数据-指定列，没有指定的字段会被隐藏 🔖
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> PocExport2(ExportProcInput input)
    {
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        var dt = await db.Ado.UseStoredProcedure().GetDataTableAsync(input.ProcId, input.ProcParams);

        var headers = new Dictionary<string, Tuple<string, int>>();
        var index = 1;
        foreach (var val in input.EHeader)
        {
            headers.Add(val.Key.ToUpper(), new Tuple<string, int>(val.Value, index));
            index++;
        }
        var excelExporter = new ExcelExporter();
        var da = await excelExporter.ExportAsByteArray(dt, new ProcExporterHeaderFilter(headers));
        return new FileContentResult(da, "application/octet-stream") { FileDownloadName = input.ProcId + ".xlsx" };
    }

    /// <summary>
    /// 根据模板导出存储过程数据 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<IActionResult> PocExport(ExportProcByTMPInput input)
    {
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        var dt = await db.Ado.UseStoredProcedure().GetDataTableAsync(input.ProcId, input.ProcParams);

        var excelExporter = new ExcelExporter();
        string template = AppDomain.CurrentDomain.BaseDirectory + "/wwwroot/template/" + input.Template + ".xlsx";
        var bs = await excelExporter.ExportBytesByTemplate(dt, template);
        return new FileContentResult(bs, "application/octet-stream") { FileDownloadName = input.ProcId + ".xlsx" };
    }

    /// <summary>
    /// 获取存储过程返回表-Oracle、达梦参数顺序不能错 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<DataTable> ProcTable(BaseProcInput input)
    {
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        return await db.Ado.UseStoredProcedure().GetDataTableAsync(input.ProcId, input.ProcParams);
    }

    /// <summary>
    /// 获取存储过程返回数据集-Oracle、达梦参数顺序不能错
    /// Oracle 返回table、table1，其他返回table1、table2。适用于报表、复杂详细页面等 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<DataSet> CommonDataSet(BaseProcInput input)
    {
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        return await db.Ado.UseStoredProcedure().GetDataSetAllAsync(input.ProcId, input.ProcParams);
    }

    ///// <summary>
    ///// 根据配置表获取对映存储过程
    ///// </summary>
    ///// <param name="input"></param>
    ///// <returns></returns>
    //public async Task<DataTable> ProcEnitybyConfig(BaseProcInput input)
    //{
    //    var key = "ProcConfig";
    //    var ds = _sysCacheService.Get<Dictionary<string, string>>(key);
    //    if (ds == null || ds.Count == 0 || !ds.ContainsKey(input.ProcId))
    //    {
    //        var datas = await _db.Queryable<ProcConfig>().ToListAsync();
    //        ds = datas.ToDictionary(m => m.ProcId, m => m.ProcName);
    //        _sysCacheService.Set(key, ds);
    //    }
    //    var procName = ds[input.ProcId];
    //    return await _db.Ado.UseStoredProcedure().GetDataTableAsync(procName, input.ProcParams);
    //}
}