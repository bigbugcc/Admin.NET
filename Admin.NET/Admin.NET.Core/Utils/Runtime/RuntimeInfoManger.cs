// Copyright (c) 2021-Present XiHanFun and contributors.
// Licensed under the MIT License.

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