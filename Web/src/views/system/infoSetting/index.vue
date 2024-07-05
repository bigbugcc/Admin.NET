<template>
	<div>
		<el-card shadow="hover" v-loading="state.isLoading">
			<el-descriptions title="系统信息配置" :column="2" :border="true">
				<template #title>
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Setting /> </el-icon> 系统信息配置
				</template>
				<el-descriptions-item label="系统图标" :span="2">
					<!-- <template #label>
						<div class="cell-item">
							<el-icon><ele-PictureRounded /></el-icon> 系统图标
						</div>
					</template> -->
					<el-upload class="avatar-uploader" :showFileList="false" :autoUpload="false" accept=".jpg,.png,.svg" action="" :limit="1" :onChange="handleUploadChange">
						<img v-if="state.formData.sysLogo" :src="state.formData.sysLogo" class="avatar" />
						<SvgIcon v-else class="avatar-uploader-icon" name="ele-Plus" :size="28" />
					</el-upload>
				</el-descriptions-item>
				<el-descriptions-item label="系统主标题">
					<el-input v-model="state.formData.sysTitle" />
				</el-descriptions-item>
				<el-descriptions-item label="系统副标题">
					<el-input v-model="state.formData.sysViceTitle" />
				</el-descriptions-item>
				<el-descriptions-item label="系统描述" :span="2">
					<el-input v-model="state.formData.sysViceDesc" />
				</el-descriptions-item>
				<el-descriptions-item label="水印内容" :span="2">
					<el-input v-model="state.formData.sysWatermark" />
				</el-descriptions-item>
				<el-descriptions-item label="版权说明" :span="2">
					<el-input v-model="state.formData.sysCopyright" />
				</el-descriptions-item>
				<el-descriptions-item label="ICP备案号">
					<el-input v-model="state.formData.sysIcp" />
				</el-descriptions-item>
				<el-descriptions-item label="ICP地址">
					<el-input v-model="state.formData.sysIcpUrl" />
				</el-descriptions-item>
				<template #extra>
					<el-button type="primary" icon="ele-SuccessFilled" @click="onSave">保存</el-button>
				</template>
			</el-descriptions>
		</el-card>
	</div>
</template>

<script setup lang="ts" name="sysInfoSetting">
import { nextTick, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysConfigApi } from '/@/api-services';
import { ElMessage } from 'element-plus';
import { fileToBase64 } from '/@/utils/base64Conver';

const state = reactive({
	isLoading: false,
	file: undefined as any,
	formData: {
		sysLogoBlob: undefined,
		sysLogo: '',
		sysTitle: '',
		sysViceTitle: '',
		sysViceDesc: '',
		sysWatermark: '',
		sysCopyright: '',
		sysIcp: '',
		sysIcpUrl: '',
	},
});

// 通过onChange方法获得文件列表
const handleUploadChange = (file: any) => {
	state.file = file;
	// 改变 sysLogo，显示预览
	state.formData.sysLogo = URL.createObjectURL(state.file.raw);
};

// 保存
const onSave = async () => {
	// 如果有选择图标，则转换为 base64
	let sysLogoBase64 = '';
	if (state.file) {
		sysLogoBase64 = (await fileToBase64(state.file.raw)) as string;
	}

	try {
		state.isLoading = true;
		const res = await getAPI(SysConfigApi).apiSysConfigSaveSysInfoPost({
			sysLogoBase64: sysLogoBase64,
			sysTitle: state.formData.sysTitle,
			sysViceTitle: state.formData.sysViceTitle,
			sysViceDesc: state.formData.sysViceDesc,
			sysWatermark: state.formData.sysWatermark,
			sysCopyright: state.formData.sysCopyright,
			sysIcp: state.formData.sysIcp,
			sysIcpUrl: state.formData.sysIcpUrl,
		});
		if (res.data!.type !== 'success') return;

		// 清空 file 变量
		state.file = undefined;
		await loadData();
		ElMessage.success('保存成功');
	} finally {
		nextTick(() => {
			state.isLoading = false;
		});
	}
};

// 加载数据
const loadData = async () => {
	try {
		state.isLoading = true;
		const res = await getAPI(SysConfigApi).apiSysConfigSysInfoGet();
		if (res.data!.type !== 'success') return;

		const result = res.data.result;
		state.formData = {
			sysLogoBlob: undefined,
			sysLogo: result.sysLogo,
			sysTitle: result.sysTitle,
			sysViceTitle: result.sysViceTitle,
			sysViceDesc: result.sysViceDesc,
			sysWatermark: result.sysWatermark,
			sysCopyright: result.sysCopyright,
			sysIcp: result.sysIcp,
			sysIcpUrl: result.sysIcpUrl,
		};
	} finally {
		nextTick(() => {
			state.isLoading = false;
		});
	}
};

loadData();
</script>

<style lang="scss" scoped>
.avatar-uploader .avatar {
	width: 100px;
	height: 100px;
	display: block;
}

:deep(.avatar-uploader) .el-upload {
	border: 1px dashed var(--el-border-color);
	cursor: pointer;
	position: relative;
	overflow: hidden;
	transition: var(--el-transition-duration-fast);
}

:deep(.avatar-uploader) .el-upload:hover {
	border-color: var(--el-color-primary);
}

.el-icon.avatar-uploader-icon {
	color: #8c939d;
	width: 100px;
	height: 100px;
	text-align: center;
}
</style>
