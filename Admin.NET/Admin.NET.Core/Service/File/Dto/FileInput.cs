// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

public class FileInput : BaseIdInput
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    public string FileType { get; set; }

    /// <summary>
    /// 是否公开
    /// 若为true则所有人都可以查看，默认只有自己或有权限的可以查看
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// 文件Url
    /// </summary>
    public string? Url { get; set; }
}

public class PageFileInput : BasePageInput
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}

public class DeleteFileInput : BaseIdInput
{
}

public class UploadFileFromBase64Input
{
    /// <summary>
    /// 文件内容
    /// </summary>
    public string FileDataBase64 { get; set; }

    /// <summary>
    /// 文件类型( "image/jpeg",)
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// 文件名称
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 保存路径
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    public string FileType { get; set; }

    /// <summary>
    /// 是否公开
    /// 若为true则所有人都可以查看，默认只有自己或有权限的可以查看
    /// </summary>
    public bool IsPublic { get; set; }
}

/// <summary>
/// 上传文件
/// </summary>
public class FileUploadInput
{
    /// <summary>
    /// 文件
    /// </summary>
    [Required]
    public IFormFile File { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    public string FileType { get; set; }

    /// <summary>
    /// 是否公开
    /// 若为true则所有人都可以查看，默认只有自己或有权限的可以查看
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// 文件路径
    /// </summary>
    public string Path { get; set; }
    /// <summary>
    /// 所属实体ID
    /// </summary>
    public long BelongId { get; set; }
    /// <summary>
    /// 关联对象Id
    /// </summary>
    public long RelationId { get; set; }
    /// <summary>
    /// 关联对象名称
    /// </summary>
    public string RelationName { get; set; }
}

/// <summary>
/// 查询关联查询输入
/// </summary>
public class RelationQueryInput
{
    /// <summary>
    /// 关联对象名称
    /// </summary>
    public string RelationName { get; set; }

    /// <summary>
    /// 关联对象Id
    /// </summary>
    public long? RelationId { get; set; }
    /// <summary>
    /// 文件，多个以","分割
    /// </summary>
    public string FileTypes { get; set; }

    /// <summary>
    /// 所属Id
    /// </summary>
    public long? BelongId { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public string[] GetFileTypeBS()
    {
        return FileTypes.Split(',');
    }
}

public class FileOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// URL
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 大小
    /// </summary>
    public long SizeKb { get; set; }

    /// <summary>
    /// 后缀
    /// </summary>
    public string Suffix { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// 文件类别
    /// </summary>
    public string FileType { get; set; }

    /// <summary>
    /// 是否公开
    /// 若为true则所有人都可以查看，默认只有自己或有权限的可以查看
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// 上传人
    /// </summary>
    public string CreateUserName { get; set; }

    /// <summary>
    /// 上传时间
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 关联对象名称
    /// </summary>
    public string RelationName { get; set; }

    /// <summary>
    /// 关联对象Id
    /// </summary>
    public long? RelationId { get; set; }

    /// <summary>
    /// 所属Id
    /// </summary>
    public long? BelongId { get; set; }
}