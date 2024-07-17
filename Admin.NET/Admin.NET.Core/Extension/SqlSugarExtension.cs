// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Text.Json;

namespace Admin.NET.Core;

/// <summary>
/// Sqlsugar 动态查询扩展方法
/// </summary>
public static class SqlSugarExtension
{
    public static ISugarQueryable<T> SearchBy<T>(this ISugarQueryable<T> queryable, BaseFilter filter)
    {
        return queryable.SearchByKeyword(filter.Keyword)
                .AdvancedSearch(filter.Search)
                .AdvancedFilter(filter.Filter);
    }

    public static ISugarQueryable<T> SearchByKeyword<T>(this ISugarQueryable<T> queryable, string keyword)
    {
        return queryable.AdvancedSearch(new Search { Keyword = keyword });
    }

    public static ISugarQueryable<T> AdvancedSearch<T>(this ISugarQueryable<T> queryable, Search search)
    {
        if (!string.IsNullOrWhiteSpace(search?.Keyword))
        {
            var paramExpr = Expression.Parameter(typeof(T));

            Expression right = Expression.Constant(false);

            if (search.Fields?.Any() is true)
            {
                foreach (string field in search.Fields)
                {
                    MemberExpression propertyExpr = GetPropertyExpression(field, paramExpr);

                    var left = AddSearchPropertyByKeyword<T>(propertyExpr, search.Keyword);

                    right = Expression.Or(left, right);
                }
            }
            else
            {
                var properties = typeof(T).GetProperties()
                         .Where(prop => Nullable.GetUnderlyingType(prop.PropertyType) == null
                             && !prop.PropertyType.IsEnum
                             && Type.GetTypeCode(prop.PropertyType) != TypeCode.Object);

                foreach (var property in properties)
                {
                    var propertyExpr = Expression.Property(paramExpr, property);

                    var left = AddSearchPropertyByKeyword<T>(propertyExpr, search.Keyword);

                    right = Expression.Or(left, right);
                }
            }

            var lambda = Expression.Lambda<Func<T, bool>>(right, paramExpr);

            return queryable.Where(lambda);
        }

        return queryable;
    }

    public static ISugarQueryable<T> AdvancedFilter<T>(this ISugarQueryable<T> queryable, Filter filter)
    {
        if (filter is not null)
        {
            var parameter = Expression.Parameter(typeof(T));

            Expression binaryExpresioFilter;

            if (filter.Logic.HasValue)
            {
                if (filter.Filters is null) throw new ArgumentException("The Filters attribute is required when declaring a logic");
                binaryExpresioFilter = CreateFilterExpression(filter.Logic.Value, filter.Filters, parameter);
            }
            else
            {
                var filterValid = GetValidFilter(filter);
                binaryExpresioFilter = CreateFilterExpression(filterValid.Field!, filterValid.Operator.Value, filterValid.Value, parameter);
            }

            var lambda = Expression.Lambda<Func<T, bool>>(binaryExpresioFilter, parameter);

            return queryable.Where(lambda);
        }
        return queryable;
    }

    private static Expression CombineFilter(
           FilterLogicEnum filterLogic,
           Expression bExpresionBase,
           Expression bExpresion)
    {
        return filterLogic switch
        {
            FilterLogicEnum.And => Expression.And(bExpresionBase, bExpresion),
            FilterLogicEnum.Or => Expression.Or(bExpresionBase, bExpresion),
            FilterLogicEnum.Xor => Expression.ExclusiveOr(bExpresionBase, bExpresion),
            _ => throw new ArgumentException("FilterLogic is not valid.", nameof(filterLogic)),
        };
    }

    private static Filter GetValidFilter(Filter filter)
    {
        if (string.IsNullOrEmpty(filter.Field)) throw new ArgumentException("The field attribute is required when declaring a filter");
        if (filter.Operator.IsNullOrEmpty()) throw new ArgumentException("The Operator attribute is required when declaring a filter");
        return filter;
    }

    private static Expression CreateFilterExpression(
        FilterLogicEnum filterLogic,
        IEnumerable<Filter> filters,
        ParameterExpression parameter)
    {
        Expression filterExpression = default!;

        foreach (var filter in filters)
        {
            Expression bExpresionFilter;

            if (filter.Logic.HasValue)
            {
                if (filter.Filters is null) throw new ArgumentException("The Filters attribute is required when declaring a logic");
                bExpresionFilter = CreateFilterExpression(filter.Logic.Value, filter.Filters, parameter);
            }
            else
            {
                var filterValid = GetValidFilter(filter);
                bExpresionFilter = CreateFilterExpression(filterValid.Field!, filterValid.Operator.Value, filterValid.Value, parameter);
            }

            filterExpression = filterExpression is null ? bExpresionFilter : CombineFilter(filterLogic, filterExpression, bExpresionFilter);
        }

        return filterExpression;
    }

    private static Expression CreateFilterExpression(
        string field,
        FilterOperatorEnum filterOperator,
        object? value,
        ParameterExpression parameter)
    {
        var propertyExpresion = GetPropertyExpression(field, parameter);
        var valueExpresion = GeValuetExpression(field, value, propertyExpresion.Type);
        return CreateFilterExpression(propertyExpresion, valueExpresion, filterOperator);
    }

