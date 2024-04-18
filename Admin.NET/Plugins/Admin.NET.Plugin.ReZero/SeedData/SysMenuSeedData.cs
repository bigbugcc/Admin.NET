// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Admin.NET.Core;

namespace Admin.NET.Plugin.ReZero;

/// <summary>
/// 超级API菜单表种子数据
/// </summary>
public class SysMenuSeedData : ISqlSugarEntitySeedData<SysMenu>
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SysMenu> HasData()
    {
        return new[]
        {
            new SysMenu{ Id=1310000000651, Pid=1310000000601, Title="超级API", Path="/develop/reZero", Name="sysReZero", Component="Layout", Icon="ele-MagicStick", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=140 },
            new SysMenu{ Id=1310000000661, Pid=1310000000651, Title="动态接口", Path="/develop/reZero/dynamicApi", Name="sysReZeroDynamicApi", Component="layout/routerView/iframe", Icon="ele-Edit", Type=MenuTypeEnum.Menu, IsIframe=true, OutLink="http://localhost:5005/xxx", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },
            new SysMenu{ Id=1310000000671, Pid=1310000000651, Title="接口管理", Path="/develop/reZero/interface", Name="sysReZeroInterface", Component="layout/routerView/iframe", Icon="ele-Edit", Type=MenuTypeEnum.Menu, IsIframe=true, OutLink="http://localhost:5005/xxx", CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=110 },
        };
    }
}