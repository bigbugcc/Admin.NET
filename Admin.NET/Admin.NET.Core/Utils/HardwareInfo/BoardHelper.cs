// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 主板帮助类
/// </summary>
public static class BoardHelper
{
    /// <summary>
    /// 主板信息
    /// </summary>
    /// <remarks>
    /// 推荐使用，默认有缓存
    /// </remarks>
    public static BoardInfo BoardInfos => Cache.Default.GetOrAdd("BoardInfos", _ => GetBoardInfos(), 120 * 60);

    /// <summary>
    /// 获取主板信息
    /// </summary>
    /// <returns></returns>
    public static BoardInfo GetBoardInfos()
    {
        BoardInfo boardInfo = new();

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // 首先尝试使用 dmidecode 获取主板信息
                var output = ShellHelper.Bash("dmidecode -t baseboard").Trim();
                var lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

                if (lines.Length != 0)
                {
                    boardInfo.Product = GetParmValueSafe(lines, "Product Name", ':');
                    boardInfo.Manufacturer = GetParmValueSafe(lines, "Manufacturer", ':');
                    boardInfo.SerialNumber = GetParmValueSafe(lines, "Serial Number", ':');
                    boardInfo.Version = GetParmValueSafe(lines, "Version", ':');
                }

                // 如果主板信息为空或无效（常见于虚拟化环境），尝试获取系统信息
                if (string.IsNullOrWhiteSpace(boardInfo.Product) ||
                    string.IsNullOrWhiteSpace(boardInfo.Manufacturer) ||
                    boardInfo.Product.Equals("Not Specified", StringComparison.OrdinalIgnoreCase) ||
                    boardInfo.Manufacturer.Equals("Not Specified", StringComparison.OrdinalIgnoreCase))
                {
                    var systemOutput = ShellHelper.Bash("dmidecode -t system").Trim();
                    var systemLines = systemOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

                    if (systemLines.Length != 0)
                    {
                        boardInfo.Product = GetParmValueSafe(systemLines, "Product Name", ':');
                        boardInfo.Manufacturer = GetParmValueSafe(systemLines, "Manufacturer", ':');
                        boardInfo.SerialNumber = GetParmValueSafe(systemLines, "Serial Number", ':');
                        boardInfo.Version = GetParmValueSafe(systemLines, "Version", ':');
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var output = ShellHelper.Bash("system_profiler SPHardwareDataType").Trim();
                var lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                if (lines.Length != 0)
                {
                    boardInfo.Product = GetParmValueSafe(lines, "Model Identifier", ':');
                    boardInfo.Manufacturer = GetParmValueSafe(lines, "Chip", ':');
                    boardInfo.SerialNumber = GetParmValueSafe(lines, "Serial Number (system)", ':');
                    boardInfo.Version = GetParmValueSafe(lines, "Hardware UUID", ':');
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var output = ShellHelper.Cmd("wmic", "baseboard get Product,Manufacturer,SerialNumber,Version /Value").Trim();
                var lines = output.Split(Environment.NewLine);
                if (lines.Length != 0)
                {
                    boardInfo.Product = GetParmValueSafe(lines, "Product", '=');
                    boardInfo.Manufacturer = GetParmValueSafe(lines, "Manufacturer", '=');
                    boardInfo.SerialNumber = GetParmValueSafe(lines, "SerialNumber", '=');
                    boardInfo.Version = GetParmValueSafe(lines, "Version", '=');
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error("获取主板信息出错，" + ex.Message);
        }

        return boardInfo;

        // 安全的参数值获取方法，避免异常
        string GetParmValueSafe(string[] lines, string parm, char separator)
        {
            try
            {
                var line = lines.FirstOrDefault(s => s.StartsWith(parm));
                if (line != null && line.Contains(separator))
                {
                    var parts = line.Split(separator);
                    return parts.Length > 1 ? parts[1].Trim() : string.Empty;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}

/// <summary>
/// 主板信息
/// </summary>
public record BoardInfo
{
    /// <summary>
    /// 型号
    /// </summary>
    public string Product { get; set; } = string.Empty;

    /// <summary>
    /// 制造商
    /// </summary>
    public string Manufacturer { get; set; } = string.Empty;

    /// <summary>
    /// 序列号
    /// </summary>
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// 版本号
    /// </summary>
    public string Version { get; set; } = string.Empty;
}