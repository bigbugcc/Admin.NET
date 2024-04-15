// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统字典类型表种子数据
/// </summary>
public class SysDictTypeSeedData : ISqlSugarEntitySeedData<SysDictType>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysDictType> HasData()
    {
        return new[]
        {
            new SysDictType{ Id=1300000000101, Name="代码生成控件类型", Code="code_gen_effect_type", OrderNo=100, Remark="代码生成控件类型", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictType{ Id=1300000000102, Name="代码生成查询类型", Code="code_gen_query_type", OrderNo=101, Remark="代码生成查询类型", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictType{ Id=1300000000103, Name="代码生成.NET类型", Code="code_gen_net_type", OrderNo=102, Remark="代码生成.NET类型", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictType{ Id=1300000000104, Name="代码生成方式", Code="code_gen_create_type", OrderNo=103, Remark="代码生成方式", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictType{ Id=1300000000105, Name="代码生成基类", Code="code_gen_base_class", OrderNo=104, Remark="代码生成基类", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2022-02-10 00:00:00") },
            new SysDictType{ Id=1300000000106, Name="代码生成打印类型", Code="code_gen_print_type", OrderNo=105, Remark="代码生成打印类型", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2023-12-04 00:00:00") },
            new SysDictType{ Id=1300000000201, Name="机构类型", Code="org_type", OrderNo=201, Remark="机构类型", Status=StatusEnum.Enable, CreateTime=DateTime.Parse("2023-02-10 00:00:00") },
        };
    }
}