// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 枚举拓展
/// </summary>
public static class EnumExtension
{
    // 枚举显示字典缓存
    private static readonly ConcurrentDictionary<Type, Dictionary<int, string>> EnumDisplayValueDict = new();

    // 枚举值字典缓存
    private static readonly ConcurrentDictionary<Type, Dictionary<int, string>> EnumNameValueDict = new();

    // 枚举类型缓存
    private static ConcurrentDictionary<string, Type> _enumTypeDict;

    /// <summary>
    /// 获取枚举对象Key与名称的字典（缓存）
    /// </summary>
    /// <param name="enumType"></param>
    /// <returns></returns>
    public static Dictionary<int, string> GetEnumDictionary(this Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("Type '" + enumType.Name + "' is not an enum.");

        // 查询缓存
        var enumDic = EnumNameValueDict.TryGetValue(enumType, out var value) ? value : new Dictionary<int, string>();
        if (enumDic.Count != 0)
            return enumDic;
        // 取枚举类型的Key/Value字典集合
        enumDic = GetEnumDictionaryItems(enumType);

        // 缓存
        EnumNameValueDict[enumType] = enumDic;

        return enumDic;
    }

    /// <summary>
    /// 获取枚举对象Key与名称的字典
    /// </summary>
    /// <param name="enumType"></param>
    /// <returns></returns>
    private static Dictionary<int, string> GetEnumDictionaryItems(this Type enumType)
    {
        // 获取类型的字段，初始化一个有限长度的字典
        var enumFields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        Dictionary<int, string> enumDic = new(enumFields.Length);

        // 遍历字段数组获取key和name
        foreach (var enumField in enumFields)
        {
            var intValue = (int)enumField.GetValue(enumType)!;
            enumDic[intValue] = enumField.Name;
        }

        return enumDic;
    }

    /// <summary>
    /// 获取枚举类型key与描述的字典（缓存）
    /// </summary>
    /// <param name="enumType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static Dictionary<int, string> GetEnumDescDictionary(this Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("Type '" + enumType.Name + "' is not an enum.");

        // 查询缓存
        var enumDic = EnumDisplayValueDict.TryGetValue(enumType, out var value)
            ? value
            : new Dictionary<int, string>();
        if (enumDic.Count != 0)
            return enumDic;
        // 取枚举类型的Key/Value字典集合
        enumDic = GetEnumDescDictionaryItems(enumType);

        // 缓存
        EnumDisplayValueDict[enumType] = enumDic;

        return enumDic;
    }

    /// <summary>
    /// 获取枚举类型key与描述的字典（没有描述则获取name）
    /// </summary>
    /// <param name="enumType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static Dictionary<int, string> GetEnumDescDictionaryItems(this Type enumType)
    {
        // 获取类型的字段，初始化一个有限长度的字典
        var enumFields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        Dictionary<int, string> enumDic = new(enumFields.Length);

        // 遍历字段数组获取key和name
        foreach (var enumField in enumFields)
        {
            var intValue = (int)enumField.GetValue(enumType)!;
            var desc = enumField.GetDescriptionValue<DescriptionAttribute>();
            enumDic[intValue] = desc != null && !string.IsNullOrEmpty(desc.Description) ? desc.Description : enumField.Name;
        }

        return enumDic;
    }

    /// <summary>
    /// 从程序集中查找指定枚举类型
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public static Type TryToGetEnumType(Assembly assembly, string typeName)
    {
        // 枚举缓存为空则重新加载枚举类型字典
        _enumTypeDict ??= LoadEnumTypeDict(assembly);

        // 按名称查找
        return _enumTypeDict.TryGetValue(typeName, out var value) ? value : null;
    }

    /// <summary>
    /// 从程序集中加载所有枚举类型
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    private static ConcurrentDictionary<string, Type> LoadEnumTypeDict(Assembly assembly)
    {
        // 取程序集中所有类型
        var typeArray = assembly.GetTypes();

        // 过滤非枚举类型，转成字典格式并返回
        var dict = typeArray.Where(o => o.IsEnum).ToDictionary(o => o.Name, o => o);
        ConcurrentDictionary<string, Type> enumTypeDict = new(dict);
        return enumTypeDict;
    }

    /// <summary>
    /// 获取枚举的Description
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetDescription(this Enum value)
    {
        return value.GetType().GetField(value.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }

    /// <summary>
    /// 获取枚举的Description
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetDescription(this object value)
    {
        return value.GetType().GetField(value.ToString()!)?.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }

    /// <summary>
    /// 获取枚举的Theme
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetTheme(this object value)
    {
        return value.GetType().GetField(value.ToString()!)?.GetCustomAttribute<ThemeAttribute>()?.Theme;
    }

    /// <summary>
    /// 将枚举转成枚举信息集合
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<EnumEntity> EnumToList(this Type type)
    {
        if (!type.IsEnum)
            throw new ArgumentException("Type '" + type.Name + "' is not an enum.");
        var arr = Enum.GetNames(type);
        return arr.Select(sl =>
        {
            var item = Enum.Parse(type, sl);
            return new EnumEntity
            {
                Name = item.ToString(),
                Describe = item.GetDescription() ?? item.ToString(),
                Theme = item.GetTheme() ?? string.Empty,
                Value = item.GetHashCode()
            };
        }).ToList();
    }

    /// <summary>
    /// 枚举ToList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<T> EnumToList<T>(this Type type)
    {
        if (!type.IsEnum)
            throw new ArgumentException("Type '" + type.Name + "' is not an enum.");
        var arr = Enum.GetNames(type);
        return arr.Select(name => (T)Enum.Parse(type, name)).ToList();
    }
}

/// <summary>
/// 枚举实体
/// </summary>
public class EnumEntity
{
    /// <summary>
    /// 枚举的描述
    /// </summary>
    public string Describe { get; set; }

    /// <summary>
    /// 枚举的样式
    /// </summary>
    public string Theme { get; set; }

    /// <summary>
    /// 枚举名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 枚举对象的值
    /// </summary>
    public int Value { get; set; }
}