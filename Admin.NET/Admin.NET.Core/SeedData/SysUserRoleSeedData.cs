// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 系统用户角色表种子数据
/// </summary>
public class SysUserRoleSeedData : ISqlSugarEntitySeedData<SysUserRole>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysUserRole> HasData()
    {
        return new[]
        {
            new SysUserRole{ Id=1300000000101, UserId=1300000000111, RoleId=1300000000101 },
            new SysUserRole{ Id=1300000000102, UserId=1300000000112, RoleId=1300000000102 },
            new SysUserRole{ Id=1300000000103, UserId=1300000000113, RoleId=1300000000103 },
            new SysUserRole{ Id=1300000000104, UserId=1300000000114, RoleId=1300000000104 },
            new SysUserRole{ Id=1300000000105, UserId=1300000000115, RoleId=1300000000105 },
        };
    }
}