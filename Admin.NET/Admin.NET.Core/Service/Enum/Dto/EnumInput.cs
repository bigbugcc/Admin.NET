// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

/// <summary>
/// 枚举输入参数
/// </summary>
public class EnumInput
{
    /// <summary>
    /// 枚举类型名称
    /// </summary>
    /// <example>AccountTypeEnum</example>
    [Required(ErrorMessage = "枚举类型不能为空")]
    public string EnumName { get; set; }
}

public class QueryEnumDataInput
{
    /// <summary>
    /// 实体名称
    /// </summary>
    /// <example>SysUser</example>
    [Required(ErrorMessage = "实体名称不能为空")]
    public string EntityName { get; set; }

    /// <summary>
    /// 字段名称
    /// </summary>
    /// <example>AccountType</example>
    [Required(ErrorMessage = "字段名称不能为空")]
    public string FieldName { get; set; }
}