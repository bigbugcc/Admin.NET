// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统用户表种子数据
/// </summary>
public class SysUserSeedData : ISqlSugarEntitySeedData<SysUser>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysUser> HasData()
    {
        var encryptPassword = CryptogramUtil.Encrypt("123456");

        return new[]
        {
            new SysUser{ Id=1300000000101, Account="superadmin", Password=encryptPassword, NickName="超级管理员", RealName="超级管理员", Phone="18012345678", Birthday=DateTime.Parse("2000-01-01"), Sex=GenderEnum.Male, AccountType=AccountTypeEnum.SuperAdmin, Remark="超级管理员", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), TenantId=1300000000001 },
            new SysUser{ Id=1300000000111, Account="admin", Password=encryptPassword, NickName="系统管理员", RealName="系统管理员", Phone="18012345678", Birthday=DateTime.Parse("2000-01-01"), Sex=GenderEnum.Male, AccountType=AccountTypeEnum.SysAdmin, Remark="系统管理员", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrgId=1300000000101, PosId=1300000000102, TenantId=1300000000001 },
            new SysUser{ Id=1300000000112, Account="user1", Password=encryptPassword, NickName="部门主管", RealName="部门主管", Phone="18012345678", Birthday=DateTime.Parse("2000-01-01"), Sex=GenderEnum.Female, AccountType=AccountTypeEnum.NormalUser, Remark="部门主管", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrgId=1300000000102, PosId=1300000000108, TenantId=1300000000001 },
            new SysUser{ Id=1300000000113, Account="user2", Password=encryptPassword, NickName="部门职员", RealName="部门职员", Phone="18012345678", Birthday=DateTime.Parse("2000-01-01"), Sex=GenderEnum.Female, AccountType=AccountTypeEnum.NormalUser, Remark="部门职员", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrgId=1300000000103, PosId=1300000000110, TenantId=1300000000001 },
            new SysUser{ Id=1300000000114, Account="user3", Password=encryptPassword, NickName="普通用户", RealName="普通用户", Phone="18012345678", Birthday=DateTime.Parse("2000-01-01"), Sex=GenderEnum.Female, AccountType=AccountTypeEnum.NormalUser, Remark="普通用户", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrgId=1300000000104, PosId=1300000000115, TenantId=1300000000001 },
            new SysUser{ Id=1300000000115, Account="user4", Password=encryptPassword, NickName="其他", RealName="其他", Phone="18012345678", Birthday=DateTime.Parse("2000-01-01"), Sex=GenderEnum.Female, AccountType=AccountTypeEnum.Member, Remark="会员", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrgId=1300000000105, PosId=1300000000116, TenantId=1300000000001 },
        };
    }
}