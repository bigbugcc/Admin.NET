// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求�?
//
// 本项目主要遵�?MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中�?LICENSE-MIT �?LICENSE-APACHE 文件�?
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任�?

namespace Admin.NET.Core;

/// <summary>
/// SqlSugar 实体仓储
/// </summary>
/// <typeparam name="T"></typeparam>
public class SqlSugarRepository<T> : SimpleClient<T>, ISqlSugarRepository<T> where T : class, new()
{
    public SqlSugarRepository()
    {
        var iTenant = SqlSugarSetup.ITenant; // App.GetRequiredService<ISqlSugarClient>().AsTenant();
        base.Context = iTenant.GetConnectionScope(SqlSugarConst.MainConfigId);

        // 若实体贴有多库特性，则返回指定库连接
        if (typeof(T).IsDefined(typeof(TenantAttribute), false))
        {
            base.Context = iTenant.GetConnectionScopeWithAttr<T>();
            return;
        }

        // 若实体贴有日志表特性，则返回日志库连接
        if (typeof(T).IsDefined(typeof(LogTableAttribute), false))
        {
            if (iTenant.IsAnyConnection(SqlSugarConst.LogConfigId))
                base.Context = iTenant.GetConnectionScope(SqlSugarConst.LogConfigId);
            return;
        }

        // 若实体贴有系统表特性，则返回默认库连接
        if (typeof(T).IsDefined(typeof(SysTableAttribute), false))
            return;

        // 若未贴任何表特性或当前未登录或是默认租户Id，则返回默认库连�?
        var tenantId = App.User?.FindFirst(ClaimConst.TenantId)?.Value;
        if (string.IsNullOrWhiteSpace(tenantId) || tenantId == SqlSugarConst.MainConfigId) return;

        // 根据租户Id切换库连�? 为空则返回默认库连接
        var sqlSugarScopeProviderTenant = App.GetRequiredService<SysTenantService>().GetTenantDbConnectionScope(long.Parse(tenantId));
        if (sqlSugarScopeProviderTenant == null) return;
        base.Context = sqlSugarScopeProviderTenant;
    }

    #region 分表操作

    public async Task<bool> SplitTableInsertAsync(T input)
    {
        return await base.AsInsertable(input).SplitTable().ExecuteCommandAsync() > 0;
    }

    public async Task<bool> SplitTableInsertAsync(List<T> input)
    {
        return await base.AsInsertable(input).SplitTable().ExecuteCommandAsync() > 0;
    }

    public async Task<bool> SplitTableUpdateAsync(T input)
    {
        return await base.AsUpdateable(input).SplitTable().ExecuteCommandAsync() > 0;
    }

    public async Task<bool> SplitTableUpdateAsync(List<T> input)
    {
        return await base.AsUpdateable(input).SplitTable().ExecuteCommandAsync() > 0;
    }

    public async Task<bool> SplitTableDeleteableAsync(T input)
    {
        return await base.Context.Deleteable(input).SplitTable().ExecuteCommandAsync() > 0;
    }

    public async Task<bool> SplitTableDeleteableAsync(List<T> input)
    {
        return await base.Context.Deleteable(input).SplitTable().ExecuteCommandAsync() > 0;
    }

    public Task<T> SplitTableGetFirstAsync(Expression<Func<T, bool>> whereExpression)
    {
        return base.AsQueryable().SplitTable().FirstAsync(whereExpression);
    }

    public Task<bool> SplitTableIsAnyAsync(Expression<Func<T, bool>> whereExpression)
    {
        return base.Context.Queryable<T>().Where(whereExpression).SplitTable().AnyAsync();
    }

    public Task<List<T>> SplitTableGetListAsync()
    {
        return Context.Queryable<T>().SplitTable().ToListAsync();
    }

    public Task<List<T>> SplitTableGetListAsync(Expression<Func<T, bool>> whereExpression)
    {
        return Context.Queryable<T>().Where(whereExpression).SplitTable().ToListAsync();
    }

    public Task<List<T>> SplitTableGetListAsync(Expression<Func<T, bool>> whereExpression, string[] tableNames)
    {
        return Context.Queryable<T>().Where(whereExpression).SplitTable(t => t.InTableNames(tableNames)).ToListAsync();
    }

    #endregion 分表操作
}