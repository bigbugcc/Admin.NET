// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using OnceMi.AspNetCore.OSS;

namespace Admin.NET.Core;

/// <summary>
/// 文件上传配置选项
/// </summary>
public sealed class UploadOptions : IConfigurableOptions
{
    /// <summary>
    /// 路径
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 大小
    /// </summary>
    public long MaxSize { get; set; }

    /// <summary>
    /// 上传格式
    /// </summary>
    public List<string> ContentType { get; set; }

    /// <summary>
    /// 启用文件MD5验证
    /// </summary>
    /// <remarks>防止重复上传</remarks>
    public bool EnableMd5 { get; set; }
}

/// <summary>
/// 对象存储配置选项
/// </summary>
public sealed class OSSProviderOptions : OSSOptions, IConfigurableOptions
{
    /// <summary>
    /// 是否启用OSS存储
    /// </summary>
    public bool IsEnable { get; set; }

    /// <summary>
    /// 自定义桶名称 不能直接使用Provider来替代桶名称
    /// 例：阿里云 1.只能包括小写字母，数字，短横线（-）2.必须以小写字母或者数字开头 3.长度必须在3-63字节之间
    /// </summary>
    public string Bucket { get; set; }
}