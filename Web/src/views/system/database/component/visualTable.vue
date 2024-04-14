<template>
	<div class="sys-databaseVisual-container">
		<div style="height: calc(100vh - 60px)">
			<RelationGraph ref="graphRef" :options="graphOptions" :on-node-click="onNodeClick" :on-line-click="onLineClick">
				<template #graph-plug>
					<!-- To facilitate understanding, the CSS style code is directly embedded here. -->
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
							<div style="height: 5px; width: 80px; background-color: rgba(159, 23, 227, 0.65)"></div>
						</div>
						<div style="margin-left: 10px">
							一对一
							<div style="height: 5px; width: 80px; background-color: rgba(29, 169, 245, 0.76)"></div>
						</div>
					</div>
				</template>

				<template #canvas-plug>
					<!--- You can put some elements that are not allowed to be dragged here --->
				</template>

				<template #node="{ node }">
					<div style="width: 350px; background-color: #f39930">
						<!---------------- if node a ---------------->
						<div style="height: 30px; display: flex; align-items: center; justify-content: center">{{ node.text }} - 【{{ node.data.columns.length }}列】</div>
						<table class="c-data-table">
							<tr>
								<th>列名</th>
								<th>类型</th>
							</tr>
							<template v-for="column of node.data.columns" :key="column.columnName">
								<tr>
									<td>
										<div :id="`${node.id}-${column.columnName}`" style="background-color: var(--el-color-primary-light-3)">{{ column.columnName }}</div>
									</td>
									<td>{{ column.dataType }}</td>
								</tr>
							</template>
						</table>
					</div>
				</template>
			</RelationGraph>
		</div>
	</div>
</template>

<script lang="ts" setup name="databaseVisual">
import { onMounted, reactive, ref } from 'vue';
import { useRoute } from 'vue-router';

import RelationGraph from 'relation-graph/vue3';
import type { RGOptions, RGNode, RGLine, RGLink, RGUserEvent, RGJsonData, RelationGraphComponent } from 'relation-graph/vue3';

import { getAPI } from '/@/utils/axios-utils';
import { SysDatabaseApi } from '/@/api-services/api';
import { DbColumnOutput, DbTableInfo } from '/@/api-services/models';

const route = useRoute();
const state = reactive({
	loading: false,
	loading1: false,
	dbData: [] as any,
	configId: '' as any,
	tableData: [] as Array<DbTableInfo>,
	visualTable: [],
	visualRTable: [],
	tableName: '',
	columnData: [] as Array<DbColumnOutput>,
	queryParams: {
		name: undefined,
		code: undefined,
	},
	editPosTitle: '',
	appNamespaces: [] as Array<String>, // 存储位置
	//graphOptions: {
	//    defaultNodeBorderWidth: 0,
	//    defaultNodeColor: "rgba(238, 178, 94, 1)",
	//    allowSwitchLineShape: true,
	//    allowSwitchJunctionPoint: true,
	//    defaultLineShape: 1,
	//    layouts: [
	//      {
	//        label: "自动布局",
	//        layoutName: "force",
	//        layoutClassName: "seeks-layout-force",
	//        distance_coefficient: 3
	//      },
	//    ],
	//    defaultJunctionPoint: "border",
	//  },
});

onMounted(async () => {
	state.configId = route.query.configId;
	console.log(state.configId);

	showGraph();
});

const graphRef = ref<RelationGraphComponent | null>(null);
const graphOptions: RGOptions = {
	debug: false,
	allowSwitchLineShape: true,
	allowSwitchJunctionPoint: true,
	allowShowDownloadButton: true,
	defaultJunctionPoint: 'border',
	// placeOtherNodes: false,
	placeSingleNode: false,
	graphOffset_x: -200,
	graphOffset_y: 100,
	defaultLineMarker: {
		markerWidth: 20,
		markerHeight: 20,
		refX: 3,
		refY: 3,
		data: 'M 0 0, V 6, L 4 3, Z',
	},
	layout: {
		layoutName: 'fixed',
	},
	// You can refer to the parameters in "Graph" for setting here
};

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
				// Costomer key have to in data
				columns: visualColumnList.filter((col: any) => col.tableName === table.tableName),
			},
		};
	});
	const graphLines = columnRelationList.map((relation: any) => {
		return {
			from: relation.sourceTableName + '-' + relation.sourceColumnName, // HtmlElement id
			to: relation.targetTableName + '-' + relation.targetColumnName, // HtmlElement id
			color: relation.type === 'ONE_TO_ONE' ? 'rgba(29,169,245,0.76)' : 'rgba(159,23,227,0.65)',
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
	background-color: #1da9f5;
	color: #ffffff;
	border-radius: 5px;
}
</style>
