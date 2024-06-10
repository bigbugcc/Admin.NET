<template lang="">
	<div class="flow-container">
		<el-dialog v-model="state.isShowDialog" :width="800" draggable :close-on-click-modal="false">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span>{{ props.title }}</span>
				</div>
			</template>
			<el-timeline>
				<el-timeline-item
					v-for="(activity, index) in state.activities"
					:key="index"
					:icon="activity.icon"
					:type="activity.type"
					:color="activity.color"
					:size="activity.size"
					:hollow="activity.hollow"
					:timestamp="activity.timestamp"
					placement="top"
				>
					<el-card shadow="hover">
						<h4>{{ activity.content }}</h4>
						<br />
						<div class="demo-type">
							<el-avatar :icon="UserFilled" />
						</div>
					</el-card>
				</el-timeline-item>
			</el-timeline>
			<!-- <template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">取 消</el-button>
				</span>
			</template> -->
		</el-dialog>
	</div>
</template>

<script setup lang="ts">
import { reactive, ref, onMounted } from 'vue';
import type { FormRules } from 'element-plus';

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
	ruleForm: {},
	circleUrl: 'https://cube.elemecdn.com/3/7c/3ea6beec64369c2642b92c6726f1epng.png',
	activities: [
		{
			content: 'Event start',
			timestamp: '2018-04-12 20:46',
			size: 'large',
			type: 'primary',
			user: 'admin',
		},
		{
			content: 'Approved',
			timestamp: '2018-04-13',
			color: '#0bbd87',
			user: 'admin',
		},
		{
			content: 'Approved',
			timestamp: '2018-04-11',
			color: '#0bbd87',
			user: 'admin',
		},
		{
			content: 'Success',
			timestamp: '2018-04-11 20:46',
			hollow: true,
			user: 'admin',
		},
	],
});

const rules = ref<FormRules>({});

onMounted(() => {});

const openDialog = (row: any) => {
	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.isShowDialog = true;
};

const closeDialog = () => {
	emit('reloadTable');
	state.isShowDialog = false;
};

const cancel = () => {
	state.isShowDialog = false;
};

defineExpose({ openDialog });
</script>

<style scoped lang="scss">
:deep(.el-select),
:deep(.el-input-number) {
	width: 100%;
}
.demo-type {
	display: flex;
}
.demo-type > div {
	flex: 1;
	text-align: center;
}

.demo-type > div:not(:last-child) {
	border-right: 1px solid var(--el-border-color);
}
</style>
