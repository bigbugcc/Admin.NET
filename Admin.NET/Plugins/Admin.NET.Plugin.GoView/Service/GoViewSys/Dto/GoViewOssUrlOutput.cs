// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Plugin.GoView.Service.Dto;

/// <summary>
/// 获取 OSS 上传接口输出
/// </summary>
public class GoViewOssUrlOutput
{
    /// <summary>
    /// 桶名
    /// </summary>
    public string BucketName { get; set; }

    /// <summary>
    /// BucketURL 地址
    /// </summary>
    public string BucketURL { get; set; }
}