// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Plugin.GoView.Enum;

/// <summary>
/// GoView 项目状态
/// </summary>
[Description("GoView 项目状态")]
public enum GoViewProState
{
    /// <summary>
    /// 未发布
    /// </summary>
    [Description("未发布")]
    UnPublish = -1,

    /// <summary>
    /// 已发布
    /// </summary>
    [Description("已发布")]
    Published = 1,
}