namespace Admin.NET.Plugin.Flow;

/// <summary>
/// 审批流输出参数
/// </summary>
public class ApprovalFlowOutput
{
    /// <summary>
    /// 主键Id
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// 编号
    /// </summary>
    public string? Code { get; set; }
    
    /// <summary>
    /// 名称
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// 表单
    /// </summary>
    public string? FormJson { get; set; }
    
    /// <summary>
    /// 流程
    /// </summary>
    public string? FlowJson { get; set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
    
    /// <summary>
    /// 创建者Id
    /// </summary>
    public long? CreateUserId { get; set; }
    
    /// <summary>
    /// 创建者姓名
    /// </summary>
    public string? CreateUserName { get; set; }
    
    /// <summary>
    /// 修改者Id
    /// </summary>
    public long? UpdateUserId { get; set; }
    
    /// <summary>
    /// 修改者姓名
    /// </summary>
    public string? UpdateUserName { get; set; }
    
    /// <summary>
    /// 创建者部门Id
    /// </summary>
    public long? CreateOrgId { get; set; }
    
    /// <summary>
    /// 创建者部门名称
    /// </summary>
    public string? CreateOrgName { get; set; }
    
    /// <summary>
    /// 软删除
    /// </summary>
    public bool IsDelete { get; set; }
    
}
 

