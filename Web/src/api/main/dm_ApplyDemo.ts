import request from '/@/utils/request';
import { downloadByData } from '/@/utils/download';
enum Api {
	AddDm_ApplyDemo = '/api/dm_ApplyDemo/add',
	DeleteDm_ApplyDemo = '/api/dm_ApplyDemo/delete',
	UpdateDm_ApplyDemo = '/api/dm_ApplyDemo/update',
	PageDm_ApplyDemo = '/api/dm_ApplyDemo/page',
	DetailDm_ApplyDemo = '/api/dm_ApplyDemo/detail',
}

// 增加申请示例
export const addDm_ApplyDemo = (params?: any) =>
	request({
		url: Api.AddDm_ApplyDemo,
		method: 'post',
		data: params,
	});

// 删除申请示例
export const deleteDm_ApplyDemo = (params?: any) =>
	request({
		url: Api.DeleteDm_ApplyDemo,
		method: 'post',
		data: params,
	});

// 编辑申请示例
export const updateDm_ApplyDemo = (params?: any) =>
	request({
		url: Api.UpdateDm_ApplyDemo,
		method: 'post',
		data: params,
	});

// 分页查询申请示例
export const pageDm_ApplyDemo = (params?: any) =>
	request({
		url: Api.PageDm_ApplyDemo,
		method: 'post',
		data: params,
	});

// 详情申请示例
export const detailDm_ApplyDemo = (id: any) =>
	request({
		url: Api.DetailDm_ApplyDemo,
		method: 'get',
		data: { id },
	});

//导出数据
export const exportDm_ApplyDemo = (params?: any) =>
	request({
		url: '/api/dm_ApplyDemo/export',
		method: 'post',
		data: params,
		responseType: 'arraybuffer',
	}).then((res) => {
		downloadByData(res.data, 'export.xlsx', 'application/octet-stream');
	});
