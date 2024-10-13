// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 模糊查询条件
/// </summary>
public class Search
{
    /// <summary>
    /// 字段名称集合
    /// </summary>
    public List<string> Fields { get; set; }

    /// <summary>
    /// 关键字
    /// </summary>
    public string? Keyword { get; set; }
}

/// <summary>
/// 筛选过滤条件
/// </summary>
public class Filter
{
    /// <summary>
    /// 过滤条件
    /// </summary>
    public FilterLogicEnum? Logic { get; set; }

    /// <summary>
    /// 筛选过滤条件子项
    /// </summary>
    public IEnumerable<Filter>? Filters { get; set; }

    /// <summary>
    /// 字段名称
    /// </summary>
    public string? Field { get; set; }

    /// <summary>
    /// 逻辑运算符
    /// </summary>
    public FilterOperatorEnum? Operator { get; set; }

    /// <summary>
    /// 字段值
    /// </summary>
    public object? Value { get; set; }
}

/// <summary>
/// 过滤条件基类
/// </summary>
public abstract class BaseFilter
{
    /// <summary>
    /// 模糊查询条件
    /// </summary>
    public Search? Search { get; set; }

    /// <summary>
    /// 模糊查询关键字
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 筛选过滤条件
    /// </summary>
    public Filter? Filter { get; set; }
}