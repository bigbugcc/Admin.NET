// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统常量服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 280)]
[AllowAnonymous]
public class SysConstService : IDynamicApiController, ITransient
{
    private readonly SysCacheService _sysCacheService;

    public SysConstService(SysCacheService sysCacheService)
    {
        _sysCacheService = sysCacheService;
    }

    /// <summary>
    /// 获取所有常量列表 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取所有常量列表")]
    public async Task<List<ConstOutput>> GetList()
    {
        var key = $"{CacheConst.KeyConst}list";
        var constlist = _sysCacheService.Get<List<ConstOutput>>(key);
        if (constlist == null)
        {
            var typeList = GetConstAttributeList();
            constlist = typeList.Select(u => new ConstOutput
            {
                Name = u.CustomAttributes.ToList().FirstOrDefault()?.ConstructorArguments.ToList().FirstOrDefault().Value?.ToString() ?? u.Name,
                Code = u.Name,
                Data = GetData(Convert.ToString(u.Name))
            }).ToList();
            _sysCacheService.Set(key, constlist);
        }
        return await Task.FromResult(constlist);
    }

    /// <summary>
    /// 根据类名获取常量数据 🔖
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    [DisplayName("根据类名获取常量数据")]
    public async Task<List<ConstOutput>> GetData([Required] string typeName)
    {
        var key = $"{CacheConst.KeyConst}{typeName.ToUpper()}";
        var constlist = _sysCacheService.Get<List<ConstOutput>>(key);
        if (constlist == null)
        {
            var typeList = GetConstAttributeList();
            var type = typeList.FirstOrDefault(u => u.Name == typeName);

            var isEnum = type.BaseType.Name == "Enum";
            constlist = type.GetFields()?
                .Where(isEnum, u => u.FieldType.Name == typeName)
                .Select(u => new ConstOutput
                {
                    Name = u.Name,
                    Code = isEnum ? (int)u.GetValue(BindingFlags.Instance) : u.GetValue(BindingFlags.Instance)
                }).ToList();
            _sysCacheService.Set(key, constlist);
        }
        return await Task.FromResult(constlist);
    }

    /// <summary>
    /// 获取常量特性类型列表
    /// </summary>
    /// <returns></returns>
    private List<Type> GetConstAttributeList()
    {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(u => u.GetTypes())
            .Where(u => u.CustomAttributes.Any(c => c.AttributeType == typeof(ConstAttribute))).ToList();
    }
}