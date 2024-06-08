<template>
	<div class="sys-codeGenPreview-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="90vw">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<div :class="[state.current?.endsWith('.cs') ? 'cs-style' : state.current?.endsWith('.vue') ? 'vue-style' : 'js-style']">
				<el-segmented v-model="state.current" :options="state.options" block @change="handleChange">
					<template #default="{ item }">
						<div class="pd4">
							<SvgIcon :name="item.icon" class="mb4" />
							<div>{{ item.value }}</div>
						</div>
					</template>
				</el-segmented>
			</div>
			<div ref="monacoEditorRef" style="width: 100%; height: 660px; margin-top: 6px"></div>
			<template #footer>
				<span class="dialog-footer">
					<el-button icon="ele-Close" @click="cancel">关 闭</el-button>
					<el-button icon="ele-CopyDocument" type="primary" @click="handleCopy">复 制</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="sysPreviewCode">
import { reactive, ref, nextTick, toRaw } from 'vue';
import * as monaco from 'monaco-editor';
import EditorWorker from 'monaco-editor/esm/vs/editor/editor.worker?worker';
import commonFunction from '/@/utils/commonFunction';

import { getAPI } from '/@/utils/axios-utils';
import { SysCodeGenApi } from '/@/api-services/api';

const { copyText } = commonFunction();

const props = defineProps({
	title: String,
});
const monacoEditorRef = ref();
const state = reactive({
	isShowDialog: false,
	options: [], //分段器的选项
	current: '', // 选中的分段
	codes: [], //预览的代码
});

// 防止 monaco 报黄
self.MonacoEnvironment = {
	getWorker: (_: string, label: string) => new EditorWorker(),
};

// 初始化monacoEditor对象
var monacoEditor: any = null;
const initMonacoEditor = () => {
	monacoEditor = monaco.editor.create(monacoEditorRef.value, {
		theme: 'vs-dark', // 主题 vs vs-dark hc-black
		value: '', // 默认显示的值
		language: 'csharp',
		formatOnPaste: true,
		wordWrap: 'on', //自动换行，注意大小写
		wrappingIndent: 'indent',
		folding: true, // 是否折叠
		foldingHighlight: true, // 折叠等高线
		foldingStrategy: 'indentation', // 折叠方式  auto | indentation
		showFoldingControls: 'always', // 是否一直显示折叠 always | mouSEOver
		disableLayerHinting: true, // 等宽优化
		emptySelectionClipboard: false, // 空选择剪切板
		selectionClipboard: false, // 选择剪切板
		automaticLayout: true, // 自动布局
		codeLens: false, // 代码镜头
		scrollBeyondLastLine: false, // 滚动完最后一行后再滚动一屏幕
		colorDecorators: true, // 颜色装饰器
		accessibilitySupport: 'auto', // 辅助功能支持  "auto" | "off" | "on"
		lineNumbers: 'on', // 行号 取值： "on" | "off" | "relative" | "interval" | function
		lineNumbersMinChars: 5, // 行号最小字符   number
		//enableSplitViewResizing: false,
		readOnly: false, //是否只读  取值 true | false
	});
};

// 打开弹窗
const openDialog = async (row: any) => {
	state.isShowDialog = true;
	const { data } = await getAPI(SysCodeGenApi).apiSysCodeGenPreviewPost(row);
	state.codes = data.result ?? [];
	state.options = Object.keys(data.result).map((fileName: string) => ({
		value: fileName,
		icon: fileName?.endsWith('.cs') ? 'fa fa-hashtag' : fileName?.endsWith('.vue') ? 'fa fa-vimeo' : 'fa fa-file-code-o',
	}));
	state.current = state.options?.[0]?.value ?? '';
	if (monacoEditor == null) initMonacoEditor();
	// 防止取不到
	nextTick(() => {
		monacoEditor.setValue(state.codes[state.current]);
	});
};

// 分段器改变时切换代码
const handleChange = (current: any) => {
	monacoEditor.setValue(state.codes[current]);
};

// 取消
const cancel = () => {
	state.isShowDialog = false;
};

//复制代码
const handleCopy = () => {
	copyText(state.codes[state.current]);
};

// 导出对象
defineExpose({ openDialog });
</script>

<style scoped>
.cs-style .el-segmented {
	--el-segmented-item-selected-bg-color: #5c2d91;
}
.vue-style .el-segmented {
	--el-segmented-item-selected-bg-color: #42b883;
}
.js-style .el-segmented {
	--el-segmented-item-selected-bg-color: #e44d26;
}
</style>
