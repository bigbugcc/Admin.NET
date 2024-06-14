// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// Type 扩展
/// </summary>
public  static class TypeExtension
{
    /// <summary>
    /// 根据指定Attribute获取属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<string> GetPropertyNames<T>(this Type type)
        where T : Attribute
    {
        var allProperties = type.GetProperties();

        var properties = allProperties.Where(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(T)));

        return properties.Select(x => x.Name).ToList() ;
    }

    /// <summary>
    /// 获取查询表达式
    /// </summary>
    /// <param name="type"></param>
    /// <param name="options"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static LambdaExpression GetConditionExpression<T>(this Type type, List<T> options, string fieldName)
    {
        ParameterExpression parameter = Expression.Parameter(type, "c");
        Expression right = Expression.Constant(false);
        options.ForEach(option =>
        {
            Expression left = Expression.Equal(
                 Expression.Property(parameter, type.GetProperty(fieldName)),
                 Expression.Constant(option)
            );
            right = Expression.Or(left, right);
        });
        var finalExpression = Expression.Lambda(right, new ParameterExpression[] { parameter });
        return finalExpression;
    }

    /// <summary>
    /// 获取查询表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="option"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static LambdaExpression GetConditionExpression<T>(this Type type, T option, string fieldName)
    {
        ParameterExpression parameter = Expression.Parameter(type, "c");
        Expression expression = Expression.Equal(
                 Expression.Property(parameter, type.GetProperty(fieldName)),
                 Expression.Constant(option)
        );
        var finalExpression = Expression.Lambda(expression, new ParameterExpression[] { parameter });
        return finalExpression;
    }

    /// <summary>
    /// 获取查询表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="options"></param>
    /// <param name="fieldNames"></param>
    /// <returns></returns>
    public static LambdaExpression GetConditionExpression<T>(this Type type,List<T> options,List<string> fieldNames)
    {
        ParameterExpression parameter = Expression.Parameter(type, "c");
        Expression right = Expression.Constant(false);
        fieldNames.ForEach(filedName =>
        {
            options.ForEach(option =>
            {
                Expression left = Expression.Equal(
                     Expression.Property(parameter, type.GetProperty(filedName)),
                     Expression.Constant(option)
                );
                right = Expression.Or(left, right);
            });
        });
        var finalExpression = Expression.Lambda(right, new ParameterExpression[] { parameter });
        return finalExpression;
    }

    /// <summary>
    /// 获取查询表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="option"></param>
    /// <param name="fieldNames"></param>
    /// <returns></returns>
    public static LambdaExpression GetConditionExpression<T>(this Type type,T option,List<string> fieldNames)
    {
        ParameterExpression parameter = Expression.Parameter(type, "c");
        Expression right = Expression.Constant(false);
        fieldNames.ForEach(filedName =>
        {
            Expression left = Expression.Equal(
                 Expression.Property(parameter, type.GetProperty(filedName)),
                 Expression.Constant(option)
            );
            right = Expression.Or(left, right);
        });
        var finalExpression = Expression.Lambda(right, new ParameterExpression[] { parameter });
        return finalExpression;
    }
}
