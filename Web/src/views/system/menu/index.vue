<template>
	<div class="sys-menu-container">
		<el-card shadow="hover" class="card-tight">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="租户" v-if="userStore.userInfos.accountType == 999">
					<TenantSelect v-model="state.queryParams.tenantId" />
				</el-form-item>
				<el-form-item label="菜单名称">
					<el-input v-model="state.queryParams.title" placeholder="菜单名称" clearable />
				</el-form-item>
				<el-form-item label="类型">
					<g-sys-dict v-model="state.queryParams.type" code="MenuTypeEnum" render-as="select" placeholder="类型"
						clearable />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysMenu:list'"> 查询
						</el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					</el-button-group>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" icon="ele-Plus" @click="openAddMenu" v-auth="'sysMenu:add'"> 新增
					</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.menuData" v-loading="state.loading" row-key="id"
				:tree-props="{ children: 'children', hasChildren: 'hasChildren' }" border>
				<el-table-column label="菜单名称" header-align="center">
					<template #default="scope">
                        <div class="menu-icon-box">
                            <span class="vertical-center"><SvgIcon :name="scope.row.icon" class="el-icon" /></span>
                            <span class="ml10 vertical-center">{{ scope.row.title }}</span>
                        </div>
					</template>
				</el-table-column>
				<el-table-column label="类型" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<g-sys-dict v-model="scope.row.type" code="MenuTypeEnum" />
					</template>
				</el-table-column>
				<el-table-column prop="path" label="路由路径" header-align="center" show-overflow-tooltip />
				<el-table-column prop="component" label="组件路径" align="center" show-overflow-tooltip />
				<el-table-column prop="permission" label="权限标识" align="center" show-overflow-tooltip />
				<el-table-column prop="orderNo" label="排序" width="70" align="center" show-overflow-tooltip />
				<el-table-column label="状态" width="80" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-switch v-model="scope.row.status" :active-value="1" :inactive-value="2" size="small"
							@change="changeStatus(scope.row)" v-auth="'sysUser:setStatus'" />
					</template>
				</el-table-column>
				<el-table-column label="修改记录" width="100" align="center" show-overflow-tooltip>
					<template #default="scope">
						<ModifyRecord :data="scope.row" />
					</template>
				</el-table-column>
				<el-table-column label="操作" width="210" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Edit" text type="primary" @click="openEditMenu(scope.row)"
							v-auth="'sysMenu:update'"> 编辑 </el-button>
						<el-button icon="ele-Delete" text type="danger" @click="delMenu(scope.row)"
							v-auth="'sysMenu:delete'"> 删除 </el-button>
						<el-button icon="ele-CopyDocument" text type="primary" @click="openCopyMenu(scope.row)"
							v-auth="'sysMenu:add'"> 复制 </el-button>
					</template>
				</el-table-column>
			</el-table>
		</el-card>

		<EditMenu ref="editMenuRef" :title="state.editMenuTitle" :menuData="state.allMenuData" @handleQuery="handleQuery" />
	</div>
</template>

<script lang="ts" setup name="sysMenu">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import EditMenu from '/@/views/system/menu/component/editMenu.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysMenuApi } from '/@/api-services/api';
import { SysMenu, UpdateMenuInput } from '/@/api-services/models';
import { useUserInfo } from "/@/stores/userInfo";
import TenantSelect from '/@/views/system/tenant/component/tenantSelect.vue';

const userStore = useUserInfo();
const editMenuRef = ref<InstanceType<typeof EditMenu>>();
const state = reactive({
	loading: false,
	menuData: [] as Array<SysMenu>,
	allMenuData: [] as Array<SysMenu>,
	queryParams: {
		tenantId: undefined,
		title: undefined,
		type: undefined,
	},
	editMenuTitle: '',
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
	const res = await getAPI(SysMenuApi).apiSysMenuListGet(state.queryParams.title, state.queryParams.type, state.queryParams.tenantId);
	state.menuData = res.data.result ?? [];
	state.loading = false;
};

// 重置操作
const resetQuery = () => {
	state.queryParams.title = undefined;
	state.queryParams.type = undefined;
	handleQuery();
};

// 获取菜单树数据
const getMenuTreeData = async () => {
	const res = await getAPI(SysMenuApi).apiSysMenuListGet(undefined, undefined, state.queryParams.tenantId);
	state.allMenuData = res.data.result ?? [];
	return state.menuData;
}

// 打开新增页面
const openAddMenu = () => {
	getMenuTreeData();
	const data = { type: 2, isHide: false, isKeepAlive: true, isAffix: false, isIframe: false, tenantId: undefined, status: 1, orderNo: 100 };
	data.tenantId = state.queryParams.tenantId;
	state.editMenuTitle = '添加菜单';
	editMenuRef.value?.openDialog(data);
};

// 打开编辑页面
const openEditMenu = (row: any) => {
	getMenuTreeData();
	state.editMenuTitle = '编辑菜单';
	editMenuRef.value?.openDialog(row);
};

// 打开复制页面
const openCopyMenu = (row: any) => {
	state.editMenuTitle = '复制菜单';
	var copyRow = JSON.parse(JSON.stringify(row)) as UpdateMenuInput;
	copyRow.id = 0;
	copyRow.title = '';
	editMenuRef.value?.openDialog(copyRow);
};

// 删除当前行
const delMenu = (row: any) => {
	ElMessageBox.confirm(`确定删除菜单：【${row.title}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysMenuApi).apiSysMenuDeletePost({ id: row.id });
			handleQuery();
			ElMessage.success('删除成功');
		})
		.catch(() => { });
};

// 修改状态
const changeStatus = async (row: any) => {
	await getAPI(SysMenuApi)
		.apiSysMenuSetStatusPost({ id: row.id, status: row.status })
		.then(() => {
			ElMessage.success('菜单状态设置成功');
		})
		.catch(() => {
			row.status = row.status == 1 ? 2 : 1;
		});
};
</script>

<style lang="scss" scoped>
.el-table .cell.el-tooltip {
    display: inline-flex;
    align-items: center;
}
.menu-icon-box {
    display: inline-block;

    .vertical-center { 
        display: inline-flex;
        align-items: center;
        vertical-align: middle;
        line-height: 1em;
        height: 1em;
    }
    //.el-icon { vertical-align: middle; }
}
</style>