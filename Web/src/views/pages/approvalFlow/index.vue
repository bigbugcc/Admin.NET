<template>
    <div class="labApprovalFlow-container">
        <el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
            <el-form :model="state.queryParams" ref="queryForm" labelWidth="90">
                <el-row>
                    <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10">
                        <el-form-item label="关键字">
                            <el-input v-model="state.queryParams.searchKey" clearable="" placeholder="请输入模糊查询关键字"/>
                        </el-form-item>
                    </el-col>
                    <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="showAdvanceQueryUI">
                        <el-form-item label="编号">
                            <el-input v-model="state.queryParams.code" clearable="" placeholder="请输入编号"/>
                        </el-form-item>
                    </el-col>
                    <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="showAdvanceQueryUI">
                        <el-form-item label="名称">
                            <el-input v-model="state.queryParams.name" clearable="" placeholder="请输入名称"/>
                        </el-form-item>
                    </el-col>
                    <el-col :xs="24" :sm="12" :md="12" :lg="8" :xl="4" class="mb10" v-if="showAdvanceQueryUI">
                        <el-form-item label="备注">
                            <el-input v-model="state.queryParams.remark" clearable="" placeholder="请输入备注"/>
                        </el-form-item>
                    </el-col>
                    <el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb10">
                        <el-form-item>
                            <el-button-group style="display: flex; align-items: center;">
                                <el-button type="primary"  icon="ele-Search" @click="handleQuery" v-auth="'labApprovalFlow:page'"> 查询 </el-button>
                                <el-button icon="ele-Refresh" @click="() => state.queryParams = {}"> 重置 </el-button>
                                <el-button icon="ele-ZoomIn" @click="changeAdvanceQueryUI" v-if="!showAdvanceQueryUI" style="margin-left:5px;"> 高级查询 </el-button>
                                <el-button icon="ele-ZoomOut" @click="changeAdvanceQueryUI" v-if="showAdvanceQueryUI" style="margin-left:5px;"> 隐藏 </el-button>
                                <el-button type="primary" style="margin-left:5px;" icon="ele-Plus" @click="openAddLabApprovalFlow" v-auth="'labApprovalFlow:add'"> 新增 </el-button>
                            </el-button-group>
                        </el-form-item>
                    </el-col>
                </el-row>
            </el-form>
        </el-card>
        <el-card class="full-table" shadow="hover" style="margin-top: 8px">
            <el-table
                :data="state.tableData"
                style="width: 100%"
                v-loading="state.loading"
                tooltip-effect="light"
                row-key="id"
                border="">
                <el-table-column type="index" label="序号" width="55" align="center"/>
                <el-table-column prop="code" label="编号" width="140" show-overflow-tooltip="" />
                <el-table-column prop="name" label="名称" width="120" show-overflow-tooltip="" />
                <el-table-column prop="formJson" label="表单" align="center" width="120" show-overflow-tooltip="" >
                    <template #default="scope">
                        <el-button icon="ele-Edit" size="small" text="" type="primary" @click="openEditFormDialog(scope.row)" v-auth="'labCellInspectRecord:update'"> 表单 </el-button>
                    </template>
                </el-table-column>
                <el-table-column prop="flowJson" label="流程" align="center" width="120" show-overflow-tooltip="" >
                    <template #default="scope">
                        <el-button icon="ele-Edit" size="small" text="" type="primary" @click="openEditFlowDialog(scope.row)" v-auth="'labCellInspectRecord:update'"> 流程 </el-button>
                    </template>
                </el-table-column>
                <el-table-column label="修改记录" align="center" show-overflow-tooltip>
                    <template #default="scope">
                        <ModifyRecord :data="scope.row" />
                    </template>
                </el-table-column>
                <el-table-column label="操作" width="200" align="center" fixed="right" show-overflow-tooltip="" v-if="auth('approvalFlow:update') || auth('approvalFlow:delete')">
                    <template #default="scope">
                        <el-button icon="ele-View" size="small" text="" type="primary" @click="openDetailDialog(scope.row)" v-auth="'approvalFlow:detail'"> 查看 </el-button>
                        <el-button icon="ele-Edit" size="small" text="" type="primary" @click="openEditApprovalFlow(scope.row)" v-auth="'approvalFlow:update'"> 编辑 </el-button>
                        <el-button icon="ele-Delete" size="small" text="" type="primary" @click="delApprovalFlow(scope.row)" v-auth="'approvalFlow:delete'"> 删除 </el-button>
                    </template>
                </el-table-column>
            </el-table>
            <el-pagination
                v-model:currentPage="state.tableParams.page"
                v-model:page-size="state.tableParams.pageSize"
                :total="state.tableParams.total"
                :page-sizes="[10, 20, 50, 100, 200, 500]"
                small=""
                background=""
                @size-change="handleSizeChange"
                @current-change="handleCurrentChange"
                layout="total, sizes, prev, pager, next, jumper" />
            <detailDialog
                ref="detailDialogRef"
                :title="editTitle"
                @reloadTable="handleQuery" />
            <printDialog
                ref="printDialogRef"
                :title="editTitle"
                @reloadTable="handleQuery" />
            <editDialog
                ref="editDialogRef"
                :title="editTitle"
                @reloadTable="handleQuery" />
            <editFormDialog
                ref="editFormDialogRef"
                :title="editTitle"
                @reloadTable="handleQuery" />
            <editFlowDialog
                ref="editFlowDialogRef"
                :title="editTitle"
                @updateFlow="handleFlow"
                @reloadTable="handleQuery" />
        </el-card>
    </div>
