namespace Admin.NET.Application;

/// <summary>
/// 申请示例输出参数
/// </summary>
public class Dm_ApplyDemoDto
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

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
    /// CreateTime
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// UpdateTime
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// CreateUserId
    /// </summary>
    public long? CreateUserId { get; set; }

    /// <summary>
    /// CreateUserName
    /// </summary>
    public string? CreateUserName { get; set; }

    /// <summary>
    /// UpdateUserId
    /// </summary>
    public long? UpdateUserId { get; set; }

    /// <summary>
    /// UpdateUserName
    /// </summary>
    public string? UpdateUserName { get; set; }

    /// <summary>
    /// IsDelete
    /// </summary>
    public bool IsDelete { get; set; }

}
