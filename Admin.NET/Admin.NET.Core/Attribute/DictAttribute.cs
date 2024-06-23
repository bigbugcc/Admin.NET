// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 字典值合规性校验特性
/// </summary>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class DictAttribute : ValidationAttribute, ITransient
{
    /// <summary>
    /// 字典值合规性校验特性
    /// </summary>
    /// <param name="dictTypeCode"></param>
    /// <param name="errorMessage"></param>
    public DictAttribute(string dictTypeCode, string errorMessage = "字典值不合法！")
    {
        DictTypeCode = dictTypeCode;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// 字典值合规性校验
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var valueAsString = value?.ToString();

        // 判断是否允许空值
        if (AllowNullValue && value == null) return ValidationResult.Success;

        // 是否忽略空字符串
        if (AllowEmptyStrings && string.IsNullOrEmpty(valueAsString)) return ValidationResult.Success;

        var sysDictDataServiceProvider = validationContext.GetRequiredService<SysDictDataService>();
        var dictDataList = sysDictDataServiceProvider.GetDataList(DictTypeCode).Result;

        // 使用HashSet来提高查找效率
        var dictCodes = new HashSet<string>(dictDataList.Select(u => u.Code));

        if (!dictCodes.Contains(valueAsString))
            return new ValidationResult($"提示：{ErrorMessage}|字典【{DictTypeCode}】不包含【{valueAsString}】！");
        else
            return ValidationResult.Success;
    }

    /// <summary>
    /// 字典编码
    /// </summary>
    public string DictTypeCode { get; set; }

    /// <summary>
    /// 是否允许空字符串
    /// </summary>
    public bool AllowEmptyStrings { get; set; } = false;

    /// <summary>
    /// 允许空值，有值才验证，默认 false
    /// </summary>
    public bool AllowNullValue { get; set; } = false;
}