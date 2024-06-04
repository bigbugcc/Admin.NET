// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统信息保存输入参数
/// </summary>
public class InfoSaveInput
{
    /// <summary>
    /// 系统图标（Data URI scheme base64 编码）
    /// </summary>
    public string SysLogoBase64 { get; set; }

    /// <summary>
    /// 系统主标题
    /// </summary>
    public string SysTitle { get; set; }

    /// <summary>
    /// 系统副标题
    /// </summary>
    public string SysViceTitle { get; set; }

    /// <summary>
    /// 系统描述
    /// </summary>
    public string SysViceDesc { get; set; }

    /// <summary>
    /// 水印内容
    /// </summary>
    public string SysWatermark { get; set; }

    /// <summary>
    /// 版权说明
    /// </summary>
    public string SysCopyright { get; set; }

    /// <summary>
    /// ICP备案号
    /// </summary>
    public string SysIcp { get; set; }

    /// <summary>
    /// ICP地址
    /// </summary>
    public string SysIcpUrl { get; set; }
}