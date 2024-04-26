// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统机构表种子数据
/// </summary>
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
            new SysOrg{ Id=1300000000101, Pid=0, Name="XXX公司", Code="1001", Type="101", Level=1, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="XXX公司", TenantId=1300000000001 },
            new SysOrg{ Id=1300000000102, Pid=1300000000101, Name="市场部", Code="100101", Level=2, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="市场部", TenantId=1300000000001 },
            new SysOrg{ Id=1300000000103, Pid=1300000000101, Name="研发部", Code="100102", Level=2, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="研发部", TenantId=1300000000001 },
            new SysOrg{ Id=1300000000104, Pid=1300000000101, Name="财务部", Code="100103", Level=2, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="财务部", TenantId=1300000000001 },
            new SysOrg{ Id=1300000000105, Pid=1300000000104, Name="财务部1", Code="10010301", Level=3, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="财务部1", TenantId=1300000000001 },
            new SysOrg{ Id=1300000000106, Pid=1300000000104, Name="财务部2", Code="10010302", Level=3, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="财务部2", TenantId=1300000000001 },

            new SysOrg{ Id=1300000000201, Pid=0, Name="分公司1", Code="1002", Type="201", Level=1, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="分公司1", TenantId=1300000000001 },
            new SysOrg{ Id=1300000000202, Pid=1300000000201, Name="市场部", Code="100201", Level=2, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="市场部", TenantId=1300000000001 },
            new SysOrg{ Id=1300000000203, Pid=1300000000201, Name="研发部", Code="100202", Level=2, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="研发部", TenantId=1300000000001 },
            new SysOrg{ Id=1300000000204, Pid=1300000000201, Name="财务部", Code="100203", Level=2, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="财务部", TenantId=1300000000001 },

            new SysOrg{ Id=1300000000301, Pid=0, Name="分公司2", Code="1003", Type="201", Level=1, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="分公司2", TenantId=1300000000001 },
            new SysOrg{ Id=1300000000302, Pid=1300000000301, Name="市场部", Code="100301", Level=2, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="市场部", TenantId=1300000000001 },
            new SysOrg{ Id=1300000000303, Pid=1300000000301, Name="研发部", Code="100302", Level=2, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="市场部", TenantId=1300000000001 },
            new SysOrg{ Id=1300000000304, Pid=1300000000301, Name="财务部", Code="100303", Level=2, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), Remark="市场部", TenantId=1300000000001 },
        };
    }
}