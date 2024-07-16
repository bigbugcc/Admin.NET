<template>
	<el-card shadow="hover" header="我的日程" class="item-background">
		<template #header>
			<el-icon style="display: inline; vertical-align: middle"> <ele-Calendar /> </el-icon>
			<span style=""> 我的日程 </span>
			<el-button type="primary" icon="ele-CirclePlus" round plain @click="openAddSchedule" style="float: right">添加日程</el-button>
		</template>

		<div class="custome-canlendar">
			<el-calendar v-model="state.calendarValue">
				<template #date-cell="{ data }">
					<div @click="handleClickDate(data)">
						<div class="spandate">{{ data.day.split('-').slice(2).join('-') }}</div>
						<div v-for="(item, key) in state.ScheduleData" :key="key">
							<el-badge v-if="FormatDate(data.day) == FormatDate(item.scheduleTime)" is-dot class="item"></el-badge>
						</div>
					</div>

					<div style="font-size: 10px">
						{{ solarDate2lunar(data.day) }}
					</div>
				</template>
			</el-calendar>
		</div>
		<div class="schedule-list">
			<div class="item" v-for="(item, index) in state.TodayScheduleData" :key="index" @click="openEditSchedule(item)">
				<el-icon style="display: inline; vertical-align: middle"> <ele-Calendar /> </el-icon>
				<span class="content" style="padding-left: 10px">{{ item.content }}</span>
			</div>
		</div>

		<EditSchedule ref="editScheduleRef" :title="state.editTitle" @handleQuery="handleQuery"> </EditSchedule>
	</el-card>
</template>

<script lang="ts">
export default {
	title: '日程',
	icon: 'ele-Odometer',
	description: '日程演示',
};
</script>

<script setup lang="ts">
import { reactive, onMounted, ref } from 'vue';
import { dayjs } from 'element-plus';
import calendar from '/@/utils/calendar.js';

import EditSchedule from '/@/views/home/widgets/components/scheduleEdit.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysScheduleApi } from '/@/api-services/api';
import { SysSchedule } from '/@/api-services/models';

const editScheduleRef = ref<InstanceType<typeof EditSchedule>>();
const state = reactive({
	ScheduleData: [] as Array<SysSchedule>, // 日程列表数据
	TodayScheduleData: [] as Array<SysSchedule>, // 当天列表数据
	calendarValue: new Date(),
	queryParams: {
		startTime: new Date(),
		endTime: new Date(),
	},
	editTitle: '',
});

// 页面初始化
onMounted(async () => {
	await handleQuery();
});

// 查询操作
const handleQuery = async () => {
	debugger;
	state.queryParams.startTime = GetMonthFirstDay(state.calendarValue);
	state.queryParams.endTime = GetMonthLastDay(state.calendarValue);

	let params = Object.assign(state.queryParams);
	var res = await getAPI(SysScheduleApi).apiSysSchedulePagePost(params);
	state.ScheduleData = res.data.result ?? [];
	if (state.ScheduleData.length > 0) {
		state.TodayScheduleData = state.ScheduleData.filter((item) => {
			return FormatDate(item.scheduleTime) == FormatDate(state.calendarValue);
		});
	}
};

// 农历转换
const solarDate2lunar = (solarDate: any) => {
	var solar = solarDate.split('-');
	var lunar = calendar.solar2lunar(solar[0], solar[1], solar[2]);
	// return solar[1] + '-' + solar[2] + '\n' + lunar.IMonthCn + lunar.IDayCn;
	return lunar.IMonthCn + lunar.IDayCn;
};

// 按天查询
const handleQueryByDate = async (date: any) => {
	state.queryParams.startTime = FormatDateDelHMS(date);
	state.queryParams.endTime = FormatDateEndHMS(date);
	let params = Object.assign(state.queryParams);
	var res = await getAPI(SysScheduleApi).apiSysSchedulePagePost(params);
	state.TodayScheduleData = res.data.result ?? [];
};

// 打开新增页面
const openAddSchedule = () => {
	state.editTitle = '添加日程';
	editScheduleRef.value?.openDialog({ id: undefined, status: 1, orderNo: 100 });
};

// 打开编辑页面
const openEditSchedule = async (row: any) => {
	state.editTitle = '编辑日程';
	editScheduleRef.value?.openDialog(row, true);
};

// 点击日历中的日期
async function handleClickDate(data: any) {
	await handleQueryByDate(data.day);
}

function GetMonthFirstDay(date: any) {
	var newDate = new Date(date);
	newDate.setDate(1);
	newDate.setHours(0);
	newDate.setMinutes(0);
	newDate.setSeconds(0);
	return newDate;
}

function GetMonthLastDay(date: any) {
	var newDate = new Date(date);
	newDate.setMonth(newDate.getMonth() + 1);
	newDate.setDate(0);
	newDate.setHours(0);
	newDate.setMinutes(0);
	newDate.setSeconds(0);
	return newDate;
}

// 去掉时分秒的日期
function FormatDateDelHMS(date: any) {
	var newDate = new Date(date);
	newDate.setHours(0);
	newDate.setMinutes(0);
	newDate.setSeconds(0);
	return newDate;
}

function FormatDateEndHMS(date: any) {
	var newDate = new Date(date);
	newDate.setHours(23);
	newDate.setMinutes(59);
	newDate.setSeconds(59);
	return newDate;
}

// 格式化日期
function FormatDate(date: any) {
	return dayjs(date).format('YYYY-MM-DD');
}
</script>

<style lang="scss" scoped>
.custome-canlendar {
	position: relative;
	text-align: center;

	:deep(.el-calendar) {
		.el-calendar-table .el-calendar-day {
			width: 100%;
			height: 100%;
		}

		.el-calendar__body {
			padding: 5px 0;
		}

		.el-calendar-table .el-calendar-day {
			position: relative;
		}

		td .spandate {
			margin: auto;
			width: 26px;
			height: 26px;
			line-height: 26px;
			border-radius: 50%;
		}
		td.is-selected .spandate {
			width: 26px;
			height: 26px;
			line-height: 26px;
			border-radius: 50%;
			color: #fff;
			background-color: var(--el-color-primary);
		}
		/*小红点样式*/
		.el-badge {
			position: absolute;
			left: 0;
			bottom: -8px;
			width: 100%;
		}
	}
}

// 日程列表
.schedule-list {
	padding: 0 20px 10px;
	overflow-y: auto;
	height: 150px;
	.item {
		position: relative;
		margin-bottom: 5px;
		padding: 0 11px;
		line-height: 24px;
		background-color: #f1f1f1;
		cursor: pointer;

		&::before {
			position: absolute;
			left: 0;
			top: 0;
			height: 100%;
			content: '';
			width: 3px;
			background: var(--el-color-primary);
		}

		.date {
			margin-right: 5px;
			font-size: 14px;
		}
		.content {
			color: #666;
			font-size: 14px;
		}
	}
}
</style>
