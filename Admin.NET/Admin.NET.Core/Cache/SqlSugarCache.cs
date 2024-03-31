// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// SqlSugar二级缓存
/// </summary>
public class SqlSugarCache : ICacheService
{
    /// <summary>
    /// 系统缓存服务
    /// </summary>
    private static readonly SysCacheService _cache = App.GetService<SysCacheService>();

    public void Add<V>(string key, V value)
    {
        _cache.Set($"{CacheConst.SqlSugar}{key}", value);
    }

    public void Add<V>(string key, V value, int cacheDurationInSeconds)
    {
        _cache.Set($"{CacheConst.SqlSugar}{key}", value, TimeSpan.FromSeconds(cacheDurationInSeconds));
    }

    public bool ContainsKey<V>(string key)
    {
        return _cache.ExistKey($"{CacheConst.SqlSugar}{key}");
    }

    public V Get<V>(string key)
    {
        return _cache.Get<V>($"{CacheConst.SqlSugar}{key}");
    }

    public IEnumerable<string> GetAllKey<V>()
    {
        return _cache.GetKeysByPrefixKey(CacheConst.SqlSugar);
    }

    public V GetOrCreate<V>(string key, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
    {
        return _cache.GetOrAdd<V>($"{CacheConst.SqlSugar}{key}", (cacheKey) =>
        {
            return create();
        }, cacheDurationInSeconds);
    }

    public void Remove<V>(string key)
    {
        _cache.Remove(key); // SqlSugar调用Remove方法时，key中已包含了CacheConst.SqlSugar前缀
    }
}