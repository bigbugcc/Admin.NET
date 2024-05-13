<template>
	<input class="el-upload__input" ref="reffile" name="file" @change="fileChange($event)" :accept="$props.accept" type="file" />
	<el-button icon="ele-Upload" @click="onClick">
		<slot></slot>
	</el-button>
</template>

<script setup lang="ts" name="ImportButton">
import { reactive, ref, onMounted, watch } from 'vue';
import { request2 } from '/@/utils/request';
import { ElMessage } from 'element-plus';

// 定义父组件传过来的值
const props = defineProps({
	accept: {
		type: String,
	},
	param: {
		type: Object,
		default: () => {},
	},
	url: {
		type: String,
	},
});

// 定义子组件向父组件传值/事件
const emit = defineEmits(['success', 'error']);
const reffile = ref();
const state = reactive({
	form: {} as any,
});

watch(
	() => props.param,
	(value) => {
		if (value) {
			state.form = Object.assign({}, { ...value });
		}
	},
	{
		immediate: true,
		deep: true,
	}
);

// 上传文件
const onClick = () => {
	reffile.value.click();
};

function fileChange(event: any) {
	if (!event.currentTarget.files || event.currentTarget.files.length == 0) {
		return;
	}
	var file = event.currentTarget.files[0];
	let formData = new FormData();
	formData.append('file', file);
	for (const key in state.form) {
		const element = state.form[key];
		formData.append(key, element);
	}
	request2({
		url: props.url,
		method: 'post',
		data: formData,
		headers: {
			'Content-Type': 'multipart/form-data',
		},
	})
		.then((res: any) => {
			ElMessage.success(res);
			reffile.value.files = null;
			emit('success', res);
		})
		.catch((res: any) => {
			alert('上传错误');
		});
}
</script>
