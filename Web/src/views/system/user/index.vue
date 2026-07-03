<template>
	<div class="sys-user-container">
		<el-splitter class="smallbar-el-splitter">
			<el-splitter-panel size="20%" :min="200">
				<OrgTree ref="orgTreeRef" @node-click="nodeClick" />
			</el-splitter-panel>
			<el-splitter-panel :min="200" style="overflow: auto; display: flex; flex-direction: column;">
                <el-card shadow="hover" class="card-tight">
					<el-form :model="state.queryParams" ref="queryForm" :inline="true">
						<el-form-item label="账号">
							<el-input v-model="state.queryParams.account" placeholder="账号" clearable />
						</el-form-item>
						<el-form-item label="姓名">
							<el-input v-model="state.queryParams.realName" placeholder="姓名" clearable />
						</el-form-item>
						<el-form-item label="职位名称">
							<el-input v-model="state.queryParams.posName" placeholder="职位名称" clearable />
						</el-form-item>
						<el-form-item label="手机号码">
							<el-input v-model="state.queryParams.phone" placeholder="手机号码" clearable />
						</el-form-item>
						<el-form-item>
							<el-button-group>
								<el-button type="primary" icon="ele-Search" @click="handleQuery"
									v-auth="'sysUser:page'"> 查询 </el-button>
								<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
							</el-button-group>
						</el-form-item>
						<el-form-item>
							<el-button type="primary" icon="ele-Plus" @click="openAddUser" v-auth="'sysUser:add'"> 新增
							</el-button>
						</el-form-item>
					</el-form>
				</el-card>

				<el-card class="full-table" shadow="hover" style="margin-top: 5px">
					<el-table :data="state.userData" style="width: 100%" v-loading="state.loading" border>
						<el-table-column type="index" label="序号" width="55" align="center" fixed />
						<el-table-column label="头像" width="80" align="center" show-overflow-tooltip>
							<template #default="scope">
								<el-avatar :src="scope.row.avatar" size="small">{{ scope.row.nickName?.slice(0, 1) ??
									scope.row.realName?.slice(0, 1) }} </el-avatar>
							</template>
						</el-table-column>
						<el-table-column prop="account" label="账号" width="120" align="center" show-overflow-tooltip />
						<!-- <el-table-column prop="nickName" label="昵称" width="120" align="center" show-overflow-tooltip /> -->
						<el-table-column prop="realName" label="姓名" width="120" align="center" show-overflow-tooltip />
						<el-table-column prop="phone" label="手机号码" width="120" align="center" show-overflow-tooltip />
						<!-- <el-table-column label="出生日期" width="100" align="center" show-overflow-tooltip>
							<template #default="scope">
								{{ formatDate(new Date(scope.row.birthday), 'YYYY-mm-dd') }}
							</template>
						</el-table-column>
						<el-table-column label="性别" width="70" align="center" show-overflow-tooltip>
							<template #default="scope">
								<el-tag v-if="scope.row.sex === 1" type="success">男</el-tag>
								<el-tag v-else-if="scope.row.sex === 2" type="danger">女</el-tag>
								<el-tag v-else-if="scope.row.sex === 0" type="info">未知</el-tag>
								<el-tag v-else-if="scope.row.sex === 9" type="info">未说明</el-tag>
							</template>
						</el-table-column> -->
						<el-table-column label="账号类型" width="110" align="center" show-overflow-tooltip>
							<template #default="scope">
								<g-sys-dict v-model="scope.row.accountType" code="AccountTypeEnum" />
							</template>
						</el-table-column>
						<el-table-column prop="roleName" label="角色集合" min-width="150" align="center"
							show-overflow-tooltip />
						<el-table-column prop="orgName" label="所属机构" min-width="120" align="center"
							show-overflow-tooltip />
						<el-table-column prop="posName" label="职位名称" min-width="120" align="center"
							show-overflow-tooltip />
						<el-table-column label="状态" width="70" align="center" show-overflow-tooltip>
							<template #default="scope">
								<TagSwitch v-model="scope.row.status" :active-value="1" :inactive-value="2" code="StatusEnum" @change="changeStatus(scope.row)" v-auth="'sysUser:setStatus'" />
							</template>
						</el-table-column>
						<el-table-column prop="orderNo" label="排序" width="70" align="center" show-overflow-tooltip />
						<el-table-column label="修改记录" width="100" align="center" show-overflow-tooltip>
							<template #default="scope">
								<ModifyRecord :data="scope.row" />
							</template>
						</el-table-column>
						<el-table-column label="操作" width="120" align="center" fixed="right" show-overflow-tooltip>
							<template #default="scope">
								<el-tooltip content="编辑" placement="top">
									<el-button icon="ele-Edit" text type="primary" v-auth="'sysUser:update'"
										@click="openEditUser(scope.row)"> </el-button>
								</el-tooltip>
								<el-tooltip content="删除" placement="top">
									<el-button icon="ele-Delete" text type="danger" v-auth="'sysUser:delete'"
										@click="delUser(scope.row)"> </el-button>
								</el-tooltip>
								<el-dropdown>
									<el-button icon="ele-MoreFilled" size="small" text type="primary"
										style="padding-left: 12px" />
									<template #dropdown>
										<el-dropdown-menu>
                                            <span v-auth="'sysUser:add'">
                                                <el-dropdown-item icon="ele-CopyDocument" text type="primary"
                                                    @click="openCopyMenu(scope.row)"
                                                >复制</el-dropdown-item>
                                            </span>
											<span v-auth="'sysUser:resetPwd'">
                                                <el-dropdown-item icon="ele-RefreshLeft" text type="danger"
												    @click="resetUserPwd(scope.row)"
                                                 >重置密码</el-dropdown-item>
                                            </span>
											<span v-auth="'sysUser:unlockLogin'">
                                                <el-dropdown-item icon="ele-Unlock" text type="primary"
												    @click="unlockLogin(scope.row)"
                                                >解除锁定</el-dropdown-item>
                                            </span>
											
										</el-dropdown-menu>
									</template>
								</el-dropdown>
							</template>
						</el-table-column>
					</el-table>
					<el-pagination size="small" background 
                        v-model:currentPage="state.tableParams.page" 
                        v-model:page-size="state.tableParams.pageSize"
                        :total="state.tableParams.total"
                        :page-sizes="[10, 20, 50, 100]"
                        @size-change="handleSizeChange"
                        @current-change="handleCurrentChange" 
                        layout="total, sizes, prev, pager, next, jumper" 
                    />
				</el-card>
			</el-splitter-panel>
		</el-splitter>

		<EditUser ref="editUserRef" :title="state.editUserTitle" :orgTreeData="state.orgTreeData" @handleQuery="handleQuery" />
	</div>
