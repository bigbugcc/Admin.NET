<template>
	<div class="sys-user-reg-way-container">
		<el-card shadow="hover" class="card-tight"v-auth="'sysUserRegWay:list'">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="租户" v-if="userStore.userInfos.accountType == 999">
					<TenantSelect v-model="state.queryParams.tenantId" clearable />
				</el-form-item>
				<el-form-item label="关键字">
					<el-input v-model="state.queryParams.keyword" placeholder="关键字" clearable />
				</el-form-item>
				<el-form-item label="名称">
					<el-input v-model="state.queryParams.name" placeholder="名称" clearable />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery"> 查询
						</el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openAddRegWay" v-auth="'sysUserRegWay:add'"> 新增
					</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.regWayData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="序号" width="55" align="center" fixed />
				<el-table-column prop="name" label="名称" align="center" show-overflow-tooltip />
				<el-table-column prop="orgName" label="机构" align="center" show-overflow-tooltip />
				<el-table-column prop="roleName" label="角色" align="center" show-overflow-tooltip />
				<el-table-column prop="posName" label="职位" align="center" show-overflow-tooltip />
				<el-table-column prop="orderNo" label="排序" width="70" show-overflow-tooltip />
				<el-table-column label="修改记录" width="100" align="center" show-overflow-tooltip>
					<template #default="scope">
						<ModifyRecord :data="scope.row" />
					</template>
				</el-table-column>
				<el-table-column label="操作" width="200" fixed="right" align="center" show-overflow-tooltip v-if="auths(['sysUserRegWay:update', 'sysUserRegWay:delete'])">
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditRegWay(scope.row)" v-auth="'sysUserRegWay:update'"> 编辑 </el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="delRegWay(scope.row)" v-auth="'sysUserRegWay:delete'"> 删除 </el-button>
					</template>
				</el-table-column>
			</el-table>
		</el-card>
		<EditRegWay ref="editRegWayRef" :title="state.editRegWayTitle" @handleQuery="handleQuery" />
	</div>
</template>

<script lang="ts" setup name="sysUserRegWay">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { UserRegWayOutput } from '/@/api-services/models';
import { SysUserRegWayApi} from '/@/api-services/api';
import { auths } from "/@/utils/authFunction";
import { useUserInfo } from "/@/stores/userInfo";
import EditRegWay from './component/editRegWay.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';
import TenantSelect from '/@/views/system/tenant/component/tenantSelect.vue';

const userStore = useUserInfo();
const editRegWayRef = ref<InstanceType<typeof EditRegWay>>();
const state = reactive({
	loading: false,
	regWayData: [] as Array<UserRegWayOutput>,
	queryParams: {
		name: undefined,
		keyword: undefined,
		tenantId: undefined,
	},
	editRegWayTitle: '',
});

onMounted(async () => {
	if (userStore.userInfos.accountType == 999) {
		state.queryParams.tenantId = userStore.userInfos.currentTenantId as any;
	}
	handleQuery();
});

// 查询操作
const handleQuery = async () => {
	state.loading = true;
	state.regWayData = await getAPI(SysUserRegWayApi).apiSysUserRegWayListPost(state.queryParams).then(res => res.data.result ?? []);
	state.loading = false;
};

// 重置操作
const resetQuery = () => {
	state.queryParams.name = undefined;
	state.queryParams.keyword = undefined;
	state.queryParams.tenantId = undefined;
	handleQuery();
};

// 打开新增页面
const openAddRegWay = () => {
	state.editRegWayTitle = '添加注册方案';
	editRegWayRef.value?.openDialog({ tenantId: state.queryParams.tenantId, orderNo: 100 });
};

// 打开编辑页面
const openEditRegWay = (row: any) => {
	state.editRegWayTitle = '编辑注册方案';
	editRegWayRef.value?.openDialog(row);
};

// 删除
const delRegWay = (row: any) => {
	ElMessageBox.confirm(`确定删除方案：【${row.name}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	}).then(async () => {
			await getAPI(SysUserRegWayApi).apiSysUserRegWayDeletePost({ id: row.id });
			handleQuery();
			ElMessage.success('删除成功');
	}).catch(() => { });
};
</script>
