// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

 
namespace Admin.NET.Core.Service;
/// <summary>
/// 用户日程服务
/// </summary>
public class SysUserScheduleService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysUserSchedule> _sysUserSchedule;

    public SysUserScheduleService(UserManager userManager
        , SqlSugarRepository<SysUserSchedule> sysUserSchedle)
    {
        _userManager = userManager;
        _sysUserSchedule = sysUserSchedle;
    }

    /// <summary>
    /// 获取日程详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取日程详情")]
    public async Task<SysUserSchedule> GetDetail([FromQuery] UserScheduleInput input)
    {
        return await _sysUserSchedule.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 获取日程列表 
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取日程列表")]
    public async Task<List<SysUserSchedule>> Page(PageUserScheduleInput input)
    {
        return await _sysUserSchedule.AsQueryable()
            .Where(z => z.UserId == _userManager.UserId)
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()), z => z.ScheduleTime >= input.StartTime)
            .WhereIF(!string.IsNullOrWhiteSpace(input.EndTime.ToString()), z => z.ScheduleTime <= input.EndTime)
            .OrderBy(z => z.CreateTime, OrderByType.Asc)
            .ToListAsync();
    }


    /// <summary>
    /// 增加日程
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加日程")]
    public async Task<long> AddUserSchedule(AddUserScheduleInput input)
    {
        input.UserId = _userManager.UserId;

        var newOrg = await _sysUserSchedule.AsInsertable(input.Adapt<SysUserSchedule>()).ExecuteReturnEntityAsync();
        return newOrg.Id;
    }

    /// <summary>
    /// 更新日程
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新日程")]
    public async Task UpdateUserSchedule(UpdateUserScheduleInput input)
    {
        await _sysUserSchedule.AsUpdateable(input.Adapt<SysUserSchedule>()).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除日程
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除日程")]
    public async Task DeleteUserSchedule(DeleteUserScheduleInput input)
    {
        var sysUserSchedule = await _sysUserSchedule.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);

        await _sysUserSchedule.DeleteAsync(sysUserSchedule);
    }
}
