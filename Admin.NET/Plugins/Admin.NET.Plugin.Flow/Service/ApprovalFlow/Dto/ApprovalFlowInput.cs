using Admin.NET.Core;
using System.ComponentModel.DataAnnotations;

namespace Admin.NET.Plugin.Flow;

/// <summary>
/// 审批流基础输入参数
/// </summary>
public class ApprovalFlowBaseInput
{
    /// <summary>
    /// 编号
    /// </summary>
    public virtual string? Code { get; set; }
    
    /// <summary>
    /// 名称
    /// </summary>
    public virtual string? Name { get; set; }
    
    /// <summary>
    /// 表单
    /// </summary>
    public virtual string? FormJson { get; set; }
    
    /// <summary>
    /// 流程
    /// </summary>
    public virtual string? FlowJson { get; set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    public virtual string? Remark { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public virtual DateTime? CreateTime { get; set; }
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public virtual DateTime? UpdateTime { get; set; }
    
    /// <summary>
    /// 创建者Id
    /// </summary>
    public virtual long? CreateUserId { get; set; }
    
    /// <summary>
    /// 创建者姓名
    /// </summary>
    public virtual string? CreateUserName { get; set; }
    
    /// <summary>
    /// 修改者Id
    /// </summary>
    public virtual long? UpdateUserId { get; set; }
    
    /// <summary>
    /// 修改者姓名
    /// </summary>
    public virtual string? UpdateUserName { get; set; }
    
    /// <summary>
    /// 创建者部门Id
    /// </summary>
    public virtual long? CreateOrgId { get; set; }
    
    /// <summary>
    /// 创建者部门名称
    /// </summary>
    public virtual string? CreateOrgName { get; set; }
    
    /// <summary>
    /// 软删除
    /// </summary>
    public virtual bool IsDelete { get; set; }
    
}

/// <summary>
/// 审批流分页查询输入参数
/// </summary>
public class ApprovalFlowInput : BasePageInput
{
    /// <summary>
    /// 关键字查询
    /// </summary>
    public string? SearchKey { get; set; }

    /// <summary>
    /// 编号
    /// </summary>
    public string? Code { get; set; }
    
    /// <summary>
    /// 名称
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
    
}

/// <summary>
/// 审批流增加输入参数
/// </summary>
public class AddApprovalFlowInput : ApprovalFlowBaseInput
{
    /// <summary>
    /// 软删除
    /// </summary>
    [Required(ErrorMessage = "软删除不能为空")]
    public override bool IsDelete { get; set; }
    
}

/// <summary>
/// 审批流删除输入参数
/// </summary>
public class DeleteApprovalFlowInput : BaseIdInput
{
}

/// <summary>
/// 审批流更新输入参数
/// </summary>
public class UpdateApprovalFlowInput : ApprovalFlowBaseInput
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [Required(ErrorMessage = "主键Id不能为空")]
    public long Id { get; set; }
    
}

/// <summary>
/// 审批流主键查询输入参数
/// </summary>
public class QueryByIdApprovalFlowInput : DeleteApprovalFlowInput
{

}
