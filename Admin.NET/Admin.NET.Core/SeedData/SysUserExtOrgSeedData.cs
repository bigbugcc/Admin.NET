// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 系统用户扩展机构表种子数据
/// </summary>
public class SysUserExtOrgSeedData : ISqlSugarEntitySeedData<SysUserExtOrg>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysUserExtOrg> HasData()
    {
        return new[]
        {
            new SysUserExtOrg{ Id=1300000000101, UserId=1300000000111, OrgId=1300000000202, PosId=1300000000106 },
            new SysUserExtOrg{ Id=1300000000102, UserId=1300000000114, OrgId=1300000000302, PosId=1300000000108  }
        };
    }
}