</template>

<script lang="ts" setup name="sysUser">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import OrgTree from '/@/views/system/org/component/orgTree.vue';
import EditUser from '/@/views/system/user/component/editUser.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';
//import CallBar from '/@/components/callTel/callBar.vue';
//import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { getAPI } from '/@/utils/axios-utils';
import { SysUserApi, SysOrgApi } from '/@/api-services/api';
import { SysUser, UpdateUserInput, OrgTreeOutput } from '/@/api-services/models';
import TagSwitch from '/@/components/TagSwitch/index.vue';

const orgTreeRef = ref<InstanceType<typeof OrgTree>>();
const editUserRef = ref<InstanceType<typeof EditUser>>();
const state = reactive({
	loading: false,
	tenantList: [] as Array<any>,
	userData: [] as Array<SysUser>,
	orgTreeData: [] as Array<OrgTreeOutput>,
	queryParams: {
		orgId: -1,
		account: undefined,
		realName: undefined,
		phone: undefined,
		posName: undefined
	},
	tenantId: undefined,
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	editUserTitle: '',
});

onMounted(async () => {
	await loadOrgData();
	await handleQuery();
});

// 查询机构数据
const loadOrgData = async () => {
	state.loading = true;
	let res = await getAPI(SysOrgApi).apiSysOrgTreeGet(0);
	state.orgTreeData = res.data.result ?? [];
	state.loading = false;
};

// 查询操作
const handleQuery = async () => {
	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	let res = await getAPI(SysUserApi).apiSysUserPagePost(params);
	state.userData = res.data.result?.items ?? [];
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// 重置操作
const resetQuery = async () => {
	state.queryParams.orgId = -1;
	state.queryParams.account = undefined;
	state.queryParams.realName = undefined;
	state.queryParams.phone = undefined;
	state.queryParams.posName = undefined;
	await handleQuery();
};

// 打开新增页面
const openAddUser = () => {
	state.editUserTitle = '添加账号';
	editUserRef.value?.openDialog({ id: undefined, birthday: '2000-01-01', sex: 1, tenantId: state.tenantId, orderNo: 100, cardType: 0, cultureLevel: 5 });
};

// 打开编辑页面
const openEditUser = (row: any) => {
	state.editUserTitle = '编辑账号';
	editUserRef.value?.openDialog(row);
};

// 打开复制页面
const openCopyMenu = (row: any) => {
	state.editUserTitle = '复制账号';
	var copyRow = JSON.parse(JSON.stringify(row)) as UpdateUserInput;
	copyRow.id = 0;
	copyRow.account = '';
	editUserRef.value?.openDialog(copyRow);
};

// 删除
const delUser = (row: any) => {
	ElMessageBox.confirm(`确定删除账号：【${row.account}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysUserApi).apiSysUserDeletePost({ id: row.id });
			await handleQuery();
			ElMessage.success('删除成功');
		})
		.catch(() => { });
};

// 改变页面容量
const handleSizeChange = (val: number) => {
	state.tableParams.pageSize = val;
	handleQuery();
};

// 改变页码序号
const handleCurrentChange = async (val: number) => {
	state.tableParams.page = val;
	await handleQuery();
};

// 修改状态
const changeStatus = async (row: any) => {
	await getAPI(SysUserApi)
		.apiSysUserSetStatusPost({ id: row.id, status: row.status })
		.then(() => {
			ElMessage.success('账号状态设置成功');
		})
		.catch(() => {
			row.status = row.status == 1 ? 2 : 1;
		});
};

// 重置密码
const resetUserPwd = async (row: any) => {
	ElMessageBox.confirm(`确定重置密码：【${row.account}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysUserApi)
				.apiSysUserResetPwdPost({ id: row.id })
				.then((res) => {
					ElMessage.success(`密码重置成功为：${res.data.result}`);
				});
		})
		.catch(() => { });
};

// 解除登录锁定
const unlockLogin = async (row: any) => {
	ElMessageBox.confirm(`确定解除登录锁定：【${row.account}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysUserApi)
				.apiSysUserUnlockLoginPost({ id: row.id })
				.then(() => {
					ElMessage.success('解除登录锁定成功');
				});
		})
		.catch(() => { });
};

// 树组件点击
const nodeClick = async (node: any) => {
	state.queryParams.orgId = node.id;
	state.queryParams.account = undefined;
	state.queryParams.realName = undefined;
	state.queryParams.phone = undefined;
	state.tenantId = node.tenantId;
	await handleQuery();
};
</script>

<style scoped lang="scss">
.el-form--inline .el-form-item,.el-form-item:last-of-type {
    margin: 5px 15px;
}
</style>