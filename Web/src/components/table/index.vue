<template>
	<div class="table-container">
		<div v-if="!hideTool" class="table-header mb8">
			<div>
				<slot name="command"></slot>
			</div>
			<div v-loading="state.exportLoading" class="table-footer-tool">
                <el-dropdown v-if="!config.hideExport" split-button trigger="click" @click="onExportTable">
                    <!-- <el-button icon="ele-Download"  /> -->
                     导出
                    <template #dropdown>
                        <el-dropdown-menu>
                            <el-dropdown-item @click="onExportTable">导出本页数据</el-dropdown-item>
                            <el-dropdown-item @click="onExportTableAll">导出全部数据</el-dropdown-item>
                        </el-dropdown-menu>
                    </template>
                </el-dropdown>

                <el-button-group>
                    <el-button icon="ele-Refresh" v-if="!config.hideRefresh" title="刷新" @click="() => onRefreshTable()" />
                    <el-button icon="ele-Switch" v-if="state.haveFixed" :title="state.switchFixedContent" :color="state.fixedIconColor" @click="switchFixed" />
                    <el-button icon="ele-Printer" v-if="!config.hidePrint" title="打印" @click="onPrintTable"  />
                    <el-popover v-if="!config.hideSet" placement="bottom-end" trigger="click" transition="el-zoom-in-top" popper-class="table-tool-popper" :width="200" :persistent="false" @show="onSetTable">
                        <template #reference>
                            <el-button icon="ele-Setting"  />
                        </template>
                        <template #default>
                            <div class="tool-box">
                                <el-checkbox v-model="state.checkListAll" :indeterminate="state.checkListIndeterminate" class="ml10 mr1" label="列显示" @change="onCheckAllChange" />
                                <el-checkbox v-model="getConfig.isSerialNo" class="ml12 mr1" label="序号" />
                                <el-checkbox v-if="getConfig.showSelection" v-model="getConfig.isSelection" class="ml12 mr1" label="多选" />
                                <el-tooltip content="拖动进行排序" placement="top-start">
                                    <SvgIcon style="position: absolute; right: 15px; line-height: 32px;" name="fa fa-question-circle-o" :size="17" class="ml11 cursor-pointer" color="#909399" />
                                </el-tooltip>
                            </div>
                            <el-scrollbar>
                                <div ref="toolSetRef" class="tool-sortable">
                                    <div class="tool-sortable-item" v-for="v in columns" :key="v.prop" :data-key="v.prop" :data-fixed="v.hideCheck ?? false">
                                        <i class="fa fa-arrows-alt handle"></i>
                                        <el-checkbox v-model="v.isCheck" size="default" class="ml12 mr8" :label="v.label" @change="onCheckChange" />
                                    </div>
                                </div>
                            </el-scrollbar>
                        </template>
                    </el-popover>
                </el-button-group>
			</div>
		</div>
		<el-table
            ref="tableRef"
            :data="state.data"
            :border="setBorder"
            :stripe="setStripe"
            v-bind="$attrs"
            row-key="id"
            default-expand-all
            style="width: 100%"
            v-loading="state.loading"
            :default-sort="defaultSort"
            @selection-change="onSelectionChange"
            @sort-change="sortChange"
		>
			<el-table-column type="selection" :reserve-selection="true" :width="30" v-if="config.isSelection && config.showSelection" />
			<el-table-column type="index" :fixed="state.currentFixed && state.serialNoFixed" label="序号" align="center" :width="60" v-if="config.isSerialNo" />
			<el-table-column v-for="(item, index) in setHeader" :key="index" v-bind="item">
				<template #header v-if="!item.children && $slots[item.prop]">
					<slot :name="`${item.prop}header`" />
				</template>
				<!-- 自定义列插槽，插槽名为columns属性的prop -->
				<template #default="scope" v-if="!item.children && $slots[item.prop]">
					<formatter v-if="item.formatter" :fn="item.formatter(scope.row, scope.column, scope.cellValue, scope.index)"> </formatter>
					<slot v-else :name="item.prop" v-bind="scope"></slot>
				</template>
				<template v-else-if="!item.children" v-slot="scope">
					<formatter v-if="item.formatter" :fn="item.formatter(scope.row, scope.column, scope.cellValue, scope.index)"> </formatter>
					<template v-else-if="item.type === 'image'">
						<el-image
						    :style="{ width: `${item.width}px`, height: `${item.height}px` }"
						    :src="scope.row[item.prop]"
						    :zoom-rate="1.2"
						    :preview-src-list="[scope.row[item.prop]]"
						    preview-teleported
						    fit="cover"
						/>
					</template>
					<template v-else>
						{{ getProperty(scope.row, item.prop) }}
					</template>
				</template>
                <template v-else-if="item.children" v-slot="scope">
                    <!-- 多表头支持，只需要在列定义里面添加children数组定义，数组内的定义与column完全相同 -->
                    <el-table-column v-for="(childrenItem, childrenIndex) in item.children" :key="childrenIndex" v-bind="childrenItem">
                        <template #default="scope" v-if="$slots[childrenItem.prop]">
                            <formatter v-if="childrenItem.formatter" :fn="childrenItem.formatter(scope.row, scope.column, scope.cellValue, scope.index)"> </formatter>
                            <slot v-else :name="childrenItem.prop" v-bind="scope"></slot>
                        </template>
                        <template v-else v-slot="scope">
                            <formatter v-if="childrenItem.formatter" :fn="childrenItem.formatter(scope.row, scope.column, scope.cellValue, scope.index)"> </formatter>
                            <template v-else-if="childrenItem.type === 'image'">
                                <el-image
                                    :style="{ width: `${childrenItem.width}px`, height: `${childrenItem.height}px` }"
                                    :src="scope.row[childrenItem.prop]"
                                    :zoom-rate="1.2"
                                    :preview-src-list="[scope.row[childrenItem.prop]]"
                                    preview-teleported
                                    fit="cover"
                                />
                            </template>
                            <template v-else>
                                {{ getProperty(scope.row, childrenItem.prop) }}
                            </template>
                        </template>
                    </el-table-column>
                </template>
			</el-table-column>
		</el-table>
		<div v-if="!config.hidePagination && state.showPagination" class="table-footer mt2">
			<el-pagination 
                v-model:current-page="state.page.page"
                v-model:page-size="state.page.pageSize"
                size="small"
                :pager-count="5"
                :page-sizes="config.pageSizes"
                :total="state.total"
                layout="total, sizes, prev, pager, next, jumper"
                background
                @size-change="onHandleSizeChange"
                @current-change="onHandleCurrentChange"
            >
            </el-pagination>
		</div>
	</div>
