// 大名科技（天津）有限公司版权所有  电话：18020030720  QQ：515096995
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统存储过程服务（适用于导出、图表查询）
/// </summary>
public class SysProcService : IDynamicApiController, ITransient
{
    /// <summary>
    /// 根据模板导出存储过程数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<IActionResult> ExportPocByTemp(ExportProcByTempInput input)
    {
        var dt = await GetProcTable(input);

        var excelExporter = new ExcelExporter();
        var template = AppDomain.CurrentDomain.BaseDirectory + "/wwwroot/Template/" + input.Template + ".xlsx";
        var byteData = await excelExporter.ExportBytesByTemplate(dt, template);
        return new FileContentResult(byteData, "application/octet-stream") { FileDownloadName = input.ProcId + ".xlsx" };
    }

    /// <summary>
    /// 导出存储过程数据（指定导出列，没有指定的字段会被隐藏）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<IActionResult> ExportPocByColumn(ExportProcInput input)
    {
        var dt = await GetProcTable(input);

        var excelExporter = new ExcelExporter();
        var headers = new Dictionary<string, Tuple<string, int>>();
        var index = 1;
        foreach (var val in input.EHeader)
        {
            headers.Add(val.Key.ToUpper(), new Tuple<string, int>(val.Value, index));
            index++;
        }
        var byteData = await excelExporter.ExportAsByteArray(dt, new ProcExporterHeaderFilter(headers));
        return new FileContentResult(byteData, "application/octet-stream") { FileDownloadName = input.ProcId + ".xlsx" };
    }

    /// <summary>
    /// 读取存储过程返回表（Oracle、达梦参数顺序不能错）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private async Task<DataTable> GetProcTable(BaseProcInput input)
    {
        var db = SqlSugarSetup.ITenant.GetConnectionScope(input.ConfigId);
        return await db.Ado.UseStoredProcedure().GetDataTableAsync(input.ProcId, input.ProcParams);
    }

    /// <summary>
    /// 读取存储过程返回数据集（Oracle、达梦参数顺序不能错）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private async Task<DataSet> GetProcDataSet(BaseProcInput input)
    {
        var db = SqlSugarSetup.ITenant.GetConnectionScope(input.ConfigId);
        return await db.Ado.UseStoredProcedure().GetDataSetAllAsync(input.ProcId, input.ProcParams);
    }

    ///// <summary>
    ///// 根据配置表读取对映存储过程
    ///// </summary>
    ///// <param name="input"></param>
    ///// <returns></returns>
    //public async Task<DataTable> ProcEnitybyConfig(BaseProcInput input)
    //{
    //    string key = "ProcConfig";
    //    var ds = _sysCacheService.Get<Dictionary<string, string>>(key);
    //    if (ds == null || ds.Count == 0 || !ds.ContainsKey(input.ProcId))
    //    {
    //        var datas = await _db.Queryable<ProcConfig>().ToListAsync();
    //        ds = datas.ToDictionary(m => m.ProcId, m => m.ProcName);
    //        _sysCacheService.Set(key, ds);
    //    }
    //    string procName = ds[input.ProcId];
    //    return await _db.Ado.UseStoredProcedure()
    //        .GetDataTableAsync(procName, input.ProcParams);
    //}
}