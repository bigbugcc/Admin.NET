// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Service;

public class UserExtOrgInput : BaseIdInput
{
    /// <summary>
    /// 机构Id
    /// </summary>
    public long OrgId { get; set; }

    /// <summary>
    /// 职位Id
    /// </summary>
    public long PosId { get; set; }

    /// <summary>
    /// 工号
    /// </summary>
    public string JobNum { get; set; }

    /// <summary>
    /// 职级
    /// </summary>
    public string PosLevel { get; set; }

    /// <summary>
    /// 入职日期
    /// </summary>
    public DateTime? JoinDate { get; set; }
}