<template>
	<div class="sys-databaseVisual-container" style="height: calc(100vh - 60px)">
		<RelationGraph ref="graphRef" :options="graphOptions" :on-node-click="onNodeClick" :on-line-click="onLineClick">
			<template #graph-plug>
				<div
					style="
						z-index: 300;
						position: absolute;
						left: 10px;
						top: calc(100% - 50px);
						font-size: 12px;
						background-color: #ffffff;
						border: #efefef solid 1px;
						border-radius: 10px;
						width: 260px;
						height: 40px;
						display: flex;
						align-items: center;
						justify-content: center;
					"
				>
					图例：
					<div>
						一对多
						<div style="height: 5px; width: 80px; background-color: rgba(0, 255, 0, 0.5)"></div>
					</div>
					<div style="margin-left: 10px">
						一对一
						<div style="height: 5px; width: 80px; background-color: rgba(255, 0, 0, 0.5)"></div>
					</div>
				</div>
			</template>

			<template #canvas-plug>
				<!--- You can put some elements that are not allowed to be dragged here --->
			</template>

			<template #node="{ node }">
				<div style="width: 500px; background-color: #f39930">
					<div style="height: 30px; display: flex; align-items: center; justify-content: center">{{ node.text }} - 【{{ node.data.columns.length }}列】</div>
					<table class="c-data-table">
						<tr>
							<th>列名</th>
							<th>类型</th>
							<th>长度</th>
							<th>描述</th>
						</tr>
						<template v-for="column of node.data.columns" :key="column.columnName">
							<tr>
								<td>
									<div :id="`${node.id}-${column.columnName}`">{{ column.columnName }}</div>
								</td>
								<td>{{ column.dataType }}</td>
								<td>{{ column.dataLength }}</td>
								<td>{{ column.columnDescription }}</td>
							</tr>
						</template>
					</table>
				</div>
			</template>
		</RelationGraph>
	</div>
</template>

<script lang="ts" setup name="databaseVisual">
import { onMounted, reactive, ref } from 'vue';
import { useRoute } from 'vue-router';

import RelationGraph from 'relation-graph/vue3';
import type { RGOptions, RGNode, RGLine, RGLink, RGUserEvent, RGJsonData, RelationGraphComponent } from 'relation-graph/vue3';

import { getAPI } from '/@/utils/axios-utils';
import { SysDatabaseApi } from '/@/api-services/api';

const route = useRoute();
const state = reactive({
	loading: false,
	configId: '' as any,
});

const graphRef = ref<RelationGraphComponent | null>(null);
const graphOptions: RGOptions = {
	defaultJunctionPoint: 'border',
};

onMounted(async () => {
	state.configId = route.query.configId;
	showGraph();
});

// 获取可视化表和字段
const showGraph = async () => {
	var res = await getAPI(SysDatabaseApi).apiSysDatabaseVisualDbTableGet();
	const visualTableList: any = res.data.result?.visualTableList;
	const visualColumnList: any = res.data.result?.visualColumnList;
	const columnRelationList: any = res.data.result?.columnRelationList;

	const graphNodes = visualTableList.map((table: any) => {
		const { tableName, tableComents, x, y } = table;
		return {
			id: tableName,
			text: tableComents,
			x,
			y,
			nodeShape: 1,
			data: {
				columns: visualColumnList.filter((col: any) => col.tableName === table.tableName),
			},
		};
	});
	const graphLines = columnRelationList.map((relation: any) => {
		return {
			from: relation.sourceTableName + '-' + relation.sourceColumnName, // HtmlElement id
			to: relation.targetTableName + '-' + relation.targetColumnName, // HtmlElement id
			color: relation.type === 'ONE_TO_ONE' ? 'rgba(255,0,0,0.5)' : 'rgba(0,255,0,0.5)',
			text: '',
			fromJunctionPoint: 'left',
			toJunctionPoint: 'lr',
			lineShape: 6,
			lineWidth: 3,
		};
	});
	const graphJsonData: RGJsonData = {
		nodes: graphNodes,
		lines: [],
		elementLines: graphLines,
	};
	const graphInstance = graphRef.value?.getInstance();
	if (graphInstance) {
		await graphInstance.setJsonData(graphJsonData);
		await graphInstance.moveToCenter();
		await graphInstance.zoomToFit();
	}
};

const onNodeClick = (nodeObject: RGNode, $event: RGUserEvent) => {
	console.log('onNodeClick:', nodeObject);
};

const onLineClick = (lineObject: RGLine, linkObject: RGLink, $event: RGUserEvent) => {
	console.log('onLineClick:', lineObject);
};
</script>

<style lang="scss" scoped>
::v-deep(.relation-graph) {
	.rel-node-shape-1 {
		overflow: hidden;
	}
}
.c-data-table {
	background-color: #ffffff;
	border-collapse: collapse;
	width: 100%;
}
.c-data-table td,
.c-data-table th {
	border: 1px solid #f39930;
	color: #333333;
	padding: 5px;
	padding-left: 20px;
	padding-right: 20px;
}
.c-data-table td div,
.c-data-table th div {
	background-color: var(--el-color-primary-light-3);
	color: #ffffff;
	border-radius: 5px;
}
</style>
