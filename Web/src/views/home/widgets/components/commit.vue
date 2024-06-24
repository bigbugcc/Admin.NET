<template>
	<el-card shadow="hover" header="更新记录">
		<template #header>
			<el-button type="primary" icon="ele-Refresh" round @click="refresh">更新记录</el-button>
		</template>
		<div class="commit" v-loading="state.loading" >
			<el-timeline style="max-width: 600px" v-if="state.list.length > 0">
				<el-timeline-item v-for="(item, index) in state.list" :key="index" :timestamp="formatDate(new Date(item.commit.committer.date), 'YYYY-mm-dd HH:MM:SS')">
					<el-link :href="item.html_url" target="_blank"> {{ item.commit.message }}</el-link>
				</el-timeline-item>
			</el-timeline>
			<el-empty v-else description="空"></el-empty>
		</div>
	</el-card>
</template>

<script lang="ts">
export default {
	title: '更新记录',
	icon: 'ele-Message',
	description: '当前项目更新记录',
};
</script>

<script setup lang="ts" name="commit">
import { reactive, onMounted } from 'vue';
import { formatDate } from '/@/utils/formatTime';

const state = reactive({
	loading: false,
	list: [] as any,
});

const getList = () => {
	axios({
		method: 'get',
		url: 'https://gitee.com/api/v5/repos/zuohuaijun/Admin.NET/commits',
		params: {
			page: 1,
			per_page: 10,
		},
	}).then((res: any) => {
		state.list = res.data;
		state.loading = false;
	});
};

const refresh = () => {
	state.loading = true;
	getList();
};

onMounted(() => {
	state.loading = true;
	getList();
});
</script>

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