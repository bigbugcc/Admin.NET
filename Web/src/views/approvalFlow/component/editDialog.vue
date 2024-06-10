<template>
	<div class="labApprovalFlow-container">
		<el-dialog v-model="state.isShowDialog" :width="800" draggable>
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span>{{ props.title }}</span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto" :rules="rules">
				<el-tabs>
					<el-tab-pane label="基本信息">
						<el-row :gutter="35">
							<el-form-item v-show="false">
								<el-input v-model="state.ruleForm.id" />
							</el-form-item>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="编号" prop="code">
									<el-input v-model="state.ruleForm.code" placeholder="请输入编号" maxlength="32" show-word-limit clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="名称" prop="name" :rules="[{ required: true, message: '名称不能为空', trigger: 'blur' }]">
									<el-input v-model="state.ruleForm.name" placeholder="请输入名称" maxlength="32" show-word-limit clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="状态" prop="status" :rules="[{ required: true, message: '状态不能为空', trigger: 'blur' }]">
									<el-select clearable v-model="state.ruleForm.status" placeholder="请选择状态">
										<el-option v-for="(item, index) in getEnumStatusData" :key="index" :value="item.value" :label="`${item.describe} [${item.value}]`"></el-option>
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="备注" prop="remark">
									<el-input v-model="state.ruleForm.remark" placeholder="请输入备注" type="textarea" maxlength="255" show-word-limit clearable />
								</el-form-item>
							</el-col>
						</el-row>
					</el-tab-pane>
					<el-tab-pane label="扩展信息">
						<el-row :gutter="35">
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="表单" prop="formJson">
									<el-input v-model="state.ruleForm.formJson" placeholder="请输入表单" type="textarea" maxlength="0" show-word-limit clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="流程" prop="flowJson">
									<el-input v-model="state.ruleForm.flowJson" placeholder="请输入流程" type="textarea" maxlength="0" show-word-limit clearable />
								</el-form-item>
							</el-col>
						</el-row>
					</el-tab-pane>
				</el-tabs>
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
import { ElMessage } from 'element-plus';
import type { FormRules } from 'element-plus';

import { getAPI } from '/@/utils/axios-utils';
import { ApprovalFlowApi } from '/@/api-services/_approvalFlow/api';
import { SysEnumApi } from '/@/api-services/api';

const getEnumStatusData = ref<any>([]);

// 父级传递来的参数
var props = defineProps({
	title: {
		type: String,
		default: '',
	},
	labStatus: {
		type: Array,
		default: () => [],
	},
});
// 父级传递来的函数，用于回调
const emit = defineEmits(['reloadTable']);

// 定义变量内容
const ruleFormRef = ref();
const state = reactive({
	loading: false,
	isShowDialog: false,
	ruleForm: {} as any,
});

// 自行添加其他规则
const rules = ref<FormRules>({
	name: [
		{
			pattern: /^(?!^[0-9].*$).*/,
			message: '不能以数字开头',
			trigger: 'blur',
		},
	],
});

// 页面加载时
onMounted(async () => {
	// getEnumStatusData.value = (await getAPI(SysEnumApi).apiSysEnumEnumDataListGet('LabStatusEnum')).data.result ?? [];
});

// 打开弹窗
const openDialog = async (row: any) => {
	let rowData = JSON.parse(JSON.stringify(row));
	state.ruleForm = rowData.id ? (await getAPI(ApprovalFlowApi).apiApprovalFlowDetailGet(rowData.id)).data.result : rowData;
	state.isShowDialog = true;
};

// 关闭弹窗
const closeDialog = () => {
	emit('reloadTable');
	state.isShowDialog = false;
};

// 取消
const cancel = () => {
	state.isShowDialog = false;
};

// 提交
const submit = async () => {
	ruleFormRef.value.validate(async (isValid: boolean, fields?: any) => {
		if (isValid) {
			if (state.ruleForm.id == undefined || state.ruleForm.id == null || state.ruleForm.id == 0) {
				await getAPI(ApprovalFlowApi).apiApprovalFlowAddPost(state.ruleForm);
			} else {
				await getAPI(ApprovalFlowApi).apiApprovalFlowUpdatePost(state.ruleForm);
			}
			closeDialog();
		} else {
			ElMessage({
				message: `表单有${Object.keys(fields).length}处验证失败，请修改后再提交`,
				type: 'error',
			});
		}
	});
};

// 将属性或者函数暴露给父组件
defineExpose({ openDialog });
</script>

<style scoped lang="scss">
:deep(.el-select),
:deep(.el-input-number) {
	width: 100%;
}
</style>
