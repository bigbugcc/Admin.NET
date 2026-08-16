// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 树扩展方法
/// </summary>
public static class TreeExtensions
{
    /// <summary>
    /// 转为树形结构
    /// </summary>
    /// <typeparam name="T">树节点数据类型</typeparam>
    /// <param name="source">源数据集合</param>
    /// <param name="isChild">判断父子关系的函数，第一个参数为父级，第二个参数为子级</param>
    /// <returns>转换后的树形结构根节点集合</returns>
    public static IEnumerable<TreeNode<T>> ToTree<T>(this IEnumerable<T> source, Func<T, T, bool> isChild)
    {
        var nodes = source.Select(value => new TreeNode<T>(value)).ToList();
        var visited = new HashSet<T>();

        foreach (var node in nodes)
        {
            if (visited.Contains(node.Value))
            {
                continue;
            }

            var stack = new Stack<TreeNode<T>>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (visited.Contains(current.Value))
                {
                    throw new InvalidOperationException("转为树形结构时，循环依赖检测到");
                }

                _ = visited.Add(current.Value);

                foreach (var child in nodes.Where(child => isChild(current.Value, child.Value)))
                {
                    current.Children.Add(child);
                    stack.Push(child);
                }
            }
        }

        return nodes.Where(node => !nodes.Any(n => n.Children.Contains(node)));
    }

    /// <summary>
    /// 根据主键和父级主键生成树形结构
    /// </summary>
    /// <typeparam name="T">树节点数据类型</typeparam>
    /// <param name="source">源数据集合</param>
    /// <param name="keySelector">主键选择器</param>
    /// <param name="parentKeySelector">父级主键选择器</param>
    /// <returns>树形结构</returns>
    public static IEnumerable<TreeNode<T>> ToTree<T>(this IEnumerable<T> source, Func<T, object> keySelector, Func<T, object> parentKeySelector)
    {
        var nodes = source.Select(value => new TreeNode<T>(value)).ToList();
        var lookup = nodes.ToLookup(node => parentKeySelector(node.Value), node => node);

        foreach (var node in nodes)
        {
            node.Children.AddRange(lookup[keySelector(node.Value)]);
        }

        return nodes.Where(node => !nodes.Any(n => keySelector(n.Value).Equals(parentKeySelector(node.Value))));
    }

    /// <summary>
    /// 添加子节点
    /// </summary>
    /// <typeparam name="T">树节点数据类型</typeparam>
    /// <param name="parent">父节点</param>
    /// <param name="value">要添加的子节点值</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddChild<T>(this TreeNode<T> parent, T value)
    {
        ArgumentNullException.ThrowIfNull(parent);

        parent.Children.Add(new TreeNode<T>(value));
    }

    /// <summary>
    /// 添加子节点到指定的父节点
    /// </summary>
    /// <typeparam name="T">树节点数据类型</typeparam>
    /// <param name="source">源数据集合</param>
    /// <param name="parent">父节点对象</param>
    /// <param name="child">子节点对象</param>
    /// <param name="keySelector">主键选择器</param>
    /// <param name="parentKeySelector">父级主键选择器</param>
    public static void AddChild<T>(this IEnumerable<TreeNode<T>> source, T parent, T child, Func<T, object> keySelector, Func<T, object> parentKeySelector)
    {
        if (parent is null)
        {
            throw new ArgumentNullException(nameof(parent), "父节点不能为空");
        }

        if (child is null)
        {
            throw new ArgumentNullException(nameof(child), "子节点不能为空");
        }

        var parentNode = source
                .DepthFirstTraversal()
                .FirstOrDefault(node => keySelector(node.Value).Equals(keySelector(parent)))
            ?? throw new InvalidOperationException("在树中未找到父节点");

        _ = parentKeySelector.Invoke(child).SetPropertyValue("Children", keySelector(parent));
        parentNode.Children.Add(new TreeNode<T>(child));
    }

    /// <summary>
    /// 删除节点
    /// </summary>
    /// <typeparam name="T">树节点数据类型</typeparam>
    /// <param name="root">根节点</param>
    /// <param name="value">要删除的节点值</param>
    /// <returns>如果成功删除节点则返回 true，否则返回 false</returns>
    public static bool RemoveNode<T>(this TreeNode<T>? root, T value)
    {
        if (root is null)
        {
            return false;
        }

        foreach (var child in root.Children.ToList())
        {
            if (EqualityComparer<T>.Default.Equals(child.Value, value))
            {
                _ = root.Children.Remove(child);
                return true;
            }

            if (RemoveNode(child, value))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 深度优先遍历 (DFS)
    /// </summary>
    /// <typeparam name="T">树节点数据类型</typeparam>
    /// <param name="root">根节点</param>
    /// <returns>深度优先遍历的节点序列</returns>
    public static IEnumerable<TreeNode<T>> DepthFirstTraversal<T>(this TreeNode<T>? root)
    {
        if (root is null)
        {
            yield break;
        }

        yield return root;

        foreach (var child in root.Children)
        {
            foreach (var descendant in child.DepthFirstTraversal())
            {
                yield return descendant;
            }
        }
    }

    /// <summary>
    /// 深度优先遍历 (DFS) - 遍历树形结构中所有节点
    /// </summary>
    /// <typeparam name="T">树节点数据类型</typeparam>
    /// <param name="source">树形结构根节点集合</param>
    /// <returns>深度优先遍历的节点序列</returns>
    public static IEnumerable<TreeNode<T>> DepthFirstTraversal<T>(this IEnumerable<TreeNode<T>>? source)
    {
        if (source is null)
        {
            yield break;
        }

        foreach (var root in source)
        {
            foreach (var node in root.DepthFirstTraversal())
            {
                yield return node;
            }
        }
    }

    /// <summary>
    /// 广度优先遍历 (BFS)
    /// </summary>
    /// <typeparam name="T">树节点数据类型</typeparam>
    /// <param name="root">根节点</param>
    /// <returns>广度优先遍历的节点序列</returns>
    public static IEnumerable<TreeNode<T>> BreadthFirstTraversal<T>(this TreeNode<T>? root)
    {
        if (root is null)
        {
            yield break;
        }

        var queue = new Queue<TreeNode<T>>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            yield return current;

            foreach (var child in current.Children)
            {
                queue.Enqueue(child);
            }
        }
    }

    /// <summary>
    /// 查找节点 (DFS)
    /// </summary>
    /// <typeparam name="T">树节点数据类型</typeparam>
    /// <param name="root">根节点</param>
    /// <param name="value">要查找的节点值</param>
    /// <returns>找到的节点，如果未找到则返回 null</returns>
    public static TreeNode<T>? FindNode<T>(this TreeNode<T> root, T value)
    {
        return root.DepthFirstTraversal().FirstOrDefault(node => EqualityComparer<T>.Default.Equals(node.Value, value));
    }

    /// <summary>
    /// 获取节点路径
    /// </summary>
    /// <typeparam name="T">树节点数据类型</typeparam>
    /// <param name="root">根节点</param>
    /// <param name="value">要获取路径的节点值</param>
    /// <returns>从根节点到目标节点的路径，如果未找到则返回 null</returns>
    public static List<TreeNode<T>>? GetPath<T>(this TreeNode<T> root, T value)
    {
        var path = new List<TreeNode<T>>();
        return FindPath(root, value, path) ? path : null;
    }

    /// <summary>
    /// 获取树的高度
    /// </summary>
    /// <typeparam name="T">树节点数据类型</typeparam>
    /// <param name="root">根节点</param>
    /// <returns>树的高度，空树返回 0</returns>
    public static int GetHeight<T>(this TreeNode<T>? root)
    {
        return root is null ? 0 : 1 + root.Children.Select(child => child.GetHeight()).DefaultIfEmpty(0).Max();
    }

    /// <summary>
    /// 获取叶子节点
    /// </summary>
    /// <typeparam name="T">树节点数据类型</typeparam>
    /// <param name="root">根节点</param>
    /// <returns>所有叶子节点的集合</returns>
    public static IEnumerable<TreeNode<T>> GetLeafNodes<T>(this TreeNode<T> root)
    {
        return root.DepthFirstTraversal().Where(node => node.Children.Count == 0);
    }

    #region 私有方法

    /// <summary>
    /// 查找路径
    /// </summary>
    /// <typeparam name="T">树节点数据类型</typeparam>
    /// <param name="node">当前节点</param>
    /// <param name="value">要查找的节点值</param>
    /// <param name="path">路径记录</param>
    /// <returns>如果找到目标节点则返回 true，否则返回 false</returns>
    private static bool FindPath<T>(TreeNode<T>? node, T value, List<TreeNode<T>> path)
    {
        if (node is null)
        {
            return false;
        }

        path.Add(node);

        if (EqualityComparer<T>.Default.Equals(node.Value, value))
        {
            return true;
        }

        foreach (var child in node.Children)
        {
            if (FindPath(child, value, path))
            {
                return true;
            }
        }

        path.RemoveAt(path.Count - 1);
        return false;
    }

    #endregion 私有方法

    /// <summary>
    /// 设置对象属性值
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="entity"></param>
    /// <param name="propertyName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool SetPropertyValue<TEntity, TValue>(this TEntity entity, string propertyName, TValue value)
    {
        var objectType = typeof(TEntity);
        var propertyInfo = objectType.GetProperty(propertyName);
        if (propertyInfo is null || !propertyInfo.PropertyType.IsGenericType)
        {
            throw new ArgumentException(string.Format("属性 '{0}' 不存在，或者不是类型 '{1}' 中的泛型类型。", propertyName, objectType.Name));
        }

        var paramObj = Expression.Parameter(objectType);
        var paramVal = Expression.Parameter(typeof(TValue));
        var bodyVal = Expression.Convert(paramVal, propertyInfo.PropertyType);

        // 获取设置属性的值的方法
        var setMethod = propertyInfo.GetSetMethod(true);

        // 如果只是只读,则 setMethod==null
        if (setMethod is null)
        {
            return false;
        }

        var body = Expression.Call(paramObj, setMethod, bodyVal);
        var setValue = Expression.Lambda<Action<TEntity, TValue>>(body, paramObj, paramVal).Compile();
        setValue(entity, value);

        return true;
    }
}

/// <summary>
/// 树节点数据传输对象
/// </summary>
/// <typeparam name="T"></typeparam>
public class TreeNode<T>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="value">节点值</param>
    public TreeNode(T value)
    {
        Value = value;
    }

    /// <summary>
    /// 节点值
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// 子节点
    /// </summary>
    public List<TreeNode<T>> Children { get; set; } = [];
}