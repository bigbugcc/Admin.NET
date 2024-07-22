// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using NewLife.Caching.Queues;
using Newtonsoft.Json;
using System.Threading.Channels;

namespace Admin.NET.Core;

/// <summary>
/// Redis自定义事件源存储器
/// </summary>
/// <remarks>
/// 在集群部署时，一般每一个消息只由一个服务节点消费一次。
/// 有些特殊情情要通知到服务器群中的每一个节点(比如需要强制加载某些配置、重点服务等)，
/// 在这种情况下就要以“broadcast:”开头来定义EventId，
/// 本系统会把“broadcast:”开头的事件视为“广播消息”保证集群中的每一个服务节点都能消费得到这个消息
/// </remarks>
public sealed class RedisEventSourceStorer : IEventSourceStorer, IDisposable
{
    /// <summary>
    /// 消费者
    /// </summary>
    private readonly EventConsumer<ChannelEventSource> _eventConsumer;

    /// <summary>
    /// 内存通道事件源存储器
    /// </summary>
    private readonly Channel<IEventSource> _channel;

    private IProducerConsumer<ChannelEventSource> _queueSingle;

    private RedisStream<string> _queueBroadcast;

    private ILogger<RedisEventSourceStorer> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="cacheProvider">Redis 连接对象</param>
    /// <param name="routeKey">路由键</param>
    /// <param name="capacity">存储器最多能够处理多少消息，超过该容量进入等待写入</param>
    public RedisEventSourceStorer(ICacheProvider cacheProvider, string routeKey, int capacity)
    {
        _logger = App.GetRequiredService<ILogger<RedisEventSourceStorer>>();

        // 配置通道，设置超出默认容量后进入等待
        var boundedChannelOptions = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        // 创建有限容量通道
        _channel = Channel.CreateBounded<IEventSource>(boundedChannelOptions);

        //_redis = redis as FullRedis;

        // 创建广播消息订阅者，即所有服务器节点都能收到消息（用来发布重启、Reload配置等消息）
        FullRedis redis = (FullRedis)cacheProvider.Cache;
        var clusterOpt = App.GetConfig<ClusterOptions>("Cluster", true);
        _queueBroadcast = redis.GetStream<string>(routeKey + ":broadcast");
        _queueBroadcast.Group = clusterOpt.ServerId;//根据服务器标识分配到不同的分组里
        _queueBroadcast.Expire = TimeSpan.FromSeconds(10);//消息10秒过期（）
        _queueBroadcast.ConsumeAsync(OnConsumeBroadcast);

        // 创建队列消息订阅者，只要有一个服务节点消费了消息即可
        _queueSingle = redis.GetQueue<ChannelEventSource>(routeKey + ":single");
        _eventConsumer = new EventConsumer<ChannelEventSource>(_queueSingle);

        // 订阅消息写入 Channel
        _eventConsumer.Received += async (send, cr) =>
        {
            // var oriColor = Console.ForegroundColor;
            try
            {
                ChannelEventSource ces = (ChannelEventSource)cr;
                await ConsumeChannelEventSourceAsync(ces, ces.CancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "处理Received中的消息产生错误！");
            }
        };
        _eventConsumer.Start();
    }

    private async Task OnConsumeBroadcast(string source, Message message, CancellationToken token)
    {
        ChannelEventSource ces = JsonConvert.DeserializeObject<ChannelEventSource>(source);
        await ConsumeChannelEventSourceAsync(ces, token);
    }

    private async Task ConsumeChannelEventSourceAsync(ChannelEventSource ces, CancellationToken cancel = default)
    {
        // 打印测试事件
        if (ces.EventId != null && ces.EventId.IndexOf(":Test") > 0)
        {
            var oriColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"有消息要处理{ces.EventId},{ces.Payload}");
            Console.ForegroundColor = oriColor;
        }
        await _channel.Writer.WriteAsync(ces, cancel);
    }

    /// <summary>
    /// 将事件源写入存储器
    /// </summary>
    /// <param name="eventSource">事件源对象</param>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns><see cref="ValueTask"/></returns>
    public async ValueTask WriteAsync(IEventSource eventSource, CancellationToken cancellationToken)
    {
        // 空检查
        if (eventSource == default)
            throw new ArgumentNullException(nameof(eventSource));

        // 这里判断是否是 ChannelEventSource 或者 自定义的 EventSource
        if (eventSource is ChannelEventSource source)
        {
            // 异步发布
            await Task.Factory.StartNew(() =>
            {
                if (source.EventId != null && source.EventId.StartsWith("broadcast:"))
                {
                    string str = JsonConvert.SerializeObject(source);
                    _queueBroadcast.Add(str);
                }
                else
                {
                    _queueSingle.Add(source);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, System.Threading.Tasks.TaskScheduler.Default);
        }
        else
        {
            // 处理动态订阅问题
            await _channel.Writer.WriteAsync(eventSource, cancellationToken);
        }
    }

    /// <summary>
    /// 从存储器中读取一条事件源
    /// </summary>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns>事件源对象</returns>
    public async ValueTask<IEventSource> ReadAsync(CancellationToken cancellationToken)
    {
        // 读取一条事件源
        var eventSource = await _channel.Reader.ReadAsync(cancellationToken);
        return eventSource;
    }

    /// <summary>
    /// 释放非托管资源
    /// </summary>
    public async void Dispose()
    {
        await _eventConsumer.Stop();
        GC.SuppressFinalize(this);
    }
}