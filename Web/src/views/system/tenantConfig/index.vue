<template>
	<div class="sys-config-container">
		<el-card shadow="hover" class="card-tight">
			<TableSearch :search="tb.tableData.search" @search="onSearch" />
		</el-card>
		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<Table ref="tableRef" v-bind="tb.tableData" :getData="getData" :exportChangeData="exportChangeData" @sortHeader="onSortHeader" @selectionChange="tableSelection" border>
				<template #command>
					<el-button type="primary" icon="ele-Plus" @click="openAddConfig" v-auth="'sysTenantConfig:add'"> 新增 </el-button>

					<el-button v-if="state.selectlist.length > 0" type="danger" icon="ele-Delete" @click="bacthDelete" v-auth="'sysTenantConfig:batchDelete'"> 批量删除 </el-button>
				</template>
				<template #sysFlag="scope">
					<g-sys-dict v-model="scope.row.sysFlag" code="YesNoEnum" />
				</template>
				<template #remark="scope">
					<ModifyRecord :data="scope.row" />
				</template>
				<template #action="scope">
					<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditConfig(scope.row)" v-auth="'sysTenantConfig:update'"> 编辑 </el-button>
					<el-button icon="ele-Delete" size="small" text type="danger" @click="delConfig(scope.row)" v-auth="'sysTenantConfig:delete'" :disabled="scope.row.sysFlag === 1"> 删除 </el-button>
				</template>
			</Table>
		</el-card>
		<EditConfig ref="editConfigRef" :title="state.editConfigTitle" :groupList="state.groupList" @updateData="updateData" />
	</div>
</template>

<script lang="ts" setup name="sysTenantConfig">
import { onMounted, reactive, ref, defineAsyncComponent, nextTick } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { auth } from '/@/utils/authFunction';
import { getAPI } from '/@/utils/axios-utils';
import { SysTenantConfigApi } from '/@/api-services/api';
import ModifyRecord from '/@/components/table/modifyRecord.vue';
import EditConfig from '/@/views/system/tenantConfig/component/editConfig.vue';
//import { EmptyObjectType, RefType } from '/@/types/global';

// 引入组件
const Table = defineAsyncComponent(() => import('/@/components/table/index.vue'));
const TableSearch = defineAsyncComponent(() => import('/@/components/table/search.vue'));
const editConfigRef = ref<InstanceType<typeof EditConfig>>();
const tableRef = ref<RefType>();

const state = reactive({
	editConfigTitle: '',
	selectlist: [] as EmptyObjectType[],
	groupList: [] as Array<String>,
});

const tb = reactive<TableDemoState>({
	tableData: {
		// 表头内容（必传，注意格式）
		columns: [
			{ prop: 'name', minWidth: 150, label: '配置名称', headerAlign: 'center', sortable: 'custom', isCheck: true, hideCheck: true },
			{ prop: 'code', minWidth: 150, label: '配置编码', headerAlign: 'center', toolTip: true, sortable: 'custom', isCheck: true },
			{ prop: 'value', minWidth: 150, label: '属性值', headerAlign: 'center', isCheck: true },
			{ prop: 'sysFlag', width: 100, label: '内置参数', align: 'center', isCheck: true },
			{ prop: 'groupCode', width: 120, label: '分组编码', align: 'center', sortable: 'custom', isCheck: true },
			{ prop: 'orderNo', width: 80, label: '排序', align: 'center', sortable: 'custom', isCheck: true },
			{ prop: 'remark', width: 100, label: '修改记录', align: 'center', headerAlign: 'center', showOverflowTooltip: true, isCheck: true },
			{ prop: 'action', width: 140, label: '操作', type: 'action', align: 'center', isCheck: true, fixed: 'right', hideCheck: true },
		],
		// 配置项（必传）
		config: {
			isStripe: true, // 是否显示表格斑马纹
			isBorder: false, // 是否显示表格边框
			isSerialNo: true, // 是否显示表格序号
			isSelection: true, // 是否勾选表格多选
			showSelection: auth('sysTenantConfig:batchDelete'), //是否显示表格多选
			pageSize: 50, // 每页条数
			hideExport: false, //是否隐藏导出按钮
			exportFileName: '系统参数', //导出报表的文件名，不填写取应用名称
		},
		// 搜索表单，动态生成（传空数组时，将不显示搜索，type有3种类型：input,date,select）
		search: [
			{ label: '配置名称', prop: 'name', placeholder: '搜索配置名称', required: false, type: 'input' },
			{ label: '配置编码', prop: 'code', placeholder: '搜索配置编码', required: false, type: 'input' },
			// { label: '创建时间', prop: 'time', placeholder: '请选择', required: false, type: 'date' },
		],
		param: {},
		defaultSort: {
			prop: 'orderNo',
			order: 'ascending',
		},
	},
});
const getData = (param: any) => {
	return getAPI(SysTenantConfigApi)
		.apiSysTenantConfigPagePost(param)
		.then((res) => {
			return res.data;
		});
};
const exportChangeData = (data: Array<EmptyObjectType>) => {
	data.forEach((item) => {
		item.sysFlag = item.sysFlag == 1 ? '是' : '否';
	});
	return data;
};
// 拖动显示列排序回调
const onSortHeader = (data: object[]) => {
	tb.tableData.columns = data;
};
// 搜索点击时表单回调
const onSearch = (data: EmptyObjectType) => {
	tb.tableData.param = Object.assign({}, tb.tableData.param, { ...data });
	nextTick(() => {
		tableRef.value.pageReset();
	});
};

const getGroupList = async () => {
	const res = await getAPI(SysTenantConfigApi).apiSysTenantConfigGroupListGet();
	const groupSearch = {
		label: '分组编码',
		prop: 'groupCode',
		placeholder: '请选择',
		required: false,
		type: 'select',
		options: [],
	} as TableSearchType;
	state.groupList = res.data.result ?? [];
	res.data.result?.forEach((item) => {
		if(item) groupSearch.options?.push({ label: item, value: item });
	});
	let group = tb.tableData.search.filter((item) => {
		return item.prop == 'groupCode';
	});
	if (group.length == 0) {
		tb.tableData.search.push(groupSearch);
	} else {
		group[0] = groupSearch;
	}
};

//表格多选事件
const tableSelection = (data: EmptyObjectType[]) => {
	state.selectlist = data;
};

onMounted(async () => {
	getGroupList();
});

// 更新数据
const updateData = () => {
	tableRef.value.handleList();
	getGroupList();
};

// 打开新增页面
const openAddConfig = () => {
	state.editConfigTitle = '添加配置';
	editConfigRef.value?.openDialog({ sysFlag: 2, orderNo: 100 });
};

// 打开编辑页面
const openEditConfig = (row: any) => {
	state.editConfigTitle = '编辑配置';
	editConfigRef.value?.openDialog(row);
};

// 删除
const delConfig = (row: any) => {
	ElMessageBox.confirm(`确定删除配置：【${row.name}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysTenantConfigApi).apiSysTenantConfigDeletePost({ id: row.id });
			tableRef.value.handleList();
			ElMessage.success('删除成功');
		})
		.catch(() => {});
};

//批量删除
const bacthDelete = () => {
	if (state.selectlist.length == 0) return false;
	ElMessageBox.confirm(`确定批量删除【${state.selectlist[0].name}】等${state.selectlist.length}个配置?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			const ids = state.selectlist.map((item) => {
				return item.id;
			});
			await getAPI(SysTenantConfigApi).apiSysTenantConfigBatchDeletePost(ids);
			tableRef.value.pageReset();
			ElMessage.success('删除成功');
		})
		.catch(() => {});
};
</script>
