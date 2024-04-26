// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Security.Claims;

namespace Admin.NET.Core.Service;

/// <summary>
/// 权限验证
/// </summary>
public class IdentityService : ITransient
{
    private readonly IHttpContextAccessor _context;
    private readonly List<APIJSON_Role> _roles;

    public IdentityService(IHttpContextAccessor context, IOptions<APIJSONOptions> roles)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _roles = roles.Value.Roles;
    }

    /// <summary>
    /// 获取当前用户Id
    /// </summary>
    /// <returns></returns>
    public string GetUserIdentity()
    {
        return _context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    /// <summary>
    /// 获取当前用户权限名称
    /// </summary>
    /// <returns></returns>
    public string GetUserRoleName()
    {
        return _context.HttpContext.User.FindFirstValue(ClaimTypes.Role);
    }

    /// <summary>
    /// 获取当前用户权限
    /// </summary>
    /// <returns></returns>
    public APIJSON_Role GetRole()
    {
        var role = string.IsNullOrEmpty(GetUserRoleName())
            ? _roles.FirstOrDefault()
            : _roles.FirstOrDefault(it => it.RoleName.Equals(GetUserRoleName(), StringComparison.CurrentCultureIgnoreCase));
        return role;
    }

    /// <summary>
    /// 获取当前表的可查询字段
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public (bool, string) GetSelectRole(string table)
    {
        var role = GetRole();
        if (role == null || role.Select == null || role.Select.Table == null)
            return (false, $"appsettings.json权限配置不正确！");

        var tablerole = role.Select.Table.FirstOrDefault(it => it == "*" || it.Equals(table, StringComparison.CurrentCultureIgnoreCase));
        if (string.IsNullOrEmpty(tablerole))
            return (false, $"表名{table}没权限查询！");

        var index = Array.IndexOf(role.Select.Table, tablerole);
        var selectrole = role.Select.Column[index];
        return (true, selectrole);
    }

    /// <summary>
    /// 当前列是否在角色里面
    /// </summary>
    /// <param name="col"></param>
    /// <param name="selectrole"></param>
    /// <returns></returns>
    public bool ColIsRole(string col, string[] selectrole)
    {
        if (selectrole.Contains("*")) return true;

        if (col.Contains('(') && col.Contains(')'))
        {
            var reg = new Regex(@"\(([^)]*)\)");
            var match = reg.Match(col);
            return selectrole.Contains(match.Result("$1"), StringComparer.CurrentCultureIgnoreCase);
        }
        else
        {
            return selectrole.Contains(col, StringComparer.CurrentCultureIgnoreCase);
        }
    }
}