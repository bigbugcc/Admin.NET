<template lang="">
	<div class="flow-container">
		<el-dialog v-model="state.isShowDialog" :width="800" draggable :close-on-click-modal="false">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span>{{ props.title }}</span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="库定位器" prop="configId" :rules="[{ required: true, message: '库定位器不能为空', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.configId" placeholder="库名" filterable @change="dbChanged()" class="w100">
								<el-option v-for="item in state.dbData" :key="item.configId" :label="item.configId" :value="item.configId" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="表定位器" prop="tableName" :rules="[{ required: true, message: '表定位器不能为空', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.tableName" value-key="value" filterable clearable class="w100">
								<el-option v-for="item in state.tableData" :key="item.name" :label="item.name + ' [ ' + item.description + ' ]'" :value="item.name" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="操作" prop="typeName" :rules="[{ required: true, message: '操作不能为空', trigger: 'blur' }]">
							<el-select v-model="state.ruleForm.typeName" value-key="value" filterable clearable class="w100">
								<el-option v-for="item in state.typeData" :key="item.name" :label="item.name + ' ( ' + item.value + ' )' + ' [ ' + item.description + ' ]'" :value="item.value" />
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">取 消</el-button>
					<el-button type="primary" @click="submit">确 定</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts">
import { reactive, ref, onMounted } from 'vue';
import type { FormRules } from 'element-plus';

import { getAPI } from '/@/utils/axios-utils';
import { ApprovalFlowApi } from '/@/api-services/_approvalFlow/api';
import { ApprovalFlowOutput, UpdateApprovalFlowInput } from '/@/api-services/_approvalFlow/models';
import { SysDatabaseApi, SysCodeGenApi } from '/@/api-services/api';
import { DbTableInfo } from '/@/api-services/models';

var props = defineProps({
	title: {
		type: String,
		default: '',
	},
});

const emit = defineEmits(['reloadTable']);
const ruleFormRef = ref();

const state = reactive({
	loading: false,
	isShowDialog: false,
	ruleSource: {} as UpdateApprovalFlowInput,
	ruleForm: {} as any,
	dbData: [] as any,
	tableData: [] as Array<DbTableInfo>,
	typeData: [
		{
			name: 'Add',
			value: 'add',
			description: '新增',
		},
		{
			name: 'Update',
			value: 'update',
			description: '更新',
		},
		{
			name: 'Delete',
			value: 'delete',
			description: '删除',
		},
		{
			name: 'Select',
			value: 'select',
			description: '查询',
		},
		{
			name: 'Export',
			value: 'export',
			description: '导出',
		},
	],
});

const rules = ref<FormRules>({});

onMounted(async () => {
	var resDb = await getAPI(SysCodeGenApi).apiSysCodeGenDatabaseListGet();
	state.dbData = resDb.data.result;
});

const openDialog = (row: ApprovalFlowOutput) => {
	state.ruleSource = row as UpdateApprovalFlowInput;
	state.ruleForm = row.formJson ? JSON.parse(row.formJson) : {};
	state.isShowDialog = true;

	dbChanged();
};

const closeDialog = () => {
	emit('reloadTable');
	state.isShowDialog = false;
};

const cancel = () => {
	state.isShowDialog = false;
};

const submit = async () => {
	state.ruleSource.formJson = JSON.stringify(state.ruleForm);
	await getAPI(ApprovalFlowApi).apiApprovalFlowUpdatePost(state.ruleSource);
	closeDialog();
};

// db改变
const dbChanged = async () => {
	if (state.ruleForm.configId === '') return;

	var res = await getAPI(SysDatabaseApi).apiSysDatabaseTableListConfigIdGet(state.ruleForm.configId);
	state.tableData = res.data.result ?? [];
};

defineExpose({ openDialog });
</script>

<style scoped lang="scss">
:deep(.el-select),
:deep(.el-input-number) {
	width: 100%;
}
</style>
