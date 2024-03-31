// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 反射工具类
/// </summary>
public static class ReflectionUtil
{
    /// <summary>
    /// 获取字段特性
    /// </summary>
    /// <param name="field"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetDescriptionValue<T>(this FieldInfo field) where T : Attribute
    {
        // 获取字段的指定特性，不包含继承中的特性
        object[] customAttributes = field.GetCustomAttributes(typeof(T), false);

        // 如果没有数据返回null
        return customAttributes.Length > 0 ? (T)customAttributes[0] : null;
    }
}