<template>
	<div class="el-table-container">
		<table class="el-table el-table-middle">
			<thead class="el-table-thead">
				<tr>
					<th v-for="(citem, ci) in $props.columns" :key="ci" v-show="citem.ifShow == undefined ? true : citem.ifShow" style="text-align: center">
						{{ citem.label }}
					</th>
					<th style="text-align: center" v-show="!$props.disabled">操作</th>
				</tr>
			</thead>
			<tbody class="el-table-tbody">
				<tr v-for="(item, index) in vm.value" :key="index">
					<td v-for="(citem, ci) in $props.columns" :key="ci" style="text-align: center" v-show="citem.ifShow == undefined ? true : citem.ifShow">
						<component v-if="citem.component != 'el-select'" :is="citem.component" v-model="item[citem.field]" v-bind="renderComponentProp(citem)" :disabled="$props.disabled" />
						<el-select v-else v-model="item[citem.field]" v-bind="renderComponentProp(citem)" :disabled="$props.disabled">
							<el-option v-for="(sitem, si) in citem.componentProps.options" :key="sitem.value" :label="sitem.value" :value="sitem.code" />
						</el-select>
					</td>
					<td style="text-align: center" v-show="!$props.disabled">
						<el-button type="danger" @click="del(item, index)">删除</el-button>
					</td>
				</tr>
			</tbody>
			<tfoot>
				<tr>
					<td v-for="(citem, ci) in $props.columns" :key="ci" style="text-align: center" v-show="citem.ifShow == undefined ? true : citem.ifShow">
						<component v-if="citem.component != 'el-select'" :is="citem.component" v-model="vm.formData[citem.field]" v-bind="renderComponentProp(citem)" />
						<el-select v-else v-model="vm.formData[citem.field]" v-bind="renderComponentProp(citem)">
							<el-option v-for="(sitem, si) in citem.componentProps.options" :key="sitem.value" :label="sitem.value" :value="sitem.code" />
						</el-select>
					</td>
					<td class="el-table-cell el-table-cell-ellipsis" style="text-align: center" v-show="!$props.disabled">
						<el-button type="primary" @click="add">添加</el-button>
					</td>
				</tr>
			</tfoot>
		</table>
	</div>
</template>

<script lang="ts" setup>
import { reactive } from 'vue';
import { ElMessage, dayjs } from 'element-plus';
import AsyncValidator from 'async-validator';
import { isDate, isString } from 'lodash-es';

const props = defineProps({
	value: {
		type: Array<any>,
		default: () => [],
	},
	columns: {
		type: Array<any>,
		default: () => [],
	},
	rules: {
		type: Object,
	},
	params: {
		type: Object,
	},
	disabled: {
		type: Boolean,
		default: false,
	},
});
const emit = defineEmits(['add', 'delete', 'update:value']);
var vm = reactive({ formData: {} as any, value: [] as any[] });
function renderComponentProp(item: any) {
	let componentProps = item.componentProps || {};

	let disabled = item.disabled || componentProps.disabled || props.disabled;
	const propsData: any = {
		...componentProps,
		disabled: disabled,
	};

	return propsData;
}

function del(record: any, index: number) {
	vm.value.splice(index, 1);
	emit('update:value', vm.value);
	emit('delete', { value: vm.value, record });
}

async function add() {
	let msgs: string[] = [];

	for (const field in props.rules) {
		let rule = props.rules[field];

		let val = vm.formData[field];
		if (val) {
			if (isDate(val)) {
				val = dayjs(val).format(props.columns.filter((m) => m.field == field)[0].format || 'YYYY-MM-DD');
				vm.formData[field] = val;
			} else if (!isString(val)) {
				val = val.toString();
			}
		}
		const validator = new AsyncValidator({
			[field]: rule,
		});
		await validator.validate({ [field]: val }, { firstFields: true }).catch((error: any) => {
			var _a, _b;
			const { errors, fields } = error;
			if (!errors || !fields) {
				console.error(error);
			}
			if (errors) {
				msgs.push((_b = (_a = errors == null ? void 0 : errors[0]) == null ? void 0 : _a.message) != null ? _b : `${field} 必填！`);
			}
		});
	}
	if (msgs.length > 0) {
		ElMessage.error(msgs.join('。'));
		return;
	}
	for (const key in props.params) {
		vm.formData[key] = props.params[key];
	}
	if (!vm.value) {
		vm.value = [];
	}
	vm.value.push({ ...vm.formData });
	vm.formData = {};
	emit('update:value', vm.value);
	emit('add', { value: vm.value, record: vm.formData });
}
</script>
