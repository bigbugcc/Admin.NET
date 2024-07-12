// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

public static class SqlSugarFilterExtension
{
    /// <summary>
    /// 根据指定Attribute获取属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    private static List<string> GetPropertyNames<T>(this Type type)
        where T : Attribute
    {
        var allProperties = type.GetProperties();

        var properties = allProperties.Where(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(T)));

        return properties.Select(x => x.Name).ToList();
    }

    /// <summary>
    /// 获取过滤表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="owners"></param>
    /// <returns></returns>
    public static LambdaExpression GetConditionExpression<T>(this Type type, List<long> owners)
        where T : Attribute
    {
        var fieldNames = type.GetPropertyNames<T>();

        ParameterExpression parameter = Expression.Parameter(type, "c");
        Expression right = Expression.Constant(false);
        fieldNames.ForEach(filedName =>
        {
            owners.ForEach(owner =>
            {
                Expression left = Expression.Equal(
                     Expression.Property(parameter, type.GetProperty(filedName)),
                     Expression.Constant(owner)
                );
                right = Expression.Or(left, right);
            });
        });
        var finalExpression = Expression.Lambda(right, new ParameterExpression[] { parameter });
        return finalExpression;
    }

}
