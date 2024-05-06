using Admin.NET.Core;

namespace Admin.NET.Plugin.Flow.Entity;

/// <summary>
/// 审批流信息表
/// </summary>
[SugarTable("ApprovalFlow", "审批流信息表")]
public class ApprovalFlow : EntityBaseData
{
    /// <summary>
    /// 编号
    /// </summary>
    [SugarColumn(ColumnName = "Code", ColumnDescription = "编号", Length = 32)]
    public string? Code { get; set; }
    
    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(ColumnName = "Name", ColumnDescription = "名称", Length = 32)]
    public string? Name { get; set; }
    
    /// <summary>
    /// 表单
    /// </summary>
    [SugarColumn(ColumnName = "FormJson", ColumnDescription = "表单", Length = 0)]
    public string? FormJson { get; set; }
    
    /// <summary>
    /// 流程
    /// </summary>
    [SugarColumn(ColumnName = "FlowJson", ColumnDescription = "流程", Length = 0)]
    public string? FlowJson { get; set; }
    
    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnName = "Status", ColumnDescription = "状态")]
    public int? Status { get; set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnName = "Remark", ColumnDescription = "备注", Length = 255)]
    public string? Remark { get; set; }
    
}
