// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统文件表
/// </summary>
[SugarTable(null, "系统文件表")]
[SysTable]
[SugarIndex("index_{table}_F", nameof(FileName), OrderByType.Asc)]
public partial class SysFile : EntityTenantBaseData
{
    /// <summary>
    /// 提供者
    /// </summary>
    [SugarColumn(ColumnDescription = "提供者", Length = 128)]
    [MaxLength(128)]
    public string? Provider { get; set; }

    /// <summary>
    /// 仓储名称
    /// </summary>
    [SugarColumn(ColumnDescription = "仓储名称", Length = 128)]
    [MaxLength(128)]
    public string? BucketName { get; set; }

    /// <summary>
    /// 文件名称（源文件名）
    /// </summary>
    [SugarColumn(ColumnDescription = "文件名称", Length = 128)]
    [MaxLength(128)]
    public string? FileName { get; set; }

    /// <summary>
    /// 文件后缀
    /// </summary>
    [SugarColumn(ColumnDescription = "文件后缀", Length = 16)]
    [MaxLength(16)]
    public string? Suffix { get; set; }

    /// <summary>
    /// 存储路径
    /// </summary>
    [SugarColumn(ColumnDescription = "存储路径", Length = 512)]
    [MaxLength(512)]
    public string? FilePath { get; set; }

    /// <summary>
    /// 文件大小KB
    /// </summary>
    [SugarColumn(ColumnDescription = "文件大小KB")]
    public long SizeKb { get; set; }

    /// <summary>
    /// 文件大小信息-计算后的
    /// </summary>
    [SugarColumn(ColumnDescription = "文件大小信息", Length = 64)]
    [MaxLength(64)]
    public string? SizeInfo { get; set; }

    /// <summary>
    /// 外链地址-OSS上传后生成外链地址方便前端预览
    /// </summary>
    [SugarColumn(ColumnDescription = "外链地址", Length = 512)]
    [MaxLength(512)]
    public string? Url { get; set; }

    /// <summary>
    /// 文件MD5
    /// </summary>
    [SugarColumn(ColumnDescription = "文件MD5", Length = 128)]
    [MaxLength(128)]
    public string? FileMd5 { get; set; }

    /// <summary>
    /// 关联对象名称（如子对象）
    /// </summary>
    [SugarColumn(ColumnDescription = "关联对象名称", Length = 128)]
    [MaxLength(128)]
    public string? RelationName { get; set; }

    /// <summary>
    /// 关联对象Id
    /// </summary>
    [SugarColumn(ColumnDescription = "关联对象Id")]
    public long? RelationId { get; set; }

    /// <summary>
    /// 所属Id（如主对象）
    /// </summary>
    [SugarColumn(ColumnDescription = "所属Id")]
    public long? BelongId { get; set; }

    /// <summary>
    /// 文件类别
    /// </summary>
    [SugarColumn(ColumnDescription = "文件类别", Length = 128)]
    [MaxLength(128)]
    public string? FileType { get; set; }

    /// <summary>
    /// 是否公开
    /// 若为true则所有人都可以查看，默认只有自己或有权限的可以查看
    /// </summary>
    [SugarColumn(ColumnDescription = "是否公开")]
    public bool IsPublic { get; set; } = false;
}