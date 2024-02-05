<template>
	<div class="dm_ApplyDemo-container">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="queryParams" ref="queryForm" labelWidth="90">
				<el-row>
					<el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10">
						<el-form-item label="关键字">
							<el-input v-model="queryParams.searchKey" clearable="" placeholder="请输入模糊查询关键字" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="showAdvanceQueryUI">
						<el-form-item label="机构类型">
							<el-select clearable="" v-model="queryParams.orgType" placeholder="请选择机构类型">
								<el-option v-for="(item, index) in dl('org_type')" :key="index" :value="item.code" :label="`[${item.code}] ${item.value}`" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="showAdvanceQueryUI">
						<el-form-item label="申请号">
							<el-input v-model="queryParams.applyNO" clearable="" placeholder="请输入申请号" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="showAdvanceQueryUI">
						<el-form-item label="申请时间">
							<el-date-picker placeholder="请选择申请时间" value-format="YYYY/MM/DD" type="daterange" v-model="queryParams.applicatDateRange" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="showAdvanceQueryUI">
						<el-form-item label="申请金额">
							<el-input-number v-model="queryParams.amount" clearable="" placeholder="请输入申请金额" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="showAdvanceQueryUI"> </el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="showAdvanceQueryUI">
						<el-form-item label="备注">
							<el-input v-model="queryParams.remark" clearable="" placeholder="请输入备注" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="6" :lg="6" :xl="6" class="mb10">
						<el-form-item>
							<el-button-group>
								<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'dm_ApplyDemo:page'"> 查询 </el-button>
								<el-button icon="ele-Refresh" @click="() => (queryParams = {})"> 重置 </el-button>
								<el-button icon="ele-ZoomIn" @click="changeAdvanceQueryUI" v-if="!showAdvanceQueryUI"> 高级 </el-button>
								<el-button icon="ele-ZoomOut" @click="changeAdvanceQueryUI" v-if="showAdvanceQueryUI"> 隐藏 </el-button>
							</el-button-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="6" :lg="6" :xl="6" class="mb10"> 
								<el-button type="primary" icon="ele-Plus" @click="openAddDm_ApplyDemo" v-auth="'dm_ApplyDemo:add'"> 新增 </el-button>
								
								<el-button icon="ele-Download" @click="exportData" > 导出数据 </el-button>

								<ImportButton accept=".xlsx" url="/api/dm_ApplyDemo/import" @success="handleQuery">导入数据</ImportButton> 
					</el-col>
				</el-row>
			</el-form>
		</el-card>
		<el-card class="full-table" shadow="hover" style="margin-top: 8px">
			<el-table :data="tableData" style="width: 100%" v-loading="loading" tooltip-effect="light" row-key="id" @sort-change="sortChange" border="">
				<el-table-column type="index" label="序号" width="55" align="center" />
				<el-table-column prop="orgType" label="机构类型" width="105" show-overflow-tooltip="">
					<template #default="scope">
						<el-tag :type="di('org_type', scope.row.orgType)?.tagType"> {{ di('org_type', scope.row.orgType)?.value }} </el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="applyNO" label="申请号" width="105" show-overflow-tooltip="" />
				<el-table-column prop="applicatDate" label="申请时间" width="180" show-overflow-tooltip="" />
				<el-table-column prop="amount" label="申请金额" width="90" show-overflow-tooltip="" />
				<el-table-column prop="isNotice" label="是否通知" width="120" show-overflow-tooltip="">
					<template #default="scope">
						<el-tag v-if="scope.row.isNotice"> 是 </el-tag>
						<el-tag type="danger" v-else> 否 </el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="remark" label="备注" width="90" show-overflow-tooltip="" />
				<el-table-column label="操作" width="140" align="center" fixed="right" show-overflow-tooltip="" v-if="auth('dm_ApplyDemo:edit') || auth('dm_ApplyDemo:delete')">
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text="" type="primary" @click="openEditDm_ApplyDemo(scope.row)" v-auth="'dm_ApplyDemo:edit'"> 编辑 </el-button>
						<el-button icon="ele-Delete" size="small" text="" type="primary" @click="delDm_ApplyDemo(scope.row)" v-auth="'dm_ApplyDemo:delete'"> 删除 </el-button>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination
				v-model:currentPage="tableParams.page"
				v-model:page-size="tableParams.pageSize"
				:total="tableParams.total"
				:page-sizes="[10, 20, 50, 100, 200, 500]"
				small=""
				background=""
				@size-change="handleSizeChange"
				@current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper"
			/>
			<printDialog ref="printDialogRef" :title="printDm_ApplyDemoTitle" @reloadTable="handleQuery" />
			<editDialog ref="editDialogRef" :title="editDm_ApplyDemoTitle" @reloadTable="handleQuery" />
		</el-card>
	</div>
</template>

<script lang="ts" setup name="dm_ApplyDemo">
import { ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { auth } from '/@/utils/authFunction';
import { getDictDataItem as di, getDictDataList as dl } from '/@/utils/dict-utils';

import printDialog from '/@/views/system/print/component/hiprint/preview.vue';
import editDialog from '/@/views/main/dm_ApplyDemo/component/editDialog.vue';
import { pageDm_ApplyDemo, deleteDm_ApplyDemo,exportDm_ApplyDemo as exportData } from '/@/api/main/dm_ApplyDemo';
import ImportButton from '/@/components/importButton/index.vue';

const showAdvanceQueryUI = ref(false);
const printDialogRef = ref();
const editDialogRef = ref();
const loading = ref(false);
const tableData = ref<any>([]);
const queryParams = ref<any>({});
const tableParams = ref({
	page: 1,
	pageSize: 10,
	total: 0,
});

const printDm_ApplyDemoTitle = ref('');
const editDm_ApplyDemoTitle = ref('');

// 改变高级查询的控件显示状态
const changeAdvanceQueryUI = () => {
	showAdvanceQueryUI.value = !showAdvanceQueryUI.value;
};

// 查询操作
const handleQuery = async () => {
	loading.value = true;
	var res = await pageDm_ApplyDemo(Object.assign(queryParams.value, tableParams.value));
	tableData.value = res.data.result?.items ?? [];
	tableParams.value.total = res.data.result?.total;
	loading.value = false;
};

// 列排序
const sortChange = async (column: any) => {
	queryParams.value.field = column.prop;
	queryParams.value.order = column.order;
	await handleQuery();
};

// 打开新增页面
const openAddDm_ApplyDemo = () => {
	editDm_ApplyDemoTitle.value = '添加申请示例';
	editDialogRef.value.openDialog({});
};

// 打开打印页面
const openPrintDm_ApplyDemo = async (row: any) => {
	printDm_ApplyDemoTitle.value = '打印申请示例';
};

// 打开编辑页面
const openEditDm_ApplyDemo = (row: any) => {
	editDm_ApplyDemoTitle.value = '编辑申请示例';
	editDialogRef.value.openDialog(row);
};

// 删除
const delDm_ApplyDemo = (row: any) => {
	ElMessageBox.confirm(`确定要删除吗?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await deleteDm_ApplyDemo(row);
			handleQuery();
			ElMessage.success('删除成功');
		})
		.catch(() => {});
};

// 改变页面容量
const handleSizeChange = (val: number) => {
	tableParams.value.pageSize = val;
	handleQuery();
};

// 改变页码序号
const handleCurrentChange = (val: number) => {
	tableParams.value.page = val;
	handleQuery();
};

handleQuery();
</script>
<style scoped>
:deep(.el-ipnut),
:deep(.el-select),
:deep(.el-input-number) {
	width: 100%;
}
</style>
