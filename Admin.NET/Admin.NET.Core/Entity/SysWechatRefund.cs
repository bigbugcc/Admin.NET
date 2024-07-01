// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统微信支付退款表
/// </summary>
[SugarTable(null, "系统微信支付退款表")]
[SysTable]
[SugarIndex("idx_{table}_WechatPayId", nameof(WechatPayId), OrderByType.Asc)]
public partial class SysWechatRefund : EntityBase
{
    /// <summary>
    /// 定单主键
    /// </summary>
    [SugarColumn(ColumnDescription = "定单主键")]
    public long WechatPayId { get; set; }

    /// <summary>
    /// 商户退款号
    /// </summary>
    [SugarColumn(ColumnDescription = "商户退款号")]
    [Required]
    public virtual string OutRefundNumber { get; set; }

    /// <summary>
    /// 退款订单号
    /// </summary>
    [SugarColumn(ColumnDescription = "退款订单号")]
    [Required]
    public virtual string TransactionId { get; set; }

    /// <summary>
    /// 退款原因
    /// </summary>
    [SugarColumn(ColumnDescription = "退款原因")]
    public string? Reason { get; set; }

    /// <summary>
    /// 退款渠道
    /// </summary>
    [SugarColumn(ColumnDescription = "退款渠道")]
    public string? Channel { get; set; }

    /// <summary>
    /// 退款入账账户
    /// </summary>
    /// <remarks>
    /// 取当前退款单的退款入账方，有以下几种情况：
    /// 1）退回银行卡：{银行名称}{卡类型}{ 卡尾号}
    /// 2）退回支付用户零钱: 支付用户零钱
    /// 3）退还商户: 商户基本账户商户结算银行账户
    /// 4）退回支付用户零钱通: 支付用户零钱通
    /// </remarks>
    [SugarColumn(ColumnDescription = "退款入账账户")]
    public string? UserReceivedAccount { get; set; }

    /// <summary>
    /// 退款状态
    /// </summary>
    [SugarColumn(ColumnDescription = "退款状态")]
    public string? TradeState { get; set; }

    /// <summary>
    /// 交易状态描述
    /// </summary>
    [SugarColumn(ColumnDescription = "交易状态描述")]
    public string? TradeStateDescription { get; set; }

    /// <summary>
    /// 订单总金额
    /// </summary>
    [SugarColumn(ColumnDescription = "退款金额")]
    public int Refund { get; set; }

    /// <summary>
    /// 支完成时间
    /// </summary>
    [SugarColumn(ColumnDescription = "完成时间")]
    public DateTime? SuccessTime { get; set; }



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

}