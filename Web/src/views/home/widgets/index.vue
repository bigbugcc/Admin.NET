<template>
	<div style="height: 100vh; overflow: hidden">
		<div class="noticebar" style="display: flex">
			<NoticeBar />
			<div class="editlayout">
				<el-tooltip content="编辑/保存布局" placement="bottom">
					<el-button v-if="customizing" type="warning" icon="ele-Check" circle plain @click="save"></el-button>
					<el-button v-else type="warning" icon="ele-Edit" circle plain @click="custom"></el-button>
				</el-tooltip>
			</div>
		</div>

		<div :class="['widgets-home', customizing ? 'customizing' : '']" ref="main">
			<div class="widgets-content">
				<!-- <div class="widgets-top">
				<div class="widgets-top-title">控制台</div>
				<div class="widgets-top-actions">
					<el-button v-if="customizing" type="primary" icon="ele-Check" round @click="save">完成</el-button>
					<el-button v-else type="primary" icon="ele-Edit" round @click="custom">自定义</el-button>
				</div>
			</div> -->
				<div class="widgets" ref="widgetsRef">
					<div class="widgets-wrapper">
						<div v-if="nowCompsList.length <= 0" class="no-widgets">
							<el-empty description="没有部件啦" :image-size="300"></el-empty>
						</div>
						<el-row :gutter="8">
							<el-col v-for="(item, index) in grid.layout" :key="index" :md="item" :xs="24">
								<VueDraggable v-model="grid.copmsList[index]" :animation="200" group="grid" handle=".customize-overlay" class="draggable-box">
									<div v-for="item in grid.copmsList[index]" :key="item">
										<div class="widgets-item mb8">
											<component :is="allComps[item]"></component>
											<div v-if="customizing" class="customize-overlay">
												<el-button class="close" type="danger" plain icon="ele-Close" @click="remove(item)"></el-button>
												<label v-if="allComps[item]">
													<el-icon> <component :is="allComps[item].icon" /> </el-icon>{{ allComps[item].title }}
												</label>
											</div>
										</div>
									</div>
								</VueDraggable>
							</el-col>
						</el-row>
					</div>
				</div>
			</div>

			<div v-if="customizing" class="widgets-aside">
				<div class="widgets-top">
					<div class="widgets-aside-title">
						<el-icon><ele-CirclePlusFilled /></el-icon>添加部件
					</div>
					<div class="widgets-top-actions">
						<div class="widgets-aside-close" @click="close">
							<el-icon><ele-Close /></el-icon>
						</div>
					</div>
				</div>
				<el-container>
					<el-header style="height: auto">
						<div class="selectLayout">
							<div class="selectLayout-item item01" :class="{ active: grid.layout.join(',') === '12,6,6' }" @click="setLayout([12, 6, 6])">
								<el-row :gutter="2">
									<el-col :span="12"><span></span></el-col>
									<el-col :span="6"><span></span></el-col>
									<el-col :span="6"><span></span></el-col>
								</el-row>
							</div>
							<div class="selectLayout-item item02" :class="{ active: grid.layout.join(',') === '24,16,8' }" @click="setLayout([24, 16, 8])">
								<el-row :gutter="2">
									<el-col :span="24"><span></span></el-col>
									<el-col :span="16"><span></span></el-col>
									<el-col :span="8"><span></span></el-col>
								</el-row>
							</div>
							<div class="selectLayout-item item03" :class="{ active: grid.layout.join(',') === '24' }" @click="setLayout([24])">
								<el-row :gutter="2">
									<el-col :span="24"><span></span></el-col>
									<el-col :span="24"><span></span></el-col>
									<el-col :span="24"><span></span></el-col>
								</el-row>
							</div>
							<div class="selectLayout-item item01" :class="{ active: grid.layout.join(',') === '6,12,6' }" @click="setLayout([6, 12, 6])">
								<el-row :gutter="2">
									<el-col :span="6"><span></span></el-col>
									<el-col :span="12"><span></span></el-col>
									<el-col :span="6"><span></span></el-col>
								</el-row>
							</div>
							<div class="selectLayout-item item02" :class="{ active: grid.layout.join(',') === '24,6,12,6' }" @click="setLayout([24, 6, 12, 6])">
								<el-row :gutter="2">
									<el-col :span="24"><span></span></el-col>
									<el-col :span="6"><span></span></el-col>
									<el-col :span="12"><span></span></el-col>
									<el-col :span="6"><span></span></el-col>
								</el-row>
							</div>
							<div class="selectLayout-item item05" :class="{ active: grid.layout.join(',') === '24,6,12,6,24' }" @click="setLayout([24, 6, 12, 6, 24])">
								<el-row :gutter="2">
									<el-col :span="24"><span></span></el-col>
									<el-col :span="6"><span></span></el-col>
									<el-col :span="12"><span></span></el-col>
									<el-col :span="6"><span></span></el-col>
									<el-col :span="24"><span></span></el-col>
								</el-row>
							</div>
						</div>
					</el-header>
					<el-main class="nopadding">
						<div class="widgets-list">
							<div v-if="myCompsList.length <= 0" class="widgets-list-nodata">
								<el-empty description="没有部件啦" :image-size="60"></el-empty>
							</div>
							<div v-for="item in myCompsList" :key="item.title" class="widgets-list-item">
								<div class="item-logo">
									<el-icon>
										<component :is="item.icon" />
									</el-icon>
								</div>
								<div class="item-info">
									<h2>{{ item.title }}</h2>
									<p>{{ item.description }}</p>
								</div>
								<div class="item-actions">
									<el-button type="primary" icon="ele-Plus" @click="push(item)"></el-button>
								</div>
							</div>
						</div>
					</el-main>
					<el-footer style="height: 51px">
						<el-button @click="backDefault">恢复默认</el-button>
					</el-footer>
				</el-container>
			</div>
		</div>
	</div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, nextTick } from 'vue';
