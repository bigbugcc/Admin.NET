<template>
	<div class="sys-cache-container">
		<div>
			<NoticeBar text="系统缓存数据管理，请慎重操作！" style="margin: 4px" />
		</div>

		<splitpanes class="default-theme">
			<pane size="20">
				<el-card shadow="hover" header="缓存列表" v-loading="state.loading" style="height: 100%" body-style="height:100%; overflow:auto">
					<template #header>
						<div class="card-header">
							<span>缓存列表</span>
							<div>
								<el-button icon="ele-Refresh" size="small" type="success" circle plain @click="handleQuery" v-auth="'sysCache:keyList'" />
								<el-button icon="ele-DeleteFilled" size="small" type="danger" circle plain @click="clearCache" v-auth="'sysCache:clear'"> </el-button>
							</div>
						</div>
					</template>
					<el-tree
						ref="treeRef"
						class="filter-tree"
						style="padding-bottom: 60px"
						:data="state.cacheData"
						node-key="id"
						:props="{ children: 'children', label: 'name' }"
						@node-click="nodeClick"
						:default-checked-keys="state.cacheData"
						highlight-current
						check-strictly
						default-expand-all
						accordion
					/>
				</el-card>
			</pane>
			<pane size="80">
				<el-card shadow="hover" header="缓存数据" v-loading="state.loading1" style="height: 100%" body-style="height:100%; overflow:auto">
					<template #header>
						<div class="card-header">
							<span>{{ `缓存数据${state.cacheKey ? `【${state.cacheKey}】` : ''}` }}</span>
							<el-button icon="ele-Delete" size="small" type="danger" @click="delCache" v-auth="'sysCache:delete'"> 删除缓存 </el-button>
						</div>
					</template>
					<vue-json-pretty :data="state.cacheValue" showLength showIcon showLineNumber showSelectController style="padding-bottom: 60px" />
				</el-card>
			</pane>
		</splitpanes>
	</div>
</template>

<script lang="ts" setup name="sysCache">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage, ElTree } from 'element-plus';
import NoticeBar from '/@/components/noticeBar/index.vue';
import VueJsonPretty from 'vue-json-pretty';
import 'vue-json-pretty/lib/styles.css';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';

import { getAPI } from '/@/utils/axios-utils';
import { SysCacheApi } from '/@/api-services';

const treeRef = ref<InstanceType<typeof ElTree>>();
const currentNode = ref<any>({});
const state = reactive({
	loading: false,
	loading1: false,
	cacheData: [] as any,
	cacheValue: undefined as any,
	cacheKey: undefined,
});

onMounted(async () => {
	await handleQuery();
});

// 查询操作
const handleQuery = async () => {
	state.cacheData = [];
	state.cacheValue = undefined;
	state.cacheKey = undefined;

	state.loading = true;
	var res = await getAPI(SysCacheApi).apiSysCacheKeyListGet();
	let keyList: any = res.data.result;

	// 构造树（以分号分割）
	for (let i = 0; i < keyList.length; i++) {
		let keyNames = keyList[i].split(':');
		let pName = keyNames[0];
		if (state.cacheData.filter((x: any) => x.name == pName).length === 0) {
			state.cacheData.push({
				id: keyNames.length > 1 ? 0 : keyList[i],
				name: pName,
				children: [],
			});
		}
		if (keyNames.length > 1) {
			let pNode = state.cacheData.filter((x: any) => x.name == pName)[0] || {};
			pNode.children.push({
				id: keyList[i],
				name: keyList[i].substr(pName.length + 1),
			});
		}
	}
	state.loading = false;
};

// 删除
const delCache = () => {
	if (currentNode.value.id == 0) {
		ElMessage.warning('禁止删除顶层缓存');
		return;
	}
	ElMessageBox.confirm(`确定删除缓存：【${currentNode.value.id}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysCacheApi).apiSysCacheDeleteKeyPost(currentNode.value.id);
			await handleQuery();
			state.cacheValue = undefined;
			state.cacheKey = undefined;
			ElMessage.success('删除成功');
		})
		.catch(() => {});
};

// 清空
const clearCache = () => {
	ElMessageBox.confirm(`确认清空所有缓存?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysCacheApi).apiSysCacheClearPost();
			await handleQuery();
			state.cacheValue = undefined;
			state.cacheKey = undefined;
			ElMessage.success('清空成功');
		})
		.catch(() => {});
};

// 树点击
const nodeClick = async (node: any) => {
	if (node.id == 0) return;

	currentNode.value = node;
	state.loading1 = true;
	var res = await getAPI(SysCacheApi).apiSysCacheValueKeyGet(node.id);
	// state.cacheValue = JSON.parse(res.data.result);
	var result = res.data.result;
	if (typeof result == 'string') {
		try {
			var obj = JSON.parse(result);
			if (typeof obj == 'object') {
				state.cacheValue = obj;
			} else {
				state.cacheValue = result;
			}
		} catch (e) {
			state.cacheValue = result;
		}
	} else {
		state.cacheValue = result;
	}

	state.cacheKey = node.id;
	state.loading1 = false;
};
</script>

<style lang="scss" scoped>
.card-header {
	display: flex;
	justify-content: space-between;
	align-items: center;
}
</style>
