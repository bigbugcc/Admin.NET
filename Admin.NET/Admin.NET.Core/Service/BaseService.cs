// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 实体操作基服务
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class BaseService<TEntity> : IDynamicApiController where TEntity : class, new()
{
    private readonly SqlSugarRepository<TEntity> _rep;

    public BaseService(SqlSugarRepository<TEntity> rep)
    {
        _rep = rep;
    }

    /// <summary>
    /// 获取详情 🔖
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [DisplayName("获取详情")]
    public virtual async Task<TEntity> GetDetail(long id)
    {
        return await _rep.GetByIdAsync(id);
    }

    /// <summary>
    /// 获取集合 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("获取集合")]
    public virtual async Task<List<TEntity>> GetList()
    {
        return await _rep.GetListAsync();
    }

    ///// <summary>
    ///// 获取实体分页 🔖
    ///// </summary>
    ///// <param name="input"></param>
    ///// <returns></returns>
    //[ApiDescriptionSettings(Name = "Page")]
    //[DisplayName("获取实体分页")]
    //public async Task<SqlSugarPagedList<TEntity>> GetPage([FromQuery] BasePageInput input)
    //{
    //    return await _rep.AsQueryable().ToPagedListAsync(input.Page, input.PageSize);
    //}

    /// <summary>
    /// 增加 🔖
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加")]
    public virtual async Task<bool> Add(TEntity entity)
    {
        return await _rep.InsertAsync(entity);
    }

    /// <summary>
    /// 更新 🔖
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新")]
    public virtual async Task<int> Update(TEntity entity)
    {
        return await _rep.AsUpdateable(entity).IgnoreColumns(true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除 🔖
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除")]
    public virtual async Task<bool> Delete(long id)
    {
        return await _rep.DeleteByIdAsync(id);
    }
}