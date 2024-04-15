// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// Redis 消息扩展
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventConsumer<T> : IDisposable
{
    private Task _consumerTask;
    private CancellationTokenSource _consumerCts;

    /// <summary>
    /// 消费者
    /// </summary>
    public IProducerConsumer<T> Consumer { get; }

    /// <summary>
    /// ConsumerBuilder
    /// </summary>
    public FullRedis Builder { get; set; }

    /// <summary>
    /// 消息回调
    /// </summary>
    public event EventHandler<T> Received;

    /// <summary>
    /// 构造函数
    /// </summary>
    public EventConsumer(FullRedis redis, string routeKey)
    {
        Builder = redis;
        Consumer = Builder.GetQueue<T>(routeKey);
    }

    /// <summary>
    /// 启动
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Start()
    {
        if (Consumer is null)
        {
            throw new InvalidOperationException("Subscribe first using the Consumer.Subscribe() function");
        }
        if (_consumerTask != null)
        {
            return;
        }
        _consumerCts = new CancellationTokenSource();
        var ct = _consumerCts.Token;
        _consumerTask = Task.Factory.StartNew(() =>
        {
            while (!ct.IsCancellationRequested)
            {
                var cr = Consumer.TakeOne(10);
                if (cr == null) continue;
                Received?.Invoke(this, cr);
            }
        }, ct, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    /// <summary>
    /// 停止
    /// </summary>
    /// <returns></returns>
    public async Task Stop()
    {
        if (_consumerCts == null || _consumerTask == null) return;
        _consumerCts.Cancel();
        try
        {
            await _consumerTask;
        }
        finally
        {
            _consumerTask = null;
            _consumerCts = null;
        }
    }

    /// <summary>
    /// 释放
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 释放
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_consumerTask != null)
            {
                Stop().Wait();
            }
            Builder.Dispose();
        }
    }
}