import { VueDraggable } from 'vue-draggable-plus';
import { clone } from '/@/utils/arrayOperation';
import allComps from './components/index';
import { Local } from '/@/utils/storage';
import NoticeBar from '/@/components/noticeBar/index.vue';

interface Grid {
	layout: number[];
	copmsList: string[][];
}
const defaultGrid = {
	layout: [12, 6, 6],
	copmsList: [
		['welcome', 'myapp', 'commit'],
		['about', 'version'],
		['timer', 'schedule'],
	],
};

const customizing = ref<boolean>(false);
const allCompsList = ref(allComps);
const widgetsRef = ref<HTMLElement | null>(null);
const grid = ref<Grid>(clone(defaultGrid));

onMounted(() => {
	const savedGrid = Local.get('grid');
	if (savedGrid) {
		grid.value = savedGrid;
	}
});

const availableCompsList = computed(() => {
	const compsList = [];
	for (const key in allCompsList.value) {
		const comp = allCompsList.value[key];
		compsList.push({
			key,
			title: comp.title,
			icon: comp.icon,
			description: comp.description,
		});
	}
	const activeComps = grid.value.copmsList.flat();
	return compsList.map((comp) => ({
		...comp,
		disabled: activeComps.includes(comp.key),
	}));
});

const myCompsList = computed(() => {
	const myGrid = Local.get('DASHBOARDGRID') || ['welcome', 'myapp', 'version', 'timer', 'echarts', 'about', 'commit', 'schedule'];
	return availableCompsList.value.filter((comp) => !comp.disabled && myGrid.includes(comp.key));
});

const nowCompsList = computed(() => grid.value.copmsList.flat());

// 开启自定义
const custom = () => {
	customizing.value = true;
	const oldWidth = widgetsRef.value?.offsetWidth || 0;
	nextTick(() => {
		if (widgetsRef.value) {
			const scale = widgetsRef.value.offsetWidth / oldWidth;
			widgetsRef.value.style.setProperty('transform', `scale(${scale})`);
		}
	});
};

// 设置布局
const setLayout = (layout: number[]) => {
	grid.value.layout = layout;
	const diff = grid.value.layout.length - grid.value.copmsList.length;
	if (diff < 0) {
		grid.value.copmsList = [...grid.value.copmsList.slice(0, grid.value.layout.length - 1), grid.value.copmsList.slice(grid.value.layout.length - 1).flat()];
	} else if (diff > 0) {
		grid.value.copmsList = grid.value.copmsList.concat(Array.from({ length: diff }, () => []));
	}
};

// 追加
const push = (item: any) => {
	grid.value.copmsList[0].push(item.key);
};

// 隐藏组件
const remove = (item: string) => {
	grid.value.copmsList = grid.value.copmsList.map((list) => list.filter((comp) => comp !== item));
};

// 保存
const save = () => {
	customizing.value = false;
	widgetsRef.value?.style.removeProperty('transform');
	Local.set('grid', grid.value);
};

// 恢复默认
const backDefault = () => {
	customizing.value = false;
	widgetsRef.value?.style.removeProperty('transform');
	grid.value = clone(defaultGrid);
	Local.remove('grid');
};

// 关闭
const close = () => {
	customizing.value = false;
	widgetsRef.value?.style.removeProperty('transform');
	grid.value = Local.get('grid') ? Local.get('grid') : defaultGrid;
};
</script>

<style scoped lang="scss">
.widgets-home {
	display: flex;
	flex-direction: row;
	flex: 1;
	height: 100%;
}
.widgets-content {
	flex: 1;
	overflow: auto;
	overflow-x: hidden;
	padding: 4px;
}
.widgets-aside {
	width: 360px;
	background: #fff;
	box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
	position: relative;
	overflow: auto;
	padding-top: 10px;
}
.widgets-aside-title {
	margin-top: 10px;
	margin-left: 10px;
	font-size: 14px;
	display: flex;
	align-items: center;
	justify-content: center;
}
.widgets-aside-title i {
	margin-right: 10px;
	font-size: 18px;
}
.widgets-aside-close {
	font-size: 18px;
	width: 30px;
	height: 30px;
	display: flex;
	align-items: center;
	justify-content: center;
	border-radius: 3px;
	cursor: pointer;
}
.widgets-aside-close:hover {
	background: rgba(180, 180, 180, 0.1);
}

