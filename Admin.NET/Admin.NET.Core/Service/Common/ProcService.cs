// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.NET.Core.Service;

/// <summary>
/// 存储过程服务
/// 适用于导出、图表查询
/// </summary>
public class ProcService : IDynamicApiController, ITransient
{

    /// <summary>
    /// Post导出存储过程数据，指定导出列，没有指定的字段会被隐藏
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> PocExport2(ExportProcInput input)
    {
        ISqlSugarClient _db = App.GetService<ISqlSugarClient>();
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        var rs = await db.Ado.UseStoredProcedure()
                .GetDataTableAsync(input.ProcId, input.ProcParams);

        var excelExporter = new Magicodes.ExporterAndImporter.Excel.ExcelExporter();
        Dictionary<string, Tuple<string, int>> headers = new Dictionary<string, Tuple<string, int>>();
        var i = 1;
        foreach (var val in input.EHeader)
        {
            headers.Add(val.Key.ToUpper(), new Tuple<string, int>(val.Value, i));
            i++;
        }
        var da = await excelExporter.ExportAsByteArray(rs, new ProcExporterHeaderFilter(headers));

        return new FileContentResult(da, "application/octet-stream") { FileDownloadName = input.ProcId + ".xlsx" };
    }

    /// <summary>
    /// 根据模板导出存储过程数据
    /// </summary>
    /// <returns></returns> 
    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> PocExport(ExportProcByTMPInput input)
    {
        ISqlSugarClient _db = App.GetService<ISqlSugarClient>();
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        var rs = await db.Ado.UseStoredProcedure()
                .GetDataTableAsync(input.ProcId, input.ProcParams);

        var excelExporter = new Magicodes.ExporterAndImporter.Excel.ExcelExporter();

        string template = AppDomain.CurrentDomain.BaseDirectory + "/wwwroot/Template/" + input.Template + ".xlsx";
        var bs = await excelExporter.ExportBytesByTemplate(rs, template);
        return new FileContentResult(bs, "application/octet-stream") { FileDownloadName = input.ProcId + ".xlsx" };
    }
    /// <summary>
    /// 读取存储过程返回表
    /// 注意Oracle,达梦参数顺序不能错
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns> 
    [HttpPost]
    public async Task<DataTable> ProcTable(BaseProcInput input)
    { 
        ISqlSugarClient _db = App.GetService<ISqlSugarClient>();
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        return await db.Ado.UseStoredProcedure()
                .GetDataTableAsync(input.ProcId, input.ProcParams);
    }

    /// <summary>
    /// 读取存储过程返回数据集
    /// 注意Oracle,达梦参数顺序不能错；Oracle 返回table、table1，其他返回table1、table2
    /// 适用于报表、复杂详细页面等
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<DataSet> CommonDataSet(BaseProcInput input)
    {
        ISqlSugarClient _db = App.GetService<ISqlSugarClient>();
        var db = _db.AsTenant().GetConnectionScope(input.ConfigId);
        return await db.Ado.UseStoredProcedure()
            .GetDataSetAllAsync(input.ProcId, input.ProcParams);
    }
    /*
     * 
    //根据配置表读取对映存储过程
    public async Task<DataTable> ProcEnitybyConfig(BaseProcInput input)
    {
        string key = "ProcConfig";
        var ds = _sysCacheService.Get<Dictionary<string, string>>(key);
        if (ds == null || ds.Count == 0 || !ds.ContainsKey(input.ProcId))
        {
            var datas = await _db.Queryable<ProcConfig>().ToListAsync();
            ds = datas.ToDictionary(m => m.ProcId, m => m.ProcName);
            _sysCacheService.Set(key, ds);
        }
        string procName = ds[input.ProcId];
        return await _db.Ado.UseStoredProcedure()
            .GetDataTableAsync(procName, input.ProcParams);
    }
    */
}
