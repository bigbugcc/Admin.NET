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
    private static List<string> GetPropertyNames<T>(this Type type) where T : Attribute
    {
        return type.GetProperties()
            .Where(p => p.CustomAttributes.Any(x => x.AttributeType == typeof(T)))
            .Select(x => x.Name).ToList();
    }

    /// <summary>
    /// 获取过滤表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="owners"></param>
    /// <returns></returns>
    public static LambdaExpression GetConditionExpression<T>(this Type type, List<long> owners) where T : Attribute
    {
        var fieldNames = type.GetPropertyNames<T>();
        ParameterExpression parameter = Expression.Parameter(type, "c");
        Expression userSelf = Expression.Constant(type.Name == nameof(SysUser));
        userSelf = Expression.AndAlso(userSelf, Expression.Call(
            Expression.Constant(owners),
            nameof(List<long>.Contains),
            null,
            Expression.Property(parameter, nameof(SysUser.Id))
            ));

        Expression right = userSelf;
        ConstantExpression ownersCollection = Expression.Constant(owners);
        foreach (var fieldName in fieldNames)
        {
            var property = type.GetProperty(fieldName);
            Expression memberExp = Expression.Property(parameter, property!);

            // 如果属性是可为空的类型，则转换为其基础类型
            var baseType = Nullable.GetUnderlyingType(property.PropertyType);
            if (baseType != null) memberExp = Expression.Convert(memberExp, baseType);

            // 调用ownersCollection.Contains方法，检查是否包含属性值
            right = Expression.OrElse(Expression.Call(
                typeof(Enumerable),
                nameof(Enumerable.Contains),
                new[] { memberExp.Type },
                ownersCollection,
                memberExp
            ), right);
        }
        return Expression.Lambda(right, parameter);
    }
}