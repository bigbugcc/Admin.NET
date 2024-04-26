// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Plugin.DingTalk;
using Furion.Schedule;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Admin.NET.Plugin.Job;

/// <summary>
/// 同步钉钉用户job
/// </summary>
[JobDetail("SyncDingTalkUserJob", Description = "同步钉钉用户", GroupName = "default", Concurrent = false)]
[Daily(TriggerId = "SyncDingTalkUserTrigger", Description = "同步钉钉用户")]
public class SyncDingTalkUserJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDingTalkApi _dingTalkApi;
    private readonly ILogger _logger;

    public SyncDingTalkUserJob(IServiceScopeFactory scopeFactory, IDingTalkApi dingTalkApi, ILoggerFactory loggerFactory)
    {
        _scopeFactory = scopeFactory;
        _dingTalkApi = dingTalkApi;
        _logger = loggerFactory.CreateLogger("System.Logging.LoggingMonitor");
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();
        var _sysUserRep = serviceScope.ServiceProvider.GetRequiredService<SqlSugarRepository<SysUser>>();
        var _dingTalkUserRepo = serviceScope.ServiceProvider.GetRequiredService<SqlSugarRepository<DingTalkUser>>();
        var _dingTalkOptions = serviceScope.ServiceProvider.GetRequiredService<IOptions<DingTalkOptions>>();

        // 获取Token
        var tokenRes = await _dingTalkApi.GetDingTalkToken(_dingTalkOptions.Value.ClientId, _dingTalkOptions.Value.ClientSecret);
        if (tokenRes.ErrCode != 0)
            throw Oops.Oh(tokenRes.ErrMsg);

        var dingTalkUserList = new List<DingTalkEmpRosterFieldVo>();
        var offset = 0;
        while (offset >= 0)
        {
            // 获取用户Id列表
            var userIdsRes = await _dingTalkApi.GetDingTalkCurrentEmployeesList(tokenRes.AccessToken, new GetDingTalkCurrentEmployeesListInput
            {
                StatusList = "2,3,5,-1",
                Size = 50,
                Offset = offset
            });
            if (!userIdsRes.Success)
            {
                _logger.LogError(userIdsRes.ErrMsg);
                break;
            }
            // 根据用户Id获取花名册
            var rosterRes = await _dingTalkApi.GetDingTalkCurrentEmployeesRosterList(tokenRes.AccessToken, new GetDingTalkCurrentEmployeesRosterListInput()
            {
                UserIdList = string.Join(",", userIdsRes.Result.DataList),
                FieldFilterList = $"{DingTalkConst.NameField},{DingTalkConst.JobNumberField},{DingTalkConst.MobileField}",
                AgentId = _dingTalkOptions.Value.AgentId
            });
            if (!rosterRes.Success)
            {
                _logger.LogError(rosterRes.ErrMsg);
                break;
            }
            dingTalkUserList.AddRange(rosterRes.Result);
            if (userIdsRes.Result.NextCursor == null)
            {
                break;
            }
            // 保存分页游标
            offset = (int)userIdsRes.Result.NextCursor;
        }

        // 判断新增还是更新
        var sysDingTalkUserIdList = await _dingTalkUserRepo.AsQueryable().Select(u => new
        {
            u.Id,
            u.DingTalkUserId
        }).ToListAsync();

        var uDingTalkUser = dingTalkUserList.Where(u => sysDingTalkUserIdList.Any(m => m.DingTalkUserId == u.UserId)); // 需要更新的用户Id
        var iDingTalkUser = dingTalkUserList.Where(u => !sysDingTalkUserIdList.Any(m => m.DingTalkUserId == u.UserId)); // 需要新增的用户Id

        // 新增钉钉用户
        var iUser = iDingTalkUser.Select(res => new DingTalkUser
        {
            DingTalkUserId = res.UserId,
            Name = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.NameField).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
            Mobile = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.MobileField).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
            JobNumber = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.JobNumberField).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
        }).ToList();
        if (iUser.Count > 0)
        {
            await _dingTalkUserRepo.CopyNew().AsInsertable(iUser).ExecuteCommandAsync();
        }

        // 更新钉钉用户
        var uUser = uDingTalkUser.Select(res => new DingTalkUser
        {
            Id = sysDingTalkUserIdList.Where(u => u.DingTalkUserId == res.UserId).Select(u => u.Id).FirstOrDefault(),
            DingTalkUserId = res.UserId,
            Name = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.NameField).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
            Mobile = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.MobileField).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
            JobNumber = res.FieldDataList.Where(u => u.FieldCode == DingTalkConst.JobNumberField).Select(u => u.FieldValueList.Select(m => m.Value).FirstOrDefault()).FirstOrDefault(),
        }).ToList();
        if (uUser.Count > 0)
        {
            await _dingTalkUserRepo.CopyNew().AsUpdateable(uUser).UpdateColumns(u => new
            {
                u.DingTalkUserId,
                u.Name,
                u.Mobile,
                u.JobNumber,
                u.UpdateTime,
                u.UpdateUserName,
                u.UpdateUserId,
            }).ExecuteCommandAsync();
        }

        // 通过系统用户账号(工号)，更新钉钉用户表里面的系统用户Id
        var sysUser = await _sysUserRep.AsQueryable()
            .Select(u => new
            {
                u.Id,
                u.Account
            }).ToListAsync();
        var sysDingTalkUser = await _dingTalkUserRepo.AsQueryable()
            .Where(u => sysUser.Any(m => m.Account == u.JobNumber))
            .Select(u => new
            {
                u.Id,
                u.JobNumber,
                u.Mobile
            }).ToListAsync();
        var uSysDingTalkUser = sysDingTalkUser.Select(u => new DingTalkUser
        {
            Id = u.Id,
            SysUserId = sysUser.Where(m => m.Account == u.JobNumber).Select(u => u.Id).FirstOrDefault(),
        }).ToList();

        await _dingTalkUserRepo.CopyNew().AsUpdateable(uSysDingTalkUser).UpdateColumns(u => new
        {
            u.SysUserId,
            u.UpdateTime,
            u.UpdateUserName,
            u.UpdateUserId,
        }).ExecuteCommandAsync();

        var originColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("【" + DateTime.Now + "】同步钉钉用户");
        Console.ForegroundColor = originColor;
    }
}