// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统访问日志服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 340)]
public class SysLogVisService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysLogVis> _sysLogVisRep;

    public SysLogVisService(SqlSugarRepository<SysLogVis> sysLogVisRep)
    {
        _sysLogVisRep = sysLogVisRep;
    }

    /// <summary>
    /// 获取访问日志分页列表 🔖
    /// </summary>
    /// <returns></returns>
    [SuppressMonitor]
    [DisplayName("获取访问日志分页列表")]
    public async Task<SqlSugarPagedList<SysLogVis>> Page(PageLogInput input)
    {
        return await _sysLogVisRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()), u => u.CreateTime >= input.StartTime)
            .WhereIF(!string.IsNullOrWhiteSpace(input.EndTime.ToString()), u => u.CreateTime <= input.EndTime)
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 清空访问日志 🔖
    /// </summary>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Clear"), HttpPost]
    [DisplayName("清空访问日志")]
    public async Task<bool> Clear()
    {
        return await _sysLogVisRep.DeleteAsync(u => u.Id > 0);
    }
}