.widgets-top {
	margin-bottom: 8px;
	display: flex;
	justify-content: space-between;
	align-items: center;
}
.widgets-top-title {
	// font-size: 18px;
	// font-weight: bold;
	color: #999;
}

.widgets {
	transform-origin: top left;
	transition: transform 0.15s;
}

.draggable-box {
	height: 100%;
}

.customizing .widgets-wrapper {
	margin-right: -360px;
}
.customizing .widgets-wrapper .el-col {
	padding-bottom: 15px;
}
.customizing .widgets-wrapper .draggable-box {
	border: 1px dashed var(--el-color-primary);
	padding: 15px;
}
.customizing .widgets-wrapper .no-widgets {
	display: none;
}
.customizing .widgets-item {
	position: relative;
}

.customize-overlay {
	position: absolute;
	top: 0;
	right: 0;
	bottom: 0;
	left: 0;
	z-index: 1;
	display: flex;
	flex-direction: column;
	align-items: center;
	justify-content: center;
	background: rgba(255, 255, 255, 0.9);
	cursor: move;
}
.customize-overlay label {
	background: var(--el-color-primary);
	color: #fff;
	height: 40px;
	padding: 0 30px;
	border-radius: 40px;
	font-size: 18px;
	display: flex;
	align-items: center;
	justify-content: center;
	cursor: move;
}
.customize-overlay label i {
	margin-right: 15px;
	font-size: 24px;
}
.customize-overlay .close {
	position: absolute;
	padding-right: 6px;
	width: 30px;
	height: 30px;
	top: 15px;
	right: 15px;
}
.customize-overlay .close:focus,
.customize-overlay .close:hover {
	background: var(--el-button-hover-color);
}

.widgets-list-item {
	display: flex;
	flex-direction: row;
	padding: 15px;
	align-items: center;
}
.widgets-list-item .item-logo {
	width: 40px;
	height: 40px;
	border-radius: 50%;
	background: rgba(180, 180, 180, 0.1);
	display: flex;
	align-items: center;
	justify-content: center;
	font-size: 18px;
	margin-right: 15px;
	color: #6a8bad;
}
.widgets-list-item .item-info {
	flex: 1;
}
.widgets-list-item .item-info h2 {
	font-size: 16px;
	font-weight: normal;
	cursor: default;
}
.widgets-list-item .item-info p {
	font-size: 12px;
	color: #999;
	cursor: default;
}
.widgets-list-item:hover {
	background: rgba(180, 180, 180, 0.1);
}

.widgets-wrapper .sortable-ghost {
	opacity: 0.5;
}

.layout-list {
	height: 120px;
}

.selectLayout {
	width: 100%;
	height: auto;
	display: flex;
	flex-wrap: wrap;
}
.selectLayout-item {
	margin: 5px;
	width: 60px;
	height: 60px;
	border: 2px solid var(--el-border-color-light);
	padding: 5px;
	cursor: pointer;
	margin-right: 15px;
}
.selectLayout-item span {
	display: block;
	background: var(--el-border-color-light);
	height: 46px;
}
.selectLayout-item.item02 span {
	height: 30px;
}
.selectLayout-item.item02 .el-col:nth-child(1) span {
	height: 14px;
	margin-bottom: 2px;
}
.selectLayout-item.item03 span {
	height: 14px;
	margin-bottom: 2px;
}
.selectLayout-item.item05 span {
	height: 15px;
}
.selectLayout-item.item05 .el-col:first-child span {
	height: 14px;
	margin-bottom: 2px;
}
.selectLayout-item.item05 .el-col:last-child span {
	height: 14px;
	margin-top: 2px;
}
.selectLayout-item:hover {
	border-color: var(--el-color-primary);
}
.selectLayout-item.active {
	border-color: var(--el-color-primary);
}
.selectLayout-item.active span {
	background: var(--el-color-primary);
}

.dark {
	.widgets-aside {
		background: #2b2b2b;
	}
	.customize-overlay {
		background: rgba(43, 43, 43, 0.9);
	}
}

@media (max-width: 992px) {
	.customizing .widgets {
		transform: scale(1) !important;
	}
	.customizing .widgets-aside {
		width: 100%;
		position: absolute;
		top: 50%;
		right: 0;
		bottom: 0;
	}
	.customizing .widgets-wrapper {
		margin-right: 0;
	}
}
.noticebar {
	margin: 5px;
}
.editlayout {
	top: 18px;
	right: 25px;
	position: absolute;
}
</style>
