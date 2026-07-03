<template>
	<div class="sys-database-container">
		<el-card shadow="hover" class="card-tight">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true" v-loading="state.loading">
				<el-form-item label="库名">
					<el-select v-model="state.configId" placeholder="库名" filterable @change="handleQueryTable">
						<el-option v-for="item in state.dbData" :key="item.configId" :label="item.dbNickName" :value="item.configId" />
					</el-select>
				</el-form-item>
				<el-form-item label="表名">
					<el-select v-model="state.tableName" placeholder="表名" filterable clearable @change="handleQueryColumn">
                        <template #label="{ label, value }">
                            <div class="flex flex-items-center">
                                <span>{{ value }}</span>
                                <span class="desc">{{ label }}</span>
                            </div>
                        </template>
						<el-option v-for="item in state.tableData" :key="item.name" :data="item" :label="item.description" :value="item.name">
                            <div class="flex flex-items-center">
                                <span style="flex: 1">{{ item.name }}</span>
                                <el-tag type="info" size="small">{{ item.description }}</el-tag>
                            </div>
                        </el-option>
					</el-select>
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button icon="ele-Plus" type="primary" @click="openAddTable"> 增加表 </el-button>
						<el-button icon="ele-Edit" @click="openEditTable"> 编辑表 </el-button>
						<el-button icon="ele-Delete" type="danger" @click="delTable" disabled> 删除表 </el-button>
						<el-button icon="ele-View" @click="visualTable"> 可视化 </el-button>
					</el-button-group>
					<el-button-group style="padding-left: 10px">
						<el-button icon="ele-Plus" @click="openAddColumn"> 增加列 </el-button>
						<el-button icon="ele-Plus" @click="openGenDialog"> 生成实体 </el-button>
						<el-popover placement="bottom" title="温馨提示" :width="200" trigger="hover" content="如果是刚刚生成的实体，请重启服务后再生成种子">
							<template #reference>
								<el-button icon="ele-Plus" @click="openGenSeedDataDialog"> 生成种子 </el-button>
							</template>
						</el-popover>
					</el-button-group>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.columnData" style="width: 100%" v-loading="state.loading1" border>
				<el-table-column type="index" label="序号" width="55" align="center" />
				<el-table-column prop="dbColumnName" label="字段名" show-overflow-tooltip />
				<el-table-column prop="dataType" label="数据类型" align="center" show-overflow-tooltip />
				<el-table-column prop="isPrimarykey" label="主键" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success" v-if="scope.row.isPrimarykey === true">是</el-tag>
						<el-tag type="info" v-else>否</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="isIdentity" label="自增" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success" v-if="scope.row.isIdentity === true">是</el-tag>
						<el-tag type="info" v-else>否</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="isNullable" label="可空" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag v-if="scope.row.isNullable === true">是</el-tag>
						<el-tag type="info" v-else>否</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="length" label="长度" width="70" align="center" show-overflow-tooltip />
				<el-table-column prop="decimalDigits" label="精度" width="70" align="center" show-overflow-tooltip />
				<el-table-column prop="defaultValue" label="默认值" align="center" show-overflow-tooltip />
				<el-table-column prop="columnDescription" label="描述" header-align="center" show-overflow-tooltip />
				<el-table-column label="操作" width="195" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Top" size="small" text type="primary" @click="moveColumn(scope.row, 'up')" :disabled="scope.$index === 0" title="上移"></el-button>
						<el-button icon="ele-Bottom" size="small" text type="primary" @click="moveColumn(scope.row, 'down')" :disabled="scope.$index === state.columnData.length - 1" title="下移"></el-button>
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditColumn(scope.row)">编辑</el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="delColumn(scope.row)">删除</el-button>
					</template>
				</el-table-column>
			</el-table>
		</el-card>

		<EditTable ref="editTableRef" @handleQueryTable="handleQueryTable" />
		<EditColumn ref="editColumnRef" @handleQueryColumn="handleQueryColumn" />
		<AddTable ref="addTableRef" @addTableSubmitted="addTableSubmitted" />
		<AddColumn ref="addColumnRef" @handleQueryColumn="handleQueryColumn" />
		<GenEntity ref="genEntityRef" @handleQueryColumn="handleQueryColumn" :application-namespaces="state.appNamespaces" />
		<GenSeedData ref="genSeedDataRef" :application-namespaces="state.appNamespaces" />
	</div>
</template>

