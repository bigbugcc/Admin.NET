using Magicodes.ExporterAndImporter.Core;

namespace Admin.NET.Application;

/// <summary>
/// 导入对象
/// </summary>
public class Dm_ApplyDemoInport
{
    public long Id { get; set; }

    /// <summary>
    /// 机构类型
    /// </summary>
    [ImportDict(TargetPropName = "OrgType", TypeCode = "org_type")]
    [Required(ErrorMessage = "机构类型为必填内容")] 
    [ExporterHeader(DisplayName = "机构类型")]
    [ImporterHeader(Name = "机构类型")]
    public string OrgType { get; set; }

    /// <summary>
    /// 申请号
    /// </summary>
    [ImporterHeader(Name = "申请号")]
    [ExporterHeader(DisplayName = "申请号")]
    public string? ApplyNO { get; set; }
     

    /// <summary>
    /// 申请时间
    /// </summary>
    [ImporterHeader(Name = "申请时间")]
    [ExporterHeader(DisplayName = "申请时间")]
    public DateTime? ApplicatDate { get; set; }

    /// <summary>
    /// 申请金额
    /// </summary>
    [ImporterHeader(Name = "申请金额")]
    [ExporterHeader(DisplayName = "申请金额")]
    public DateTime Amount { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [ImporterHeader(Name = "备注")]
    [ExporterHeader(DisplayName = "备注")]
    public string Remark { get; set; }

}
