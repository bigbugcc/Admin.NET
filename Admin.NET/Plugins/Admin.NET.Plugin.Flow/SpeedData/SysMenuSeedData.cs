// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

using Admin.NET.Core;

namespace Admin.NET.Plugin.Report;

/// <summary>
/// 审批流菜单表种子数据
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
            new SysMenu{ Id=1300000000201, Pid=0, Title="平台功能", Path="/pages", Name="Pages", Component="Layout", Icon="ele-MagicStick", Type=MenuTypeEnum.Dir, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=140 },
            new SysMenu{ Id=1300000000211, Pid=1300000000201, Title="审批流管理", Path="/pages/approvalFlow", Name="approvalFlow", Component="/pages/approvalFlow/index", Icon="ele-Menu", Type=MenuTypeEnum.Menu, CreateTime=DateTime.Parse("2022-02-10 00:00:00"), OrderNo=100 },  
        };
    }
}