// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

using Novell.Directory.Ldap;

namespace Admin.NET.Core;

/// <summary>
/// 系统域登录信息配置表服务
/// </summary>
[ApiDescriptionSettings(Order = 100)]
public class SysLdapService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysLdap> _sysLdapRep;
    private readonly SqlSugarRepository<SysUserLdap> _sysUserLdapRep;

    public SysLdapService(SqlSugarRepository<SysLdap> rep, SqlSugarRepository<SysUserLdap> sysUserLdapRep)
    {
        _sysLdapRep = rep;
        _sysUserLdapRep = sysUserLdapRep;
    }

    /// <summary>
    /// 获取系统域登录信息配置分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<SqlSugarPagedList<SysLdap>> Page(SysLdapInput input)
    {
        return await _sysLdapRep.AsQueryable()
             .WhereIF(!string.IsNullOrWhiteSpace(input.SearchKey), u => u.Host.Contains(input.SearchKey.Trim()))
             .WhereIF(!string.IsNullOrWhiteSpace(input.Host), u => u.Host.Contains(input.Host.Trim()))
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 增加系统域登录信息配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    public async Task<long> Add(AddSysLdapInput input)
    {
        var entity = input.Adapt<SysLdap>();
        entity.BindPass = CryptogramUtil.Encrypt(input.BindPass);
        await _sysLdapRep.InsertAsync(entity);
        return entity.Id;
    }

    /// <summary>
    /// 更新系统域登录信息配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    public async Task Update(UpdateSysLdapInput input)
    {
        var entity = input.Adapt<SysLdap>();
        if (!string.IsNullOrEmpty(input.BindPass) && input.BindPass.Length < 32)
        {
            entity.BindPass = CryptogramUtil.Encrypt(input.BindPass); // 加密
        }
        await _sysLdapRep.AsUpdateable(entity).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除系统域登录信息配置
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    public async Task Delete(DeleteSysLdapInput input)
    {
        var entity = await _sysLdapRep.GetFirstAsync(u => u.Id == input.Id) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        await _sysLdapRep.FakeDeleteAsync(entity);  // 假删除
        //await _rep.DeleteAsync(entity);  // 真删除
    }

    /// <summary>
    /// 获取系统域登录信息配置详情
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Detail")]
    public async Task<SysLdap> GetDetail([FromQuery] DetailSysLdapInput input)
    {
        return await _sysLdapRep.GetFirstAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 获取系统域登录信息配置列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "List")]
    public async Task<List<SysLdap>> GetList([FromQuery] SysLdapInput input)
    {
        return await _sysLdapRep.AsQueryable().Select<SysLdap>().ToListAsync();
    }

    /// <summary>
    /// 账号验证
    /// </summary>
    /// <param name="account">域用户</param>
    /// <param name="password">密码</param>
    /// <param name="tenantId">租户</param>
    /// <returns></returns>
    [NonAction]
    public async Task<bool> Auth(long tenantId, string account, string password)
    {
        var ldap = await _rep.GetFirstAsync(u => u.TenantId == tenantId) ?? throw Oops.Oh(ErrorCodeEnum.D1002);
        LdapConnection conn = new LdapConnection();
        try
        {
            conn.Connect(ldap.Host, ldap.Port);
            conn.Bind(ldap.Version, ldap.BindDn, ldap.BindPass);
            var userEntitys = conn.Search(ldap.BaseDn, LdapConnection.ScopeSub, $"{ldap.AuthFilter}={account}", null, false);
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
            var attr = new LdapAttribute("userPassword", password);
            ldapConn.Bind(dn, password);
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
            ldapConn.Disconnect();
        }
        return true;
    }
}