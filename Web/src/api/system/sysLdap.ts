import request from '/@/utils/request';
enum Api {
  AddSysLdap = '/api/sysLdap/add',
  DeleteSysLdap = '/api/sysLdap/delete',
  UpdateSysLdap = '/api/sysLdap/update',
  PageSysLdap = '/api/sysLdap/page',
  DetailSysLdap = '/api/sysLdap/detail',
}

// 增加系统域登录信息配置表
export const addSysLdap = (params?: any) =>
	request({
		url: Api.AddSysLdap,
		method: 'post',
		data: params,
	});

// 删除系统域登录信息配置表
export const deleteSysLdap = (params?: any) => 
	request({
			url: Api.DeleteSysLdap,
			method: 'post',
			data: params,
		});

// 编辑系统域登录信息配置表
export const updateSysLdap = (params?: any) => 
	request({
			url: Api.UpdateSysLdap,
			method: 'post',
			data: params,
		});

// 分页查询系统域登录信息配置表
export const pageSysLdap = (params?: any) => 
	request({
			url: Api.PageSysLdap,
			method: 'post',
			data: params,
		});

// 详情系统域登录信息配置表
export const detailSysLdap = (id: any) => 
	request({
			url: Api.DetailSysLdap,
			method: 'get',
			data: { id },
		});