<script lang="ts" setup name="sysDatabase">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { useRouter } from 'vue-router';
import EditTable from '/@/views/system/database/component/editTable.vue';
import EditColumn from '/@/views/system/database/component/editColumn.vue';
import AddTable from '/@/views/system/database/component/addTable.vue';
import AddColumn from '/@/views/system/database/component/addColumn.vue';
import GenEntity from '/@/views/system/database/component/genEntity.vue';
import GenSeedData from '/@/views/system/database/component/genSeedData.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysDatabaseApi, SysCodeGenApi } from '/@/api-services/api';
import { DbColumnOutput, DbTableInfo, DbColumnInput, DeleteDbTableInput, DeleteDbColumnInput, MoveDbColumnInput } from '/@/api-services/models';

const editTableRef = ref<InstanceType<typeof EditTable>>();
const editColumnRef = ref<InstanceType<typeof EditColumn>>();
const addTableRef = ref<InstanceType<typeof AddTable>>();
const addColumnRef = ref<InstanceType<typeof AddColumn>>();
const genEntityRef = ref<InstanceType<typeof GenEntity>>();
const genSeedDataRef = ref<InstanceType<typeof GenSeedData>>();
const router = useRouter();
const state = reactive({
	loading: false,
	loading1: false,
	dbData: [] as any,
	configId: '',
	tableData: [] as Array<DbTableInfo>,
	tableName: '',
	columnData: [] as Array<DbColumnOutput>,
	queryParams: {
		name: undefined,
		code: undefined,
	},
	appNamespaces: [] as Array<String>, // 存储位置
});

onMounted(async () => {
	state.loading = true;
	var res = await getAPI(SysDatabaseApi).apiSysDatabaseListGet();
	state.dbData = res.data.result;
	state.loading = false;

	let appNamesRes = await getAPI(SysCodeGenApi).apiSysCodeGenApplicationNamespacesGet();
	state.appNamespaces = appNamesRes.data.result as Array<string>;
});

// 增加表
const addTableSubmitted = (e: any) => {
	handleQueryTable();
	state.tableName = e;
	handleQueryColumn();
};

// 表查询操作
const handleQueryTable = async () => {
	state.tableName = '';
	state.columnData = [];
	state.loading = true;

	var res = await getAPI(SysDatabaseApi).apiSysDatabaseTableListConfigIdGet(state.configId);
	let tableData = res.data.result ?? [];
	state.tableData = [];
	tableData.forEach((element: any) => {
		//排除zero_开头的表
		if (!element.name.startsWith('zero_')) {
			state.tableData.push(element);
		}
	});
	state.loading = false;
};

// 列查询操作
const handleQueryColumn = async () => {
	state.columnData = [];
	if (state.tableName == '' || typeof state.tableName == 'undefined') return;

	state.loading1 = true;
	var res = await getAPI(SysDatabaseApi).apiSysDatabaseColumnListTableNameConfigIdGet(state.tableName, state.configId);
	state.columnData = res.data.result ?? [];
	state.loading1 = false;
};

// 打开表编辑页面
const openEditTable = () => {
	if (state.configId == '' || state.tableName == '') {
		ElMessage({
			type: 'error',
			message: `请选择库名和表名!`,
		});
		return;
	}
	var res = state.tableData.filter((u: any) => u.name == state.tableName);
	var table: any = {
		configId: state.configId,
		tableName: state.tableName,
		oldTableName: state.tableName,
		description: res[0].description,
	};
	editTableRef.value?.openDialog(table);
};

// 打开实体生成页面
const openGenDialog = () => {
	if (state.configId == '' || state.tableName == '') {
		ElMessage({
			type: 'error',
			message: `请选择库名和表名!`,
		});
		return;
	}
	// var res = state.tableData.filter((u: any) => u.name == state.tableName);
	var table: any = {
		configId: state.configId,
		tableName: state.tableName,
		position: state.appNamespaces[0],
	};
	genEntityRef.value?.openDialog(table);
};

// 生成种子数据页面
const openGenSeedDataDialog = () => {
	if (state.configId == '' || state.tableName == '') {
		ElMessage({
			type: 'error',
			message: `请选择库名和表名!`,
		});
		return;
	}
	var table: any = {
		configId: state.configId,
		tableName: state.tableName,
		position: state.appNamespaces[0],
	};
	genSeedDataRef.value?.openDialog(table);
};

