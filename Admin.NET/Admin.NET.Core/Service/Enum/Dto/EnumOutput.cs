// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

/// <summary>
/// 枚举类型输出参数
/// </summary>
public class EnumTypeOutput
{
    /// <summary>
    /// 枚举类型描述
    /// </summary>
    public string TypeDescribe { get; set; }

    /// <summary>
    /// 枚举类型名称
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// 枚举类型备注
    /// </summary>
    public string TypeRemark { get; set; }

    /// <summary>
    /// 枚举实体
    /// </summary>
    public List<EnumEntity> EnumEntities { get; set; }
}