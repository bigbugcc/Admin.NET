// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

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