// 打开表增加页面
const openAddTable = () => {
	if (state.configId == '') {
		ElMessage({
			type: 'error',
			message: `请选择库名!`,
		});
		return;
	}
	var table: any = {
		configId: state.configId,
		tableName: '',
		oldTableName: '',
		description: '',
	};
	addTableRef.value?.openDialog(table);
};

// 打开列编辑页面
const openEditColumn = (row: any) => {
	var column: any = {
		configId: state.configId,
		tableName: row.tableName,
		columnName: row.dbColumnName,
		oldColumnName: row.dbColumnName,
		description: row.columnDescription,
		defaultValue: row.defaultValue,
	};
	editColumnRef.value?.openDialog(column);
};

// 打开列增加页面
const openAddColumn = () => {
	if (state.configId == '' || state.tableName == '') {
		ElMessage({
			type: 'error',
			message: `请选择库名和表名!`,
		});
		return;
	}
	const addRow: DbColumnInput = {
		configId: state.configId,
		tableName: state.tableName,
		columnDescription: '',
		dataType: '',
		dbColumnName: '',
		decimalDigits: 0,
		isIdentity: 0,
		isNullable: 0,
		isPrimarykey: 0,
		length: 0,
		// key: 0,
		// editable: true,
		// isNew: true,
	};
	addColumnRef.value?.openDialog(addRow);
};

// 删除表
const delTable = () => {
	if (state.tableName == '') {
		ElMessage({
			type: 'error',
			message: `请选择表名!`,
		});
		return;
	}
	ElMessageBox.confirm(`确定删除表：【${state.tableName}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			const deleteDbTableInput: DeleteDbTableInput = {
				configId: state.configId,
				tableName: state.tableName,
			};
			await getAPI(SysDatabaseApi).apiSysDatabaseDeleteTablePost(deleteDbTableInput);
			handleQueryTable();
			ElMessage.success('表删除成功');
		})
		.catch(() => {});
};

// 删除列
const delColumn = (row: any) => {
	ElMessageBox.confirm(`确定删除列：【${row.dbColumnName}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			const eleteDbColumnInput: DeleteDbColumnInput = {
				configId: state.configId,
				tableName: state.tableName,
				dbColumnName: row.dbColumnName,
			};
			await getAPI(SysDatabaseApi).apiSysDatabaseDeleteColumnPost(eleteDbColumnInput);
			handleQueryColumn();
			ElMessage.success('列删除成功');
		})
		.catch(() => {});
};

const moveColumn = (row: any, direction: 'up' | 'down') => {
	const { columnData, tableName, configId } = state;
	const currentIndex = columnData.findIndex((item) => item.dbColumnName === row.dbColumnName);

	// 边界检查与反馈
	if (direction === 'up' && currentIndex === 0) {
		ElMessage.warning('已处于首位，无法上移');
		return;
	}
	if (direction === 'down' && currentIndex === columnData.length - 1) {
		ElMessage.warning('已处于末位，无法下移');
		return;
	}

	// 计算目标位置
	const targetIndex = direction === 'up' ? currentIndex - 1 : currentIndex + 1;
	const targetColumn = columnData[targetIndex];
	const columnName = direction === 'up' ? targetColumn.dbColumnName : row.dbColumnName;
	const afterColumnName = direction === 'up' ? row.dbColumnName : targetColumn.dbColumnName;

	ElMessageBox.confirm(`确定将列【${row.dbColumnName}】${direction === 'up' ? '上移' : '下移'}?`, '操作确认', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	}).then(async () => {
			try {
				const moveParams: MoveDbColumnInput = {
					configId,
					tableName,
					columnName,
					afterColumnName,
				};

				// 调用API
				await getAPI(SysDatabaseApi).apiSysDatabaseMoveColumnPost(moveParams);

				handleQueryColumn();
				ElMessage.success('列位置已更新');
			} catch (error: any) {
				ElMessage.error(`操作失败: ${error.message || '未知错误'}`);
			}
		})
		.catch(() => {});
};

// 可视化表
const visualTable = () => {
	if (state.configId == '') {
		ElMessage({
			type: 'error',
			message: `请选择库名!`,
		});
		return;
	}
	router.push(`/develop/database/visual?configId=${state.configId}`);
};
</script>

<style lang="scss" scoped>
.el-select__placeholder {
    .desc {
        color: var(--el-color-info); 
        font-size: var(--el-font-size-extra-small);
        //font-style: italic;
        margin-left: 5px;
    }
}
</style>