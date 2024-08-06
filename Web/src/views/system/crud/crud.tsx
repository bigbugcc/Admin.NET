
import { dict, compute } from '@fast-crud/fast-crud';
import { shallowRef, ref } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysNoticeApi } from '/@/api-services/api';
import { ElMessage } from 'element-plus';
export default function ({ expose }) {
    const pageRequest = async (query) => {
        const params = {
            page: query.currentPage,
            pageSize: query.pageSize,
            field: query.field,
            order: query.order,
            descStr: 'desc'
        } as PageFileInput;
        const result = await getAPI(SysNoticeApi).apiSysNoticePagePost(params);
        return result;
    };
    const editRequest = async ({ form, row }: EditReq) => {
        if (form.id == null) {
            form.id = row.id;
        }
        return await getAPI(SysNoticeApi)
            .apiSysNoticeUpdatePost(form)
            .then((rsp: any) => {
                if (rsp.data.code == 200) {
                    ElMessage.success('修改成功！');
                } else {
                    ElMessage.error('修改失败：' + rsp.data.message);
                }
            });
    };
    const delRequest = async ({ row }: DelReq) => {
        return await getAPI(SysNoticeApi)
            .apiSysNoticeDeletePost(row);
    };
    const addRequest = async ({ form }: AddReq) => {
        return await getAPI(SysNoticeApi)
            .apiSysNoticeAddPost(form);
    };
    const selectedIds = ref([]);
    const onSelectionChange = (changed) => {
        selectedIds.value = changed;
    };
    return {
        selectedIds,
        crudOptions: {
            container: {
                is: 'fs-layout-card'
            },
            form: {
                wrapper: {
                    // is: 'el-drawer',
                    // width: '80%',
                    draggable: false,
                    closeOnEsc: false,
                    maskClosable: false,
                }
            },
            search: {
                show: true,
            },
            actionbar: {
            },
            toolbar: {
                show: true,
                buttons: {
                    search: { show: true },
                    refresh: { show: true },
                    compact: { show: true },
                    export: { show: true },
                    columns: { show: true },
                },
            },
            table: {
                scrollX: 725,
                bordered: false,
                rowKey: (row) => row.id,
                checkedRowKeys: selectedIds,
                'onUpdate:checkedRowKeys': onSelectionChange,
            },
            pagination: {
                show: true
            },
            request: {
                pageRequest,
                addRequest,
                editRequest,
            },
            rowHandle: {
                fixed: "right",
                align: "center",
                width: 200,
                buttons: {
                    view: { show: true },
                    edit: { show: true }
                }
            },
            columns: {
                _checked: {
                    title: '选择',
                    form: { show: false },
                    column: {
                        type: 'selection',
                        align: 'center',
                        width: '55px',
                        columnSetDisabled: true,
                        disabled(row) {
                            return row.account === 'gvanet';
                        },
                    },
                },
                type: {
                    title: '类型',
                    type: 'dict-select',
                    search: { show: true, col: { span: 6 } },
                    column: {
                        align: "center",
                        width: '120px',
                    },
                    dict: dict({
                        value: 'id',
                        label: 'text',
                        data: [
                            { id: '1', text: '通知' },
                            { id: '2', text: '公告' }
                        ],
                    }),
                    form: {
                        col: { span: 24 },
                        rule: [
                            { required: true, message: '请输入类型' }
                        ],
                    }
                },
                title: {
                    title: '标题',
                    type: 'text',
                    search: { show: true, col: { span: 6 } },
                    column: {
                        align: "center",
                        width: 'auto',
                    },
                    form: {
                        col: { span: 24 },
                        rule: [
                            { required: true, message: '请输入标题' }
                        ],
                    }
                },
                content: {
                    title: '内容',
                    type: 'editor-wang5',
                    search: { show: false, col: { span: 6 } },
                    column: {
                        show: false,
                    },
                    form: {
                        col: { span: 24 },
                        rule: [
                            { required: true, message: '请输入内容' }
                        ],
                        component: {
                            disabled: compute(({ form }) => {
                                return form.disabled;
                            }),
                            id: '1', // 当同一个页面有多个editor时，需要配置不同的id
                            config: {},
                            uploader: {
                                type: 'form',
                                buildUrl(res) {
                                    return res.url;
                                },
                            },
                        },
                    }
                }
            },
        },
    };
}