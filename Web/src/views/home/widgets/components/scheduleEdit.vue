<template>
    <div class="sys-userSchedule-container">
        <el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
            <template #header>
                <div style="color: #fff">
                    <el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
                    <span> {{ props.title }} </span>
                </div>
            </template>
            <el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
                <el-row :gutter="35">
                    <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
                        <el-form-item label="日程时间" prop="scheduleTime" :rules="[{ required: true, message: '日程时间不能为空', trigger: 'blur' }]">
                            <el-date-picker v-model="state.ruleForm.scheduleTime" type="date" placeholder="请选择日程时间" format="YYYY-MM-DD" value-format="YYYY-MM-DD HH:mm:ss" />
                        </el-form-item>
                    </el-col>
                    <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
                        <el-form-item label="日程内容" prop="content" :rules="[{ required: true, message: '内容内容不能为空', trigger: 'blur' }]">
                            <el-input v-model="state.ruleForm.content" placeholder="内容内容" clearable type="textarea" />
                        </el-form-item>
                    </el-col>
                </el-row>
            </el-form>
            <template #footer>
                <span class="dialog-footer">
                    <el-button @click="cancel">取 消</el-button>
                    <el-button type="primary" @click="submit">确 定</el-button>
                </span>
            </template>
        </el-dialog>
    </div>
</template>

<script lang="ts" setup name="sysEditUserSchedule">
    import { onMounted, reactive, ref } from 'vue';
    import { dayjs, ElMessageBox, ElNotification } from 'element-plus';

    import { getAPI } from '/@/utils/axios-utils';
    import { SysUserScheduleApi } from '/@/api-services/api';
    import { SysUserSchedule, UpdateUserScheduleInput } from '/@/api-services/models';

    const props = defineProps({
    	title: String,
    	userScheduleData: Array<SysUserSchedule>,
    });
    const emits = defineEmits(['handleQuery']);
    const ruleFormRef = ref();
    const state = reactive({
    	isShowDialog: false,
    	ruleForm: {} as UpdateUserScheduleInput,
    });

    onMounted(async () => {});

    // 打开弹窗
    const openDialog = (row: any) => {
    	ruleFormRef.value?.resetFields();

    	state.ruleForm = JSON.parse(JSON.stringify(row));

    	console.log(JSON.stringify(state.ruleForm));
    	console.log(state.ruleForm.scheduleTime);
    	state.ruleForm.scheduleTime = dayjs(state.ruleForm.scheduleTime ?? new Date()).format('YYYY-MM-DD HH:mm:ss');
    	state.isShowDialog = true;
    	console.log(JSON.stringify(state.ruleForm.scheduleTime));
    	console.log(JSON.stringify(state.ruleForm));
    };

    // 关闭弹窗
    const closeDialog = () => {
    	emits('handleQuery', true);
    	state.isShowDialog = false;
    };

    // 取消
    const cancel = () => {
    	state.isShowDialog = false;
    };

    // 提交
    const submit = () => {
    	console.log(JSON.stringify(state.ruleForm));

    	ruleFormRef.value.validate(async (valid: boolean) => {
    		if (!valid) return;
    		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
    			await getAPI(SysUserScheduleApi).apiSysUserScheduleUpdatePost(state.ruleForm);
    		} else {
    			await getAPI(SysUserScheduleApi).apiSysUserScheduleAddPost(state.ruleForm);
    		}
    		closeDialog();
    	});
    };

    // 导出对象
    defineExpose({ openDialog });
</script>
