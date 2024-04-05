// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core;

public interface ISqlSugarRepository<T> : ISugarRepository, ISimpleClient<T> where T : class, new()
{
    /// <summary>
    /// 分表批量创建数据
    /// </summary>
    /// <param name="input">数据</param>
    /// <returns></returns>
    Task<bool> SplitTableInsertAsync(List<T> input);
    /// <summary>
    /// 分表创建数据
    /// </summary>
    /// <param name="input">数据</param>
    /// <returns></returns>
    Task<bool> SplitTableInsertAsync(T input);
    /// <summary>
    /// 分表批量删除数据
    /// </summary>
    /// <param name="input">数据</param>
    /// <returns></returns>
    Task<bool> SplitTableDeleteableAsync(List<T> input);
    /// <summary>
    /// 分表删除数据
    /// </summary>
    /// <param name="input">数据</param>
    /// <returns></returns>
    Task<bool> SplitTableDeleteableAsync(T input);
    /// <summary>
    /// 分表批量更新数据
    /// </summary>
    /// <param name="input">数据</param>
    /// <returns></returns>
    Task<bool> SplitTableUpdateAsync(List<T> input);
    /// <summary>
    /// 分表更新数据
    /// </summary>
    /// <param name="input">数据</param>
    /// <returns></returns>
    Task<bool> SplitTableUpdateAsync(T input);
    /// <summary>
    /// 分表判断是否存在
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<bool> SplitTableIsAnyAsync(Expression<Func<T, bool>> whereExpression);
    /// <summary>
    /// 分表获取列表
    /// </summary>
    /// <param name="whereExpression">条件</param>
    /// <returns></returns>
    Task<List<T>> SplitTableGetListAsync(Expression<Func<T, bool>> whereExpression);
    /// <summary>
    /// 分表获取列表
    /// </summary>
    /// <returns></returns>
    Task<List<T>> SplitTableGetListAsync();
    /// <summary>
    /// 分表GetFirstAsyn
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<T> SplitTableGetFirstAsync(Expression<Func<T, bool>> whereExpression);
    /// <summary>
    /// 分表获取列表
    /// </summary>
    /// <param name="whereExpression">条件</param>
    /// <param name="tableNames">分表表名</param>
    /// <returns></returns>
    Task<List<T>> SplitTableGetListAsync(Expression<Func<T, bool>> whereExpression, string[] tableNames);
}
