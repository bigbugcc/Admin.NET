using Magicodes.ExporterAndImporter.Core;
using Newtonsoft.Json;

namespace Admin.NET.Application;

/// <summary>
/// 申请示例输出参数
/// </summary>
public class Dm_ApplyDemoOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// OrgType
    /// </summary>
    [ExporterHeader(DisplayName = "机构类型")]
    public long? OrgType { get; set; }

    /// <summary>
    /// ApplyNO
    /// </summary>
    [ExporterHeader(DisplayName = "申请号")]
    public string ApplyNO { get; set; }

    /// <summary>
    /// ApplicatDate
    /// </summary>
    [JsonConverter(typeof(ChinaDateTimeConverterDate))]
    [ExporterHeader("申请时间",format:"yyyy-MM-dd")]
    public DateTime ApplicatDate { get; set; }

    /// <summary>
    /// Amount
    /// </summary>
    [ExporterHeader(DisplayName = "申请金额")]
    public decimal Amount { get; set; }
    
    /// <summary>
    /// IsNotice
    /// </summary>
    public bool IsNotice { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    [ExporterHeader(DisplayName = "备注")]
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
 

