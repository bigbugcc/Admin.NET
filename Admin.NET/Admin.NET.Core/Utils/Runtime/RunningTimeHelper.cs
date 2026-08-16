// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统运行时间
/// </summary>
public static class RunningTimeHelper
{
    /// <summary>
    /// 处理器信息
    /// </summary>
    /// <remarks>
    /// 推荐使用，默认有缓存
    /// </remarks>
    public static string RunningTime => Cache.Default.GetOrAdd("RunningTime", _ => GetRunningTime(), 60);

    /// <summary>
    /// 获取系统运行时间
    /// </summary>
    public static string GetRunningTime()
    {
        var runTime = string.Empty;

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var output = ShellHelper.Bash("uptime -s").Trim();
                var timeSpan = DateTime.Now - output.Trim().ParseToDateTime();
                runTime = timeSpan.FormatTimeSpanToString();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var output = ShellHelper.Bash("uptime | tail -n -1").Trim();
                // 提取运行时间部分
                var startIndex = output.IndexOf("up ", StringComparison.Ordinal) + 3;
                var endIndex = output.IndexOf(" user", StringComparison.Ordinal);
                var uptime = output[startIndex..endIndex].Trim();
                // 解析运行时间并转换为标准格式
                var uptimeSpan = ParseUptime(uptime);
                runTime = uptimeSpan.FormatTimeSpanToString();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var output = ShellHelper.Cmd("wmic", "OS get LastBootUpTime /Value").Trim();
                var outputArr = output.Split('=');
                if (outputArr.Length != 0)
                {
                    var timeSpan = DateTime.Now - outputArr[1].Split('.')[0].FormatStringToDate();
                    runTime = timeSpan.FormatTimeSpanToString();
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error("获取系统运行时间出错，" + ex.Message);
        }

        return runTime;
    }

    /// <summary>
    /// 解析运行时间
    /// </summary>
    /// <param name="uptime"></param>
    /// <returns></returns>
    private static TimeSpan ParseUptime(string uptime)
    {
        var parts = uptime.Split(',');
        int days = 0, hours = 0, minutes = 0;

        foreach (var part in parts)
        {
            var trimmedPart = part.Trim();

            if (trimmedPart.Contains("day"))
            {
                days = int.Parse(trimmedPart.Split(' ')[0]);
            }
            else if (trimmedPart.Contains(':'))
            {
                var timeParts = trimmedPart.Split(':');
                hours = int.Parse(timeParts[0]);
                minutes = int.Parse(timeParts[1]);
            }
        }

        return new TimeSpan(days, hours, minutes, 0);
    }
}