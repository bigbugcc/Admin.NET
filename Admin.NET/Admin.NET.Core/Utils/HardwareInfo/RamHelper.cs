// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 内存帮助类
/// </summary>
public static class RamHelper
{
    /// <summary>
    /// 内存信息
    /// </summary>
    /// <remarks>
    /// 推荐使用，默认有缓存
    /// </remarks>
    public static RamInfo RamInfos => Cache.Default.GetOrAdd("RamInfos", _ => GetRamInfos(), 5 * 60);

    /// <summary>
    /// 获取内存信息
    /// </summary>
    /// <returns></returns>
    public static RamInfo GetRamInfos()
    {
        var ramInfo = new RamInfo();

        try
        {
            // 单位是 Byte
            var totalMemoryParts = 0L;
            var usedMemoryParts = 0L;
            var freeMemoryParts = 0L;
            var availableMemoryParts = 0L;
            var buffersCached = 0L;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // 使用更详细的内存信息获取
                var output = ShellHelper.Bash("cat /proc/meminfo").Trim();
                var lines = output.Split('\n');

                foreach (var line in lines)
                {
                    var parts = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        var key = parts[0].Trim();
                        var valueStr = parts[1].Trim().Replace(" kB", "");
                        if (long.TryParse(valueStr, out var value))
                        {
                            var bytes = value * 1024;
                            switch (key)
                            {
                                case "MemTotal":
                                    totalMemoryParts = bytes;
                                    break;

                                case "MemFree":
                                    freeMemoryParts = bytes;
                                    break;

                                case "MemAvailable":
                                    availableMemoryParts = bytes;
                                    break;

                                case "Buffers":
                                    buffersCached += bytes;
                                    break;

                                case "Cached":
                                    buffersCached += bytes;
                                    break;
                            }
                        }
                    }
                }

                usedMemoryParts = totalMemoryParts - freeMemoryParts - buffersCached;
                if (availableMemoryParts == 0)
                {
                    availableMemoryParts = freeMemoryParts;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // 获取总内存
                var totalOutput = ShellHelper.Bash("sysctl -n hw.memsize").Trim();
                if (long.TryParse(totalOutput, out totalMemoryParts))
                {
                    // 获取内存压力信息
                    var vmStatOutput = ShellHelper.Bash("vm_stat").Trim();
                    var lines = vmStatOutput.Split('\n');

                    long pageSize = 4096; // 默认页面大小
                    var pageSizeOutput = ShellHelper.Bash("sysctl -n hw.pagesize").Trim();
                    if (long.TryParse(pageSizeOutput, out var ps))
                    {
                        pageSize = ps;
                    }

                    long freePages = 0, wiredPages = 0, activePages = 0, inactivePages = 0;

                    foreach (var line in lines)
                    {
                        if (line.Contains("Pages free:"))
                        {
                            var match = RegexHelper.OneOrMoreNumbersRegex().Match(line);
                            if (match.Success && long.TryParse(match.Value, out freePages)) { }
                        }
                        else if (line.Contains("Pages wired down:"))
                        {
                            var match = RegexHelper.OneOrMoreNumbersRegex().Match(line);
                            if (match.Success && long.TryParse(match.Value, out wiredPages)) { }
                        }
                        else if (line.Contains("Pages active:"))
                        {
                            var match = RegexHelper.OneOrMoreNumbersRegex().Match(line);
                            if (match.Success && long.TryParse(match.Value, out activePages)) { }
                        }
                        else if (line.Contains("Pages inactive:"))
                        {
                            var match = RegexHelper.OneOrMoreNumbersRegex().Match(line);
                            if (match.Success && long.TryParse(match.Value, out inactivePages)) { }
                        }
                    }

                    freeMemoryParts = freePages * pageSize;
                    usedMemoryParts = (wiredPages + activePages) * pageSize;
                    availableMemoryParts = (freePages + inactivePages) * pageSize;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var output = ShellHelper.Cmd("wmic", "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value").Trim();
                var lines = output.Split(Environment.NewLine);
                if (lines.Length != 0)
                {
                    totalMemoryParts = lines.First(s => s.StartsWith("TotalVisibleMemorySize")).Split('=')[1].ParseToLong() * 1024;
                    freeMemoryParts = lines.First(s => s.StartsWith("FreePhysicalMemory")).Split('=')[1].ParseToLong() * 1024;
                    usedMemoryParts = totalMemoryParts - freeMemoryParts;
                    availableMemoryParts = freeMemoryParts;
                }
            }

            // 设置内存信息
            ramInfo.TotalBytes = totalMemoryParts;
            ramInfo.UsedBytes = usedMemoryParts;
            ramInfo.FreeBytes = freeMemoryParts;
            ramInfo.AvailableBytes = availableMemoryParts;
            ramInfo.BuffersCachedBytes = buffersCached;

            ramInfo.UsagePercentage = totalMemoryParts > 0
                ? Math.Round((double)usedMemoryParts / totalMemoryParts * 100, 2)
                : 0;

            ramInfo.AvailablePercentage = totalMemoryParts > 0
                ? Math.Round((double)availableMemoryParts / totalMemoryParts * 100, 2)
                : 0;
        }
        catch (Exception ex)
        {
            Log.Error("获取内存信息出错，" + ex.Message);
        }

        return ramInfo;
    }
}

/// <summary>
/// 内存信息
/// </summary>
public record RamInfo
{
    /// <summary>
    /// 总内存大小（字节）
    /// </summary>
    public long TotalBytes { get; set; }

    /// <summary>
    /// 已用内存大小（字节）
    /// </summary>
    public long UsedBytes { get; set; }

    /// <summary>
    /// 空闲内存大小（字节）
    /// </summary>
    public long FreeBytes { get; set; }

    /// <summary>
    /// 可用内存大小（字节）
    /// </summary>
    public long AvailableBytes { get; set; }

    /// <summary>
    /// 缓冲区和缓存大小（字节）
    /// </summary>
    public long BuffersCachedBytes { get; set; }

    /// <summary>
    /// 内存使用率（%）
    /// </summary>
    public double UsagePercentage { get; set; }

    /// <summary>
    /// 可用内存占比（%）
    /// </summary>
    public double AvailablePercentage { get; set; }
}