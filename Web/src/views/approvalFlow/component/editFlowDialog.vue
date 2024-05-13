<template>
	<div class="flow-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" fullscreen>
			<template #header>
				<div style="color: #fff">
					<span>{{ props.title }}</span>
				</div>
			</template>
			<div class="f-content">
				<div class="f-container">
					<div class="f-switch">
						<el-switch v-model="state.value2" @change="change" class="mb-2" active-text="打开框选" inactive-text="关闭框选" />
					</div>
					<PanelControl v-if="lf" :lf="lf" @catData="getData"></PanelControl>
					<div class="f-container-c" ref="container" id="container"></div>
					<PanelNode v-if="lf" :lf="lf"></PanelNode>
					<el-drawer title="属性" v-model="drawer" :direction="direction" size="500px" :before-close="handleClose">
						<PropertyDialog v-if="drawer" :nodeData="state.nodeData" :lf="lf" @setPropertiesFinish="handleClose"></PropertyDialog>
					</el-drawer>
					<el-dialog title="数据" v-model="dataVisible" width="50%">
						<PanelDataDialog :graphData="state.graphData"></PanelDataDialog>
					</el-dialog>
				</div>
			</div>
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
import { reactive, ref, nextTick } from 'vue';
import { ElMessageBox } from 'element-plus';

import LogicFlow from '@logicflow/core';
import { BpmnElement, InsertNodeInPolyline, Menu, MiniMap, SelectionSelect, Snapshot } from '@logicflow/extension';
import '@logicflow/core/dist/style/index.css';
import '@logicflow/extension/lib/style/index.css';

import RegisterEdge from './LogicFlow/Register/RegisterEdge';
import RegisterNode from './LogicFlow/Register/RegisterNode';
import PanelNode from './LogicFlow/Panel/PanelNode.vue';
import PanelControl from './LogicFlow/Panel/PanelControl.vue';
import PanelDataDialog from './LogicFlow/Panel/PanelDataDialog.vue';
import PropertyDialog from './LogicFlow/Property/PropertyDialog.vue';

import { getAPI } from '/@/utils/axios-utils';
import { ApprovalFlowApi } from '/@/api-services/_approvalFlow/api';
import { ApprovalFlowOutput, UpdateApprovalFlowInput } from '/@/api-services/_approvalFlow/models';

var props = defineProps({
	title: {
		type: String,
		default: '',
	},
});

const emit = defineEmits(['reloadTable', 'updateFlow']);
const flowData = ref({});
const lf = ref<InstanceType<typeof LogicFlow>>();

const drawer = ref(false);
const direction = ref('rtl');
const dataVisible = ref(false);

const state = reactive({
	loading: false,
	isShowDialog: false,
	ruleSource: {} as UpdateApprovalFlowInput,
	nodeData: {},
	graphData: {},
});

const openDialog = (row: ApprovalFlowOutput) => {
	state.ruleSource = row as UpdateApprovalFlowInput;
	// 初始化数据
	if (state.ruleSource.flowJson) {
		flowData.value = JSON.parse(state.ruleSource.flowJson);
	} else {
		flowData.value = {
			nodes: [],
			edges: [],
		};
	}
	state.isShowDialog = true;
	nextTick(() => {
		// 初始化画布
		initGraph();
	});
	console.log('open');
};

const closeDialog = () => {
	emit('reloadTable');
	state.isShowDialog = false;
	console.log('close');
};

const cancel = () => {
	state.isShowDialog = false;
	console.log('cancel');
};

// 保存流程设计
const submit = async () => {
	flowData.value = lf.value?.getGraphData();
	state.ruleSource.flowJson = JSON.stringify(flowData.value);
	await getAPI(ApprovalFlowApi).apiApprovalFlowUpdatePost(state.ruleSource);
	emit('updateFlow', flowData.value);
	closeDialog();
};

const initGraph = () => {
	// 初始化画布
	const container: HTMLElement = document.querySelector('#container')!;
	// 配置项
	const config = {
		stopScrollGraph: true, // 禁止鼠标滚动移动画布
		stopZoomGraph: true, // 禁止缩放
		metaKeyMultipleSelected: true,
		// 背景网格大小
		grid: {
			size: 10,
			type: 'dot',
		},
		// 快捷键
		keyboard: {
			enabled: true,
		},
		// 辅助线
		snapline: true,
	};
	lf.value = new LogicFlow({
		...config,
		plugins: [
			BpmnElement,
			// 作栋节点自动插入边
			InsertNodeInPolyline,
			// 右键菜单
			Menu,
			// 迷你图
			MiniMap,
			// 框选
			SelectionSelect,
			// 快照
			Snapshot,
		],
		container: container,
		width: container.clientWidth,
		height: container.clientHeight,
	});
	// 设置主题
	lf.value.setTheme({
		snapline: {
			stroke: '#1E90FF', // 对齐线颜色
			strokeWidth: 1, // 对齐线宽度
		},
	});
	// 注册自定义节点
	RegisterNode.Register(lf.value);
	// 注册自定义边
	RegisterEdge.Register(lf.value);
	// 监听节点点击事件
	lf.value.on('node:click', ({ data }) => {
		state.nodeData = data;
		drawer.value = true;
	});
	// 监听边点击事件
	lf.value.on('edge:click', ({ data }) => {
		state.nodeData = data;
		drawer.value = true;
	});
	// 渲染数据
	lf.value.render(flowData.value);
	// 画布居中
	lf.value.focusOn({ coordinate: { x: 300, y: 300 } });
};

// 框选
const change = (val: boolean) => {
	if (val) {
		lf.value?.extension.selectionSelect.openSelectionSelect();
	} else {
		lf.value?.extension.selectionSelect.closeSelectionSelect();
	}
};

// 获取数据
const getData = () => {
	var data = lf.value?.getGraphData();
	state.graphData = data;
	dataVisible.value = true;
};

// 关闭属性界面提醒
const handleClose = (done: () => void) => {
	ElMessageBox.confirm('确认要关闭当前属性编辑?')
		.then(() => {
			done();
		})
		.catch(() => {
			// catch error
		});
};

defineExpose({ openDialog });
</script>

<style scoped lang="scss">
:deep(.el-tabs__nav-scroll) {
	width: 70%;
	margin: 0 auto;
}
.flow-container {
	:deep(.el-dialog) {
		.el-dialog__header {
			display: none !important;
		}
		.el-dialog__body {
			max-height: calc(100vh - 45px) !important;
		}
	}
}
.f-content {
	display: flex;
	flex-grow: 1;
	z-index: 1;
	margin: -2px -19px -20px -19px;
	height: calc(100vh - 100px) !important;

	.f-container {
		flex-grow: 1;
		position: relative;

		.f-switch {
			position: absolute;
			z-index: 2;
			top: -22px;
			left: 5px;

			.el-switch {
				margin-right: 10px;
			}
		}

		.el-drawer {
			height: 80%;
			overflow: auto;
			margin-top: -30px;
			z-index: !important;
		}

		.f-container-c {
			position: absolute;
			width: 100%;
			height: 100%;
		}
	}
}
</style>
