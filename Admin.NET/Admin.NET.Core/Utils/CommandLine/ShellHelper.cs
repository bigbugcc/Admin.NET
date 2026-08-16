// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// ShellHelper
/// </summary>
public static class ShellHelper
{
    /// <summary>
    /// Unix 系统命令
    /// </summary>
    /// <param name="command">要执行的 Unix/Linux 命令</param>
    /// <returns>命令执行后的标准输出结果</returns>
    public static string Bash(string command)
    {
        var output = string.Empty;
        var escapedArgs = command.Replace(@"""", @"\""");

        ProcessStartInfo info = new()
        {
            FileName = @"/bin/bash",
            // /bin/bash -c 后面接命令 ，而 /bin/bash 后面接执行的脚本
            Arguments = $"""
                         -c "{escapedArgs}"
                         """,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(info);
        if (process is null)
        {
            return output;
        }

        output = process.StandardOutput.ReadToEnd();
        return output;
    }

    /// <summary>
    /// Windows 系统命令
    /// </summary>
    /// <param name="fileName">要执行的程序或命令文件名</param>
    /// <param name="args">传递给程序的命令行参数</param>
    /// <returns>命令执行后的标准输出结果</returns>
    public static string Cmd(string fileName, string args)
    {
        var output = string.Empty;

        ProcessStartInfo info = new()
        {
            FileName = fileName,
            Arguments = args,
            RedirectStandardOutput = true,
            // 指定是否使用操作系统的外壳程序来启动进程，如果设置为 false，则使用 CreateProcess 函数直接启动进程
            UseShellExecute = false,
            // 指定是否在启动进程时创建一个新的窗口
            CreateNoWindow = true
        };

        using var process = Process.Start(info);
        if (process is null)
        {
            return output;
        }

        output = process.StandardOutput.ReadToEnd();
        return output;
    }
}