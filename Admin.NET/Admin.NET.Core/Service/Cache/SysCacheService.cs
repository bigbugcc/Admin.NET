// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Newtonsoft.Json;

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统缓存服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 400, Description = "系统缓存")]
public class SysCacheService : IDynamicApiController, ISingleton
{
    private static ICacheProvider _cacheProvider;
    private readonly CacheOptions _cacheOptions;

    public SysCacheService(ICacheProvider cacheProvider, IOptions<CacheOptions> cacheOptions)
    {
        _cacheProvider = cacheProvider;
        _cacheOptions = cacheOptions.Value;
    }

    /// <summary>
    /// 申请分布式锁 🔖
    /// </summary>
    /// <param name="key">要锁定的key</param>
    /// <param name="msTimeout">申请锁等待的时间，单位毫秒</param>
    /// <param name="msExpire">锁过期时间，超过该时间没有主动是放则自动是放，必须整数秒，单位毫秒</param>
    /// <param name="throwOnFailure">失败时是否抛出异常,如不抛出异常，可通过判断返回null得知申请锁失败</param>
    /// <returns></returns>
    [DisplayName("申请分布式锁")]
    public IDisposable? BeginCacheLock(string key, int msTimeout = 500, int msExpire = 10000, bool throwOnFailure = true)
    {
        try
        {
            return _cacheProvider.Cache.AcquireLock(key, msTimeout, msExpire, throwOnFailure);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 获取缓存键名集合 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取缓存键名集合")]
    public List<string> GetKeyList()
    {
        return _cacheProvider.Cache == Cache.Default
            ? [.. _cacheProvider.Cache.Keys.Where(u => u.StartsWith(_cacheOptions.Prefix)).Select(u => u[_cacheOptions.Prefix.Length..]).OrderBy(u => u)]
            : [.. ((FullRedis)_cacheProvider.Cache).Search($"{_cacheOptions.Prefix}*", 0, int.MaxValue).Select(u => u[_cacheOptions.Prefix.Length..]).OrderBy(u => u)];
    }

    /// <summary>
    /// 增加缓存
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [NonAction]
    public bool Set(string key, object value)
    {
        return !string.IsNullOrWhiteSpace(key) && _cacheProvider.Cache.Set($"{_cacheOptions.Prefix}{key}", value);
    }

    /// <summary>
    /// 增加缓存并设置过期时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expire"></param>
    /// <returns></returns>
    [NonAction]
    public bool Set(string key, object value, TimeSpan expire)
    {
        return !string.IsNullOrWhiteSpace(key) && _cacheProvider.Cache.Set($"{_cacheOptions.Prefix}{key}", value, expire);
    }

    public async Task<TR> AdGetAsync<TR>(String cacheName, Func<Task<TR>> del, TimeSpan? expiry = default) where TR : class
    {
        return await AdGetAsync<TR>(cacheName, del, [], expiry);
    }

    public async Task<TR> AdGetAsync<TR, T1>(String cacheName, Func<T1, Task<TR>> del, T1 t1, TimeSpan? expiry = default) where TR : class
    {
        return await AdGetAsync<TR>(cacheName, del, [t1], expiry);
    }

    public async Task<TR> AdGetAsync<TR, T1, T2>(String cacheName, Func<T1, T2, Task<TR>> del, T1 t1, T2 t2, TimeSpan? expiry = default) where TR : class
    {
        return await AdGetAsync<TR>(cacheName, del, [t1, t2], expiry);
    }

    public async Task<TR> AdGetAsync<TR, T1, T2, T3>(String cacheName, Func<T1, T2, T3, Task<TR>> del, T1 t1, T2 t2, T3 t3, TimeSpan? expiry = default) where TR : class
    {
        return await AdGetAsync<TR>(cacheName, del, [t1, t2, t3], expiry);
    }

    private async Task<T> AdGetAsync<T>(string cacheName, Delegate del, Object[] obs, TimeSpan? expiry) where T : class
    {
        var key = Key(cacheName, obs);
        // 使用分布式锁
        using (_cacheProvider.Cache.AcquireLock($@"lock:AdGetAsync:{cacheName}", 1000))
        {
            var value = Get<T>(key);
            if (value != null) 
            {
                return value;
            }

            value = await ((dynamic)del).DynamicInvoke(obs);
            if (value is null or ICollection { Count: 0 }) 
            {
                return value;
            }

            if (expiry == null)
            {
                Set(key, value);
            }
            else
            {
                Set(key, value, (TimeSpan)expiry);
            }
            
            return value;
        }
    }

    public T Get<T>(String cacheName, object t1)
    {
        return Get<T>(cacheName, [t1]);
    }

    public T Get<T>(String cacheName, object t1, object t2)
    {
        return Get<T>(cacheName, [t1, t2]);
    }

    public T Get<T>(String cacheName, object t1, object t2, object t3)
    {
        return Get<T>(cacheName, [t1, t2, t3]);
    }

    private T Get<T>(String cacheName, Object[] obs)
    {
        var key = Key(cacheName, obs);
        return Get<T>(key);
    }

    private static string Key(string cacheName, object[] obs)
    {
        if (obs.OfType<TimeSpan>().Any()) throw new Exception("缓存参数类型不能是:TimeSpan类型");
        StringBuilder sb = new(cacheName);
        if (obs is { Length: > 0 })
        {
            sb.Append(':');
            foreach (var a in obs) sb.Append($"<{KeySingle(a)}>");
        }
        return sb.ToString();
    }

    private static string KeySingle(object t)
    {
        return t.GetType().IsClass && !t.GetType().IsPrimitive ? JsonConvert.SerializeObject(t) : t.ToString();
    }

    /// <summary>
    /// 获取缓存的剩余生存时间
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [NonAction]
    public TimeSpan GetExpire(string key)
    {
        return _cacheProvider.Cache.GetExpire(key);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    [NonAction]
    public T Get<T>(string key)
    {
        return _cacheProvider.Cache.Get<T>($"{_cacheOptions.Prefix}{key}");
    }

    /// <summary>
    /// 删除缓存 🔖
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除缓存")]
    public int Remove(string key)
    {
        return _cacheProvider.Cache.Remove($"{_cacheOptions.Prefix}{key}");
    }

    /// <summary>
    /// 清空所有缓存 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("清空所有缓存")]
    [ApiDescriptionSettings(Name = "Clear"), HttpPost]
    public void Clear()
    {
        _cacheProvider.Cache.Clear();

        Cache.Default.Clear();
    }

    /// <summary>
    /// 检查缓存是否存在
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    [NonAction]
    public bool ExistKey(string key)
    {
        return _cacheProvider.Cache.ContainsKey($"{_cacheOptions.Prefix}{key}");
    }

    /// <summary>
    /// 根据键名前缀删除缓存 🔖
    /// </summary>
    /// <param name="prefixKey">键名前缀</param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "DeleteByPreKey"), HttpPost]
    [DisplayName("根据键名前缀删除缓存")]
    public int RemoveByPrefixKey(string prefixKey)
    {
        var delKeys = _cacheProvider.Cache == Cache.Default
            ? _cacheProvider.Cache.Keys.Where(u => u.StartsWith($"{_cacheOptions.Prefix}{prefixKey}")).ToArray()
            : ((FullRedis)_cacheProvider.Cache).Search($"{_cacheOptions.Prefix}{prefixKey}*", 0, int.MaxValue).ToArray();
        return _cacheProvider.Cache.Remove(delKeys);
    }

    /// <summary>
    /// 根据键名前缀获取键名集合 🔖
    /// </summary>
    /// <param name="prefixKey">键名前缀</param>
    /// <returns></returns>
    [DisplayName("根据键名前缀获取键名集合")]
    public List<string> GetKeysByPrefixKey(string prefixKey)
    {
        return _cacheProvider.Cache == Cache.Default
            ? _cacheProvider.Cache.Keys.Where(u => u.StartsWith($"{_cacheOptions.Prefix}{prefixKey}")).Select(u => u[_cacheOptions.Prefix.Length..]).ToList()
            : ((FullRedis)_cacheProvider.Cache).Search($"{_cacheOptions.Prefix}{prefixKey}*", 0, int.MaxValue).Select(u => u[_cacheOptions.Prefix.Length..]).ToList();
    }

    /// <summary>
    /// 获取缓存值 🔖
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [DisplayName("获取缓存值")]
    public object GetValue(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;

        if (Regex.IsMatch(key, @"%[0-9a-fA-F]{2}"))
            key = HttpUtility.UrlDecode(key);

        var fullKey = $"{_cacheOptions.Prefix}{key}";

        if (_cacheProvider.Cache == Cache.Default)
            return _cacheProvider.Cache.Get<object>(fullKey);

        if (_cacheProvider.Cache is FullRedis redisCache)
        {
            if (!redisCache.ContainsKey(fullKey))
                return null;
            try
            {
                var keyType = redisCache.TYPE(fullKey)?.ToLower();
                switch (keyType)
                {
                    case "string":
                        return redisCache.Get<string>(fullKey);

                    case "list":
                        var list = redisCache.GetList<string>(fullKey);
                        return list?.ToList();

                    case "hash":
                        var hash = redisCache.GetDictionary<string>(fullKey);
                        return hash?.ToDictionary(k => k.Key, v => v.Value);

                    case "set":
                        var set = redisCache.GetSet<string>(fullKey);
                        return set?.ToArray();

                    case "zset":
                        var sortedSet = redisCache.GetSortedSet<string>(fullKey);
                        return sortedSet?.Range(0, -1)?.ToList();

                    case "none":
                        return null;

                    default:
                        // 未知类型或特殊类型
                        return new Dictionary<string, object>
                        {
                            { "key", key },
                            { "type", keyType ?? "unknown" },
                            { "message", "无法使用标准方式获取此类型数据" }
                        };
                }
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>
                {
                    { "key", key },
                    { "error", ex.Message },
                    { "type", "exception" }
                };
            }
        }

        return _cacheProvider.Cache.Get<object>(fullKey);
    }

    /// <summary>
    /// 获取或添加缓存（在数据不存在时执行委托请求数据）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="callback"></param>
    /// <param name="expire">过期时间，单位秒</param>
    /// <returns></returns>
    [NonAction]
    public T GetOrAdd<T>(string key, Func<string, T> callback, int expire = -1)
    {
        if (string.IsNullOrWhiteSpace(key)) return default;
        return _cacheProvider.Cache.GetOrAdd($"{_cacheOptions.Prefix}{key}", callback, expire);
    }

    /// <summary>
    /// Hash匹配
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    [NonAction]
    public IDictionary<String, T> GetHashMap<T>(string key)
    {
        return _cacheProvider.Cache.GetDictionary<T>(key);
    }

    /// <summary>
    /// 批量添加HASH
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="dic"></param>
    /// <returns></returns>
    [NonAction]
    public bool HashSet<T>(string key, Dictionary<string, T> dic)
    {
        var hash = GetHashMap<T>(key);
        foreach (var v in dic)
        {
            hash.Add(v);
        }
        return true;
    }

    /// <summary>
    /// 添加一条HASH
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="hashKey"></param>
    /// <param name="value"></param>
    [NonAction]
    public void HashAdd<T>(string key, string hashKey, T value)
    {
        var hash = GetHashMap<T>(key);
        hash.Add(hashKey, value);
    }

    /// <summary>
    /// 添加或更新一条HASH
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="hashKey"></param>
    /// <param name="value"></param>
    [NonAction]
    public void HashAddOrUpdate<T>(string key, string hashKey, T value)
    {
        var hash = GetHashMap<T>(key);
        if (hash.ContainsKey(hashKey))
            hash[hashKey] = value;
        else
            hash.Add(hashKey, value);
    }

    /// <summary>
    /// 获取多条HASH
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    [NonAction]
    public List<T> HashGet<T>(string key, params string[] fields)
    {
        var hash = GetHashMap<T>(key);
        return hash.Where(t => fields.Any(c => t.Key == c)).Select(t => t.Value).ToList();
    }

    /// <summary>
    /// 获取一条HASH
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    [NonAction]
    public T HashGetOne<T>(string key, string field)
    {
        var hash = GetHashMap<T>(key);
        return hash.TryGetValue(field, out T value) ? value : default;
    }

    /// <summary>
    /// 根据KEY获取所有HASH
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    [NonAction]
    public IDictionary<string, T> HashGetAll<T>(string key)
    {
        var hash = GetHashMap<T>(key);
        return hash;
    }

    /// <summary>
    /// 删除HASH
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    [NonAction]
    public int HashDel<T>(string key, params string[] fields)
    {
        var hash = GetHashMap<T>(key);
        fields.ToList().ForEach(t => hash.Remove(t));
        return fields.Length;
    }

    ///// <summary>
    ///// 搜索HASH
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="key"></param>
    ///// <param name="searchModel"></param>
    ///// <returns></returns>
    //[NonAction]
    //public List<KeyValuePair<string, T>> HashSearch<T>(string key, SearchModel searchModel)
    //{
    //    var hash = GetHashMap<T>(key);
    //    return hash.Search(searchModel).ToList();
    //}

    ///// <summary>
    ///// 搜索HASH
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="key"></param>
    ///// <param name="pattern"></param>
    ///// <param name="count"></param>
    ///// <returns></returns>
    //[NonAction]
    //public List<KeyValuePair<string, T>> HashSearch<T>(string key, string pattern, int count)
    //{
    //    var hash = GetHashMap<T>(key);
    //    return hash.Search(pattern, count).ToList();
    //}
}