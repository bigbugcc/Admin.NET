// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 脚本执行助手类，支持执行 .sh、.ps1、.bat 脚本
/// </summary>
public static class ScriptExecutor
{
    /// <summary>
    /// 执行脚本
    /// </summary>
    /// <param name="scriptFilePath">脚本文件的完整路径</param>
    /// <param name="arguments">传递给脚本的参数</param>
    /// <returns>执行结果(标准输出和标准错误)</returns>
    public static string ExecuteScript(string scriptFilePath, string arguments = "")
    {
        if (string.IsNullOrWhiteSpace(scriptFilePath) || !File.Exists(scriptFilePath))
        {
            throw new ArgumentException("脚本文件不存在", nameof(scriptFilePath));
        }

        var fileExtension = Path.GetExtension(scriptFilePath).ToLower();
        return fileExtension switch
        {
            ".sh" => ExecuteShellScript(scriptFilePath, arguments),
            ".ps1" => ExecutePowerShellScript(scriptFilePath, arguments),
            ".bat" => ExecuteBatchScript(scriptFilePath, arguments),
            _ => throw new NotSupportedException("不支持的脚本类型")
        };
    }

    /// <summary>
    /// 执行 Shell 脚本(Linux/macOS)
    /// </summary>
    /// <param name="scriptFilePath">脚本文件的完整路径</param>
    /// <param name="arguments">传递给脚本的参数</param>
    /// <returns>执行结果</returns>
    private static string ExecuteShellScript(string scriptFilePath, string arguments)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash", // Linux/macOS 使用 bash 执行脚本
            Arguments = $"{scriptFilePath} {arguments}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        return ExecuteProcess(processStartInfo);
    }

    /// <summary>
    /// 执行 PowerShell 脚本(Windows)
    /// </summary>
    /// <param name="scriptFilePath">脚本文件的完整路径</param>
    /// <param name="arguments">传递给脚本的参数</param>
    /// <returns>执行结果</returns>
    private static string ExecutePowerShellScript(string scriptFilePath, string arguments)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "powershell", // Windows 使用 PowerShell 执行脚本
            Arguments = $"-ExecutionPolicy Bypass -File \"{scriptFilePath}\" {arguments}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        return ExecuteProcess(processStartInfo);
    }

    /// <summary>
    /// 执行批处理脚本(Windows)
    /// </summary>
    /// <param name="scriptFilePath">脚本文件的完整路径</param>
    /// <param name="arguments">传递给脚本的参数</param>
    /// <returns>执行结果</returns>
    private static string ExecuteBatchScript(string scriptFilePath, string arguments)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = scriptFilePath, // Windows 使用 cmd 执行批处理脚本
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        return ExecuteProcess(processStartInfo);
    }

    /// <summary>
    /// 执行进程并获取输出结果
    /// </summary>
    /// <param name="processStartInfo">进程启动信息</param>
    /// <returns>执行结果</returns>
    private static string ExecuteProcess(ProcessStartInfo processStartInfo)
    {
        var output = new StringBuilder();
        var error = new StringBuilder();

        try
        {
            using var process = Process.Start(processStartInfo) ?? throw new InvalidOperationException("无法启动进程");
            process.OutputDataReceived += (o, e) =>
            {
                if (e.Data is not null)
                {
                    o = output.AppendLine(e.Data);
                }
            };

            process.ErrorDataReceived += (o, e) =>
            {
                if (e.Data is not null)
                {
                    o = error.AppendLine(e.Data);
                }
            };

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception(string.Format("脚本执行失败，错误信息：{0}", error));
            }
        }
        catch (Exception ex)
        {
            return string.Format("执行过程中发生错误：{0}", ex.Message);
        }

        return output.ToString();
    }
}