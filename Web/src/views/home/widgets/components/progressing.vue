<template>
	<el-card shadow="hover" header="项目进度">
		<template #header>
			<el-icon style="display: inline; vertical-align: middle"> <ele-Odometer /> </el-icon>
			<span style=""> 项目进度 </span>
		</template>
		<div class="progress">
			<el-progress type="dashboard" :percentage="99.9" :width="160" color="var(--el-color-primary)">
				<template #default="{ percentage }">
					<div class="percentage-value">{{ percentage }}%</div>
					<div class="percentage-label">当前进度</div>
				</template>
			</el-progress>
		</div>
	</el-card>
</template>

<script lang="ts">
export default {
	title: '进度环',
	icon: 'ele-Odometer',
	description: '进度环原子组件演示',
};
</script>

<script setup lang="ts" name="progressing"></script>

<style scoped>
.progress {
	text-align: center;
}
.progress .percentage-value {
	font-size: 28px;
}
.progress .percentage-label {
	font-size: 12px;
	margin-top: 10px;
}
</style>
