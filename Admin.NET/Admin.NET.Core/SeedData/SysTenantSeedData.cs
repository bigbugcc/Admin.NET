// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 系统租户表种子数据
/// </summary>
public class SysTenantSeedData : ISqlSugarEntitySeedData<SysTenant>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysTenant> HasData()
    {
        var defaultDbConfig = App.GetOptions<DbConnectionOptions>().ConnectionConfigs[0];

        return new[]
        {
            new SysTenant{ Id=1300000000001, OrgId=1300000000101, UserId=1300000000111, Host="https://gitee.com", TenantType=TenantTypeEnum.Id, DbType=defaultDbConfig.DbType, Connection=defaultDbConfig.ConnectionString, ConfigId=SqlSugarConst.MainConfigId, Remark="系统默认", CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
        };
    }
}