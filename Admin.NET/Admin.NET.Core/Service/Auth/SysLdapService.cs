using Novell.Directory.Ldap;

namespace Admin.NET.Core;
/// <summary>
/// 系统域登录信息配置表服务
/// </summary>
[ApiDescriptionSettings(Order = 100)]
public class SysLdapService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysLdap> _rep;
    private readonly SqlSugarRepository<SysUserLdap> _repUserLdap;
    public SysLdapService(SqlSugarRepository<SysLdap> rep, SqlSugarRepository<SysUserLdap> repUserLdap)
    {
        _rep = rep;
        _repUserLdap = repUserLdap;
    }

    /// <summary>
    /// 分页查询系统域登录信息配置表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Page")]
    public async Task<SqlSugarPagedList<SysLdapOutput>> Page(SysLdapInput input)
    {
        var query = _rep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.SearchKey), u =>
                u.Host.Contains(input.SearchKey.Trim())
            )
            .WhereIF(!string.IsNullOrWhiteSpace(input.Host), u => u.Host.Contains(input.Host.Trim()))
            .Select<SysLdapOutput>();
        return await query.OrderBuilder(input).ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 增加系统域登录信息配置表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Add")]
    public async Task<long> Add(AddSysLdapInput input)
    {
        var entity = input.Adapt<SysLdap>();
        entity.BindPass = CryptogramUtil.Encrypt(input.BindPass);
        await _rep.InsertAsync(entity);
        return entity.Id;
    }

    /// <summary>
    /// 删除系统域登录信息配置表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Delete")]
    public async Task Delete(DeleteSysLdapInput input)
    {
        var entity = await _rep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _rep.FakeDeleteAsync(entity);   //假删除
        //await _rep.DeleteAsync(entity);   //真删除
    }

    /// <summary>
    /// 更新系统域登录信息配置表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Update")]
    public async Task Update(UpdateSysLdapInput input)
    {
        var entity = input.Adapt<SysLdap>();
        if (!string.IsNullOrEmpty(input.BindPass) && input.BindPass.Length < 32)
        {
            entity.BindPass = CryptogramUtil.Encrypt(input.BindPass);//未加密的字符串执行加密
        }
        await _rep.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取系统域登录信息配置表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "Detail")]
    public async Task<SysLdap> Detail([FromQuery] QueryByIdSysLdapInput input)
    {
        return await _rep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 获取系统域登录信息配置表列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [ApiDescriptionSettings(Name = "List")]
    public async Task<List<SysLdapOutput>> List([FromQuery] SysLdapInput input)
    {
        return await _rep.AsQueryable().Select<SysLdapOutput>().ToListAsync();
    }

    /// <summary>
    /// 账号验证
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <param name="password">密码</param>
    /// <param name="tenantId">租户</param>
    /// <returns></returns>
    [NonAction]
    public async Task<bool> Auth(long tenantId, long userId, string password)
    {
        var user = await _repUserLdap.GetFirstAsync(u => u.UserId == userId && u.TenantId == tenantId) ?? throw Oops.Oh(ErrorCodeEnum.D0009);
        var ldap = await _rep.GetFirstAsync(u => u.TenantId == tenantId) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        LdapConnection conn = new LdapConnection();
        try
        {
            conn.Connect(ldap.Host, ldap.Port);
            conn.Bind(ldap.Version, ldap.BindDn, ldap.BindPass);
            var userEntitys = conn.Search(ldap.BaseDn, LdapConnection.ScopeSub, $"{ldap.AuthFilter}={user.Account}", null, false);
            string dn = string.Empty;
            while (userEntitys.HasMore())
            {
                var entity = userEntitys.Next();
                var sAMAccountName = entity.GetAttribute(ldap.AuthFilter)?.StringValue;

                if (!string.IsNullOrEmpty(sAMAccountName))
                {
                    dn = entity.Dn;
                    break;
                }
            }
            if (string.IsNullOrEmpty(dn)) throw Oops.Oh(ErrorCodeEnum.D1002);
            LdapAttribute attr = new LdapAttribute("userPassword", password);
            conn.Bind(dn, password);
        }
        catch (LdapException e)
        {
            switch (e.ResultCode)
            {
                case LdapException.NoSuchObject:
                case LdapException.NoSuchAttribute:
                    throw Oops.Oh(ErrorCodeEnum.D0009);
                case LdapException.InvalidCredentials:
                    return false;
                default:
                    throw Oops.Oh(e.Message);
            }
        }
        finally
        {
            conn.Disconnect();
        }
        return true;
    }

}

