<template>
	<div class="sys-open-access-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="600px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Key /> </el-icon>
					<span> 生成签名 </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="身份标识" prop="accessKey">
							<el-input v-model="state.ruleForm.accessKey" placeholder="身份标识" readonly />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="密钥" prop="accessSecret">
							<el-input v-model="state.ruleForm.accessSecret" placeholder="密钥" readonly> </el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="接口请求地址" prop="url">
							<el-input v-model="state.ruleForm.url" placeholder="接口请求地址" class="input-with-select" clearable>
								<template #prepend>
									<el-select v-model="state.ruleForm.method" placeholder="请求方法" style="width: 100px">
										<el-option label="Get" :value="HttpMethodEnum.NUMBER_0" />
										<el-option label="Post" :value="HttpMethodEnum.NUMBER_1" />
										<el-option label="Put" :value="HttpMethodEnum.NUMBER_2" />
										<el-option label="Delete" :value="HttpMethodEnum.NUMBER_3" />
										<el-option label="Patch" :value="HttpMethodEnum.NUMBER_4" />
										<el-option label="Head" :value="HttpMethodEnum.NUMBER_5" />
										<el-option label="Options" :value="HttpMethodEnum.NUMBER_6" />
										<el-option label="Trace" :value="HttpMethodEnum.NUMBER_7" />
										<el-option label="Connect" :value="HttpMethodEnum.NUMBER_8" />
									</el-select>
								</template>
							</el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="时间戳" prop="timestamp">
							<el-input v-model="state.ruleForm.timestamp" placeholder="输入或获取时间戳" clearable>
								<template #append>
									<el-button @click="getTimeStamp">获取</el-button>
								</template>
							</el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="随机数" prop="nonce">
							<el-input v-model="state.ruleForm.nonce" placeholder="输入或获取随机数" clearable>
								<template #append>
									<el-button @click="getNonce">获取</el-button>
								</template>
							</el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="签名" prop="sign">
							<el-input v-model="state.sign" placeholder="填写信息后自动生成" readonly> </el-input>
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysOpenAccessEdit">
import { reactive, ref, watch } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysOpenAccessApi } from '/@/api-services/api';
import { GenerateSignatureInput, HttpMethodEnum } from '/@/api-services/models';

const props = defineProps({
	title: String,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as GenerateSignatureInput,
	sign: '', // 生成的签名
});

watch([() => state.ruleForm.method, () => state.ruleForm.url, () => state.ruleForm.timestamp, () => state.ruleForm.nonce], () => {
	if (
		state.ruleForm.method == undefined ||
		state.ruleForm.method == null ||
		!state.ruleForm.url ||
		!state.ruleForm.timestamp ||
		!state.ruleForm.nonce ||
		/^\d+$/.test(state.ruleForm.timestamp as unknown as string) == false // 时间戳必须为数字
	) {
		state.sign = '';
		return;
	}

	generateSign();
});

// 打开弹窗
const openDialog = (row: any) => {
	state.ruleForm = {
		accessKey: row?.accessKey,
		accessSecret: row?.accessSecret,
		method: HttpMethodEnum.NUMBER_0,
		url: '',
	};
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
};

/** 生成密钥 */
const createSecret = async () => {
	var res = await getAPI(SysOpenAccessApi).apiSysOpenAccessSecretPost();
	state.ruleForm.accessSecret = res.data.result!;
};

/** 获取当前时间戳（精确到秒） */
const getTimeStamp = () => {
	const timestamp = Math.floor(Date.now() / 1000);
	state.ruleForm.timestamp = timestamp;
};

/** 获取随机数 */
const getNonce = () => {
	var nonce = '';
	for (var i = 0; i < 6; i++) {
		nonce += Math.floor(Math.random() * 10);
	}
	state.ruleForm.nonce = nonce;
};

/** 生成签名 */
const generateSign = async () => {
	var res = await getAPI(SysOpenAccessApi).apiSysOpenAccessGenerateSignaturePost(state.ruleForm);
	state.sign = res.data.result!;
};

// 导出对象
defineExpose({ openDialog });
</script>

<style lang="scss" scoped>
:deep(.input-with-select) {
	.el-input-group__prepend {
		background-color: var(--el-fill-color-blank);
	}
}
</style>
