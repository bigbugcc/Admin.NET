// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 硬件信息管理器
/// </summary>
public static class HardwareInfoManager
{
    /// <summary>
    /// 获取完整的系统硬件信息
    /// </summary>
    /// <remarks>
    /// 推荐使用，默认有缓存
    /// </remarks>
    /// <returns>系统硬件信息</returns>
    public static SystemHardwareInfo GetSystemHardwareInfo()
    {
        return new SystemHardwareInfo
        {
            CpuInfo = CpuHelper.CpuInfos,
            RamInfo = RamHelper.RamInfos,
            DiskInfos = DiskHelper.DiskInfos,
            NetworkInfos = NetworkHelper.NetworkInfos,
            GpuInfos = GpuHelper.GpuInfos,
            BoardInfo = BoardHelper.BoardInfos
        };
    }

    /// <summary>
    /// 获取系统硬件摘要信息
    /// </summary>
    /// <returns>系统硬件摘要</returns>
    public static SystemHardwareSummary GetSystemHardwareSummary()
    {
        try
        {
            var cpuInfo = CpuHelper.CpuInfos;
            var ramInfo = RamHelper.RamInfos;
            var diskInfos = DiskHelper.DiskInfos;
            var networkInfos = NetworkHelper.NetworkInfos;
            var gpuInfos = GpuHelper.GpuInfos;

            return new SystemHardwareSummary
            {
                CpuName = cpuInfo.ProcessorName,
                CpuCores = $"{cpuInfo.PhysicalCoreCount}C/{cpuInfo.LogicalCoreCount}T",
                CpuUsage = $"{cpuInfo.UsagePercentage:F1}%",
                TotalMemory = ramInfo.TotalBytes.FormatFileSizeToString(),
                MemoryUsage = $"{ramInfo.UsagePercentage:F1}%",
                TotalDiskSpace = diskInfos.Sum(d => d.TotalSpace).FormatFileSizeToString(),
                NetworkInterfaceCount = networkInfos.Count,
                GpuCount = gpuInfos.Count,
            };
        }
        catch (Exception ex)
        {
            Log.Error(string.Format("获取系统硬件摘要失败: {0}", ex.Message));
            return new SystemHardwareSummary();
        }
    }

    /// <summary>
    /// 获取硬件信息诊断报告
    /// </summary>
    /// <returns>诊断报告</returns>
    public static HardwareDiagnosticReport GetDiagnosticReport()
    {
        var report = new HardwareDiagnosticReport();

        try
        {
            var cpuInfo = CpuHelper.CpuInfos;
            var ramInfo = RamHelper.RamInfos;
            var diskInfos = DiskHelper.DiskInfos;

            // CPU诊断
            if (cpuInfo.UsagePercentage > 90)
            {
                report.Issues.Add("CPU使用率过高 (>90%)");
            }
            if (cpuInfo.Temperature.HasValue && cpuInfo.Temperature > 80)
            {
                report.Issues.Add($"CPU温度过高 ({cpuInfo.Temperature:F1}°C)");
            }

            // 内存诊断
            if (ramInfo.UsagePercentage > 90)
            {
                report.Issues.Add("内存使用率过高 (>90%)");
            }
            if (ramInfo.AvailablePercentage < 5)
            {
                report.Issues.Add("可用内存不足 (<5%)");
            }

            // 磁盘诊断
            foreach (var disk in diskInfos)
            {
                if (disk.AvailableRate < 10)
                {
                    report.Issues.Add($"磁盘 {disk.DiskName} 可用空间不足 (<10%)");
                }
            }

            report.Status = report.Issues.Count == 0 ? "正常" : "发现问题";
        }
        catch (Exception)
        {
            report.Status = "诊断失败";
        }

        return report;
    }
}

/// <summary>
/// 系统硬件信息
/// </summary>
public record SystemHardwareInfo
{
    /// <summary>
    /// CPU信息
    /// </summary>
    public CpuInfo CpuInfo { get; set; } = new();

    /// <summary>
    /// 内存信息
    /// </summary>
    public RamInfo RamInfo { get; set; } = new();

    /// <summary>
    /// 磁盘信息列表
    /// </summary>
    public List<DiskInfo> DiskInfos { get; set; } = [];

    /// <summary>
    /// 网络接口信息列表
    /// </summary>
    public List<NetworkInfo> NetworkInfos { get; set; } = [];

    /// <summary>
    /// GPU信息列表
    /// </summary>
    public List<GpuInfo> GpuInfos { get; set; } = [];

    /// <summary>
    /// 主板信息
    /// </summary>
    public BoardInfo BoardInfo { get; set; } = new();
}

/// <summary>
/// 系统硬件摘要信息
/// </summary>
public record SystemHardwareSummary
{
    /// <summary>
    /// CPU名称
    /// </summary>
    public string CpuName { get; set; } = string.Empty;

    /// <summary>
    /// CPU核心数
    /// </summary>
    public string CpuCores { get; set; } = string.Empty;

    /// <summary>
    /// CPU使用率
    /// </summary>
    public string CpuUsage { get; set; } = string.Empty;

    /// <summary>
    /// 总内存
    /// </summary>
    public string TotalMemory { get; set; } = string.Empty;

    /// <summary>
    /// 内存使用率
    /// </summary>
    public string MemoryUsage { get; set; } = string.Empty;

    /// <summary>
    /// 总磁盘空间
    /// </summary>
    public string TotalDiskSpace { get; set; } = string.Empty;

    /// <summary>
    /// 网络接口数量
    /// </summary>
    public int NetworkInterfaceCount { get; set; }

    /// <summary>
    /// GPU数量
    /// </summary>
    public int GpuCount { get; set; }

    /// <summary>
    /// 主要GPU
    /// </summary>
    public string PrimaryGpu { get; set; } = string.Empty;
}

/// <summary>
/// 硬件诊断报告
/// </summary>
public record HardwareDiagnosticReport
{
    /// <summary>
    /// 诊断状态
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 发现的问题列表
    /// </summary>
    public List<string> Issues { get; set; } = [];

    /// <summary>
    /// 建议列表
    /// </summary>
    public List<string> Recommendations { get; set; } = [];
}