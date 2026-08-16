// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 磁盘帮助类
/// </summary>
public static class DiskHelper
{
    /// <summary>
    /// 磁盘信息
    /// </summary>
    /// <remarks>
    /// 推荐使用，默认有缓存
    /// </remarks>
    public static List<DiskInfo> DiskInfos => Cache.Default.GetOrAdd("DiskInfos", _ => GetDiskInfos(), 60 * 60);

    /// <summary>
    /// 获取磁盘信息
    /// </summary>
    /// <returns></returns>
    public static List<DiskInfo> GetDiskInfos()
    {
        List<DiskInfo> diskInfos = [];

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var output = ShellHelper.Bash(@"df -mT | awk '/^\/dev\/(sd|vd|xvd|nvme|sda|vda|mapper)/ {print $1,$2,$3,$4,$5,$6}'").Trim();
                var lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (lines.Count != 0)
                {
                    diskInfos.AddRange(from line in lines
                                       select line.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries)
                                       into rootDisk
                                       where rootDisk.Length >= 6
                                       select new DiskInfo
                                       {
                                           DiskName = rootDisk[0].Trim(),
                                           TypeName = rootDisk[1].Trim(),
                                           TotalSpace = rootDisk[2].ParseToLong() * 1024 * 1024, // MB转换为字节
                                           UsedSpace = rootDisk[3].ParseToLong() * 1024 * 1024,
                                           FreeSpace = rootDisk[4].ParseToLong() * 1024 * 1024,
                                           AvailableRate = rootDisk[2].ParseToLong() == 0
                                               ? 0
                                               : Math.Round((double)rootDisk[4].ParseToLong() / rootDisk[2].ParseToLong() * 100, 3)
                                       });
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var output = ShellHelper.Bash(@"df -k | awk '/^\/dev\/disk/ {print $1,$2,$3,$4,$6}' | tail -n +2").Trim();
                var lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (lines.Count != 0)
                {
                    diskInfos.AddRange(from line in lines
                                       select line.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries)
                                       into rootDisk
                                       where rootDisk.Length >= 5
                                       select new DiskInfo
                                       {
                                           TypeName = rootDisk[0].Trim(),
                                           TotalSpace = rootDisk[1].ParseToLong() * 1024,
                                           UsedSpace = rootDisk[2].ParseToLong() * 1024,
                                           DiskName = rootDisk[4].Trim(),
                                           FreeSpace = (rootDisk[1].ParseToLong() - rootDisk[2].ParseToLong()) * 1024,
                                           AvailableRate = rootDisk[1].ParseToLong() == 0
                                               ? 0
                                               : Math.Round((double)rootDisk[3].ParseToLong() / rootDisk[1].ParseToLong() * 100, 3)
                                       });
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var drives = DriveInfo.GetDrives().Where(d => d.IsReady).ToList();
                diskInfos.AddRange(drives.Select(item => new DiskInfo
                {
                    DiskName = item.Name,
                    TypeName = item.DriveType.ToString(),
                    TotalSpace = item.TotalSize,
                    FreeSpace = item.TotalFreeSpace,
                    UsedSpace = item.TotalSize - item.TotalFreeSpace,
                    AvailableRate = item.TotalSize == 0
                        ? 0
                        : Math.Round((double)item.TotalFreeSpace / item.TotalSize * 100, 3)
                }));
            }
        }
        catch (Exception ex)
        {
            Log.Error("获取磁盘信息出错，" + ex.Message);
        }

        return diskInfos;
    }
}

/// <summary>
/// 磁盘信息
/// </summary>
public record DiskInfo
{
    /// <summary>
    /// 磁盘名称
    /// </summary>
    public string DiskName { get; set; } = string.Empty;

    /// <summary>
    /// 磁盘类型
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// 总大小
    /// </summary>
    public long TotalSpace { get; set; }

    /// <summary>
    /// 空闲大小
    /// </summary>
    public long FreeSpace { get; set; }

    /// <summary>
    /// 已用大小
    /// </summary>
    public long UsedSpace { get; set; }

    /// <summary>
    /// 可用占比
    /// </summary>
    public double AvailableRate { get; set; }
}