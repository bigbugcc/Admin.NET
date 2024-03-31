// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统差异日志服务 💥
/// </summary>
[ApiDescriptionSettings(Order = 330)]
public class SysLogDiffService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysLogDiff> _sysLogDiffRep;

    public SysLogDiffService(SqlSugarRepository<SysLogDiff> sysLogDiffRep)
    {
        _sysLogDiffRep = sysLogDiffRep;
    }

    /// <summary>
    /// 获取差异日志分页列表 🔖
    /// </summary>
    /// <returns></returns>
    [SuppressMonitor]
    [DisplayName("获取差异日志分页列表")]
    public async Task<SqlSugarPagedList<SysLogDiff>> Page(PageLogInput input)
    {
        return await _sysLogDiffRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()), u => u.CreateTime >= input.StartTime)
            .WhereIF(!string.IsNullOrWhiteSpace(input.EndTime.ToString()), u => u.CreateTime <= input.EndTime)
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 清空差异日志 🔖
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Clear"), HttpPost]
    [DisplayName("清空差异日志")]
    public async Task<bool> Clear()
    {
        return await _sysLogDiffRep.DeleteAsync(u => u.Id > 0);
    }
}