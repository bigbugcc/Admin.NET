<template>
	<div class="sysLdap-container">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="关键字">
					<el-input v-model="state.queryParams.searchKey" clearable="" placeholder="请输入模糊查询关键字" />
				</el-form-item>
				<el-form-item label="主机">
					<el-input v-model="state.queryParams.host" clearable="" placeholder="请输入主机" />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysLdap:page'"> 查询 </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openAddSysLdap" v-auth="'sysLdap:add'"> 新增 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.tableData" style="width: 100%" v-loading="state.loading" border="">
				<el-table-column type="index" label="序号" width="55" align="center" />
				<el-table-column prop="host" label="主机" min-width="150" show-overflow-tooltip="" />
				<el-table-column prop="port" label="端口" show-overflow-tooltip="" />
				<el-table-column prop="baseDn" label="用户搜索基准" show-overflow-tooltip="" />
				<el-table-column prop="bindDn" label="绑定DN" show-overflow-tooltip="" />
				<el-table-column prop="bindPass" label="绑定密码" min-width="200" show-overflow-tooltip="" />
				<el-table-column prop="authFilter" label="用户过滤规则" show-overflow-tooltip="" />
				<el-table-column prop="version" label="Ldap版本" show-overflow-tooltip="" />
				<el-table-column prop="status" label="状态" width="80" align="center" show-overflow-tooltip="">
					<template #default="scope">
						<el-tag v-if="scope.row.status"> 是 </el-tag>
						<el-tag type="danger" v-else> 否 </el-tag>
					</template>
				</el-table-column>
				<el-table-column label="修改记录" width="100" align="center" show-overflow-tooltip>
					<template #default="scope">
						<ModifyRecord :data="scope.row" />
					</template>
				</el-table-column>
				<el-table-column label="操作" width="300" align="center" fixed="right" show-overflow-tooltip="" v-if="auth('sysLdap:update') || auth('sysLdap:delete') || auth('sysLdap:syncUser') || auth('sysLdap:syncOrg')">
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text="" type="primary" @click="openEditSysLdap(scope.row)" v-auth="'sysLdap:update'"> 编辑 </el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="delSysLdap(scope.row)" v-auth="'sysLdap:delete'"> 删除 </el-button>
						<el-button icon="ele-Refresh" size="small" text type="primary" @click="syncDomainUser(scope.row)" v-auth="'sysLdap:syncUser'"> 同步域账户 </el-button>
						<el-button icon="ele-Refresh" size="small" text type="primary" @click="syncDomainOrg(scope.row)" v-auth="'sysLdap:syncOrg'"> 同步域组织 </el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				v-model:currentPage="state.tableParams.page"
				v-model:page-size="state.tableParams.pageSize"
				:total="state.tableParams.total"
				:page-sizes="[10, 20, 50, 100, 200, 500]"
				small=""
				background=""
				@size-change="handleSizeChange"
				@current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper"
			/>
		</el-card>

		<EditLdap ref="editLdapRef" :title="state.dialogTitle" @handleQuery="handleQuery" />
	</div>
</template>

<script lang="ts" setup="" name="sysLdap">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { auth } from '/@/utils/authFunction';
import ModifyRecord from '/@/components/table/modifyRecord.vue';
import EditLdap from './component/editLdap.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysLdapApi } from '/@/api-services/api';

const editLdapRef = ref<InstanceType<typeof EditLdap>>();
const state = reactive({
	loading: false,
	tableData: [] as any,
	queryParams: {
		searchKey: undefined,
		host: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 20,
		total: 0 as any,
	},
	dialogTitle: '',
});

onMounted(async () => {
	handleQuery();
});

// 查询操作
const handleQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	var res = await getAPI(SysLdapApi).apiSysLdapPagePost(params);
	state.tableData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// 重置操作
const resetQuery = () => {
	state.queryParams.searchKey = undefined;
	state.queryParams.host = undefined;
	handleQuery();
};

// 打开新增页面
const openAddSysLdap = () => {
	state.dialogTitle = '添加系统域登录信息配置';
	editLdapRef.value?.openDialog({});
};

// 打开编辑页面
const openEditSysLdap = (row: any) => {
	state.dialogTitle = '编辑系统域登录信息配置';
	editLdapRef.value?.openDialog(row);
};

// 删除
const delSysLdap = (row: any) => {
	ElMessageBox.confirm(`确定要删除域登录信息配置：【${row.host}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysLdapApi).apiSysLdapDeletePost({ id: row.id });
			handleQuery();
			ElMessage.success('删除成功');
		})
		.catch(() => {});
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

// 同步域账户
const syncDomainUser = (row: any) => {
	ElMessageBox.confirm(`确定要同步域账户吗?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysLdapApi).apiSysLdapSyncUserPost({ id: row.id });
			handleQuery();
			ElMessage.success('删除成功');
		})
		.catch(() => {});
};

// 同步域组织
const syncDomainOrg = (row: any) => {
	ElMessageBox.confirm(`确定要同步域组织架构吗?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysLdapApi).apiSysLdapSyncOrgPost({ id: row.id });
			handleQuery();
			ElMessage.success('删除成功');
		})
		.catch(() => {});
};
</script>
