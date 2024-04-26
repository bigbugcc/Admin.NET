// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

public static class SqlSugarFilter
{
    /// <summary>
    /// 缓存全局查询过滤器（内存缓存）
    /// </summary>
    private static readonly ICache _cache = Cache.Default;

    /// <summary>
    /// 删除用户机构缓存
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dbConfigId"></param>
    public static void DeleteUserOrgCache(long userId, string dbConfigId)
    {
        var sysCacheService = App.GetRequiredService<SysCacheService>();

        // 删除用户机构集合缓存
        sysCacheService.Remove($"{CacheConst.KeyUserOrg}{userId}");
        // 删除最大数据权限缓存
        sysCacheService.Remove($"{CacheConst.KeyRoleMaxDataScope}{userId}");
        // 删除用户机构（数据范围）缓存——过滤器
        _cache.Remove($"db:{dbConfigId}:orgList:{userId}");
    }

    /// <summary>
    /// 配置用户机构集合过滤器
    /// </summary>
    public static void SetOrgEntityFilter(SqlSugarScopeProvider db)
    {
        // 若仅本人数据，则直接返回
        if (SetDataScopeFilter(db) == (int)DataScopeEnum.Self) return;

        var userId = App.User?.FindFirst(ClaimConst.UserId)?.Value;
        if (string.IsNullOrWhiteSpace(userId)) return;

        // 配置用户机构集合缓存
        var cacheKey = $"db:{db.CurrentConnectionConfig.ConfigId}:orgList:{userId}";
        var orgFilter = _cache.Get<ConcurrentDictionary<Type, LambdaExpression>>(cacheKey);
        if (orgFilter == null)
        {
            // 获取用户所属机构，保证同一作用域
            var orgIds = new List<long>();
            Scoped.Create((factory, scope) =>
            {
                var services = scope.ServiceProvider;
                orgIds = services.GetService<SysOrgService>().GetUserOrgIdList().GetAwaiter().GetResult();
            });
            if (orgIds == null || orgIds.Count == 0) return;

            // 获取业务实体数据表
            var entityTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass
                && u.IsSubclassOf(typeof(EntityBaseData)));
            if (!entityTypes.Any()) return;

            orgFilter = new ConcurrentDictionary<Type, LambdaExpression>();
            foreach (var entityType in entityTypes)
            {
                // 排除非当前数据库实体
                var tAtt = entityType.GetCustomAttribute<TenantAttribute>();
                if ((tAtt != null && db.CurrentConnectionConfig.ConfigId.ToString() != tAtt.configId.ToString()))
                    continue;

                var lambda = DynamicExpressionParser.ParseLambda(new[] {
                    Expression.Parameter(entityType, "u") }, typeof(bool), $"@0.Contains(u.{nameof(EntityBaseData.CreateOrgId)}??{default(long)})", orgIds);
                db.QueryFilter.AddTableFilter(entityType, lambda);
                orgFilter.TryAdd(entityType, lambda);
            }
            _cache.Add(cacheKey, orgFilter);
        }
        else
        {
            foreach (var filter in orgFilter)
                db.QueryFilter.AddTableFilter(filter.Key, filter.Value);
        }
    }

    /// <summary>
    /// 配置用户仅本人数据过滤器
    /// </summary>
    private static int SetDataScopeFilter(SqlSugarScopeProvider db)
    {
        var maxDataScope = (int)DataScopeEnum.All;

        var userId = App.User?.FindFirst(ClaimConst.UserId)?.Value;
        if (string.IsNullOrWhiteSpace(userId)) return maxDataScope;

        // 获取用户最大数据范围---仅本人数据
        maxDataScope = App.GetRequiredService<SysCacheService>().Get<int>(CacheConst.KeyRoleMaxDataScope + userId);
        if (maxDataScope != (int)DataScopeEnum.Self) return maxDataScope;

        // 配置用户数据范围缓存
        var cacheKey = $"db:{db.CurrentConnectionConfig.ConfigId}:dataScope:{userId}";
        var dataScopeFilter = _cache.Get<ConcurrentDictionary<Type, LambdaExpression>>(cacheKey);
        if (dataScopeFilter == null)
        {
            // 获取业务实体数据表
            var entityTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass
                && u.IsSubclassOf(typeof(EntityBaseData)));
            if (!entityTypes.Any()) return maxDataScope;

            dataScopeFilter = new ConcurrentDictionary<Type, LambdaExpression>();
            foreach (var entityType in entityTypes)
            {
                // 排除非当前数据库实体
                var tAtt = entityType.GetCustomAttribute<TenantAttribute>();
                if ((tAtt != null && db.CurrentConnectionConfig.ConfigId.ToString() != tAtt.configId.ToString()))
                    continue;

                var lambda = DynamicExpressionParser.ParseLambda(new[] {
                    Expression.Parameter(entityType, "u") }, typeof(bool), $"u.{nameof(EntityBaseData.CreateUserId)}=@0", userId);
                db.QueryFilter.AddTableFilter(entityType, lambda);
                dataScopeFilter.TryAdd(entityType, lambda);
            }
            _cache.Add(cacheKey, dataScopeFilter);
        }
        else
        {
            foreach (var filter in dataScopeFilter)
                db.QueryFilter.AddTableFilter(filter.Key, filter.Value);
        }
        return maxDataScope;
    }

    /// <summary>
    /// 配置自定义过滤器
    /// </summary>
    public static void SetCustomEntityFilter(SqlSugarScopeProvider db)
    {
        // 配置自定义缓存
        var userId = App.User?.FindFirst(ClaimConst.UserId)?.Value;
        var cacheKey = $"db:{db.CurrentConnectionConfig.ConfigId}:custom:{userId}";
        var tableFilterItemList = _cache.Get<List<TableFilterItem<object>>>(cacheKey);
        if (tableFilterItemList == null)
        {
            // 获取自定义实体过滤器
            var entityFilterTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass
                && u.GetInterfaces().Any(i => i.HasImplementedRawGeneric(typeof(IEntityFilter))));
            if (!entityFilterTypes.Any()) return;

            var tableFilterItems = new List<TableFilterItem<object>>();
            foreach (var entityFilter in entityFilterTypes)
            {
                var instance = Activator.CreateInstance(entityFilter);
                var entityFilterMethod = entityFilter.GetMethod("AddEntityFilter");
                var entityFilters = ((IList)entityFilterMethod?.Invoke(instance, null))?.Cast<object>();
                if (entityFilters == null) continue;

                foreach (var u in entityFilters)
                {
                    var tableFilterItem = (TableFilterItem<object>)u;
                    var entityType = tableFilterItem.GetType().GetProperty("type", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(tableFilterItem, null) as Type;
                    // 排除非当前数据库实体
                    var tAtt = entityType.GetCustomAttribute<TenantAttribute>();
                    if ((tAtt != null && db.CurrentConnectionConfig.ConfigId.ToString() != tAtt.configId.ToString()) ||
                        (tAtt == null && db.CurrentConnectionConfig.ConfigId.ToString() != SqlSugarConst.MainConfigId))
                        continue;

                    tableFilterItems.Add(tableFilterItem);
                    db.QueryFilter.Add(tableFilterItem);
                }
            }
            _cache.Add(cacheKey, tableFilterItems);
        }
        else
        {
            tableFilterItemList.ForEach(u =>
            {
                db.QueryFilter.Add(u);
            });
        }
    }
}

/// <summary>
/// 自定义实体过滤器接口
/// </summary>
public interface IEntityFilter
{
    /// <summary>
    /// 实体过滤器
    /// </summary>
    /// <returns></returns>
    IEnumerable<TableFilterItem<object>> AddEntityFilter();
}

///// <summary>
///// 自定义业务实体过滤器示例
///// </summary>
//public class TestEntityFilter : IEntityFilter
//{
//    public IEnumerable<TableFilterItem<object>> AddEntityFilter()
//    {
//        // 构造自定义条件的过滤器
//        Expression<Func<SysUser, bool>> dynamicExpression = u => u.Remark.Contains("xxx");
//        var tableFilterItem = new TableFilterItem<object>(typeof(SysUser), dynamicExpression);

//        return new[]
//        {
//            tableFilterItem
//        };
//    }
//}