// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 枚举转字典
/// </summary>
[JobDetail("job_EnumToDictJob", Description = "枚举转字典", GroupName = "default", Concurrent = false)]
[PeriodSeconds(1, TriggerId = "trigger_EnumToDictJob", Description = "枚举转字典", MaxNumberOfRuns = 1, RunOnStart = true)]
public class EnumToDictJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IJsonSerializerProvider _jsonSerializer;

    public EnumToDictJob(IServiceScopeFactory scopeFactory, IJsonSerializerProvider jsonSerializer)
    {
        _scopeFactory = scopeFactory;
        _jsonSerializer = jsonSerializer;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();
        var sysEnumService = serviceScope.ServiceProvider.GetService<SysEnumService>();
        var db = serviceScope.ServiceProvider.GetService<ISqlSugarClient>().CopyNew();

        var enumTypeList = sysEnumService.GetEnumTypeList();
        var enumCodeList = enumTypeList.Select(x => x.TypeName);
        var sysDictTypeCodeList = await db.Queryable<SysDictType>().Where(x => enumCodeList.Contains(x.Code)).Select(x => x.Code).ToListAsync(stoppingToken);
        // 更新的
        var uEnumType = enumTypeList.Where(x => sysDictTypeCodeList.Contains(x.TypeName)).ToList();
        var waitUpdateSysDictType = await db.Queryable<SysDictType>().Where(x => uEnumType.Any(y => y.TypeName == x.Code)).ToListAsync(stoppingToken);
        var waitUpdateSysDictTypeDict = waitUpdateSysDictType.ToDictionary(x => x.Code, x => x);
        var waitUpdateSysDictData = await db.Queryable<SysDictData>().Where(x => uEnumType.Any(y => y.TypeName == x.DictType.Code)).ToListAsync(stoppingToken);
        var uSysDictType = new List<SysDictType>();
        var uSysDictData = new List<SysDictData>();
        if (uEnumType.Count > 0)
        {
            uEnumType.ForEach(e =>
            {
                if (waitUpdateSysDictTypeDict.TryGetValue(e.TypeName, out SysDictType value))
                {
                    var uDictType = value;
                    uDictType.Name = e.TypeDescribe;
                    uDictType.Remark = e.TypeRemark;
                    var uDictData = waitUpdateSysDictData.Where(x => x.DictTypeId == uDictType.Id).ToList();
                    if (uDictData.Count > 0)
                    {
                        uDictData.ForEach(d =>
                        {
                            var enumData = e.EnumEntities.Where(x => d.Code == x.Name).FirstOrDefault();
                            if (enumData != null)
                            {
                                d.Value = enumData.Value.ToString();
                                d.Code = enumData.Name;
                                uSysDictData.Add(d);
                            }
                        });
                    }
                    if (!uSysDictType.Any(x => x.Id == uDictType.Id))
                    {
                        uSysDictType.Add(uDictType);
                    }
                }
            });
            try
            {
                db.Ado.BeginTran();
                if (uSysDictType.Count > 0)
                {
                    await db.Updateable(uSysDictType).ExecuteCommandAsync(stoppingToken);
                }
                if (uSysDictData.Count > 0)
                {
                    await db.Updateable(uSysDictData).ExecuteCommandAsync(stoppingToken);
                }
                db.Ado.CommitTran();
            }
            catch (Exception error)
            {
                db.Ado.RollbackTran();
                Log.Error($"{context.Trigger.Description}错误：" + _jsonSerializer.Serialize(error));
                throw new Exception($"{context.Trigger.Description}错误");
            }
        }
        // 添加的
        var iEnumType = enumTypeList.Where(x => !sysDictTypeCodeList.Contains(x.TypeName)).ToList();
        if (iEnumType.Count > 0)
        {
            // 需要新增的字典类型
            var iDictType = iEnumType.Select(x => new SysDictType
            {
                Id = YitIdHelper.NextId(),
                Code = x.TypeName,
                Name = x.TypeDescribe,
                Remark = x.TypeRemark,
                Status = StatusEnum.Enable,
                OrderNo = 100
            }).ToList();
            // 需要新增的字典数据
            var dictData = iEnumType.Join(iDictType, t1 => t1.TypeName, t2 => t2.Code, (t1, t2) => new
            {
                data = t1.EnumEntities.Select(x => new SysDictData
                {
                    // 性能优化，使用BulkCopyAsync必须手动获取Id
                    Id = YitIdHelper.NextId(),
                    DictTypeId = t2.Id,
                    Name = x.Describe,
                    Value = x.Value.ToString(),
                    Code = x.Name,
                    Remark = t2.Remark,
                    OrderNo = 100,
                    TagType = "info"
                }).ToList()
            }).ToList();
            var iDictData = new List<SysDictData>();
            dictData.ForEach(item =>
            {
                iDictData.AddRange(item.data);
            });
            try
            {
                db.Ado.BeginTran();
                if (iDictType.Count > 0)
                {
                    await db.Insertable(iDictType).ExecuteCommandAsync(stoppingToken);
                }
                if (iDictData.Count > 0)
                {
                    await db.Insertable(iDictData).ExecuteCommandAsync(stoppingToken);
                }
                db.Ado.CommitTran();
            }
            catch (Exception error)
            {
                db.Ado.RollbackTran();
                Log.Error($"{context.Trigger.Description}错误：" + _jsonSerializer.Serialize(error));
                throw new Exception($"{context.Trigger.Description}错误");
            }
        }

        var originColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("【" + DateTime.Now + "】系统枚举转换字典");
        Console.ForegroundColor = originColor;
    }
}