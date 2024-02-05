using Admin.NET.Core;
using System.ComponentModel.DataAnnotations;

namespace Admin.NET.Application;

/// <summary>
/// 申请示例基础输入参数
/// </summary>
public class Dm_ApplyDemoBaseInput
{
    /// <summary>
    /// 机构类型 详见字典类型 org_type
    /// </summary>
    public virtual long? OrgType { get; set; }

    /// <summary>
    /// 申请号
    /// </summary>
    public virtual string ApplyNO { get; set; }

    /// <summary>
    /// 申请时间
    /// </summary>
    public virtual DateTime ApplicatDate { get; set; }

    /// <summary>
    /// 申请金额
    /// </summary>
    public virtual decimal Amount { get; set; }

    /// <summary>
    /// 是否通知
    /// </summary>
    public virtual bool IsNotice { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public virtual string Remark { get; set; }

    /// <summary>
    /// CreateTime
    /// </summary>
    public virtual DateTime? CreateTime { get; set; }

    /// <summary>
    /// UpdateTime
    /// </summary>
    public virtual DateTime? UpdateTime { get; set; }

    /// <summary>
    /// CreateUserId
    /// </summary>
    public virtual long? CreateUserId { get; set; }

    /// <summary>
    /// CreateUserName
    /// </summary>
    public virtual string? CreateUserName { get; set; }

    /// <summary>
    /// UpdateUserId
    /// </summary>
    public virtual long? UpdateUserId { get; set; }

    /// <summary>
    /// UpdateUserName
    /// </summary>
    public virtual string? UpdateUserName { get; set; }

    /// <summary>
    /// IsDelete
    /// </summary>
    public virtual bool IsDelete { get; set; }

}

/// <summary>
/// 申请示例分页查询输入参数
/// </summary>
public class Dm_ApplyDemoInput : BasePageInput
{
    /// <summary>
    /// 关键字查询
    /// </summary>
    public string? SearchKey { get; set; }

    /// <summary>
    /// 机构类型 详见字典类型 org_type
    /// </summary>
    public long? OrgType { get; set; }

    /// <summary>
    /// 申请号
    /// </summary>
    public string ApplyNO { get; set; }

    /// <summary>
    /// 申请时间
    /// </summary>
    public DateTime ApplicatDate { get; set; }

    /// <summary>
    /// 申请金额
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 是否通知
    /// </summary>
    public bool IsNotice { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 申请时间范围
    /// </summary>
    public List<DateTime?> ApplicatDateRange { get; set; }
}

    /// <summary>
    /// 申请示例增加输入参数
    /// </summary>
    public class AddDm_ApplyDemoInput : Dm_ApplyDemoBaseInput
    {
        /// <summary>
        /// ApplyNO
        /// </summary>
        [Required(ErrorMessage = "ApplyNO不能为空")]
        public override string ApplyNO { get; set; }
        
        /// <summary>
        /// ApplicatDate
        /// </summary>
        [Required(ErrorMessage = "ApplicatDate不能为空")]
        public override DateTime ApplicatDate { get; set; }
        
        /// <summary>
        /// Amount
        /// </summary>
        [Required(ErrorMessage = "Amount不能为空")]
        public override decimal Amount { get; set; }
        
        /// <summary>
        /// IsNotice
        /// </summary>
        [Required(ErrorMessage = "IsNotice不能为空")]
        public override bool IsNotice { get; set; }
        
        /// <summary>
        /// Remark
        /// </summary>
        [Required(ErrorMessage = "Remark不能为空")]
        public override string Remark { get; set; }
        
        /// <summary>
        /// IsDelete
        /// </summary>
        [Required(ErrorMessage = "IsDelete不能为空")]
        public override bool IsDelete { get; set; }
        
    }

    /// <summary>
    /// 申请示例删除输入参数
    /// </summary>
    public class DeleteDm_ApplyDemoInput : BaseIdInput
    {
    }

    /// <summary>
    /// 申请示例更新输入参数
    /// </summary>
    public class UpdateDm_ApplyDemoInput : Dm_ApplyDemoBaseInput
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessage = "Id不能为空")]
        public long Id { get; set; }
        
    }

    /// <summary>
    /// 申请示例主键查询输入参数
    /// </summary>
    public class QueryByIdDm_ApplyDemoInput : DeleteDm_ApplyDemoInput
    {

    }
