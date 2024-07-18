// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统日程服务
/// </summary>
[ApiDescriptionSettings(Order = 295)]
public class SysScheduleService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysSchedule> _sysSchedule;

    public SysScheduleService(UserManager userManager,
        SqlSugarRepository<SysSchedule> sysSchedle)
    {
        _userManager = userManager;
        _sysSchedule = sysSchedle;
    }

    /// <summary>
    /// 获取日程列表
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取日程列表")]
    public async Task<List<SysSchedule>> Page(ListScheduleInput input)
    {
        return await _sysSchedule.AsQueryable()
            .Where(u => u.UserId == _userManager.UserId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()), u => u.ScheduleTime >= input.StartTime)
            .WhereIF(!string.IsNullOrWhiteSpace(input.EndTime.ToString()), u => u.ScheduleTime <= input.EndTime)
            .OrderBy(u => u.StarTime, OrderByType.Asc)
            .ToListAsync();
    }

    /// <summary>
    /// 获取日程详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [DisplayName("获取日程详情")]
    public async Task<SysSchedule> GetDetail(long id)
    {
        return await _sysSchedule.GetFirstAsync(u => u.Id == id);
    }

    /// <summary>
    /// 增加日程
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加日程")]
    public async Task AddUserSchedule(AddScheduleInput input)
    {
        input.UserId = _userManager.UserId;
        await _sysSchedule.InsertAsync(input.Adapt<SysSchedule>());
    }

    /// <summary>
    /// 更新日程
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新日程")]
    public async Task UpdateUserSchedule(UpdateScheduleInput input)
    {
        await _sysSchedule.AsUpdateable(input.Adapt<SysSchedule>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除日程
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除日程")]
    public async Task DeleteUserSchedule(DeleteScheduleInput input)
    {
        await _sysSchedule.DeleteAsync(u => u.Id == input.Id);
    }
    /// <summary>
    /// 设置日程状态
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("设置日程状态")]
    public async Task<int> SetStatus(ScheduleInput input)
    {
        if (!Enum.IsDefined(typeof(FinishStatusEnum), input.Status))
            throw Oops.Oh(ErrorCodeEnum.D3005);

        return await _sysSchedule.AsUpdateable()
            .SetColumns(u => u.Status == input.Status)
            .Where(u => u.Id == input.Id)
            .ExecuteCommandAsync();
    }
}