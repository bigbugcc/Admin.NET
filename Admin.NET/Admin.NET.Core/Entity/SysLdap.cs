// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

using static SKIT.FlurlHttpClient.Wechat.Api.Models.CgibinExpressBusinessAccountGetAllResponse.Types;

namespace Admin.NET.Core;
/// <summary>
/// 系统域登录信息配置表
/// </summary>
[SugarTable(null, "系统域登录信息配置表")]
[SysTable]
public class SysLdap: EntityTenant
{
    /// <summary>
    /// 主机
    /// </summary>
    [SugarColumn(ColumnDescription = "主机", Length = 128)]
    public string Host { get; set; }

    /// <summary>
    /// 端口
    /// </summary>
    [SugarColumn(ColumnDescription = "端口")]
    public Int16 Port { get; set; }

    /// <summary>
    /// 用户搜索基准
    /// </summary>
    [SugarColumn(ColumnDescription = "用户搜索基准",Length =128)]
    public string BaseDn {  get; set; }

    /// <summary>
    /// 绑定DN(有管理权限制的用户)
    /// </summary>
    [SugarColumn(ColumnDescription = "绑定DN", Length =32)]
    public string BindDn { get; set; }

    /// <summary>
    /// 绑定密码(有管理权限制的用户密码)
    /// </summary>
    [SugarColumn(ColumnDescription = "绑定密码", Length = 512)]
    public string BindPass { get; set; }

    /// <summary>
    /// 用户过滤规则
    /// </summary>
    [SugarColumn(ColumnDescription = "用户过滤规则", Length = 128)]
    public string AuthFilter { get; set; } = "sAMAccountName";

    /// <summary>
    /// Ldap版本
    /// </summary>
    [SugarColumn(ColumnDescription = "Ldap版本")]
    public Int16 Version { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public StatusEnum Status { get; set; } = StatusEnum.Enable;
}
