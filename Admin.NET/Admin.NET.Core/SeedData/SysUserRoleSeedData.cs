// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

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