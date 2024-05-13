<template>
	<div class="property-common">
		<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto" :rules="rules">
			<el-form-item v-show="false">
				<el-input v-model="nodeData.id" />
			</el-form-item>
			<el-tabs>
				<el-tab-pane label="基本信息">
					<el-row :gutter="35">
						<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
							<el-form-item label="编号" prop="id">
								<el-input v-model="nodeData.id" placeholder="请输入编号" maxlength="32" readonly show-word-limit clearable />
							</el-form-item>
						</el-col>
						<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
							<el-form-item label="属性" prop="type">
								<el-input v-model="nodeData.type" placeholder="请输入编号" maxlength="32" readonly show-word-limit clearable />
							</el-form-item>
						</el-col>
					</el-row>
				</el-tab-pane>
				<el-tab-pane label="扩展信息">
					<el-row :gutter="35"></el-row>
				</el-tab-pane>
			</el-tabs>
		</el-form>
	</div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';
import type { FormRules } from 'element-plus';

var props = defineProps({
	nodeData: Object,
});
const emit = defineEmits(['setProperties']);

const ruleFormRef = ref();

const state = reactive({
	ruleForm: {} as any,
});

const rules = ref<FormRules>({});
</script>

<style lang="scss" scoped></style>
