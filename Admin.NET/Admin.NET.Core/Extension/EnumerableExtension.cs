// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 数据集合拓展类
/// </summary>
public static class EnumerableExtension
{
    private static readonly ConcurrentDictionary<string, PropertyInfo> PropertyCache = new();

    /// <summary>
    /// 查询有父子关系的数据集
    /// </summary>
    /// <param name="list">数据集</param>
    /// <param name="idExpression">主键ID字段</param>
    /// <param name="parentIdExpression">父级字段</param>
    /// <param name="topParentIdValue">顶级节点父级字段值</param>
    /// <param name="isContainOneself">是否包含顶级节点本身</param>
    /// <returns></returns>
    public static IEnumerable<T> ToChildList<T, P>(this IEnumerable<T> list,
        Expression<Func<T, P>> idExpression,
        Expression<Func<T, P>> parentIdExpression,
        object topParentIdValue,
        bool isContainOneself = true)
    {
        if (list == null || !list.Any()) return Enumerable.Empty<T>();

        var propId = GetPropertyInfo(idExpression);
        var propParentId = GetPropertyInfo(parentIdExpression);

        // 查找所有顶级节点
        var topNodes = list.Where(item => Equals(propId.GetValue(item), topParentIdValue)).ToList();

        return TraverseHierarchy(list, propId, propParentId, topNodes, isContainOneself);
    }

    /// <summary>
    /// 查询有父子关系的数据集
    /// </summary>
    /// <param name="list">数据集</param>
    /// <param name="idExpression">主键ID字段</param>
    /// <param name="parentIdExpression">父级字段</param>
    /// <param name="topLevelPredicate">顶级节点的选择条件</param>
    /// <param name="isContainOneself">是否包含顶级节点本身</param>
    /// <returns></returns>
    public static IEnumerable<T> ToChildList<T, P>(this IEnumerable<T> list,
        Expression<Func<T, P>> idExpression,
        Expression<Func<T, P>> parentIdExpression,
        Expression<Func<T, bool>> topLevelPredicate,
        bool isContainOneself = true)
    {
        if (list == null || !list.Any()) return Enumerable.Empty<T>();

        // 获取顶级节点
        var topNodes = list.Where(topLevelPredicate.Compile()).ToList();

        if (!topNodes.Any()) return Enumerable.Empty<T>();

        var idPropertyInfo = GetPropertyInfo(idExpression);
        var parentPropertyInfo = GetPropertyInfo(parentIdExpression);

        return TraverseHierarchy(list, idPropertyInfo, parentPropertyInfo, topNodes, isContainOneself);
    }

    /// <summary>
    /// 辅助方法，从表达式中提取属性信息并使用临时缓存
    /// </summary>
    private static PropertyInfo GetPropertyInfo<T, P>(Expression<Func<T, P>> expression)
    {
        // 使用 ConcurrentDictionary 确保线程安全
        return PropertyCache.GetOrAdd(typeof(T).FullName + "." + ((MemberExpression)expression.Body).Member.Name, k =>
        {
            if (expression.Body is UnaryExpression { Operand: MemberExpression member }) return (PropertyInfo)member.Member;
            if (expression.Body is MemberExpression memberExpression) return (PropertyInfo)memberExpression.Member;
            throw Oops.Oh("表达式必须是一个属性访问: " + expression);
        });
    }

    /// <summary>
    /// 使用队列遍历层级结构
    /// </summary>
    private static IEnumerable<T> TraverseHierarchy<T>(IEnumerable<T> list,
        PropertyInfo idPropertyInfo,
        PropertyInfo parentPropertyInfo,
        List<T> topNodes,
        bool isContainOneself)
    {
        var queue = new Queue<T>(topNodes);
        var result = new HashSet<T>(topNodes);

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            var children = list.Where(item => Equals(parentPropertyInfo.GetValue(item), idPropertyInfo.GetValue(currentNode))).ToList();
            children.Where(child => result.Add(child)).ToList().ForEach(child => queue.Enqueue(child));
        }
        if (isContainOneself) return result;

        // 如果不需要包含顶级节点本身，则移除它们
        topNodes.ForEach(e => result.Remove(e));

        return result;
    }
}