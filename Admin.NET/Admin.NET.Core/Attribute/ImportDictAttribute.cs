namespace Admin.NET.Core;

/// <summary>
/// 属性字典配置
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ImportDictAttribute : Attribute
{
    /// <summary>
    /// 字典Code
    /// </summary>
    public string TypeCode { get; set; }

    ///// <summary>
    ///// 目标对象类型
    ///// </summary>
    //public Type TargetType { get; set; }

    public string TargetPropName { get; set; }
}
