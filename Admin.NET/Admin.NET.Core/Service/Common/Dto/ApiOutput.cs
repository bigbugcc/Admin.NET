// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

/// <summary>
/// 接口/动态API输出
/// </summary>
public class ApiOutput
{
    /// <summary>
    /// 组名称
    /// </summary>
    public string GroupName { get; set; }

    /// <summary>
    /// 接口名称
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// 路由名称
    /// </summary>
    public string RouteName { get; set; }
}