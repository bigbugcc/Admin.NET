using Admin.NET.Core.Service;
using Admin.NET.Application.Const;
using Admin.NET.Application.Entity;
using Microsoft.AspNetCore.Http;
using Magicodes.ExporterAndImporter.Core.Models;
using Furion.Logging;
using Yitter.IdGenerator;
namespace Admin.NET.Application;
/// <summary>
/// 申请示例服务
/// </summary>
[ApiDescriptionSettings(ApplicationConst.GroupName, Order = 100)]
public class Dm_ApplyDemoService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<Dm_ApplyDemo> _rep;
    public Dm_ApplyDemoService(SqlSugarRepository<Dm_ApplyDemo> rep)
    {
        _rep = rep;
    }

    /// <summary>
    /// 分页查询申请示例
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Page")]
    public async Task<SqlSugarPagedList<Dm_ApplyDemoOutput>> Page(Dm_ApplyDemoInput input)
    {
        ISugarQueryable<Dm_ApplyDemoOutput> query = Query(input).Select<Dm_ApplyDemoOutput>();
        return await query.OrderBuilder(input).ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private ISugarQueryable<Dm_ApplyDemo> Query(Dm_ApplyDemoInput input)
    {
        var query = _rep.AsQueryable().Where(m=>!m.IsDelete)
            .WhereIF(!string.IsNullOrWhiteSpace(input.SearchKey), u =>
                u.ApplyNO.Contains(input.SearchKey.Trim())
                || u.Remark.Contains(input.SearchKey.Trim())
            )
            .WhereIF(input.OrgType > 0, u => u.OrgType == input.OrgType)
            .WhereIF(!string.IsNullOrWhiteSpace(input.ApplyNO), u => u.ApplyNO.Contains(input.ApplyNO.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Remark), u => u.Remark.Contains(input.Remark.Trim()))
            ;
        if (input.ApplicatDateRange != null && input.ApplicatDateRange.Count > 0)
        {
            DateTime? start = input.ApplicatDateRange[0];
            query = query.WhereIF(start.HasValue, u => u.ApplicatDate > start);
            if (input.ApplicatDateRange.Count > 1 && input.ApplicatDateRange[1].HasValue)
            {
                var end = input.ApplicatDateRange[1].Value.AddDays(1);
                query = query.Where(u => u.ApplicatDate < end);
            }
        }

        return query;
    }

    /// <summary>
    /// 导出查询数据
    /// </summary>
    /// <returns></returns> 
    [HttpPost]
    public async Task<IActionResult> Export(Dm_ApplyDemoInput input)
    { 
        //如果想速度更快，推荐使用存储过程查询导出；如果想自定义导出样式，推荐使用模板。
        return await CommonUtil.ExportExcelData<Dm_ApplyDemo, Dm_ApplyDemoInport>(Query(input));
    }

    /// <summary>
    /// 增加申请示例
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Add")]
    public async Task<long> Add(AddDm_ApplyDemoInput input)
    {
        var entity = input.Adapt<Dm_ApplyDemo>();
        await _rep.InsertAsync(entity);
        return entity.Id;
    }
    /// <summary>
    /// 批量增加申请示例
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "PatchAdd")]
    public async Task<List<PathAddApplyDemoItem>> PatchAdd(List<PathAddApplyDemoItem> input)
    {
        foreach (var item in input)
        {
            var entity = item.Adapt<Dm_ApplyDemo>();
            entity.Id = YitIdHelper.NextId();
            await _rep.InsertAsync(entity);
            item.Id= entity.Id;
        }
        return input;
    }

    /// <summary>
    /// 删除申请示例
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Delete")]
    public async Task Delete(DeleteDm_ApplyDemoInput input)
    {
        var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _rep.FakeDeleteAsync(entity);   //假删除
        //await _rep.DeleteAsync(entity);   //真删除
    }

    /// <summary>
    /// 更新申请示例
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Update")]
    public async Task Update(UpdateDm_ApplyDemoInput input)
    {
        var entity = input.Adapt<Dm_ApplyDemo>();
        await _rep.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取申请示例
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "Detail")]
    public async Task<Dm_ApplyDemo> Detail([FromQuery] QueryByIdDm_ApplyDemoInput input)
    {
        return await _rep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 获取申请示例列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [HttpPost]
    [ApiDescriptionSettings(Name = "List")]
    public async Task<List<Dm_ApplyDemoOutput>> List([FromQuery] Dm_ApplyDemoInput input)
    {
        ISugarQueryable<Dm_ApplyDemoOutput> query = Query(input).Select<Dm_ApplyDemoOutput>();
        return await query.Select<Dm_ApplyDemoOutput>().ToListAsync();
    }


    /// <summary>
    /// 导入数据
    /// </summary>
    /// <param name="file">Excel文件</param> 
    /// <returns></returns> 
    [AllowAnonymous]
    public async Task<AdminResult<string>> Import([Required] IFormFile file)
    {
        if (file == null) throw Oops.Oh(ErrorCodeEnum.D8000);
        var Importer = new Magicodes.ExporterAndImporter.Excel.ExcelImporter();
        var importResult = await CommonUtil.ImportExcelData<Dm_ApplyDemoInport>(file);
        string message=string.Empty;
        if (importResult != null)
        {
            //TODO 自定义校验 
            int errorCount = 0;

            //导入数据转为对象实例 
            var ysLs = new List<Dm_ApplyDemo>();
            var ls = CommonUtil.ParseList<Dm_ApplyDemoInport, Dm_ApplyDemo>(importResult, (source, target) =>
            {
                if (target.Id == 0)
                {
                    return target;
                }
                else
                {
                    _rep.Context.Updateable(target).IgnoreColumns(true).ExecuteCommand();
                    return null;
                }
            });
            if (ls.Count > 0)
            {
                await _rep.Context.Insertable(ls).UseParameter().ExecuteCommandAsync();
            }
            _rep.Context.Ado.CommitTran();
            message = $"导入总数：{importResult.Count},导入成功：{importResult.Count- errorCount},导入失败：{ errorCount}。";
        }

        var ret = new AdminResult<string>()
        {
            Code = 200,
            Type = "success",
            Message = message,
            Result = message
        };
        return ret;
    }


}

