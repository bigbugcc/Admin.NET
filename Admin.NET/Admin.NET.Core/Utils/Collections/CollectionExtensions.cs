// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 集合扩展方法
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// 检查给定的集合对象是否为空或者没有任何项
    /// </summary>
    /// <typeparam name="T">集合元素类型</typeparam>
    /// <param name="source">要检查的集合</param>
    /// <returns>如果集合为null或空则返回true，否则返回false</returns>
    public static bool IsNullOrEmpty<T>(this ICollection<T>? source)
    {
        return source is not { Count: > 0 };
    }

    /// <summary>
    /// 如果条件成立，添加项
    /// </summary>
    /// <typeparam name="T">集合元素类型</typeparam>
    /// <param name="source">要操作的集合</param>
    /// <param name="value">要添加的值</param>
    /// <param name="flag">条件标志，为true时添加项</param>
    public static void AddIf<T>(this ICollection<T> source, T value, bool flag)
    {
        _ = NotNull(source, nameof(source));

        if (flag)
        {
            source.Add(value);
        }
    }

    /// <summary>
    /// 如果条件成立，添加项
    /// </summary>
    /// <typeparam name="T">集合元素类型</typeparam>
    /// <param name="source">要操作的集合</param>
    /// <param name="value">要添加的值</param>
    /// <param name="func">条件函数，返回true时添加项</param>
    public static void AddIf<T>(this ICollection<T> source, T value, Func<bool> func)
    {
        _ = NotNull(source, nameof(source));

        if (func())
        {
            source.Add(value);
        }
    }

    /// <summary>
    /// 如果给定的集合对象不为空，则添加一个项
    /// </summary>
    /// <typeparam name="T">集合元素类型</typeparam>
    /// <param name="source">要操作的集合</param>
    /// <param name="value">要添加的值（如果不为null）</param>
    public static void AddIfNotNull<T>(this ICollection<T> source, T value)
    {
        _ = NotNull(source, nameof(source));

        if (value is not null)
        {
            source.Add(value);
        }
    }

    /// <summary>
    /// 如果集合中尚未包含该项，则将其添加到集合中
    /// </summary>
    /// <typeparam name="T">集合中项的类型</typeparam>
    /// <param name="source">集合对象</param>
    /// <param name="item">要检查并添加的项</param>
    /// <returns>如果添加了项，则返回真(True)；如果没有添加(即项已存在)则返回假(False)</returns>
    public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
    {
        _ = NotNull(source, nameof(source));

        if (source.Contains(item))
        {
            return false;
        }

        source.Add(item);
        return true;
    }

    /// <summary>
    /// 向集合中添加尚未包含的项
    /// </summary>
    /// <typeparam name="T">集合中项的类型</typeparam>
    /// <param name="source">集合对象</param>
    /// <param name="items">要检查并添加的项的集合</param>
    /// <returns>返回添加的项的集合</returns>
    public static IEnumerable<T> AddIfNotContains<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        _ = NotNull(source, nameof(source));
        var enumerable = items as T[] ?? [.. items];
        _ = NotNull(enumerable, nameof(items));

        List<T> addedItems = [];

        foreach (var item in enumerable)
        {
            if (source.Contains(item))
            {
                continue;
            }

            source.Add(item);
            addedItems.Add(item);
        }

        return addedItems;
    }

    /// <summary>
    /// 如果集合中尚未包含满足给定谓词条件的项，则将项添加到集合中
    /// </summary>
    /// <typeparam name="T">集合中项的类型</typeparam>
    /// <param name="source">集合对象</param>
    /// <param name="predicate">决定项是否已存在于集合中的条件</param>
    /// <param name="itemFactory">返回项的工厂函数</param>
    /// <returns>如果添加了项，则返回真(True)；如果没有添加(即项已存在)则返回假(False)</returns>
    public static bool AddIfNotContains<T>(this ICollection<T> source, Func<T, bool> predicate, Func<T> itemFactory)
    {
        _ = NotNull(source, nameof(source));

        if (source.Any(predicate))
        {
            return false;
        }

        source.Add(itemFactory());
        return true;
    }

    /// <summary>
    /// 移除集合中所有满足给定谓词条件的项
    /// </summary>
    /// <typeparam name="T">集合中项的类型</typeparam>
    /// <param name="source">集合对象</param>
    /// <param name="predicate">用于移除项的条件</param>
    /// <returns>被移除项的列表</returns>
    public static IList<T> RemoveAllWhere<T>(this ICollection<T> source, Func<T, bool> predicate)
    {
        _ = NotNull(source, nameof(source));

        var items = source.Where(predicate).ToList();

        foreach (var item in items)
        {
            _ = source.Remove(item);
        }

        return items;
    }

    /// <summary>
    /// 从集合中移除所有指定的项
    /// </summary>
    /// <typeparam name="T">集合中项的类型</typeparam>
    /// <param name="source">集合对象</param>
    /// <param name="items">要移除的项的集合</param>
    public static void RemoveAll<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        _ = NotNull(source, nameof(source));
        var enumerable = items as T[] ?? [.. items];
        _ = NotNull(enumerable, nameof(items));

        foreach (var item in enumerable)
        {
            _ = source.Remove(item);
        }
    }

    /// <summary>
    /// 数据不为空判断
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static T NotNull<T>([System.Diagnostics.CodeAnalysis.NotNull] T? value, string parameterName)
    {
        return value is null ? throw new ArgumentNullException(parameterName) : value;
    }
}