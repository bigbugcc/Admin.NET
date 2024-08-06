<template>
    <div class="h-full">
        <fs-crud ref="crudRef" v-bind="crudBinding">
            <template #pagination-left>
                <fs-button icon="ion:trash-outline" @click="handleBatchDelete" />
            </template>
            <template #cell_url="scope">
                <n-tooltip trigger="hover">
                    <template #trigger>
                        <n-button>预览 </n-button>
                    </template>
                    <n-image width="120px" height="120px" :src="baseURL + '/' + scope.row.url"></n-image>
                </n-tooltip>
            </template>
        </fs-crud>
    </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, ref, nextTick } from 'vue';
import { useFs, useExpose, useCrud } from '@fast-crud/fast-crud';
import createCrudOptions from './crud';
import { getAPI } from '/@/utils/axios-utils';
import { SysNoticeApi } from '/@/api-services/api';
import { ElMessage, ElMessageBox } from 'element-plus';
const baseURL = import.meta.env.VITE_API_URL;
export default defineComponent({
    name: 'ComponentCrud',
    setup() {
        const crudRef = ref();
        const crudBinding = ref();
        const { expose } = useExpose({ crudRef, crudBinding });
        const { crudOptions, selectedIds } = createCrudOptions({ expose });
        const { resetCrudOptions } = useCrud({ expose, crudOptions });
        onMounted(() => {
            expose.doRefresh();
        });
        const handleBatchDelete = async () => {
            if (selectedIds.value?.length > 0) {
                // ElMessageBox.confirm(`确定要批量删除这${selectedIds.value.length}条记录吗`, '确认', {
                //     confirmButtonText: '确定',
                //     cancelButtonText: '取消',
                //     type: 'info',
                // }).then(async () => {
                //        await delBatchSysFile(selectedIds.value);
                //       message.success('删除成功');
                //       selectedIds.value = [];
                //       await expose.doRefresh();
                //         ElMessage.success('删除成功');
                //     })
                //     .catch(() => { });
            } else {
                // ElMessage.success('请勾选要删除的记录');
            }
        };
        return {
            crudBinding,
            crudRef,
            handleBatchDelete,
            baseURL
        };
    }
});
</script>