</template>

<script setup lang="ts" name="netxTable">
import { reactive, computed, nextTick, ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import Sortable from 'sortablejs';
import { storeToRefs } from 'pinia';
import printJs from 'print-js';
//import { EmptyObjectType } from "/@/types/global";
import formatter from '/@/components/table/formatter.vue';
import { useThemeConfig } from '/@/stores/themeConfig';
import { exportExcel } from '/@/utils/exportExcel';  //TODO: 此包会引起浏览器控制台报 Module "stream" has been externalized for browser compatibility. Cannot access "stream.Readable" in client code. 警告，建议替换

// 定义父组件传过来的值
const props = defineProps({
	// 获取数据的方法，由父组件传递
	getData: {
		type: Function,
		required: true,
	},
	// 列属性，和elementUI的Table-column 属性相同，附加属性：isCheck-是否默认勾选展示，hideCheck-是否隐藏该列的可勾选和拖拽
	columns: {
		type: Array<any>,
		default: () => [],
	},
	// 配置项：isBorder-是否显示表格边框，isSerialNo-是否显示表格序号，showSelection-是否显示表格可多选，isSelection-是否默认选中表格多选，pageSize-每页条数，hideExport-是否隐藏导出按钮，exportFileName-导出表格的文件名，空值默认用应用名称作为文件名
	config: {
		type: Object,
		default: () => ({}),
	},
	// 筛选参数
	param: {
		type: Object,
		default: () => ({}),
	},
	// 默认排序方式，{prop:"排序字段",order:"ascending or descending"}
	defaultSort: {
		type: Object,
		default: () => ({}),
	},
	// 导出报表自定义数据转换方法，不传按字段值导出
	exportChangeData: {
		type: Function,
	},
	// 打印标题
	printName: {
		type: String,
		default: () => '',
	},
});

// 定义子组件向父组件传值/事件，pageChange-翻页事件，selectionChange-表格多选事件，可以在父组件处理批量删除/修改等功能，sortHeader-拖拽列顺序事件
const emit = defineEmits(['pageChange', 'selectionChange', 'sortHeader']);

// 定义变量内容
const toolSetRef = ref();
const tableRef = ref();
const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);
const state = reactive({
	data: [] as Array<EmptyObjectType>,
	loading: false,
	exportLoading: false,
	total: 0,
	page: {
		page: 1,
		pageSize: 50,
		field: '',
		order: '',
	},
	showPagination: true,
	selectlist: [] as EmptyObjectType[],
	checkListAll: true,
	checkListIndeterminate: false,
	oldColumns: [] as EmptyObjectType[],
	columns: [] as EmptyObjectType[],
	haveFixed: false,
	currentFixed: false,
	serialNoFixed: false,
	switchFixedContent: '取消固定列',
	fixedIconColor: themeConfig.value.primary,
});

const hideTool = computed(() => {
	return props.config.hideTool ?? false;
});

