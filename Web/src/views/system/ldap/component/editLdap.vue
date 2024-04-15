<template>
	<div class="sysLdap-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="900px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto" :rules="rules">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="主机" prop="host">
							<el-input v-model="state.ruleForm.host" placeholder="请输入主机" maxlength="128" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="端口" prop="port">
							<el-input v-model="state.ruleForm.port" type="number" placeholder="请输入端口" maxlength="5" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="搜索基准" prop="baseDn">
							<el-input v-model="state.ruleForm.baseDn" placeholder="请输入用户搜索基准" maxlength="128" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="过滤规则" prop="authFilter">
							<el-input v-model="state.ruleForm.authFilter" placeholder="请输入用户过滤规则" maxlength="128" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="绑定DN" prop="bindDn">
							<el-input v-model="state.ruleForm.bindDn" placeholder="请输入有域管理权限的账户" maxlength="32" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="绑定密码" prop="bindPass">
							<el-input v-model="state.ruleForm.bindPass" placeholder="请输入有域管理权限的密码" maxlength="512" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="字段属性" prop="bindAttrAccount">
							<el-input v-model="state.ruleForm.bindAttrAccount" placeholder="请输入域账号字段属性值" maxlength="24" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="用户属性" prop="bindAttrEmployeeId">
							<el-input v-model="state.ruleForm.bindAttrEmployeeId" placeholder="请输入绑定用户EmployeeId属性！" maxlength="24" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="绑定Code属性" prop="bindAttrCode">
							<el-input v-model="state.ruleForm.bindAttrCode" placeholder="请输入绑定Code属性！" maxlength="64" show-word-limit clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="Ldap版本" prop="version">
							<el-input v-model="state.ruleForm.version" type="number" placeholder="请输入Ldap版本" maxlength="4" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="状态" prop="status">
							<el-switch v-model="state.ruleForm.status" active-text="是" inactive-text="否" />
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

<script lang="ts" setup>
import { ref, reactive } from 'vue';
import type { FormRules } from 'element-plus';

import { getAPI } from '/@/utils/axios-utils';
import { SysLdapApi } from '/@/api-services/api';

const props = defineProps({
	title: String,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as any,
});

// 打开弹窗
const openDialog = (row: any) => {
	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
};

// 关闭弹窗
const closeDialog = () => {
	emits('handleQuery');
	state.isShowDialog = false;
};

// 取消
const cancel = () => {
	state.isShowDialog = false;
};

// 提交
const submit = () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await getAPI(SysLdapApi).apiSysLdapUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysLdapApi).apiSysLdapAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

// 验证规则
const rules = ref<FormRules>({
	host: [{ required: true, message: '请输入主机！', trigger: 'blur' }],
	port: [{ required: true, message: '请输入端口！', trigger: 'blur' }],
	baseDn: [{ required: true, message: '请输入用户搜索基准！', trigger: 'blur' }],
	bindDn: [{ required: true, message: '请输入绑定DN！', trigger: 'blur' }],
	bindPass: [{ required: true, message: '请输入绑定密码！', trigger: 'blur' }],
	authFilter: [{ required: true, message: '请输入用户过滤规则！', trigger: 'blur' }],
	version: [{ required: true, message: '请输入Ldap版本！', trigger: 'blur' }],
	bindAttrAccount: [{ required: true, message: '请输入账号绑定字段！', trigger: 'blur' }],
	bindAttrEmployeeId: [{ required: true, message: '绑定用户EmployeeId属性！', trigger: 'blur' }],
	bindAttrCode: [{ required: true, message: '绑定Code属性！', trigger: 'blur' }],
});

// 导出对象
defineExpose({ openDialog });
</script>
