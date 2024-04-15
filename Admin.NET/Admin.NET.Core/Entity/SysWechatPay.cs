// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统微信支付表
/// </summary>
[SugarTable(null, "系统微信支付表")]
[SysTable]
public partial class SysWechatPay : EntityBase
{
    /// <summary>
    /// 微信商户号
    /// </summary>
    [SugarColumn(ColumnDescription = "微信商户号")]
    [Required]
    public virtual string MerchantId { get; set; }

    /// <summary>
    /// 服务商AppId
    /// </summary>
    [SugarColumn(ColumnDescription = "服务商AppId")]
    [Required]
    public virtual string AppId { get; set; }

    /// <summary>
    /// 商户订单号
    /// </summary>
    [SugarColumn(ColumnDescription = "商户订单号")]
    [Required]
    public virtual string OutTradeNumber { get; set; }

    /// <summary>
    /// 支付订单号
    /// </summary>
    [SugarColumn(ColumnDescription = "支付订单号")]
    [Required]
    public virtual string TransactionId { get; set; }

    /// <summary>
    /// 交易类型
    /// </summary>
    [SugarColumn(ColumnDescription = "交易类型")]
    public string? TradeType { get; set; }

    /// <summary>
    /// 交易状态
    /// </summary>
    [SugarColumn(ColumnDescription = "交易状态")]
    public string? TradeState { get; set; }

    /// <summary>
    /// 交易状态描述
    /// </summary>
    [SugarColumn(ColumnDescription = "交易状态描述")]
    public string? TradeStateDescription { get; set; }

    /// <summary>
    /// 付款银行类型
    /// </summary>
    [SugarColumn(ColumnDescription = "付款银行类型")]
    public string? BankType { get; set; }

    /// <summary>
    /// 订单总金额
    /// </summary>
    [SugarColumn(ColumnDescription = "订单总金额")]
    public int Total { get; set; }

    /// <summary>
    /// 用户支付金额
    /// </summary>
    [SugarColumn(ColumnDescription = "用户支付金额")]
    public int? PayerTotal { get; set; }

    /// <summary>
    /// 支付完成时间
    /// </summary>
    [SugarColumn(ColumnDescription = "支付完成时间")]
    public DateTimeOffset? SuccessTime { get; set; }

    /// <summary>
    /// 交易结束时间
    /// </summary>
    [SugarColumn(ColumnDescription = "交易结束时间")]
    public DateTimeOffset? ExpireTime { get; set; }

    /// <summary>
    /// 商品描述
    /// </summary>
    [SugarColumn(ColumnDescription = "商品描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 场景信息
    /// </summary>
    [SugarColumn(ColumnDescription = "场景信息")]
    public string? Scene { get; set; }

    /// <summary>
    /// 附加数据
    /// </summary>
    [SugarColumn(ColumnDescription = "附加数据")]
    public string? Attachment { get; set; }

    /// <summary>
    /// 优惠标记
    /// </summary>
    [SugarColumn(ColumnDescription = "优惠标记")]
    public string? GoodsTag { get; set; }

    /// <summary>
    /// 结算信息
    /// </summary>
    [SugarColumn(ColumnDescription = "结算信息")]
    public string? Settlement { get; set; }

    /// <summary>
    /// 回调通知地址
    /// </summary>
    [SugarColumn(ColumnDescription = "回调通知地址")]
    public string? NotifyUrl { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注")]
    public string? Remark { get; set; }

    /// <summary>
    /// 微信OpenId标识
    /// </summary>
    [SugarColumn(ColumnDescription = "微信OpenId标识")]
    public string? OpenId { get; set; }

    /// <summary>
    /// 关联微信用户
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(OpenId))]
    public SysWechatUser SysWechatUser { get; set; }

    /// <summary>
    /// 子商户号
    /// </summary>
    [SugarColumn(ColumnDescription = "子商户号")]
    public string? SubMerchantId { get; set; }

    /// <summary>
    /// 子商户AppId
    /// </summary>
    [SugarColumn(ColumnDescription = "回调通知地址")]
    public string? SubAppId { get; set; }

    /// <summary>
    /// 子商户唯一标识
    /// </summary>
    [SugarColumn(ColumnDescription = "子商户唯一标识")]
    public string? SubOpenId { get; set; }
}