<template>
	<div class="sys-pos-container">
		<el-card shadow="hover" class="card-tight">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="租户" v-if="userStore.userInfos.accountType == 999">
					<TenantSelect v-model="state.queryParams.tenantId" clearable />
				</el-form-item>
				<el-form-item label="职位名称">
					<el-input v-model="state.queryParams.name" placeholder="职位名称" clearable />
				</el-form-item>
				<el-form-item label="职位编码">
					<el-input v-model="state.queryParams.code" placeholder="职位编码" clearable />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysPos:list'"> 查询 </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openAddPos" v-auth="'sysPos:add'"> 新增 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.posData" style="width: 100%" v-loading="state.loading" border>
				<el-table-column type="index" label="序号" width="55" align="center" />
				<el-table-column prop="name" label="职位名称" align="center" show-overflow-tooltip />
				<el-table-column prop="code" label="职位编码" align="center" show-overflow-tooltip />
				<el-table-column prop="userList" label="在职人数" width="100" align="center" show-overflow-tooltip >
					<template #default="scope">{{ scope.row.userList?.length}}</template>
				</el-table-column>
				<el-table-column prop="userList" label="人员明细" width="120" align="center" show-overflow-tooltip >
					<template #default="scope">
						<el-popover placement="bottom" width="280" trigger="hover" :show-after="600" v-if="scope.row.userList?.length">
							<template #reference>
								<el-text type="primary" class="cursor-default">
									<el-icon><ele-InfoFilled /></el-icon>人员明细
								</el-text>
							</template>
							<el-table :data="scope.row.userList" stripe border>
								<el-table-column type="index" label="序号" width="55" align="center" />
								<el-table-column prop="account" label="账号" />
								<el-table-column prop="realName" label="姓名" />
							</el-table>
						</el-popover>
					</template>
				</el-table-column>
				<el-table-column prop="orderNo" label="排序" width="70" align="center" show-overflow-tooltip />
				<el-table-column label="状态" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<g-sys-dict v-model="scope.row.status" code="StatusEnum" />
					</template>
				</el-table-column>
				<el-table-column label="修改记录" width="100" align="center" show-overflow-tooltip>
					<template #default="scope">
						<ModifyRecord :data="scope.row" />
					</template>
				</el-table-column>
				<el-table-column label="操作" width="210" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Edit" text type="primary" @click="openEditPos(scope.row)" v-auth="'sysPos:update'"> 编辑 </el-button>
						<el-button icon="ele-Delete" text type="danger" @click="delPos(scope.row)" v-auth="'sysPos:delete'"> 删除 </el-button>
						<el-button icon="ele-CopyDocument" text type="primary" @click="openCopyMenu(scope.row)" v-auth="'sysPos:add'"> 复制 </el-button>
					</template>
				</el-table-column>
			</el-table>
		</el-card>

		<EditPos ref="editPosRef" :title="state.editPosTitle" @handleQuery="handleQuery" />
	</div>
</template>

<script lang="ts" setup name="sysPos">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import EditPos from '/@/views/system/pos/component/editPos.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysPosApi } from '/@/api-services/api';
import { SysPos, UpdatePosInput } from '/@/api-services/models';
import { useUserInfo } from "/@/stores/userInfo";

import TenantSelect from '/@/views/system/tenant/component/tenantSelect.vue';

const userStore = useUserInfo();
const editPosRef = ref<InstanceType<typeof EditPos>>();
const state = reactive({
	loading: false,
	posData: [] as Array<SysPos>,
	queryParams: {
		tenantId: undefined,
		name: undefined,
		code: undefined,
	},
	editPosTitle: '',
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
	var res = await getAPI(SysPosApi).apiSysPosListGet(state.queryParams.name, state.queryParams.code, state.queryParams.tenantId);
	state.posData = res.data.result ?? [];
	state.loading = false;
};

// 重置操作
const resetQuery = () => {
	state.queryParams.name = undefined;
	state.queryParams.code = undefined;
	handleQuery();
};

// 打开新增页面
const openAddPos = () => {
	state.editPosTitle = '添加职位';
	editPosRef.value?.openDialog({ status: 1, tenantId: state.queryParams.tenantId, orderNo: 100 });
};

// 打开编辑页面
const openEditPos = (row: any) => {
	state.editPosTitle = '编辑职位';
	editPosRef.value?.openDialog(row);
};

// 打开复制页面
const openCopyMenu = (row: any) => {
	state.editPosTitle = '复制职位';
	var copyRow = JSON.parse(JSON.stringify(row)) as UpdatePosInput;
	copyRow.id = 0;
	copyRow.name = '';
	editPosRef.value?.openDialog(copyRow);
};

// 删除
const delPos = (row: any) => {
	ElMessageBox.confirm(`确定删除职位：【${row.name}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
			.then(async () => {
				await getAPI(SysPosApi).apiSysPosDeletePost({ id: row.id });
				handleQuery();
				ElMessage.success('删除成功');
			})
			.catch(() => {});
};
</script>
