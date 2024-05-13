<template>
	<div v-if="state.pageLoading">
		<el-main>
			<el-card shadow="never">
				<el-skeleton :rows="1"></el-skeleton>
			</el-card>
			<el-card shadow="never" style="margin-top: 15px">
				<el-skeleton></el-skeleton>
			</el-card>
		</el-main>
	</div>
	<div v-else>
		<widgets></widgets>
	</div>
</template>

<script setup lang="ts" name="homePage">
import { defineAsyncComponent, onMounted, reactive } from 'vue';
// import { useHomepage } from 'src/stores/homepage';

const widgets = defineAsyncComponent(() => import('./widgets/index.vue'));
// const homepageStore = useHomepage();

const state = reactive({
	name: 'dashboard',
	pageLoading: true,
	dashboard: '0',
});

onMounted(() => {
	state.pageLoading = false;
});
</script>

<style></style>
