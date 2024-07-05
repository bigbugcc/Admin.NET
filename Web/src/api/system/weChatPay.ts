import request from '/@/utils/request';
enum Api {
	PagePayList = '/api/sysWechatPay/page',
	CreatePay = '/api/sysWechatPay/payTransactionNative',
	GetRefundListByID = '/api/sysWechatPay/listRefund',
	RefundDomestic = '/api/sysWechatPay/refundDomestic'
}
export const pagePayList = (params?: any) =>
	request({
		url: Api.PagePayList,
		method: 'post',
		data: params,
	});
export const createPay = (params?: any) =>
	request({
		url: Api.CreatePay,
		method: 'post',
		data: params,
	});
export const getRefundListByID = (params?: any) =>
	request({
		url: Api.GetRefundListByID,
		method: 'post',
		data: params,
	});
export const refundDomestic = (params?: any) =>
	request({
		url: Api.RefundDomestic,
		method: 'post',
		data: params,
	});