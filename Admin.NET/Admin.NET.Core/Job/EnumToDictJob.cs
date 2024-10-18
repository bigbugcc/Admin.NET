// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 枚举转字典
/// </summary>
[JobDetail("job_EnumToDictJob", Description = "枚举转字典", GroupName = "default", Concurrent = false)]
[PeriodSeconds(1, TriggerId = "trigger_EnumToDictJob", Description = "枚举转字典", MaxNumberOfRuns = 1, RunOnStart = true)]
public class EnumToDictJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private const int OrderOffset = 10;
    private const string DefaultTagType = "info";

    public EnumToDictJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();
        var sysEnumService = serviceScope.ServiceProvider.GetRequiredService<SysEnumService>();
        // 获取数据库连接
        var db = serviceScope.ServiceProvider.GetRequiredService<ISqlSugarClient>().CopyNew();

        // 获取枚举类型列表
        var enumTypeList = sysEnumService.GetEnumTypeList();
        var enumCodeList = enumTypeList.Select(u => u.TypeName);
        // 查询数据库中已存在的枚举类型代码
        var sysDictTypeList = await db.Queryable<SysDictType>()
                                      .Includes(d => d.Children)
                                      .Where(d => enumCodeList.Contains(d.Code))
                                      .ToListAsync(stoppingToken);
        // 更新的枚举转换字典
        var updatedEnumCodes = sysDictTypeList.Select(u => u.Code);
        var updatedEnumType = enumTypeList.Where(u => updatedEnumCodes.Contains(u.TypeName)).ToList();
        var sysDictTypeDict = sysDictTypeList.ToDictionary(u => u.Code, u => u);
        var (updatedDictTypes, updatedDictDatas, newSysDictDatas) = GetUpdatedDicts(updatedEnumType, sysDictTypeDict);

        // 新增的枚举转换字典
        var newEnumType = enumTypeList.Where(u => !updatedEnumCodes.Contains(u.TypeName)).ToList();
        var (newDictTypes, newDictDatas) = GetNewSysDicts(newEnumType);

        // 执行数据库操作
        try
        {
            await db.BeginTranAsync();

            if (updatedDictTypes.Count > 0)
                await db.Updateable(updatedDictTypes).ExecuteCommandAsync(stoppingToken);

            if (updatedDictDatas.Count > 0)
                await db.Updateable(updatedDictDatas).ExecuteCommandAsync(stoppingToken);

            if (newSysDictDatas.Count > 0)
                await db.Insertable(newSysDictDatas).ExecuteCommandAsync(stoppingToken);

            if (newDictTypes.Count > 0)
                await db.Insertable(newDictTypes).ExecuteCommandAsync(stoppingToken);

            if (newDictDatas.Count > 0)
                await db.Insertable(newDictDatas).ExecuteCommandAsync(stoppingToken);

            await db.CommitTranAsync();
        }
        catch (Exception error)
        {
            await db.RollbackTranAsync();
            Log.Error($"系统枚举转换字典操作错误：{error.Message}\n堆栈跟踪：{error.StackTrace}", error);
            throw;
        }
        var originColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"【{DateTime.Now}】系统枚举转换字典");
        Console.ForegroundColor = originColor;
    }

    /// <summary>
    /// 获取需要新增的字典列表
    /// </summary>
    /// <param name="addEnumType"></param>
    /// <returns>
    /// 一个元组，包含以下元素：
    /// <list type="table">
    ///     <item><term>SysDictTypes</term><description>字典类型列表</description></item>
    ///     <item><term>SysDictDatas</term><description>字典数据列表</description></item>
    /// </list>
    /// </returns>
    private (List<SysDictType>, List<SysDictData>) GetNewSysDicts(List<EnumTypeOutput> addEnumType)
    {
        var newDictType = new List<SysDictType>();
        var newDictData = new List<SysDictData>();
        if (addEnumType.Count <= 0)
            return (newDictType, newDictData);

        // 新增字典类型
        newDictType = addEnumType.Select(u => new SysDictType
        {
            Id = YitIdHelper.NextId(),
            Code = u.TypeName,
            Name = u.TypeDescribe,
            Remark = u.TypeRemark,
            Status = StatusEnum.Enable
        }).ToList();

        // 新增字典数据
        newDictData = addEnumType.Join(newDictType, t1 => t1.TypeName, t2 => t2.Code, (t1, t2) => new
        {
            Data = t1.EnumEntities.Select(u => new SysDictData
            {
                Id = YitIdHelper.NextId(),
                DictTypeId = t2.Id,
                Name = u.Describe,
                Value = u.Value.ToString(),
                Code = u.Name,
                Remark = t2.Remark,
                OrderNo = u.Value + OrderOffset,
                TagType = u.Theme != "" ? u.Theme : DefaultTagType,
            }).ToList()
        }).SelectMany(x => x.Data).ToList();

        return (newDictType, newDictData);
    }

    /// <summary>
    /// 获取需要更新的字典列表
    /// </summary>
    /// <param name="updatedEnumType"></param>
    /// <param name="sysDictTypeDict"></param>
    /// <returns>
    /// 一个元组，包含以下元素：
    /// <list type="table">
    ///     <item><term>SysDictTypes</term><description>更新字典类型列表</description>
    ///     </item>
    ///     <item><term>SysDictDatas</term><description>更新字典数据列表</description>
    ///     </item>
    ///     <item><term>SysDictDatas</term><description>新增字典数据列表</description>
    ///     </item>
    /// </list>
    /// </returns>
    private (List<SysDictType>, List<SysDictData>, List<SysDictData>) GetUpdatedDicts(List<EnumTypeOutput> updatedEnumType, Dictionary<string, SysDictType> sysDictTypeDict)
    {
        var updatedSysDictTypes = new List<SysDictType>();
        var updatedSysDictData = new List<SysDictData>();
        var newSysDictData = new List<SysDictData>();
        foreach (var e in updatedEnumType)
        {
            if (!sysDictTypeDict.TryGetValue(e.TypeName, out var value))
                continue;

            var updatedDictType = value;
            updatedDictType.Name = e.TypeDescribe;
            updatedDictType.Remark = e.TypeRemark;
            updatedSysDictTypes.Add(updatedDictType);
            var updatedDictData = updatedDictType.Children.Where(u => u.DictTypeId == updatedDictType.Id).ToList();

            // 遍历需要更新的字典数据
            foreach (var dictData in updatedDictData)
            {
                var enumData = e.EnumEntities.FirstOrDefault(u => dictData.Code == u.Name);
                if (enumData != null)
                {
                    dictData.Value = enumData.Value.ToString();
                    dictData.OrderNo = enumData.Value + OrderOffset;
                    dictData.Name = enumData.Describe;
                    dictData.TagType = enumData.Theme != "" ? enumData.Theme : dictData.TagType != "" ? dictData.TagType : DefaultTagType;
                    updatedSysDictData.Add(dictData);
                }
            }

            // 新增的枚举值名称列表
            var newEnumDataNameList = e.EnumEntities.Select(u => u.Name).Except(updatedDictData.Select(u => u.Code));
            foreach (var newEnumDataName in newEnumDataNameList)
            {
                var enumData = e.EnumEntities.FirstOrDefault(u => newEnumDataName == u.Name);
                if (enumData != null)
                {
                    var dictData = new SysDictData
                    {
                        Id = YitIdHelper.NextId(),
                        DictTypeId = updatedDictType.Id,
                        Name = enumData.Describe,
                        Value = enumData.Value.ToString(),
                        Code = enumData.Name,
                        Remark = updatedDictType.Remark,
                        OrderNo = enumData.Value + OrderOffset,
                        TagType = enumData.Theme != "" ? enumData.Theme : DefaultTagType,
                    };
                    dictData.TagType = enumData.Theme != "" ? enumData.Theme : dictData.TagType != "" ? dictData.TagType : DefaultTagType;
                    newSysDictData.Add(dictData);
                }
            }

            // 删除的情况暂不处理
        }

        return (updatedSysDictTypes, updatedSysDictData, newSysDictData);
    }
}