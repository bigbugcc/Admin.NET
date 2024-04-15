// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统差异日志服务 🧩
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