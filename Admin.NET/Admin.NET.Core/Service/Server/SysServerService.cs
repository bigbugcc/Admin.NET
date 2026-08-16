// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统服务器监控服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 290, Description = "服务器监控")]
public class SysServerService : IDynamicApiController, ITransient
{
    public SysServerService()
    {
    }

    /// <summary>
    /// 获取服务器硬件信息
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取服务器硬件信息")]
    public SystemHardwareInfo GetHardwareInfo()
    {
        return HardwareInfoManager.GetSystemHardwareInfo();
    }

    /// <summary>
    /// 获取服务器运行时信息
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取服务器运行时信息")]
    public SystemRuntimeInfo GetRuntimeInfo()
    {
        return RuntimeInfoManger.GetSystemRuntimeInfo();
    }

    /// <summary>
    /// 获取框架主要程序集
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取框架主要程序集")]
    public List<NuGetPackage> GetNuGetPackageInfo()
    {
        return ReflectionHelper.GetNuGetPackages("Admin.NET");
    }
}