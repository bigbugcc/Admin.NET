// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 短信配置选项
/// </summary>
public sealed class SMSOptions : IConfigurableOptions
{
    /// <summary>
    /// Aliyun
    /// </summary>
    public SMSSettings Aliyun { get; set; }
}

public sealed class SMSSettings
{
    /// <summary>
    /// AccessKey ID
    /// </summary>
    public string AccessKeyId { get; set; }

    /// <summary>
    /// AccessKey Secret
    /// </summary>
    public string AccessKeySecret { get; set; }

    /// <summary>
    /// 短信签名
    /// </summary>
    public string SignName { get; set; }

    /// <summary>
    /// 短信模板
    /// </summary>
    public string TemplateCode { get; set; }
}