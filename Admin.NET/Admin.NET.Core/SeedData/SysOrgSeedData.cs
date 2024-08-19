// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统机构表种子数据
/// </summary>
[IgnoreUpdateSeed]
public class SysOrgSeedData : ISqlSugarEntitySeedData<SysOrg>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysOrg> HasData()
    {
        return new[]
        {
            new SysOrg{ Id=SqlSugarConst.DefaultTenantId, Pid=0, Name="系统默认", Code="1001", Type="101", Level=1, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="系统默认", TenantId=SqlSugarConst.DefaultTenantId },
            new SysOrg{ Id=SqlSugarConst.DefaultTenantId + 1, Pid=SqlSugarConst.DefaultTenantId, Name="市场部", Code="100101", Level=2, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="市场部", TenantId=SqlSugarConst.DefaultTenantId },
            new SysOrg{ Id=SqlSugarConst.DefaultTenantId + 2, Pid=SqlSugarConst.DefaultTenantId, Name="开发部", Code="100102", Level=2, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="开发部", TenantId=SqlSugarConst.DefaultTenantId },
            new SysOrg{ Id=SqlSugarConst.DefaultTenantId + 3, Pid=SqlSugarConst.DefaultTenantId, Name="售后部", Code="100103", Level=2, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="售后部", TenantId=SqlSugarConst.DefaultTenantId },
            new SysOrg{ Id=SqlSugarConst.DefaultTenantId + 4, Pid=SqlSugarConst.DefaultTenantId, Name="其他", Code="10010301", Level=3, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="其他", TenantId=SqlSugarConst.DefaultTenantId },
        };
    }
}