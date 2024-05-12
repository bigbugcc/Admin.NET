// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Microsoft.AspNetCore.Http;

namespace Admin.NET.Plugin.ApprovalFlow.Service;

/// <summary>
/// 审批流程服务
/// </summary>
[ApiDescriptionSettings(ApprovalFlowConst.GroupName, Order = 100)]
public class ApprovalFlowService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<ApprovalFlow> _approvalFlowRep;

    public ApprovalFlowService(SqlSugarRepository<ApprovalFlow> approvalFlowRep)
    {
        _approvalFlowRep = approvalFlowRep;
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
        return await _approvalFlowRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.SearchKey), u => u.Code.Contains(input.SearchKey.Trim()) || u.Name.Contains(input.SearchKey.Trim()) || u.Remark.Contains(input.SearchKey.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Code), u => u.Code.Contains(input.Code.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Name), u => u.Name.Contains(input.Name.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Remark), u => u.Remark.Contains(input.Remark.Trim()))
            .Select<ApprovalFlowOutput>()
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 增加审批流
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    public async Task<long> Add(AddApprovalFlowInput input)
    {
        var entity = input.Adapt<ApprovalFlow>();
        if (input.Code == null)
        {
            entity.Code = await LastCode("");
        }
        await _approvalFlowRep.InsertAsync(entity);
        return entity.Id;
    }

    /// <summary>
    /// 更新审批流
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    public async Task Update(UpdateApprovalFlowInput input)
    {
        var entity = input.Adapt<ApprovalFlow>();
        await _approvalFlowRep.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除审批流
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    public async Task Delete(DeleteApprovalFlowInput input)
    {
        var entity = await _approvalFlowRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _approvalFlowRep.FakeDeleteAsync(entity);  // 假删除
    }

    /// <summary>
    /// 获取审批流
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<ApprovalFlow> GetDetail([FromQuery] QueryByIdApprovalFlowInput input)
    {
        return await _approvalFlowRep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 根据编码获取审批流信息
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<ApprovalFlow> GetInfo([FromQuery] string code)
    {
        return await _approvalFlowRep.GetFirstAsync(u => u.Code == code);
    }

    /// <summary>
    /// 获取审批流列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<List<ApprovalFlowOutput>> GetList([FromQuery] ApprovalFlowInput input)
    {
        return await _approvalFlowRep.AsQueryable().Select<ApprovalFlowOutput>().ToListAsync();
    }

    /// <summary>
    /// 获取今天创建的最大编号
    /// </summary>
    /// <param name="prefix"></param>
    /// <returns></returns>
    private async Task<string> LastCode(string prefix)
    {
        var today = DateTime.Now.Date;
        var count = await _approvalFlowRep.AsQueryable().Where(u => u.CreateTime >= today).CountAsync();
        return prefix + DateTime.Now.ToString("yyMMdd") + string.Format("{0:d2}", count + 1);
    }

    /// <summary>
    /// 匹配审批流程
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [NonAction]
    public async Task MatchApproval(HttpContext context)
    {
        var request = context.Request;
        var response = context.Response;

        var path = request.Path.ToString().Split("/");

        var method = request.Method;
        var qs = request.QueryString;
        var h = request.Headers;
        var b = request.Body;

        var requestHeaders = request.Headers;
        var responseHeaders = response.Headers;

        await Task.CompletedTask;
    }
}