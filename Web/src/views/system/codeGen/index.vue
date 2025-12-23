<template>
	<div class="sys-codeGen-container">
		<el-card shadow="hover" :body-style="{ padding: 5 }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="业务名">
					<el-input placeholder="业务名" clearable @keyup.enter="handleQuery" v-model="state.queryParams.busName" />
				</el-form-item>
				<el-form-item label="数据库表名">
					<el-input placeholder="数据库表名" clearable @keyup.enter="handleQuery" v-model="state.queryParams.tableName" />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysMenu:list'"> 查询 </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openAddDialog"> 增加 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.tableData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="序号" width="55" align="center" />
				<el-table-column prop="configId" label="库定位器" align="center" show-overflow-tooltip />
				<el-table-column prop="dbNickName" label="库名" align="center" show-overflow-tooltip />
				<el-table-column prop="tableName" label="表名称" align="center" show-overflow-tooltip />
				<el-table-column prop="busName" label="业务名" align="center" show-overflow-tooltip />
				<el-table-column prop="nameSpace" label="命名空间" align="center" show-overflow-tooltip />
				<el-table-column prop="authorName" label="作者姓名" align="center" show-overflow-tooltip />
				<el-table-column prop="generateType" label="生成方式" align="center" show-overflow-tooltip>
					<template #default="scope">
            <g-sys-dict v-model="scope.row.generateType" code="code_gen_create_type" />
					</template>
				</el-table-column>
				<el-table-column label="操作" width="280" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Delete" size="small" text type="danger" title="删除" @click="deleConfig(scope.row)" />
						<el-button icon="ele-Edit" size="small" text type="primary" title="编辑并完全刷新字段列表" @click="openEditDialog(scope.row)" />
						<el-button icon="ele-CopyDocument" size="small" text type="primary" title="复制" @click="openCopyDialog(scope.row)" />
						<el-button icon="ele-View" size="small" text type="primary" title="预览" @click="handlePreview(scope.row)" />
						<el-button icon="ele-Setting" size="small" text type="primary" title="配置" @click="openConfigDialog(scope.row)" />
						<el-button icon="ele-Refresh" size="small" text type="primary" title="同步字段并保留历史字段作用类型" @click="syncCodeGen(scope.row)" />
						<el-button icon="ele-Position" size="small" text type="primary" @click="handleGenerate(scope.row)">生成</el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				v-model:currentPage="state.tableParams.page"
				v-model:page-size="state.tableParams.pageSize"
				:total="state.tableParams.total"
				:page-sizes="[10, 20, 50, 100]"
				size="small"
				background
				@size-change="handleSizeChange"
				@current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper"
			/>
		</el-card>

		<EditCodeGenDialog :title="state.editTitle" ref="EditCodeGenRef" @handleQuery="handleQuery" :application-namespaces="state.applicationNamespaces" />
		<CodeConfigDialog ref="CodeConfigRef" @handleQuery="handleQuery" />
		<PreviewDialog :title="state.editTitle" ref="PreviewRef" />
	</div>
</template>

<script lang="ts" setup name="sysCodeGen">
import { onMounted, reactive, ref, defineAsyncComponent } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import EditCodeGenDialog from './component/editCodeGenDialog.vue';
import CodeConfigDialog from './component/genConfigDialog.vue';
import { downloadByUrl } from '/@/utils/download';
import { getAPI } from '/@/utils/axios-utils';
import { SysCodeGenApi } from '/@/api-services/api';
import { SysCodeGen } from '/@/api-services/models';

const PreviewDialog = defineAsyncComponent(() => import('./component/previewDialog.vue'));

const EditCodeGenRef = ref<InstanceType<typeof EditCodeGenDialog>>();
const CodeConfigRef = ref<InstanceType<typeof CodeConfigDialog>>();
const PreviewRef = ref<InstanceType<typeof PreviewDialog>>();
const state = reactive({
	loading: false,
	loading1: false,
	dbData: [] as any,
	configId: '',
	tableData: [] as Array<SysCodeGen>,
	tableName: '',
	queryParams: {
		name: undefined,
		code: undefined,
		tableName: undefined,
		busName: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	editTitle: '',
	applicationNamespaces: [] as Array<string>,
});

onMounted(async () => {
	handleQuery();
	let res = await getAPI(SysCodeGenApi).apiSysCodeGenApplicationNamespacesGet();
	state.applicationNamespaces = res.data.result as Array<string>;
});

const openConfigDialog = (row: any) => {
	CodeConfigRef.value?.openDialog(row);
};

// 表查询操作
const handleQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	let res = await getAPI(SysCodeGenApi).apiSysCodeGenPagePost(params);
	state.tableData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// 重置操作
const resetQuery = () => {
	state.queryParams.busName = undefined;
	state.queryParams.tableName = undefined;
	handleQuery();
};

// 改变页面容量
const handleSizeChange = (val: number) => {
	state.tableParams.pageSize = val;
	handleQuery();
};

// 改变页码序号
const handleCurrentChange = (val: number) => {
	state.tableParams.page = val;
	handleQuery();
};

// 打开表增加页面
const openAddDialog = () => {
	state.editTitle = '增加';
	EditCodeGenRef.value?.openDialog({
		authorName: 'Admin.NET',
		generateType: '200',
		printType: 'off',
		menuIcon: 'ele-Menu',
		pagePath: 'main',
		nameSpace: state.applicationNamespaces[0],
		generateMenu: true,
	});
};

// 打开表编辑页面
const openEditDialog = (row: any) => {
	state.editTitle = '编辑';
	EditCodeGenRef.value?.openDialog(row);
};

// 打开复制页面
const openCopyDialog = (row: any) => {
	state.editTitle = '复制';
	var copyRow = JSON.parse(JSON.stringify(row));
	copyRow.id = 0;
	copyRow.busName = '';
	copyRow.tableName = '';
  copyRow.tableUniqueList = undefined;
	EditCodeGenRef.value?.openDialog(copyRow);
};

// 删除表
const deleConfig = (row: any) => {
	ElMessageBox.confirm(`确定删除吗?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	}).then(async () => {
			await getAPI(SysCodeGenApi).apiSysCodeGenDeletePost([{ id: row.id }]);
			handleQuery();
			ElMessage.success('操作成功');
	}).catch(() => {});
};

// 同步生成
const syncCodeGen = async (row: any) => {
  ElMessageBox.confirm(`确定要同步吗? （保留历史字段作用类型）`, '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning',
  }).then(async () => {
	await getAPI(SysCodeGenApi).apiSysCodeFieldGenPost(row);
    handleQuery();
    ElMessage.success('同步字段列表成功');
  }).catch(() => {});
}

// 开始生成代码
const handleGenerate = (row: any) => {
	ElMessageBox.confirm(`确定要生成吗?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			var res = await getAPI(SysCodeGenApi).apiSysCodeGenRunLocalPost(row);
			if (res.data.result != null && res.data.result.url != null) downloadByUrl({ url: res.data.result.url });
			handleQuery();
			ElMessage.success('操作成功');
		})
		.catch(() => {});
};

// 预览代码
const handlePreview = (row: any) => {
	state.editTitle = '预览代码';
	PreviewRef.value?.openDialog(row);
};
</script>
