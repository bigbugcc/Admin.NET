// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

/// <summary>
/// 表名映射
/// </summary>
public class TableMapper : ITransient
{
    private readonly Dictionary<string, string> _options = new(StringComparer.OrdinalIgnoreCase);

    public TableMapper(IOptions<Dictionary<string, string>> options)
    {
        foreach (var item in options.Value)
        {
            _options.Add(item.Key, item.Value);
        }
    }

    /// <summary>
    /// 获取表别名
    /// </summary>
    /// <param name="oldname"></param>
    /// <returns></returns>
    public string GetTableName(string oldname)
    {
        return _options.ContainsKey(oldname) ? _options[oldname] : oldname;
    }
}