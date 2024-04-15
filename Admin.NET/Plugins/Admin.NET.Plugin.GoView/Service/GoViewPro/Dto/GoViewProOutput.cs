// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.GoView.Service;

/// <summary>
/// GoView 项目 Item
/// </summary>
public class GoViewProItemOutput
{
    /// <summary>
    /// 项目Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 项目名称
    /// </summary>
    public string ProjectName { get; set; }

    /// <summary>
    /// 项目状态
    /// </summary>
    public GoViewProState State { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 预览图片url
    /// </summary>
    public string IndexImage { get; set; }

    /// <summary>
    /// 创建者Id
    /// </summary>
    public long? CreateUserId { get; set; }

    /// <summary>
    /// 项目备注
    /// </summary>
    public string Remarks { get; set; }
}

/// <summary>
/// GoView 项目详情
/// </summary>
public class GoViewProDetailOutput : GoViewProItemOutput
{
    /// <summary>
    /// 项目内容
    /// </summary>
    public string Content { get; set; }
}

/// <summary>
/// GoView 新增项目输出
/// </summary>
public class GoViewProCreateOutput
{
    /// <summary>
    /// 项目Id
    /// </summary>
    public long Id { get; set; }
}

/// <summary>
/// GoView 上传项目输出
/// </summary>
public class GoViewProUploadOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 仓储名称
    /// </summary>
    public string BucketName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 创建者Id
    /// </summary>
    public long? CreateUserId { get; set; }

    /// <summary>
    /// 文件名称
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 文件大小KB
    /// </summary>
    public int FileSize { get; set; }

    /// <summary>
    /// 文件后缀
    /// </summary>
    public string FileSuffix { get; set; }

    /// <summary>
    /// 文件 Url
    /// </summary>
    [JsonProperty("fileurl")]
    public string FileUrl { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 修改者Id
    /// </summary>
    public long? UpdateUserId { get; set; }
}