const getProperty = (obj: any, property: any) => {
	const keys = property.split('.');
	let value = obj;
	for (const key of keys) {
		value = value[key];
	}
	return value;
};

// 设置边框显示/隐藏
const setBorder = computed(() => {
	return props.config.isBorder ? true : false;
});
// 设置斑马纹显示/隐藏
const setStripe = computed(() => {
	return props.config.isStripe ? true : false;
});
// 获取父组件 配置项（必传）
const getConfig = computed(() => {
	return props.config;
});
// 设置 tool header 数据
const setHeader = computed(() => {
	return state.columns.filter((v) => v.isCheck);
});
// tool 列显示全选改变时
const onCheckAllChange = <T,>(val: T) => {
	if (val) state.columns.forEach((v) => (v.isCheck = true));
	else state.columns.forEach((v) => (v.isCheck = false));
	state.checkListIndeterminate = false;
};
// tool 列显示当前项改变时
const onCheckChange = () => {
	const headers = state.columns.filter((v) => v.isCheck).length;
	state.checkListAll = headers === state.columns.length;
	state.checkListIndeterminate = headers > 0 && headers < state.columns.length;
};
// 表格多选改变时
const onSelectionChange = (val: EmptyObjectType[]) => {
	state.selectlist = val;
	emit('selectionChange', state.selectlist);
};
// 分页改变
const onHandleSizeChange = (val: number) => {
	state.page.pageSize = val;
	onRefreshTable();
	emit('pageChange', state.page);
};
// 改变当前页
const onHandleCurrentChange = (val: number) => {
	state.page.page = val;
	onRefreshTable();
	emit('pageChange', state.page);
};
// 列排序
const sortChange = (column: any) => {
	state.page.field = column.prop;
	state.page.order = column.order;
	onRefreshTable();
};
// 重置列表
const pageReset = () => {
	tableRef.value.clearSelection();
	state.page.page = 1;
	onRefreshTable();
};
// 导出当前页
const onExportTable = () => {
	if (setHeader.value.length <= 0) return ElMessage.error('没有勾选要导出的列');
	exportData(state.data);
};
// 全部导出
const onExportTableAll = async () => {
	if (setHeader.value.length <= 0) return ElMessage.error('没有勾选要导出的列');
	state.exportLoading = true;
	const param = Object.assign({}, props.param, { page: 1, pageSize: 9999999 });
	const res = await props.getData(param);
	state.exportLoading = false;
	const data = res.result?.items ?? [];
	exportData(data);
};
// 导出方法
const exportData = (data: Array<EmptyObjectType>) => {
	if (data.length <= 0) return ElMessage.error('没有数据可以导出');
	state.exportLoading = true;
	let exportData = JSON.parse(JSON.stringify(data));
	if (props.exportChangeData) {
		exportData = props.exportChangeData(exportData);
	}
	exportExcel(
			exportData,
			`${props.config.exportFileName ? props.config.exportFileName : themeConfig.value.globalTitle}_${new Date().toLocaleString()}`,
			setHeader.value.filter((item) => {
				return item.type != 'action';
			}),
			'导出数据'
	);
	state.exportLoading = false;
};
// 打印
const onPrintTable = () => {
    let printDiv = document.createElement('div');
    let printTitle = document.createElement('div');
    let printTable = document.createElement('table');
    let printTableHeader = document.createElement('thead');
    let printTableBody = document.createElement('tbody');

    // 构建表头
    setHeader.value.forEach((col: EmptyObjectType) => {
        if (col.prop === 'action' || !col.isCheck) {
            return;
        }
        let th = document.createElement('th');
            th.innerText = col.label;
            th.classList.add('print-table-th');
            th.style.width = col.width ? col.width + 'px' : 'auto';
            th.style.minWidth = col.minWidth ? col.minWidth + 'px' : 'auto';
            printTableHeader.appendChild(th);
    });

    // 构建表体
    state.data.forEach((row: EmptyObjectType) => {
        let tr = document.createElement('tr');
        tr.classList.add('print-table-tr');
        setHeader.value.forEach((col: EmptyObjectType) => {
            if (col.prop === 'action' || !col.isCheck) {
                return;
            }

            let td = document.createElement('td');
            td.classList.add('print-table-td');
            if (col.type === 'image') {
                let img = document.createElement('img');
                img.classList.add('print-table-img');

                // img.src = row[col.prop];
                // img.style.width = col.width + 'px';
                // img.style.height = col.height + 'px';
                td.appendChild(img);
                tr.appendChild(td);
            } else {
                td.innerText = getProperty(row, col.prop);
                td.style.textAlign = col.align ? col.align : 'left';
                tr.appendChild(td);
            }
        });
        printTableBody.appendChild(tr);
    });

    printTable.appendChild(printTableHeader);
    printTable.appendChild(printTableBody);
    printTable.classList.add('print-table');

    // 构建打印标题
    if(props.config.printName) {
        printTitle.classList.add('print-table-title');
        printTitle.innerText = props.config.printName;
        printDiv.appendChild(printTitle);
    } else {
        printTitle.style.display = 'none';
    }

    printDiv.appendChild(printTable);
    
    printJs({
        printable: printDiv,
        type: 'html',
        //header: props.config.printName,
        scanStyles: false,
        css: ['/@/theme/media/printTable.css'],
    })

    printDiv.remove()
};