</template>

<script lang="ts" setup="" name="approvalFlow">
    import { reactive, ref } from "vue";
    import { ElMessageBox, ElMessage } from "element-plus";
    import { auth } from '/@/utils/authFunction';
    import { formatDate } from '/@/utils/formatTime';

    import printDialog from '/@/views/system/print/component/hiprint/preview.vue';
    import editFormDialog from '/@/views/pages/approvalFlow/component/editFormDialog.vue';
    import detailDialog from '/@/views/pages/approvalFlow/component/detailDialog.vue';
    import editFlowDialog from '/@/views/pages/approvalFlow/component/editFlowDialog.vue';
    import editDialog from '/@/views/pages/approvalFlow/component/editDialog.vue';
    import ModifyRecord from '/@/components/table/modifyRecord.vue';

    // 接口
    import { getAPI } from '/@/utils/axios-utils';
    
    import { ApprovalFlowApi } from '/@/api-services/api';
    import { ApprovalFlowInput, ApprovalFlowOutput } from '/@/api-services/models';

    import commonFunction from '/@/utils/commonFunction';

    const showAdvanceQueryUI = ref(false);

    const detailDialogRef = ref();
    const editFormDialogRef = ref();
    const editFlowDialogRef = ref();
    const printDialogRef = ref();
    const editDialogRef = ref();

    const state = reactive({
		loading: false,
        tableData: [] as Array<ApprovalFlowOutput>,
		queryParams: {} as ApprovalFlowInput,
        tableParams: {
            page: 1,
            pageSize: 20,
            total: 0,
        }
	});
    const detailTitle = ref("");
    const editTitle = ref("");

    // 改变高级查询的控件显示状态
    const changeAdvanceQueryUI = () => {
        showAdvanceQueryUI.value = !showAdvanceQueryUI.value;
    }
    
    // 查询操作
    const handleQuery = async () => {
        state.loading = true;
        let params = Object.assign(state.queryParams, state.tableParams);
        var res = await getAPI(ApprovalFlowApi).apiApprovalFlowPagePost(params);
        state.tableData = res.data.result?.items ?? [];
        state.tableParams.total = res.data.result?.total;
        state.loading = false;
    };

    // 打开新增页面
    const openAddApprovalFlow = () => {
        editTitle.value = '添加审批流';
        editDialogRef.value.openDialog({ status: 1 });
    };

    // 打开编辑页面
    const openEditApprovalFlow = (row: ApprovalFlowOutput) => {
        editTitle.value = '编辑审批流';
        editDialogRef.value.openDialog(row);
     };
    
    // 打开打印页面
    const openEditDialog = (row: ApprovalFlowOutput) => {
        editTitle.value = '编辑审批流';
        editDialogRef.value.openDialog(row);
    };

    // 打开打印页面
    const openDetailDialog = (row: ApprovalFlowOutput) => {
        detailTitle.value = '查看审批流';
        detailDialogRef.value.openDialog(row);
    };
    const openEditFormDialog = (row: ApprovalFlowOutput) => {
        editFormTitle.value = '编辑表单';
        editFormDialogRef.value.openDialog(row);
    };

    const openEditFlowDialog = (row: ApprovalFlowOutput) => {
        editFlowTitle.value = '编辑流程';
        editFlowDialogRef.value.openDialog(row);
    };

    const handleFlow = (json: string) => {
        console.log(JSON.stringify(json));
        handleQuery();
    };
    // 删除
    const delApprovalFlow = (row: ApprovalFlowOutput) => {
        ElMessageBox.confirm(`确定要删除吗?`, "提示", {
            confirmButtonText: "确定",
            cancelButtonText: "取消",
            type: "warning",
        }).then(async () => {
            if (row.id) {
                await getAPI(ApprovalFlowApi).apiApprovalFlowDeletePost({ id: row.id });
                handleQuery();
                ElMessage.success("删除成功");
            }
        }).catch(() => {});
    };

    // 改变页面容量
    const handleSizeChange = (val: number) => {
        state.tableParams.pageSize = val;
        handleQuery();
    };

    // 改变页码序号
    const handleCurrentChange = (val: number) => {
        state.tableParams.page = val;
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

