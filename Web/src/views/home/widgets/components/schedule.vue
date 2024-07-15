<template>
    <el-card shadow="hover" header="我的日程" class="item-background">
        <template #header>
            <el-icon style="display: inline; vertical-align: middle"> <ele-Calendar /> </el-icon>
            <span style=""> 我的日程 </span>
            <el-button type="primary" icon="ele-CirclePlus" round @click="openAddSchedule">添加日程</el-button>
        </template>
        <div class="custome-canlendar">
            <div class="block">
                <div class="data-analysis">
                    <el-calendar v-model="state.calendarValue">
                        <!--选中小红点-->
                        <template #date-cell="{ data }">
                            <div @click="handleClickDate(data)">
                                <div class="spandate">{{ data.day.split('-').slice(2).join('-') }}</div>
                                <div v-for="(item, key) in state.ScheduleData" :key="key">
                                    <el-badge v-if="FormatDate(data.day) == FormatDate(item.scheduleTime)" is-dot class="item"></el-badge>
                                </div>
                            </div>
                        </template>
                    </el-calendar>
                </div>
                <div class="schedule-list">
                    <div class="item" v-for="(item,index) in state.TodayScheduleData" :key="index" @click="openEditSchedule(item)">
                        <!-- <span class="date">{{ item.start_time + '-' + item.end_time }}</span> -->
                        <span class="content">{{ item.content }}</span>
                    </div>
                </div>
            </div>
            <EditSchedule ref="editScheduleRef" :title="state.editTitle" @handleQuery="handleQuery">
            </EditSchedule>
        </div>
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
    import { dayjs, ElMessageBox, ElNotification } from 'element-plus';

    import { getAPI } from '/@/utils/axios-utils';
    import { SysUserScheduleApi } from '/@/api-services/api';
    import { SysUserSchedule } from '/@/api-services/models';
    import EditSchedule from '/@/views/home/widgets/components/scheduleEdit.vue';

    const editScheduleRef = ref<InstanceType<typeof EditSchedule>>();

    const state = reactive({
    	ScheduleData: [] as Array<SysUserSchedule>, // 日程列表数据
    	TodayScheduleData: [] as Array<SysUserSchedule>, // 当天列表数据
    	calendarValue: new Date(),
    	queryParams: {
    		startTime: new Date(),
    		endTime: new Date(),
    	},
    	editTitle: '',
    });

    onMounted(async () => {
    	handleQuery();
    });

    // 查询操作
    const handleQuery = async () => {
    	debugger;
    	state.queryParams.startTime = GetMonthFirstDay(state.calendarValue);
    	state.queryParams.endTime = GetMonthLastDay(state.calendarValue);

    	let params = Object.assign(state.queryParams);
    	var res = await getAPI(SysUserScheduleApi).apiSysUserSchedulePagePost(params);
    	state.ScheduleData = res.data.result ?? [];
    	if (state.ScheduleData.length > 0) {
    		state.TodayScheduleData = state.ScheduleData.filter((item) => {
    			return FormatDate(item.scheduleTime) == FormatDate(state.calendarValue);
    		});
    	}
    };
    //按天查询
    const handleQueryByDate = async (date) => {
    	state.queryParams.startTime = FormatDateDelHMS(date);
    	state.queryParams.endTime = FormatDateDelHMS(date);
    	let params = Object.assign(state.queryParams);
    	var res = await getAPI(SysUserScheduleApi).apiSysUserSchedulePagePost(params);
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
    	editScheduleRef.value?.openDialog(row);
    };

    // 点击日历中的日期
    function handleClickDate(data) {
    	handleQueryByDate(data.day);
    }

    function GetMonthFirstDay(date) {
    	var newDate = new Date(date);
    	newDate.setDate(1);
    	newDate.setHours(0);
    	newDate.setMinutes(0);
    	newDate.setSeconds(0);
    	return newDate;
    }
    function GetMonthLastDay(date) {
    	var newDate = new Date(date);
    	newDate.setMonth(newDate.getMonth() + 1);
    	newDate.setDate(0);
    	newDate.setHours(0);
    	newDate.setMinutes(0);
    	newDate.setSeconds(0);
    	return newDate;
    }
    /// 去掉时分秒的日期
    function FormatDateDelHMS(date) {
    	var newDate = new Date(date);
    	newDate.setHours(0);
    	newDate.setMinutes(0);
    	newDate.setSeconds(0);
    	return newDate;
    }
    // 格式化日期
    function FormatDate(date) {
    	return dayjs(date).format('YYYY-MM-DD');
    }
</script>


<style lang="scss" scoped>
    .custome-canlendar {
    	background: #fff;
    	.title {
    		padding: 13px 8px 12px 19px;
    		border-bottom: 1px solid #f2f2f2;
    		font-weight: 500;
    		color: #1a1a1a;
    		font-size: 16px;
    		position: relative;

    		&:before {
    			content: '';
    			display: inline-block;
    			height: calc(100% - 30px);
    			width: 3px;
    			margin-right: 0px;
    			background: #c70019;
    			/*margin-top: 10px;*/
    			border-radius: 5px;
    			/*margin-left: 10px;*/
    			position: absolute;
    			left: 10px;
    			top: calc(50% - 7px);
    		}
    		.rtbtn {
    			float: right;
    			:deep(span) {
    				font-size: 14px;
    			}
    		}
    	}
    }
    .block {
    	height: calc(100% - 10px);
    	overflow-y: auto;
    }
    /*日历样式修改*/
    .data-analysis {
    	position: relative;

    	:deep(.el-calendar) {
    		.el-calendar-table .el-calendar-day {
    			width: 100%;
    			height: 100%;
    		}
    		.el-calendar__header {
    			padding: 6px 10px;
    			border: 0;
    			justify-content: space-between;
    			border-bottom: #666 1px solid;
    		}

    		.el-calendar__button-group .el-button-group > .el-button span {
    			font-size: 14px;
    		}
    		.el-calendar-table thead th {
    			padding: 6px 0;
    			font-weight: bold;
    		}

    		.el-calendar__body {
    			padding: 8px 0;
    		}

    		/*去掉原本背景颜色*/
    		.el-calendar-table td:hover {
    			background: transparent;
    		}
    		/*去掉选中背景颜色*/
    		.el-calendar-table td.is-selected {
    			background: transparent !important;
    		}
    		/*修改每一小格大小*/
    		.el-calendar-table .el-calendar-day {
    			position: relative;
    			padding: 6px 8px;
    			text-align: center;
    		}
    		.el-calendar-table .el-calendar-day:hover {
    			background: transparent;
    		}

    		td .spandate {
    			margin: auto;
    			width: 26px;
    			height: 26px;
    			line-height: 26px;
    			border-radius: 50%;
    			// @include level3_fontsize();
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
    			bottom: -13px;
    			width: 100%;
    		}
    		.el-badge__content {
    			background-color: var(--el-color-primary);

    			&.is-dot {
    				width: 7px;
    				height: 7px;
    			}
    		}
    		/*日历边框颜色*/
    		.el-calendar-table tr td:first-child,
    		.el-calendar-table tr:first-child td,
    		.el-calendar-table td {
    			border: 0;
    		}
    	}
    }

    .schedule-list {
    	padding: 0 20px 10px;
    	overflow-y: auto; /* 使div可滚动 */
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