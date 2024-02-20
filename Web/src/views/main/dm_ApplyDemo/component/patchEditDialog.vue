<template>
	<div class="dm_ApplyDemo-container">
		<el-dialog v-model="isShowDialog"  width="80%" draggable="">
			<template #header>
				<div style="color: #fff">
					<!--<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>-->
					<span>{{ props.title }}</span>
				</div>
			</template>
			<TableEditor :columns="editFormSchema" :v-model:value="vm.value"></TableEditor>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel">取 消</el-button>
					<el-button type="primary" @click="submit">确 定</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>
<style scoped>
:deep(.el-select),
:deep(.el-input-number) {
	width: 100%;
}
</style>
<script lang="ts" setup>
	import { ref,onMounted, reactive } from "vue";
	import { getDictDataItem as di, getDictDataListInt as dl } from '/@/utils/dict-utils';
	import { ElMessage } from "element-plus";
	import type { FormRules } from "element-plus";
	import { addDm_ApplyDemo, updateDm_ApplyDemo, detailDm_ApplyDemo } from "/@/api/main/dm_ApplyDemo";
	import TableEditor from "/@/components/table/TableEditor.vue";

	//父级传递来的参数
	var props = defineProps({
		title: {
		type: String,
		default: "",
	},
	});
	//父级传递来的函数，用于回调
	const emit = defineEmits(["reloadTable"]);
	const ruleFormRef = ref();
	const isShowDialog = ref(false);
	const ruleForm = ref<any>({});
	//自行添加其他规则
	const rules = ref<FormRules>({
        applyNO: [{ required: true, message: '请输入申请号！', trigger: 'blur',},],
        applicatDate: [{ required: true, message: '请选择申请时间！', trigger: 'change',},],
        amount: [{ required: true, message: '请输入申请金额！', trigger: 'blur',},],
        remark: [{ required: true, message: '请输入备注！', trigger: 'blur',},],
	});
 const editFormSchema = [
  {
    label: 'id',
    field: 'id',
    component: 'el-input',
    required: false,
    colProps: { span: 6 },
    ifShow: false,
  }, 
  {
    label: '机构类型',
    field: 'orgType',
    component: 'el-select',
    componentProps: {
      options: dl('org_type'),
    },
    required: false,
    colProps: { span: 6 },
  },
  {
    label: '申请号',
    field: 'applyNO',
    component: 'el-input',
    required: false,
    colProps: { span: 6 },
  },
  {
    label: '申请时间',
    field: 'applicatDate',
    component: 'el-date-picker',
    required: false,
    colProps: { span: 6 },
  },
  {
    label: '申请金额',
    field: 'amount',
    component: 'el-input-number',
    required: false,
    colProps: { span: 6 },
  },
  {
    label: '是否通知',
    field: 'isNotice',
    component: 'el-switch',
    required: false,
    colProps: { span: 6 },
  },
  {
    label: '备注',
    field: 'remark',
    component: 'el-input',
    required: false,
    colProps: { span: 6 },
  },
];

var vm = reactive({  value: [] as any[] });
	// 打开弹窗
	const openDialog = async (row: any) => {
		// ruleForm.value = JSON.parse(JSON.stringify(row));
		// 改用detail获取最新数据来编辑
		let rowData = JSON.parse(JSON.stringify(row));
		if (rowData.id)
			ruleForm.value = (await detailDm_ApplyDemo(rowData.id)).data.result;
		else
			ruleForm.value = rowData;
		isShowDialog.value = true;
	};

	// 关闭弹窗
	const closeDialog = () => {
		emit("reloadTable");
		isShowDialog.value = false;
	};

	// 取消
	const cancel = () => {
		isShowDialog.value = false;
	};

	// 提交
	const submit = async () => {
		ruleFormRef.value.validate(async (isValid: boolean, fields?: any) => {
			if (isValid) {
				let values = ruleForm.value;
				if (ruleForm.value.id == undefined || ruleForm.value.id == null || ruleForm.value.id == "" || ruleForm.value.id == 0) {
					await addDm_ApplyDemo(values);
				} else {
					await updateDm_ApplyDemo(values);
				}
				closeDialog();
			} else {
				ElMessage({
					message: `表单有${Object.keys(fields).length}处验证失败，请修改后再提交`,
					type: "error",
				});
			}
		});
	};







	// 页面加载时
	onMounted(async () => {
	});

	//将属性或者函数暴露给父组件
	defineExpose({ openDialog });
</script>




