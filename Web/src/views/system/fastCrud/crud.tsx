import { ref } from 'vue';
import { ElMessage } from 'element-plus';
import { dict, compute, EditReq, DelReq, AddReq } from '@fast-crud/fast-crud';

import { getAPI } from '/@/utils/axios-utils';
import { SysNoticeApi } from '/@/api-services/api';
import { PageFileInput } from '/@/api-services/models';

export default function ({ expose }) {
	// 分页查询
	const pageRequest = async (query: any) => {
		const params = {
			page: query.currentPage,
			pageSize: query.pageSize,
			field: query.field,
			order: query.order,
			descStr: 'desc',
		} as PageFileInput;
		const result = await getAPI(SysNoticeApi).apiSysNoticePagePost(params);
		return result;
	};
	// 编辑
	const editRequest = async ({ form, row }: EditReq) => {
		if (form.id == null) {
			form.id = row.id;
		}
		return await getAPI(SysNoticeApi)
			.apiSysNoticeUpdatePost(form)
			.then((rsp: any) => {
				if (rsp.data.code == 200) {
					ElMessage.success('修改成功！');
				} else {
					ElMessage.error('修改失败：' + rsp.data.message);
				}
			});
	};
	// 删除
	const delRequest = async ({ row }: DelReq) => {
		return await getAPI(SysNoticeApi).apiSysNoticeDeletePost(row);
	};
	// 增加
	const addRequest = async ({ form }: AddReq) => {
		return await getAPI(SysNoticeApi).apiSysNoticeAddPost(form);
	};
	// 选择
	const selectedIds = ref([]);
	const onSelectionChange = (changed: any) => {
		selectedIds.value = changed;
	};
	return {
		selectedIds,
		crudOptions: {
			container: {
				is: 'fs-layout-card',
			},
			form: {
				wrapper: {
					// is: 'el-drawer',
					// width: '80%',
					draggable: false,
					closeOnEsc: false,
					maskClosable: false,
				},
			},
			search: {
				show: true,
			},
			actionbar: {},
			toolbar: {
				show: true,
				buttons: {
					search: { show: true },
					refresh: { show: true },
					compact: { show: true },
					export: { show: true },
					columns: { show: true },
				},
			},
			table: {
				scrollX: 725,
				bordered: false,
				rowKey: (row: any) => row.id,
				checkedRowKeys: selectedIds,
				'onUpdate:checkedRowKeys': onSelectionChange,
			},
			pagination: {
				show: true,
			},
			request: {
				pageRequest,
				addRequest,
				editRequest,
				delRequest,
			},
			rowHandle: {
				fixed: 'right',
				align: 'center',
				width: 200,
				buttons: {
					view: { show: true },
					edit: { show: true },
				},
			},
			columns: {
				_checked: {
					title: '选择',
					form: { show: false },
					column: {
						type: 'selection',
						align: 'center',
						width: '55px',
						columnSetDisabled: true,
						disabled(row: any) {
							return row.account === 'gvanet';
						},
					},
				},
				type: {
					title: '类型',
					type: 'dict-select',
					search: { show: true, col: { span: 6 } },
					column: {
						align: 'center',
						width: '120px',
					},
					dict: dict({
						value: 'id',
						label: 'text',
						data: [
							{ id: '1', text: '通知' },
							{ id: '2', text: '公告' },
						],
					}),
					form: {
						col: { span: 24 },
						rule: [{ required: true, message: '请输入类型' }],
					},
				},
				title: {
					title: '标题',
					type: 'text',
					search: { show: true, col: { span: 6 } },
					column: {
						align: 'center',
						width: 'auto',
					},
					form: {
						col: { span: 24 },
						rule: [{ required: true, message: '请输入标题' }],
					},
				},
				content: {
					title: '内容',
					type: 'editor-wang5',
					search: { show: false, col: { span: 6 } },
					column: {
						show: false,
					},
					form: {
						col: { span: 24 },
						rule: [{ required: true, message: '请输入内容' }],
						component: {
							disabled: compute(({ form }) => {
								return form.disabled;
							}),
							id: '1', // 当同一个页面有多个editor时，需要配置不同的id
							config: {},
							uploader: {
								type: 'form',
								buildUrl(res: any) {
									return res.url;
								},
							},
						},
					},
				},
			},
		},
	};
}
