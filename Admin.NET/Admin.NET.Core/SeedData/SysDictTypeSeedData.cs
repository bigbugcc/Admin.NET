﻿// 大名科技（天津）有限公司版权所有  电话：18020030720  QQ：515096995
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证

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