// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

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