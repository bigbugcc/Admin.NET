// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using NewLife.Caching.Models;

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统缓存服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 400)]
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
    /// 申请分布式锁
    /// </summary>
    /// <param name="key">要锁定的key</param>
    /// <param name="msTimeout">申请锁等待的时间，单位毫秒</param>
    /// <param name="msExpire">锁过期时间，超过该时间没有主动是放则自动是放，必须整数秒，单位毫秒</param>
    /// <param name="throwOnFailure">失败时是否抛出异常,如不抛出异常，可通过判断返回null得知申请锁失败</param>
    /// <returns></returns>
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
            ? _cacheProvider.Cache.Keys.Where(u => u.StartsWith(_cacheOptions.Prefix)).Select(u => u[_cacheOptions.Prefix.Length..]).OrderBy(u => u).ToList()
            : ((FullRedis)_cacheProvider.Cache).Search($"{_cacheOptions.Prefix}*", int.MaxValue).Select(u => u[_cacheOptions.Prefix.Length..]).OrderBy(u => u).ToList();
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
        if (string.IsNullOrWhiteSpace(key)) return false;
        return _cacheProvider.Cache.Set($"{_cacheOptions.Prefix}{key}", value);
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
        if (string.IsNullOrWhiteSpace(key)) return false;
        return _cacheProvider.Cache.Set($"{_cacheOptions.Prefix}{key}", value, expire);
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
            : ((FullRedis)_cacheProvider.Cache).Search($"{_cacheOptions.Prefix}{prefixKey}*", int.MaxValue).ToArray();
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
            : ((FullRedis)_cacheProvider.Cache).Search($"{_cacheOptions.Prefix}{prefixKey}*", int.MaxValue).Select(u => u[_cacheOptions.Prefix.Length..]).ToList();
    }

    /// <summary>
    /// 获取缓存值 🔖
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [DisplayName("获取缓存值")]
    public object GetValue(string key)
    {
        // 若Key经过URL编码则进行解码
        if (Regex.IsMatch(key, @"%[0-9a-fA-F]{2}"))
            key = HttpUtility.UrlDecode(key);

        return _cacheProvider.Cache == Cache.Default
            ? _cacheProvider.Cache.Get<object>($"{_cacheOptions.Prefix}{key}")
            : _cacheProvider.Cache.Get<string>($"{_cacheOptions.Prefix}{key}");
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
    public RedisHash<string, T> GetHashMap<T>(string key)
    {
        return _cacheProvider.Cache.GetDictionary<T>(key) as RedisHash<string, T>;
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
        return hash.HMSet(dic);
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
        var result = hash.HMGet(fields);
        return result.ToList();
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
        var result = hash.HMGet(new string[] { field });
        return result[0];
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
        return hash.GetAll();
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
        return hash.HDel(fields);
    }

    /// <summary>
    /// 搜索HASH
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="searchModel"></param>
    /// <returns></returns>
    [NonAction]
    public List<KeyValuePair<string, T>> HashSearch<T>(string key, SearchModel searchModel)
    {
        var hash = GetHashMap<T>(key);
        return hash.Search(searchModel).ToList();
    }

    /// <summary>
    /// 搜索HASH
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="pattern"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    [NonAction]
    public List<KeyValuePair<string, T>> HashSearch<T>(string key, string pattern, int count)
    {
        var hash = GetHashMap<T>(key);
        return hash.Search(pattern, count).ToList();
    }
}