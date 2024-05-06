using Admin.NET.Core.Service;
using Admin.NET.Plugin.Flow.Const;
using Admin.NET.Plugin.Flow.Entity;
using Microsoft.AspNetCore.Http;

namespace Admin.NET.Plugin.Flow;

/// <summary>
/// 审批流服务
/// </summary>
[ApiDescriptionSettings(FlowConst.GroupName, Order = 100)]
public class ApprovalFlowService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<ApprovalFlow> _rep;
    public ApprovalFlowService(SqlSugarRepository<ApprovalFlow> rep)
    {
        _rep = rep;
    }

    /// <summary>
    /// 分页查询审批流
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Page")]
    public async Task<SqlSugarPagedList<ApprovalFlowOutput>> Page(ApprovalFlowInput input)
    {
        var query = _rep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.SearchKey), u =>
                u.Code.Contains(input.SearchKey.Trim())
                || u.Name.Contains(input.SearchKey.Trim())
                || u.Remark.Contains(input.SearchKey.Trim())
            )
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Remark), u => u.Remark.Contains(input.Remark.Trim()))
            .Select<ApprovalFlowOutput>();
        return await query.ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 增加审批流
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Add")]
    public async Task<long> Add(AddApprovalFlowInput input)
    {
        var entity = input.Adapt<ApprovalFlow>();
        if (input.Code == null)
        {
            entity.Code = await LastCode("");
        }
        await _rep.InsertAsync(entity);
        return entity.Id;
    }

    /// <summary>
    /// 删除审批流
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Delete")]
    public async Task Delete(DeleteApprovalFlowInput input)
    {
        var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _rep.FakeDeleteAsync(entity);   //假删除
        //await _rep.DeleteAsync(entity);   //真删除
    }

    /// <summary>
    /// 更新审批流
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Update")]
    public async Task Update(UpdateApprovalFlowInput input)
    {
        var entity = input.Adapt<ApprovalFlow>();
        await _rep.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取审批流
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "Detail")]
    public async Task<ApprovalFlow> Detail([FromQuery] QueryByIdApprovalFlowInput input)
    {
        return await _rep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 通过 code 获取审批流信息
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "Info")]
    public async Task<ApprovalFlow> Info([FromQuery] string code)
    {
        return await _rep.GetFirstAsync(u => u.Code == code);
    }

    /// <summary>
    /// 获取审批流列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "List")]
    public async Task<List<ApprovalFlowOutput>> List([FromQuery] ApprovalFlowInput input)
    {
        return await _rep.AsQueryable().Select<ApprovalFlowOutput>().ToListAsync();
    }




    /// <summary>
    /// 获取今天创建的最大编号
    /// </summary>
    /// <param name="prefix"></param>
    /// <returns></returns>
    private async Task<string> LastCode(string prefix)
    {
        var today = DateTime.Now.Date;
        var count = await _rep.AsQueryable().Where(u => u.CreateTime >= today).CountAsync();
        return prefix + DateTime.Now.ToString("yyMMdd") + string.Format("{0:d2}", count + 1);
    }
}

