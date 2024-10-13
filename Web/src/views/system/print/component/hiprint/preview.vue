<template>
	<el-dialog v-model="state.dialogVisible" draggable :close-on-click-modal="false" :width="Number(state.width) + Number(8) + 'mm'">
		<template #header>
			<div style="color: #fff">
				<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Printer /> </el-icon>
				<span>{{ props.title }}</span>
			</div>
		</template>
		<div id="preview_content" ref="previewContentRef"></div>
		<template #footer>
			<el-button :loading="state.waitShowPrinter" type="primary" icon="ele-Printer" @click.stop="print">直接打印</el-button>
			<el-button type="primary" icon="ele-Printer" @click.stop="toPdf">导出PDF</el-button>
			<el-button key="close" @click="hideDialog"> 关闭 </el-button>
		</template>
	</el-dialog>
</template>

<script lang="ts" setup>
import { nextTick, reactive, ref } from 'vue';

var props = defineProps({
	title: {
		type: String,
		default: '',
	},
});

const state = reactive({
	dialogVisible: false,
	waitShowPrinter: false,
	width: 0, // 纸张宽 mm
	printData: {}, // 打印数据
	printType: 1, // 默认浏览器打印
	printParam: {
		printer: '', // 打印机名称
		title: '', // 打印任务名称
		color: false, // 是否打印颜色 默认 true
		copies: 1, // 打印份数 默认 1
	},
	// 打印参数
	hiprintTemplate: {} as any,
});

const previewContentRef = ref();

const showDialog = (hiprintTemplate: any, printData: {}, width = 210, printType = 1, printParam: { printer: ''; title: ''; color: false; copies: 1 }) => {
	state.dialogVisible = true;
	state.width = width;
	state.hiprintTemplate = hiprintTemplate;
	state.printData = printData;
	state.printParam = printParam;
	state.printType = printType;
	nextTick(() => {
		while (previewContentRef.value?.firstChild) {
			previewContentRef.value.removeChild(previewContentRef.value.firstChild);
		}
		const newHtml = hiprintTemplate.getHtml(printData);
		previewContentRef.value.appendChild(newHtml[0]);
	});
};

const print = () => {
	state.waitShowPrinter = true;
	// debugger;
	// 判断是否已成功连接
	if (state.printType == 2) {
		// 注意：连接是异步的
		// 已连接
		// 获取打印机列表
		const printerList = state.hiprintTemplate.getPrinterList();

		let sfcz = printerList.some((item: any) => {
			return item.name == state.printParam.printer;
		});
		if (!sfcz) {
			alert('打印机不存在');
		} else {
			// 直接打印 将使用系统设置的 默认打印机
			state.hiprintTemplate.print2(state.printData, state.printParam);

			// 发送任务到打印机成功
			state.hiprintTemplate.on('printSuccess', function (e: any) {
				state.waitShowPrinter = false;
			});
			// 发送任务到打印机失败
			state.hiprintTemplate.on('printError', function (e: any) {
				state.waitShowPrinter = false;
				alert('打印失败：' + e);
			});
		}
	} else {
		state.hiprintTemplate.print(
			state.printData,
			{},
			{
				callback: () => {
					state.waitShowPrinter = false;
				},
			}
		);
	}
};

const toPdf = () => {
	state.hiprintTemplate.toPdf(state.printData, 'PDF文件');
};

const hideDialog = () => {
	state.dialogVisible = false;
};

defineExpose({ showDialog });
</script>

<style lang="less" scoped>
:deep(.ant-modal-body) {
	padding: 0px;
}

:deep(.ant-modal-content) {
	margin-bottom: 24px;
}
</style>