// 拖拽设置
const onSetTable = () => {
	nextTick(() => {
		const sortable = Sortable.create(toolSetRef.value, {
			//handle: '.handle',
			dataIdAttr: 'data-key',
			animation: 150,
            onStart: () => {
                // 为了区分固定列和非固定列，拖动时给非固定列加背景色
                toolSetRef.value.children.forEach((element: HTMLElement) => {
                    if(element.dataset.fixed === 'false') {
                        element.classList.add('tool-sortable-item-draggable');
                    }
                });
            },
            onMove: (evt) => {
                // 禁止固定列拖动
                const srcItem = evt.dragged.dataset.fixed; // 拖动项
                const targetItem = evt.related.dataset.fixed; // 目标位置(禁止拖动到固定列位置)
                if(srcItem === 'true' || targetItem === 'true') {
                    return false;
                }
            },
			onEnd: () => {
				const headerList: EmptyObjectType[] = [];
				sortable.toArray().forEach((val: any) => {
					state.columns.forEach((v) => {
						if (v.prop === val) headerList.push({ ...v });
					});
				});
				//emit('sortHeader', headerList);
                state.columns = headerList;

                // 清除拖动时加的背景色
                toolSetRef.value.children.forEach((element: HTMLElement) => {
                    element.classList.remove('tool-sortable-item-draggable');
                });
			}
		});
	});
};

const onRefreshTable = async () => {
	state.loading = true;
	let param = Object.assign({}, props.param, { ...state.page });
	Object.keys(param).forEach((key) => param[key] === undefined && delete param[key]);
	const res = await props.getData(param);
	state.loading = false;
	if (res && res.result && res.result.items) {
		state.showPagination = true;
		state.data = res.result?.items ?? [];
		state.total = res.result?.total ?? 0;
	} else {
		state.showPagination = false;
		state.data = res && res.result ? res.result : [];
	}
};

const toggleSelection = (row: any, statu?: boolean) => {
	tableRef.value!.toggleRowSelection(row, statu);
};

const getTableData = () => {
	return state.data;
};

const setTableData = (data: Array<EmptyObjectType>, add: boolean = false) => {
	if (add) {
		// 追加, 去重
		var repeat = false;
		for (let newItem of data) {
			repeat = false;
			for (let item of state.data) {
				if (newItem.id === item.id) {
					repeat = true;
					break;
				}
			}
			if (!repeat) {
				state.data.push(newItem);
			}
		}
	} else {
		state.data = data;
	}
};

const clearFixed = () => {
	for (let item of state.columns) delete item['fixed'];
};

const switchFixed = () => {
	state.currentFixed = !state.currentFixed;
	state.switchFixedContent = state.currentFixed ? '取消固定列' : '启用固定列';
	if (state.currentFixed) {
		state.fixedIconColor = themeConfig.value.primary;
		state.columns = JSON.parse(JSON.stringify(state.oldColumns));
	} else {
		state.fixedIconColor = '';
		clearFixed();
	}
};

const refreshColumns = () => {
	state.oldColumns = JSON.parse(JSON.stringify(props.columns));
	state.columns = props.columns;
	for (let item of state.columns) {
		if (item.fixed !== undefined) {
			state.haveFixed = true;
			state.currentFixed = true;
			if (item.fixed == 'left') {
				state.serialNoFixed = true;
				break;
			}
		}
	}
};

onMounted(() => {
	if (props.defaultSort) {
		state.page.field = props.defaultSort.prop;
		state.page.order = props.defaultSort.order;
	}
	state.page.pageSize = props.config.pageSize ?? 10;
	refreshColumns();
	onRefreshTable();
});

const handleList = onRefreshTable;

// 暴露变量
defineExpose({
	pageReset,
	handleList,
	toggleSelection,
	getTableData,
	setTableData,
	refreshColumns,
});
</script>

<style scoped lang="scss">
@import url('/@/theme/tableTool.scss');
.table-container {
	display: flex !important;
	flex-direction: column;
	height: 100%;

	.el-table {
		flex: 1;
	}

	.table-footer {
		display: flex;
		justify-content: flex-end;
	}

	.table-header {
		display: flex;

		.table-footer-tool {
			flex: 1;
			display: flex;
			align-items: center;
			justify-content: flex-end;
            gap: 8px;
		}
	}
}
</style>