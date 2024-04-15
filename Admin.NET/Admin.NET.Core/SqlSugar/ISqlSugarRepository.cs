// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 分表操作仓储接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISqlSugarRepository<T> : ISugarRepository, ISimpleClient<T> where T : class, new()
{
    /// <summary>
    /// 创建数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<bool> SplitTableInsertAsync(T input);

    /// <summary>
    /// 批量创建数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<bool> SplitTableInsertAsync(List<T> input);

    /// <summary>
    /// 更新数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<bool> SplitTableUpdateAsync(T input);

    /// <summary>
    /// 批量更新数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<bool> SplitTableUpdateAsync(List<T> input);

    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<bool> SplitTableDeleteableAsync(T input);

    /// <summary>
    /// 批量删除数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<bool> SplitTableDeleteableAsync(List<T> input);

    /// <summary>
    /// 获取第一条
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<T> SplitTableGetFirstAsync(Expression<Func<T, bool>> whereExpression);

    /// <summary>
    /// 判断是否存在
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<bool> SplitTableIsAnyAsync(Expression<Func<T, bool>> whereExpression);

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <returns></returns>
    Task<List<T>> SplitTableGetListAsync();

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<List<T>> SplitTableGetListAsync(Expression<Func<T, bool>> whereExpression);

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <param name="tableNames">表名</param>
    /// <returns></returns>
    Task<List<T>> SplitTableGetListAsync(Expression<Func<T, bool>> whereExpression, string[] tableNames);
}