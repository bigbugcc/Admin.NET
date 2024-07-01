// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Furion.Logging.Extensions;
using Newtonsoft.Json;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using System.Transactions;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.ShopCouponGetResponse.Types.Result.Types.Coupon.Types.CouponDetail.Types.Discount.Types.DiscountCondidtion.Types;

namespace Admin.NET.Core.Service;

/// <summary>
/// 微信支付服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 210)]
public class SysWechatPayService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysWechatPay> _sysWechatPayRep;
    private readonly SqlSugarRepository<SysWechatRefund> _sysWechatRefundRep;

    private readonly WechatPayOptions _wechatPayOptions;
    private readonly PayCallBackOptions _payCallBackOptions;

    private readonly WechatTenpayClient _wechatTenpayClient;

    public SysWechatPayService(SqlSugarRepository<SysWechatPay> sysWechatPayUserRep,
        SqlSugarRepository<SysWechatRefund> sysWechatRefundRep,
        IOptions<WechatPayOptions> wechatPayOptions,
        IOptions<PayCallBackOptions> payCallBackOptions)
    {
        _sysWechatPayRep = sysWechatPayUserRep;
        _sysWechatRefundRep = sysWechatRefundRep;
        _wechatPayOptions = wechatPayOptions.Value;
        _payCallBackOptions = payCallBackOptions.Value;

        _wechatTenpayClient = CreateTenpayClient();
    }

    /// <summary>
    /// 初始化微信支付客户端
    /// </summary>
    /// <returns></returns>
    private WechatTenpayClient CreateTenpayClient()
    {
        var cerFilePath = App.WebHostEnvironment.ContentRootPath + _wechatPayOptions.MerchantCertificatePrivateKey;

        var tenpayClientOptions = new WechatTenpayClientOptions()
        {
            MerchantId = _wechatPayOptions.MerchantId,
            MerchantV3Secret = _wechatPayOptions.MerchantV3Secret,
            MerchantCertificateSerialNumber = _wechatPayOptions.MerchantCertificateSerialNumber,
            MerchantCertificatePrivateKey = File.Exists(cerFilePath) ? File.ReadAllText(cerFilePath) : "",
            PlatformCertificateManager = new InMemoryCertificateManager()
        };
        return new WechatTenpayClient(tenpayClientOptions);
    }
    /// <summary>
    /// 分页查询支付列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ApiDescriptionSettings(Name = "Page")]
    public async Task<SqlSugarPagedList<SysWechatPay>> Page(WechatPayPageInput input)
    {
        var query = _sysWechatPayRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.SearchKey), u => u.OutTradeNumber == input.SearchKey || u.TransactionId == input.SearchKey)
            .WhereIF(input.CreateTimeRange != null && input.CreateTimeRange.Count > 0 && input.CreateTimeRange[0].HasValue, x => x.CreateTime >= input.CreateTimeRange[0])
            .WhereIF(input.CreateTimeRange != null && input.CreateTimeRange.Count > 1 && input.CreateTimeRange[1].HasValue, x => x.CreateTime < ((DateTime)input.CreateTimeRange[1]).AddDays(1));
        return await query.OrderBuilder(input).ToPagedListAsync(input.Page, input.PageSize);
    }
    /// <summary>
    /// 查询退款信息列表
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost]
    [DisplayName("根据支付id获取退款信息列表")]
    public async Task<List<SysWechatRefund>> ListRefund([FromBody]string id)
    {
        var query = _sysWechatRefundRep.AsQueryable()
            .Where(u => u.TransactionId == id);         
        return await query.ToListAsync();
    }

    /// <summary>
    /// 生成JSAPI调起支付所需参数 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("生成JSAPI调起支付所需参数")]
    public WechatPayParaOutput GenerateParametersForJsapiPay(WechatPayParaInput input)
    {
        var data = _wechatTenpayClient.GenerateParametersForJsapiPayRequest(_wechatPayOptions.AppId, input.PrepayId);
        return new WechatPayParaOutput()
        {
            AppId = data["appId"],
            TimeStamp = data["timeStamp"],
            NonceStr = data["nonceStr"],
            Package = data["package"],
            SignType = data["signType"],
            PaySign = data["paySign"]
        };
    }

    /// <summary>
    /// 微信支付下单(商户直连) 🔖
    /// </summary>
    [DisplayName("微信支付下单(商户直连)")]
    public async Task<WechatPayTransactionOutput> CreatePayTransaction([FromBody] WechatPayTransactionInput input)
    {
        var request = new CreatePayTransactionJsapiRequest()
        {
            OutTradeNumber = DateTimeOffset.Now.ToString("yyyyMMddHHmmssfff") + (new Random()).Next(100, 1000), // 订单号
            AppId = _wechatPayOptions.AppId,
            Description = input.Description,
            Attachment = input.Attachment,
            GoodsTag = input.GoodsTag,
            ExpireTime = DateTimeOffset.Now.AddMinutes(10),
            NotifyUrl = _payCallBackOptions.WechatPayUrl,
            Amount = new CreatePayTransactionJsapiRequest.Types.Amount() { Total = input.Total },
            Payer = new CreatePayTransactionJsapiRequest.Types.Payer() { OpenId = input.OpenId }
        };
        var response = await _wechatTenpayClient.ExecuteCreatePayTransactionJsapiAsync(request);
        if (!response.IsSuccessful())
            throw Oops.Oh(response.ErrorMessage);

        var singInfo = this.GenerateParametersForJsapiPay(new WechatPayParaInput() { PrepayId = response.PrepayId });
        // 保存订单信息
        var wechatPay = new SysWechatPay()
        {
            AppId = _wechatPayOptions.AppId,
            MerchantId = _wechatPayOptions.MerchantId,
            OutTradeNumber = request.OutTradeNumber,
            Description = input.Description,
            Attachment = input.Attachment,
            GoodsTag = input.GoodsTag,
            Total = input.Total,
            OpenId = input.OpenId,
            TransactionId = "",
            Tags = input.Tags,
            BusinessId = input.BusinessId,
        };
        await _sysWechatPayRep.InsertAsync(wechatPay);

        return new WechatPayTransactionOutput()
        {
            PrepayId = response.PrepayId,
            OutTradeNumber = request.OutTradeNumber,
            SingInfo = singInfo
        };
    }

    /// <summary>
    /// 微信支付下单(商户直连)Native
    /// </summary>
    [DisplayName("微信支付下单(商户直连)Native")]
    public async Task<dynamic> CreatePayTransactionNative([FromBody] WechatPayTransactionInput input)
    {
        var request = new CreatePayTransactionNativeRequest()
        {
            OutTradeNumber = DateTimeOffset.Now.ToString("yyyyMMddHHmmssfff") + (new Random()).Next(100, 1000), // 订单号
            AppId = _wechatPayOptions.AppId,
            Description = input.Description,
            Attachment = input.Attachment,
            GoodsTag = input.GoodsTag,
            ExpireTime = DateTimeOffset.Now.AddMinutes(10),
            NotifyUrl = _payCallBackOptions.WechatPayUrl,
            Amount = new CreatePayTransactionNativeRequest.Types.Amount() { Total = input.Total },
            //Payer = new CreatePayTransactionNativeRequest.Types.Payer() { OpenId = input.OpenId }
            Scene = new CreatePayTransactionNativeRequest.Types.Scene() { ClientIp = "127.0.0.1"}
        };
        var response = await _wechatTenpayClient.ExecuteCreatePayTransactionNativeAsync(request);
        if (!response.IsSuccessful())
        {
            JsonConvert.SerializeObject(response).LogInformation();
            throw Oops.Oh(response.ErrorMessage);
        }
        // 保存订单信息
        var wechatPay = new SysWechatPay()
        {
            AppId = _wechatPayOptions.AppId,
            MerchantId = _wechatPayOptions.MerchantId,
            OutTradeNumber = request.OutTradeNumber,
            Description = input.Description,
            Attachment = input.Attachment,
            GoodsTag = input.GoodsTag,
            Total = input.Total,
            //OpenId = input.OpenId,
            TransactionId = "",
            QrcodeContent= response.QrcodeUrl,
            Tags = input.Tags,
            BusinessId = input.BusinessId,
        };
        await _sysWechatPayRep.InsertAsync(wechatPay);
        return new
        {
            request.OutTradeNumber,
            response.QrcodeUrl
        };
    }

    /// <summary>
    /// 微信支付下单(服务商模式) 🔖
    /// </summary>
    [DisplayName("微信支付下单(服务商模式)")]
    public async Task<dynamic> CreatePayPartnerTransaction([FromBody] WechatPayTransactionInput input)
    {
        var request = new CreatePayPartnerTransactionJsapiRequest()
        {
            OutTradeNumber = DateTimeOffset.Now.ToString("yyyyMMddHHmmssfff") + (new Random()).Next(100, 1000), // 订单号
            AppId = _wechatPayOptions.AppId,
            MerchantId = _wechatPayOptions.MerchantId,
            SubAppId = _wechatPayOptions.AppId,
            SubMerchantId = _wechatPayOptions.MerchantId,
            Description = input.Description,
            Attachment = input.Attachment,
            GoodsTag = input.GoodsTag,
            ExpireTime = DateTimeOffset.Now.AddMinutes(10),
            NotifyUrl = _payCallBackOptions.WechatPayUrl,
            Amount = new CreatePayPartnerTransactionJsapiRequest.Types.Amount() { Total = input.Total },
            Payer = new CreatePayPartnerTransactionJsapiRequest.Types.Payer() { OpenId = input.OpenId }
        };
        var response = await _wechatTenpayClient.ExecuteCreatePayPartnerTransactionJsapiAsync(request);
        if (!response.IsSuccessful())
            throw Oops.Oh(response.ErrorMessage);
        var singInfo = this.GenerateParametersForJsapiPay(new WechatPayParaInput() { PrepayId = response.PrepayId });
        // 保存订单信息
        var wechatPay = new SysWechatPay()
        {
            AppId = _wechatPayOptions.AppId,
            MerchantId = _wechatPayOptions.MerchantId,
            SubAppId = _wechatPayOptions.AppId,
            SubMerchantId = _wechatPayOptions.MerchantId,
            OutTradeNumber = request.OutTradeNumber,
            Description = input.Description,
            Attachment = input.Attachment,
            GoodsTag = input.GoodsTag,
            Total = input.Total,
            OpenId = input.OpenId,
            TransactionId = ""
        };
        await _sysWechatPayRep.InsertAsync(wechatPay);
        
        return new
        {
            response.PrepayId,
            request.OutTradeNumber,
            singInfo
        };
    }

    /// <summary>
    /// 获取支付订单详情(本地库) 🔖
    /// </summary>
    /// <param name="tradeId"></param>
    /// <returns></returns>
    [DisplayName("获取支付订单详情(本地库)")]
    public async Task<SysWechatPay> GetPayInfo(string tradeId)
    {
        return await _sysWechatPayRep.GetFirstAsync(u => u.OutTradeNumber == tradeId);
    }

    /// <summary>
    /// 获取支付订单详情(微信接口) 🔖
    /// </summary>
    /// <param name="tradeId"></param>
    /// <returns></returns>
    [DisplayName("获取支付订单详情(微信接口)")]
    public async Task<SysWechatPay> GetPayInfoFromWechat(string tradeId)
    {
        var request = new GetPayTransactionByOutTradeNumberRequest();
        request.OutTradeNumber = tradeId;
        var response = await _wechatTenpayClient.ExecuteGetPayTransactionByOutTradeNumberAsync(request);
        // 修改订单支付状态
        var wechatPay = await _sysWechatPayRep.GetFirstAsync(u => u.OutTradeNumber == response.OutTradeNumber
            && u.MerchantId == response.MerchantId);
        // 如果状态不一致就更新数据库中的记录
        if (wechatPay != null && wechatPay.TradeState != response.TradeState)
        {
            wechatPay.OpenId = response.Payer.OpenId;
            wechatPay.TransactionId = response.TransactionId; // 支付订单号
            wechatPay.TradeType = response.TradeType; // 交易类型
            wechatPay.TradeState = response.TradeState; // 交易状态
            wechatPay.TradeStateDescription = response.TradeStateDescription; // 交易状态描述
            wechatPay.BankType = response.BankType; // 付款银行类型
            wechatPay.Total = response.Amount.Total; // 订单总金额
            wechatPay.PayerTotal = response.Amount.PayerTotal; // 用户支付金额
            wechatPay.SuccessTime = response.SuccessTime.Value.DateTime; // 支付完成时间
            await _sysWechatPayRep.AsUpdateable(wechatPay).IgnoreColumns(true).ExecuteCommandAsync();
        }
        wechatPay = new SysWechatPay()
        {
            AppId = _wechatPayOptions.AppId,
            MerchantId = _wechatPayOptions.MerchantId,
            SubAppId = _wechatPayOptions.AppId,
            SubMerchantId = _wechatPayOptions.MerchantId,
            OutTradeNumber = request.OutTradeNumber,
            Attachment = response.Attachment,
            Total = response.Amount.Total, // 订单总金额
            TransactionId = response.TransactionId,
            TradeType = response.TradeType, // 交易类型
            TradeState = response.TradeState, // 交易状态
            TradeStateDescription = response.TradeStateDescription, // 交易状态描述
            BankType = response.BankType, // 付款银行类型
            PayerTotal = response.Amount.PayerTotal, // 用户支付金额
            SuccessTime = response.SuccessTime.Value.DateTime // 支付完成时间
        };
        return wechatPay;
    }

    /// <summary>
    /// 退款申请
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("退款申请")]
    [HttpPost]
    public async Task<dynamic> CreateRefundDomestic([FromBody] WechatPayRefundDomesticInput input)
    {
        // refund/domestic/refunds
        var request = new CreateRefundDomesticRefundRequest()
        {
            Amount = new CreateRefundDomesticRefundRequest.Types.Amount() { 
                Refund = input.Refund,
                Total = input.Total,
                Currency = "CNY"
            },
            
            OutTradeNumber = input.TradeId,
            OutRefundNumber = "R" + DateTimeOffset.Now.ToString("yyyyMMddHHmmssfff") + (new Random()).Next(100, 1000), // 订单号
            NotifyUrl = _payCallBackOptions.WechatPayUrl,
            Reason = input.Reason,
        };
        var response = await _wechatTenpayClient.ExecuteCreateRefundDomesticRefundAsync(request);
        if (string.IsNullOrEmpty(response.ErrorCode))
        {
            // 成功了，这里应该保存退款订单信息
            var wechatPay = await _sysWechatPayRep.GetFirstAsync(u => u.OutTradeNumber == response.OutTradeNumber);
            // 保存订单信息
            if (wechatPay != null)
            {
                var wechatRefund = new SysWechatRefund()
                {
                    WechatPayId = wechatPay.Id,
                    TransactionId = response.TransactionId,
                    Refund = input.Refund,
                    Reason = input.Reason,
                    OutRefundNumber = request.OutRefundNumber,
                    Channel = response.Channel,
                    UserReceivedAccount = response.UserReceivedAccount,
                };
                await _sysWechatRefundRep.InsertAsync(wechatRefund);
            }
        }
        else
        {
            throw Oops.Bah($"[{response.ErrorCode}]{response.ErrorMessage}");
        }
        return response;
    }

    /// <summary>
    /// 获取退款订单详情(微信接口)
    /// </summary>
    /// <param name="refundId"></param>
    /// <returns></returns>
    [DisplayName("获取退款订单详情(微信接口)")]
    public async Task<SysWechatRefund> GetRefundInfoFromWechat(string refundId)
    {
        var request = new GetRefundDomesticRefundByOutRefundNumberRequest();
        request.OutRefundNumber = refundId;
        var response = await _wechatTenpayClient.ExecuteGetRefundDomesticRefundByOutRefundNumberAsync(request);
        // 修改订单支付状态
        var wechatRefund = await _sysWechatRefundRep.GetFirstAsync(u => u.OutRefundNumber == refundId);
        // 如果状态不一致就更新数据库中的记录
        if (wechatRefund != null && wechatRefund.TradeState != response.Status)
        {
            wechatRefund.TransactionId = response.TransactionId; // 支付订单号
            wechatRefund.TradeState = response.Status; // 交易状态
            wechatRefund.SuccessTime = response.SuccessTime.Value.DateTime; // 支付完成时间
            await _sysWechatRefundRep.AsUpdateable(wechatRefund).IgnoreColumns(true).ExecuteCommandAsync();
            // 有退款，刷新一下订单状态
            var wechatPay = await _sysWechatPayRep.GetFirstAsync(u => u.Id == wechatRefund.WechatPayId);
            if (wechatPay!=null)
                GetPayInfoFromWechat(wechatPay.OutTradeNumber);
        }
        wechatRefund = new SysWechatRefund()
        {
            TransactionId = response.TransactionId,
            Refund = response.Amount.Refund,
            OutRefundNumber = request.OutRefundNumber,
            Channel = response.Channel,
            UserReceivedAccount = response.UserReceivedAccount,
            TradeState = response.Status, // 交易状态
            SuccessTime = response.SuccessTime.Value.DateTime, // 支付完成时间
        };
        return wechatRefund;
    }
    /// <summary>
    /// 微信支付成功回调(商户直连)
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("微信支付成功回调(商户直连)")]
    public async Task<WechatPayOutput> PayCallBack()
    {
        using var ms = new MemoryStream();
        await App.HttpContext.Request.Body.CopyToAsync(ms);
        var b = ms.ToArray();
        var callbackJson = Encoding.UTF8.GetString(b);

        var callbackModel = _wechatTenpayClient.DeserializeEvent(callbackJson);
        if ("TRANSACTION.SUCCESS".Equals(callbackModel.EventType))
        {
            try
            {
                var callbackPayResource = _wechatTenpayClient.DecryptEventResource<TransactionResource>(callbackModel);

                // 修改订单支付状态
                var wechatPay = await _sysWechatPayRep.GetFirstAsync(u => u.OutTradeNumber == callbackPayResource.OutTradeNumber
                    && u.MerchantId == callbackPayResource.MerchantId);
                if (wechatPay == null) return null;
                wechatPay.OpenId = callbackPayResource.Payer.OpenId; // 支付者标识
                //wechatPay.MerchantId = callbackResource.MerchantId; // 微信商户号
                //wechatPay.OutTradeNumber = callbackResource.OutTradeNumber; // 商户订单号
                wechatPay.TransactionId = callbackPayResource.TransactionId; // 支付订单号
                wechatPay.TradeType = callbackPayResource.TradeType; // 交易类型
                wechatPay.TradeState = callbackPayResource.TradeState; // 交易状态
                wechatPay.TradeStateDescription = callbackPayResource.TradeStateDescription; // 交易状态描述
                wechatPay.BankType = callbackPayResource.BankType; // 付款银行类型
                wechatPay.Total = callbackPayResource.Amount.Total; // 订单总金额
                wechatPay.PayerTotal = callbackPayResource.Amount.PayerTotal; // 用户支付金额
                wechatPay.SuccessTime = callbackPayResource.SuccessTime.DateTime; // 支付完成时间

                await _sysWechatPayRep.AsUpdateable(wechatPay).IgnoreColumns(true).ExecuteCommandAsync();

                return new WechatPayOutput()
                {
                    Total = wechatPay.Total,
                    Attachment = wechatPay.Attachment,
                    GoodsTag = wechatPay.GoodsTag
                };
            }
            catch(Exception ex)
            {
                "微信支付回调时出错：".LogError(ex);
            }
        }
        else if ("REFUND.SUCCESS".Equals(callbackModel.EventType))
        {
            //参考：https://pay.weixin.qq.com/docs/merchant/apis/jsapi-payment/refund-result-notice.html
            try
            {
                var callbackRefundResource = _wechatTenpayClient.DecryptEventResource<RefundResource>(callbackModel);
                // 修改订单支付状态
                var wechatRefund = await _sysWechatRefundRep.GetFirstAsync(u => u.OutRefundNumber == callbackRefundResource.OutRefundNumber);
                if (wechatRefund == null) return null;
                wechatRefund.TradeState = callbackRefundResource.RefundStatus; // 交易状态
                wechatRefund.SuccessTime = callbackRefundResource.SuccessTime.Value.DateTime; // 支付完成时间

                await _sysWechatRefundRep.AsUpdateable(wechatRefund).IgnoreColumns(true).ExecuteCommandAsync();
                // 有退款，刷新一下订单状态
                GetPayInfoFromWechat(callbackRefundResource.OutTradeNumber);

            }
            catch (Exception ex)
            {
                "微信退款回调时出错：".LogError(ex);
            }
        }
        else
        {
            callbackModel.EventType.LogInformation();
        }

        return null;
    }

    /// <summary>
    /// 微信支付成功回调(服务商模式) 🔖
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("微信支付成功回调(服务商模式)")]
    public async Task PayPartnerCallBack()
    {
        using var ms = new MemoryStream();
        await App.HttpContext.Request.Body.CopyToAsync(ms);
        var b = ms.ToArray();
        var callbackJson = Encoding.UTF8.GetString(b);

        var callbackModel = _wechatTenpayClient.DeserializeEvent(callbackJson);
        if ("TRANSACTION.SUCCESS".Equals(callbackModel.EventType))
        {
            var callbackResource = _wechatTenpayClient.DecryptEventResource<PartnerTransactionResource>(callbackModel);

            // 修改订单支付状态
            var wechatPay = await _sysWechatPayRep.GetFirstAsync(u => u.OutTradeNumber == callbackResource.OutTradeNumber
                && u.MerchantId == callbackResource.MerchantId);
            if (wechatPay == null) return;
            //wechatPay.OpenId = callbackResource.Payer.OpenId; // 支付者标识
            //wechatPay.MerchantId = callbackResource.MerchantId; // 微信商户号
            //wechatPay.OutTradeNumber = callbackResource.OutTradeNumber; // 商户订单号
            wechatPay.TransactionId = callbackResource.TransactionId; // 支付订单号
            wechatPay.TradeType = callbackResource.TradeType; // 交易类型
            wechatPay.TradeState = callbackResource.TradeState; // 交易状态
            wechatPay.TradeStateDescription = callbackResource.TradeStateDescription; // 交易状态描述
            wechatPay.BankType = callbackResource.BankType; // 付款银行类型
            wechatPay.Total = callbackResource.Amount.Total; // 订单总金额
            wechatPay.PayerTotal = callbackResource.Amount.PayerTotal; // 用户支付金额
            wechatPay.SuccessTime = callbackResource.SuccessTime.DateTime; // 支付完成时间

            await _sysWechatPayRep.AsUpdateable(wechatPay).IgnoreColumns(true).ExecuteCommandAsync();
        }
    }
}