    private static Expression CreateFilterExpression(
        MemberExpression memberExpression,
        ConstantExpression constantExpression,
        FilterOperatorEnum filterOperator)
    {
        return filterOperator switch
        {
            FilterOperatorEnum.EQ => Expression.Equal(memberExpression, constantExpression),
            FilterOperatorEnum.NEQ => Expression.NotEqual(memberExpression, constantExpression),
            FilterOperatorEnum.LT => Expression.LessThan(memberExpression, constantExpression),
            FilterOperatorEnum.LTE => Expression.LessThanOrEqual(memberExpression, constantExpression),
            FilterOperatorEnum.GT => Expression.GreaterThan(memberExpression, constantExpression),
            FilterOperatorEnum.GTE => Expression.GreaterThanOrEqual(memberExpression, constantExpression),
            FilterOperatorEnum.Contains => Expression.Call(memberExpression, nameof(FilterOperatorEnum.Contains), null, constantExpression),
            FilterOperatorEnum.StartsWith => Expression.Call(memberExpression, nameof(FilterOperatorEnum.StartsWith), null, constantExpression),
            FilterOperatorEnum.EndsWith => Expression.Call(memberExpression, nameof(FilterOperatorEnum.EndsWith), null, constantExpression),
            _ => throw new ArgumentException("Filter Operator is not valid."),
        };
    }

    private static string GetStringFromJsonElement(object value)
    {
        if (value is JsonElement) return ((JsonElement)value).GetString()!;
        if (value is string) return (string)value;
        return value?.ToString();
    }

    private static ConstantExpression GeValuetExpression(
          string field,
          object? value,
          Type propertyType)
    {
        if (value == null) return Expression.Constant(null, propertyType);

        if (propertyType.IsEnum)
        {
            string? stringEnum = GetStringFromJsonElement(value);

            if (!Enum.TryParse(propertyType, stringEnum, true, out object? valueparsed)) throw new ArgumentException(string.Format("Value {0} is not valid for {1}", value, field));

            return Expression.Constant(valueparsed, propertyType);
        }
        if (propertyType == typeof(long))
        {
            string? stringLong = GetStringFromJsonElement(value);

            if (!long.TryParse(stringLong, out long valueparsed)) throw new ArgumentException(string.Format("Value {0} is not valid for {1}", value, field));

            return Expression.Constant(valueparsed, propertyType);
        }

        if (propertyType == typeof(Guid))
        {
            string? stringGuid = GetStringFromJsonElement(value);

            if (!Guid.TryParse(stringGuid, out Guid valueparsed)) throw new ArgumentException(string.Format("Value {0} is not valid for {1}", value, field));

            return Expression.Constant(valueparsed, propertyType);
        }

        if (propertyType == typeof(string))
        {
            string? text = GetStringFromJsonElement(value);

            return Expression.Constant(text, propertyType);
        }

        if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
        {
            string? text = GetStringFromJsonElement(value);
            return Expression.Constant(ChangeType(text, propertyType), propertyType);
        }

        return Expression.Constant(ChangeType(((JsonElement)value).GetRawText(), propertyType), propertyType);
    }

    private static dynamic? ChangeType(object value, Type conversion)
    {
        var t = conversion;

        if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
        {
            if (value == null)
            {
                return null;
            }

            t = Nullable.GetUnderlyingType(t);
        }

        return Convert.ChangeType(value, t!);
    }

    private static MemberExpression GetPropertyExpression(
            string propertyName,
            ParameterExpression parameter)
    {
        Expression propertyExpression = parameter;
        foreach (string member in propertyName.Split('.'))
        {
            propertyExpression = Expression.PropertyOrField(propertyExpression, member);
        }

        return (MemberExpression)propertyExpression;
    }

    private static Expression AddSearchPropertyByKeyword<T>(
          Expression propertyExpr,
            string keyword,
            FilterOperatorEnum operatorSearch = FilterOperatorEnum.Contains)
    {
        if (propertyExpr is not MemberExpression memberExpr || memberExpr.Member is not PropertyInfo property)
        {
            throw new ArgumentException("propertyExpr must be a property expression.", nameof(propertyExpr));
        }

        ConstantExpression constant = Expression.Constant(keyword);

        MethodInfo method = operatorSearch switch
        {
            FilterOperatorEnum.Contains => typeof(string).GetMethod(nameof(FilterOperatorEnum.Contains), new Type[] { typeof(string) }),
            FilterOperatorEnum.StartsWith => typeof(string).GetMethod(nameof(FilterOperatorEnum.StartsWith), new Type[] { typeof(string) }),
            FilterOperatorEnum.EndsWith => typeof(string).GetMethod(nameof(FilterOperatorEnum.EndsWith), new Type[] { typeof(string) }),
            _ => throw new ArgumentException("Filter Operator is not valid."),
        };

        Expression selectorExpr =
               property.PropertyType == typeof(string)
                   ? propertyExpr
                   : Expression.Condition(
                       Expression.Equal(Expression.Convert(propertyExpr, typeof(object)), Expression.Constant(null, typeof(object))),
                       Expression.Constant(null, typeof(string)),
                       Expression.Call(propertyExpr, "ToString", null, null));

        return Expression.Call(selectorExpr, method, constant);
    }
}