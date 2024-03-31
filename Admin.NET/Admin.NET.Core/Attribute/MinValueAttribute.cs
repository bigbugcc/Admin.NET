// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 最小值校验
/// </summary>
public class MinValueAttribute : ValidationAttribute
{
    private double MinValue { get; set; }

    /// <summary>
    /// 最小值
    /// </summary>
    /// <param name="value"></param>
    public MinValueAttribute(double value) => this.MinValue = value;

    /// <summary>
    /// 最小值校验
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override bool IsValid(object value)
    {
        return value == null || Convert.ToDouble(value) > this.MinValue;
    }

    /// <summary>
    /// 错误信息
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public override string FormatErrorMessage(string name) => base.FormatErrorMessage(name);
}