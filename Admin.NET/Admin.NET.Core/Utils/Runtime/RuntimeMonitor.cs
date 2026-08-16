// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 运行时监控器
/// </summary>
public class RuntimeMonitor : IDisposable
{
    private readonly Process _currentProcess;
    private readonly ConcurrentQueue<PerformanceSnapshot> _snapshots;
    private readonly Timer? _monitorTimer;
    private readonly object _lockObject = new();
    private readonly int _maxSnapshotCount;
    private volatile bool _isDisposed;
    private DateTime _lastCpuTime;
    private TimeSpan _lastTotalProcessorTime;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="monitorInterval">监控间隔，默认5秒</param>
    /// <param name="maxSnapshotCount">最大快照数量，默认1000</param>
    public RuntimeMonitor(TimeSpan? monitorInterval = null, int maxSnapshotCount = 1000)
    {
        _currentProcess = Process.GetCurrentProcess();
        _snapshots = new ConcurrentQueue<PerformanceSnapshot>();
        _maxSnapshotCount = maxSnapshotCount;
        MonitorInterval = monitorInterval ?? TimeSpan.FromSeconds(5);

        // 初始化CPU时间基准
        _lastCpuTime = DateTime.UtcNow;
        _lastTotalProcessorTime = _currentProcess.TotalProcessorTime;

        // 创建定时器但不立即启动
        _monitorTimer = new Timer(CollectSnapshot, null, Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>
    /// 监控间隔
    /// </summary>
    public TimeSpan MonitorInterval { get; }

    /// <summary>
    /// 是否正在监控
    /// </summary>
    public bool IsMonitoring { get; private set; }

    /// <summary>
    /// 快照历史记录数量
    /// </summary>
    public int SnapshotCount => _snapshots.Count;

    /// <summary>
    /// 监控开始时间
    /// </summary>
    public DateTime? MonitorStartTime { get; private set; }

    /// <summary>
    /// 强制垃圾回收
    /// </summary>
    /// <param name="generation">GC代数，-1表示全部代</param>
    /// <param name="mode">GC模式</param>
    public static void ForceGarbageCollection(int generation = -1, GCCollectionMode mode = GCCollectionMode.Default)
    {
        if (generation == -1)
        {
            GC.Collect();
        }
        else
        {
            GC.Collect(generation, mode);
        }

        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    /// <summary>
    /// 获取内存压力信息
    /// </summary>
    /// <returns>内存压力信息</returns>
    public static string GetMemoryPressureInfo()
    {
        var info = GC.GetGCMemoryInfo();
        return string.Format("总内存: {0} MB, ", info.TotalAvailableMemoryBytes / 1024 / 1024) +
               string.Format("高内存负载阈值: {0} MB, ", info.HighMemoryLoadThresholdBytes / 1024 / 1024) +
               string.Format("堆大小: {0} MB, ", info.HeapSizeBytes / 1024 / 1024) +
               string.Format("内存负载: {0} MB", info.MemoryLoadBytes / 1024 / 1024);
    }

    /// <summary>
    /// 开始监控
    /// </summary>
    public void StartMonitoring()
    {
        if (IsMonitoring || _isDisposed)
        {
            return;
        }

        lock (_lockObject)
        {
            if (IsMonitoring || _isDisposed)
            {
                return;
            }

            IsMonitoring = true;
            MonitorStartTime = DateTime.Now;

            // 立即收集一次快照
            CollectSnapshot(null);

            // 启动定时器
            _monitorTimer?.Change(MonitorInterval, MonitorInterval);
        }
    }

    /// <summary>
    /// 停止监控
    /// </summary>
    public void StopMonitoring()
    {
        if (!IsMonitoring)
        {
            return;
        }

        lock (_lockObject)
        {
            if (!IsMonitoring)
            {
                return;
            }

            IsMonitoring = false;
            _monitorTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }

    /// <summary>
    /// 获取当前性能快照
    /// </summary>
    /// <returns>当前性能快照</returns>
    public PerformanceSnapshot GetCurrentSnapshot()
    {
        return CreateSnapshot();
    }

    /// <summary>
    /// 获取最新的性能快照
    /// </summary>
    /// <returns>最新的性能快照，如果没有则返回null</returns>
    public PerformanceSnapshot? GetLatestSnapshot()
    {
        return _snapshots.TryPeek(out var snapshot) ? snapshot : null;
    }

    /// <summary>
    /// 获取所有性能快照
    /// </summary>
    /// <returns>性能快照列表</returns>
    public List<PerformanceSnapshot> GetAllSnapshots()
    {
        return [.. _snapshots];
    }

    /// <summary>
    /// 获取指定时间范围内的快照
    /// </summary>
    /// <param name="timeRange">时间范围</param>
    /// <returns>快照列表</returns>
    public List<PerformanceSnapshot> GetSnapshotsInTimeRange(TimeSpan timeRange)
    {
        var cutoffTime = DateTime.Now - timeRange;
        return [.. _snapshots.Where(s => s.Timestamp >= cutoffTime)];
    }

    /// <summary>
    /// 分析性能趋势
    /// </summary>
    /// <param name="counterType">计数器类型</param>
    /// <param name="timeRange">分析时间范围，null表示分析所有数据</param>
    /// <returns>性能趋势分析结果</returns>
    public PerformanceTrend AnalyzeTrend(PerformanceCounterType counterType, TimeSpan? timeRange = null)
    {
        var snapshots = timeRange.HasValue
            ? GetSnapshotsInTimeRange(timeRange.Value)
            : GetAllSnapshots();

        if (snapshots.Count == 0)
        {
            return new PerformanceTrend
            {
                CounterType = counterType,
                SampleCount = 0,
                AnalysisTimespan = timeRange ?? TimeSpan.Zero
            };
        }

        var values = snapshots.Select(s => GetCounterValue(s, counterType)).ToList();

        var trend = new PerformanceTrend
        {
            CounterType = counterType,
            CurrentValue = values.LastOrDefault(),
            AverageValue = values.Average(),
            MinValue = values.Min(),
            MaxValue = values.Max(),
            SampleCount = values.Count,
            AnalysisTimespan = timeRange ?? (snapshots.Last().Timestamp - snapshots.First().Timestamp)
        };

        // 计算标准差
        var variance = values.Select(v => Math.Pow(v - trend.AverageValue, 2)).Average();
        trend.StandardDeviation = Math.Sqrt(variance);

        // 计算趋势（简单线性回归的斜率）
        if (values.Count >= 2)
        {
            var n = values.Count;
            var sumX = Enumerable.Range(0, n).Sum();
            var sumY = values.Sum();
            var sumXy = values.Select((v, i) => i * v).Sum();
            var sumX2 = Enumerable.Range(0, n).Select(i => i * i).Sum();

            trend.Trend = ((n * sumXy) - (sumX * sumY)) / ((n * sumX2) - (sumX * sumX));
        }

        return trend;
    }

    /// <summary>
    /// 清空所有快照
    /// </summary>
    public void ClearSnapshots()
    {
        while (_snapshots.TryDequeue(out _)) { }
    }

    /// <summary>
    /// 获取性能摘要报告
    /// </summary>
    /// <returns>性能摘要字符串</returns>
    public string GetPerformanceSummary()
    {
        var current = GetCurrentSnapshot();
        var cpuTrend = AnalyzeTrend(PerformanceCounterType.CpuUsage, TimeSpan.FromMinutes(5));
        //var memoryTrend = AnalyzeTrend(PerformanceCounterType.MemoryUsage, TimeSpan.FromMinutes(5));

        return string.Format("运行时性能摘要:\n") +
               string.Format("  CPU使用率: {0:F1}% (5分钟平均: {1:F1}%)\n", current.CpuUsage, cpuTrend.AverageValue) +
               string.Format("  内存使用: {0} MB (托管内存: {1} MB)\n", current.MemoryUsage / 1024 / 1024, current.ManagedMemory / 1024 / 1024) +
               string.Format("  GC收集: Gen0={0}, Gen1={1}, Gen2={2}\n", current.GcGen0Collections, current.GcGen1Collections, current.GcGen2Collections) +
               string.Format("  线程数: {0}, 句柄数: {1}\n", current.ThreadCount, current.HandleCount) +
               string.Format("  运行时间: {0:dd\\.hh\\:mm\\:ss}\n", current.ProcessUptime) +
               string.Format("  监控状态: {0}, 快照数: {1}", IsMonitoring ? "运行中" : "已停止", SnapshotCount);
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        StopMonitoring();
        _monitorTimer?.Dispose();
        _currentProcess?.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 获取计数器值
    /// </summary>
    /// <param name="snapshot">性能快照</param>
    /// <param name="counterType">计数器类型</param>
    /// <returns>计数器值</returns>
    private static double GetCounterValue(PerformanceSnapshot snapshot, PerformanceCounterType counterType)
    {
        return counterType switch
        {
            PerformanceCounterType.CpuUsage => snapshot.CpuUsage,
            PerformanceCounterType.MemoryUsage => snapshot.MemoryUsage,
            PerformanceCounterType.GcCollections => snapshot.GcGen0Collections + snapshot.GcGen1Collections + snapshot.GcGen2Collections,
            PerformanceCounterType.ThreadCount => snapshot.ThreadCount,
            PerformanceCounterType.HandleCount => snapshot.HandleCount,
            _ => 0
        };
    }

    /// <summary>
    /// 收集性能快照
    /// </summary>
    /// <param name="state">定时器状态</param>
    private void CollectSnapshot(object? state)
    {
        if (_isDisposed)
        {
            return;
        }

        try
        {
            var snapshot = CreateSnapshot();
            _snapshots.Enqueue(snapshot);

            // 限制快照数量
            while (_snapshots.Count > _maxSnapshotCount)
            {
                _snapshots.TryDequeue(out _);
            }
        }
        catch (Exception ex)
        {
            // 记录异常但继续监控
            Debug.WriteLine($"Failed to collect performance snapshot: {ex.Message}");
        }
    }

    /// <summary>
    /// 创建性能快照
    /// </summary>
    /// <returns>性能快照</returns>
    private PerformanceSnapshot CreateSnapshot()
    {
        _currentProcess.Refresh();

        // 计算CPU使用率
        var currentTime = DateTime.UtcNow;
        var currentTotalProcessorTime = _currentProcess.TotalProcessorTime;

        var cpuUsage = 0.0;
        var timeDelta = currentTime - _lastCpuTime;
        var cpuTimeDelta = currentTotalProcessorTime - _lastTotalProcessorTime;

        if (timeDelta.TotalMilliseconds > 0)
        {
            cpuUsage = cpuTimeDelta.TotalMilliseconds / (Environment.ProcessorCount * timeDelta.TotalMilliseconds) * 100;
            cpuUsage = Math.Max(0, Math.Min(100, cpuUsage)); // 限制在0-100%范围内
        }

        _lastCpuTime = currentTime;
        _lastTotalProcessorTime = currentTotalProcessorTime;

        return new PerformanceSnapshot
        {
            Timestamp = currentTime,
            CpuUsage = cpuUsage,
            MemoryUsage = _currentProcess.WorkingSet64,
            PrivateMemorySize = _currentProcess.PrivateMemorySize64,
            VirtualMemorySize = _currentProcess.VirtualMemorySize64,
            WorkingSet = _currentProcess.WorkingSet64,
            GcGen0Collections = GC.CollectionCount(0),
            GcGen1Collections = GC.CollectionCount(1),
            GcGen2Collections = GC.CollectionCount(2),
            ManagedMemory = GC.GetTotalMemory(false),
            ThreadCount = _currentProcess.Threads.Count,
            HandleCount = _currentProcess.HandleCount,
            UserProcessorTime = _currentProcess.UserProcessorTime,
            PrivilegedProcessorTime = _currentProcess.PrivilegedProcessorTime,
            TotalProcessorTime = _currentProcess.TotalProcessorTime,
            ProcessUptime = DateTime.Now - _currentProcess.StartTime
        };
    }
}

/// <summary>
/// 性能指标快照
/// </summary>
public record PerformanceSnapshot
{
    /// <summary>
    /// 快照时间
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// CPU使用率百分比
    /// </summary>
    public double CpuUsage { get; set; }

    /// <summary>
    /// 内存使用量（字节）
    /// </summary>
    public long MemoryUsage { get; set; }

    /// <summary>
    /// 私有内存使用量（字节）
    /// </summary>
    public long PrivateMemorySize { get; set; }

    /// <summary>
    /// 虚拟内存使用量（字节）
    /// </summary>
    public long VirtualMemorySize { get; set; }

    /// <summary>
    /// 工作集大小（字节）
    /// </summary>
    public long WorkingSet { get; set; }

    /// <summary>
    /// GC代0收集次数
    /// </summary>
    public int GcGen0Collections { get; set; }

    /// <summary>
    /// GC代1收集次数
    /// </summary>
    public int GcGen1Collections { get; set; }

    /// <summary>
    /// GC代2收集次数
    /// </summary>
    public int GcGen2Collections { get; set; }

    /// <summary>
    /// 托管内存使用量（字节）
    /// </summary>
    public long ManagedMemory { get; set; }

    /// <summary>
    /// 线程数量
    /// </summary>
    public int ThreadCount { get; set; }

    /// <summary>
    /// 句柄数量
    /// </summary>
    public int HandleCount { get; set; }

    /// <summary>
    /// 用户处理器时间
    /// </summary>
    public TimeSpan UserProcessorTime { get; set; }

    /// <summary>
    /// 特权处理器时间
    /// </summary>
    public TimeSpan PrivilegedProcessorTime { get; set; }

    /// <summary>
    /// 总处理器时间
    /// </summary>
    public TimeSpan TotalProcessorTime { get; set; }

    /// <summary>
    /// 进程运行时间
    /// </summary>
    public TimeSpan ProcessUptime { get; set; }
}

/// <summary>
/// 性能趋势分析结果
/// </summary>
public record PerformanceTrend
{
    /// <summary>
    /// 计数器类型
    /// </summary>
    public PerformanceCounterType CounterType { get; set; }

    /// <summary>
    /// 当前值
    /// </summary>
    public double CurrentValue { get; set; }

    /// <summary>
    /// 平均值
    /// </summary>
    public double AverageValue { get; set; }

    /// <summary>
    /// 最小值
    /// </summary>
    public double MinValue { get; set; }

    /// <summary>
    /// 最大值
    /// </summary>
    public double MaxValue { get; set; }

    /// <summary>
    /// 变化趋势（正数表示上升，负数表示下降）
    /// </summary>
    public double Trend { get; set; }

    /// <summary>
    /// 标准差
    /// </summary>
    public double StandardDeviation { get; set; }

    /// <summary>
    /// 样本数量
    /// </summary>
    public int SampleCount { get; set; }

    /// <summary>
    /// 分析时间范围
    /// </summary>
    public TimeSpan AnalysisTimespan { get; set; }
}

/// <summary>
/// 性能计数器类型枚举
/// </summary>
public enum PerformanceCounterType
{
    /// <summary>
    /// CPU使用率
    /// </summary>
    CpuUsage = 1,

    /// <summary>
    /// 内存使用量
    /// </summary>
    MemoryUsage = 2,

    /// <summary>
    /// GC收集次数
    /// </summary>
    GcCollections = 3,

    /// <summary>
    /// 线程数量
    /// </summary>
    ThreadCount = 4,

    /// <summary>
    /// 句柄数量
    /// </summary>
    HandleCount = 5
}