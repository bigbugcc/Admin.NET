// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using NewLife.Caching.Queues;

namespace Admin.NET.Core;

/// <summary>
/// Redis 消息队列
/// </summary>
public static class RedisQueue
{
    private static readonly ICache _cache = App.GetRequiredService<ICache>();

    /// <summary>
    /// 获取普通队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="topic"></param>
    /// <returns></returns>
    public static IProducerConsumer<T> GetQueue<T>(string topic)
    {
        var queue = (_cache as FullRedis).GetQueue<T>(topic);
        return queue;
    }

    /// <summary>
    /// 发送一个数据到队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="topic"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int AddQueue<T>(string topic, T value)
    {
        var queue = GetQueue<T>(topic);
        return queue.Add(value);
    }

    /// <summary>
    /// 发送一个数据列表到队列
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int AddQueueList<T>(string topic, List<T> value)
    {
        var queue = GetQueue<T>(topic);
        var count = queue.Count;
        var result = queue.Add(value.ToArray());
        return result - count;
    }

    /// <summary>
    /// 获取一批队列消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="topic"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static List<T> Take<T>(string topic, int count = 1)
    {
        var queue = GetQueue<T>(topic);
        var result = queue.Take(count).ToList();
        return result;
    }

    /// <summary>
    /// 获取一个队列消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="topic"></param>
    /// <returns></returns>
    public static async Task<T> TakeOneAsync<T>(string topic)
    {
        var queue = GetQueue<T>(topic);
        return await queue.TakeOneAsync(1);
    }

    /// <summary>
    /// 获取可信队列，需要确认
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="topic"></param>
    /// <returns></returns>
    public static RedisReliableQueue<T> GetRedisReliableQueue<T>(string topic)
    {
        var queue = (_cache as FullRedis).GetReliableQueue<T>(topic);
        return queue;
    }

    /// <summary>
    /// 可信队列回滚
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="retryInterval"></param>
    /// <returns></returns>
    public static int RollbackAllAck(string topic, int retryInterval = 60)
    {
        var queue = GetRedisReliableQueue<string>(topic);
        queue.RetryInterval = retryInterval;
        return queue.RollbackAllAck();
    }

    /// <summary>
    /// 发送一个数据列表到可信队列
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int AddReliableQueueList<T>(string topic, List<T> value)
    {
        var queue = (_cache as FullRedis).GetReliableQueue<T>(topic);
        var count = queue.Count;
        var result = queue.Add(value.ToArray());
        return result - count;
    }

    /// <summary>
    /// 发送一条数据到可信队列
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int AddReliableQueue<T>(string topic, T value)
    {
        var queue = (_cache as FullRedis).GetReliableQueue<T>(topic);
        var count = queue.Count;
        var result = queue.Add(value);
        return result - count;
    }

    /// <summary>
    /// 在可信队列获取一条数据
    /// </summary>
    /// <param name="topic"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ReliableTakeOne<T>(string topic)
    {
        var queue = GetRedisReliableQueue<T>(topic);
        return queue.TakeOne(1);
    }

    /// <summary>
    /// 异步在可信队列获取一条数据
    /// </summary>
    /// <param name="topic"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T> ReliableTakeOneAsync<T>(string topic)
    {
        var queue = GetRedisReliableQueue<T>(topic);
        return await queue.TakeOneAsync(1);
    }

    /// <summary>
    /// 在可信队列获取多条数据
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="count"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> ReliableTake<T>(string topic, int count)
    {
        var queue = GetRedisReliableQueue<T>(topic);
        return queue.Take(count).ToList();
    }

    /// <summary>
    /// 获取延迟队列
    /// </summary>
    /// <param name="topic"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static RedisDelayQueue<T> GetDelayQueue<T>(string topic)
    {
        var queue = (_cache as FullRedis).GetDelayQueue<T>(topic);
        return queue;
    }

    /// <summary>
    /// 发送一条数据到延迟队列
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="value"></param>
    /// <param name="delay">延迟时间。单位秒</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int AddDelayQueue<T>(string topic, T value, int delay)
    {
        var queue = GetDelayQueue<T>(topic);
        return queue.Add(value, delay);
    }

    /// <summary>
    /// 发送数据列表到延迟队列
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="value"></param>
    /// <param name="delay"></param>
    /// <typeparam name="T">延迟时间。单位秒</typeparam>
    /// <returns></returns>
    public static int AddDelayQueue<T>(string topic, List<T> value, int delay)
    {
        var queue = GetDelayQueue<T>(topic);
        queue.Delay = delay;
        return queue.Add(value.ToArray());
    }

    /// <summary>
    /// 异步在延迟队列获取一条数据
    /// </summary>
    /// <param name="topic"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T> DelayTakeOne<T>(string topic)
    {
        var queue = GetDelayQueue<T>(topic);
        return await queue.TakeOneAsync(1);
    }

    /// <summary>
    /// 在延迟队列获取多条数据
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="count"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> DelayTake<T>(string topic, int count = 1)
    {
        var queue = GetDelayQueue<T>(topic);
        return queue.Take(count).ToList();
    }
}