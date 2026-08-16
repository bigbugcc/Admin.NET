// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 运行时信息管理器
/// </summary>
public static class RuntimeInfoManger
{
    /// <summary>
    /// 获取当前运行时信息
    /// </summary>
    /// <remarks>
    /// 推荐使用，默认有缓存
    /// </remarks>
    /// <returns>运行时信息</returns>
    public static SystemRuntimeInfo GetSystemRuntimeInfo()
    {
        return new SystemRuntimeInfo
        {
            RuntimeInfo = OsPlatformHelper.RuntimeInfos,
            RunningTime = RunningTimeHelper.RunningTime
        };
    }
}

/// <summary>
/// 系统运行时信息
/// </summary>
public class SystemRuntimeInfo
{
    /// <summary>
    /// 运行时信息
    /// </summary>
    public RuntimeInfo RuntimeInfo { get; set; } = new();

    /// <summary>
    /// 运行时间
    /// </summary>
    public string RunningTime { get; set; } = string.Empty;
}