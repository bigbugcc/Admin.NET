<template>
	<div class="sys-server-container">
		<el-row :gutter="8">
			<el-col :md="12" :sm="24">
				<el-card shadow="hover" header="系统信息">
					<table class="sysInfo_table">
						<tbody>
							<tr>
								<td class="sysInfo_td">主机名称：</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.runtimeInfo?.machineName }}</td>
							</tr>
							<tr>
								<td class="sysInfo_td">操作系统：</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.runtimeInfo?.osDescription }}</td>
							</tr>
							<tr>
								<td class="sysInfo_td">系统架构：</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.runtimeInfo?.osArchitecture }}</td>
							</tr>
							<tr>
								<td class="sysInfo_td">CPU核数：</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.runtimeInfo?.processorCount }}</td>
							</tr>
							<tr>
								<td class="sysInfo_td">运行时长：</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.runtimeInfo?.systemUptime }}</td>
							</tr>
							<tr>
								<td class="sysInfo_td">外网地址：</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.runtimeInfo?.remoteIp }}</td>
							</tr>
							<tr>
								<td class="sysInfo_td">内网地址：</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.runtimeInfo?.localIp }}</td>
							</tr>
							<tr>
								<td class="sysInfo_td">运行框架：</td>
								<td class="sysInfo_td">{{ state.machineBaseInfo.runtimeInfo?.frameworkDescription }}</td>
							</tr>
						</tbody>
					</table>
				</el-card>
			</el-col>
			<el-col :md="12" :sm="24">
				<el-card shadow="hover" header="使用信息">
					<el-row>
						<el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12" style="text-align: center">
							<el-progress
								type="dashboard"
								:percentage="parseInt(state.machineUseInfo.ramRate == undefined ? 0 : state.machineUseInfo.ramRate.substr(0, state.machineUseInfo.ramRate.length - 1))"
								:color="'var(--el-color-primary)'"
							>
								<template #default>
									<span>{{ state.machineUseInfo.ramRate }}<br /></span>
									<span style="font-size: 10px">
										已用:{{ state.machineUseInfo.usedRam }}<br />
										剩余:{{ state.machineUseInfo.freeRam }}<br />
										内存使用率
									</span>
								</template>
							</el-progress>
						</el-col>
						<el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12" style="text-align: center">
							<el-progress
								type="dashboard"
								:percentage="parseInt(state.machineUseInfo.cpuRate == undefined ? 0 : state.machineUseInfo.cpuRate.substr(0, state.machineUseInfo.cpuRate.length - 1))"
								:color="'var(--el-color-primary)'"
							>
								<template #default>
									<span>{{ state.machineUseInfo.cpuRate }}<br /></span>
									<span style="font-size: 10px"> CPU使用率 </span>
								</template>
							</el-progress>
						</el-col>
					</el-row>

					<el-row>
						<table class="sysInfo_table">
							<tbody>
								<tr>
									<td class="sysInfo_td">启动时间：</td>
									<td class="sysInfo_td">{{ state.machineBaseInfo.runtimeInfo?.processStartTime }}</td>
								</tr>
								<tr>
									<td class="sysInfo_td">运行时长：</td>
									<td class="sysInfo_td">{{ state.machineBaseInfo.runtimeInfo?.systemUptime }}</td>
								</tr>
								<tr>
									<td class="sysInfo_td">网站目录：</td>
									<td class="sysInfo_td">{{ state.machineBaseInfo.runtimeInfo?.currentDirectory }}</td>
								</tr>
								<tr>
									<td class="sysInfo_td">开发环境：</td>
									<td class="sysInfo_td">{{ state.machineBaseInfo.environment }}</td>
								</tr>
								<tr>
									<td class="sysInfo_td">环境变量：</td>
									<td class="sysInfo_td">{{ state.machineBaseInfo.stage }}</td>
								</tr>
							</tbody>
						</table>
					</el-row>
				</el-card>
			</el-col>
		</el-row>

		<el-row :gutter="8">
			<el-col :md="24" :sm="24">
				<el-card shadow="hover" header="程序集信息" style="margin-top: 5px; --el-card-padding: 10px">
					<div v-for="d in state.assemblyInfo" :key="d.name" style="display: inline-block; margin: 4px; text-align: left">
						<el-tag round>
							<div style="display: inline-flex">
								<div>{{ d.packageName }}</div>
								<div style="color: black; font-size: 9px; margin-left: 3px">v{{ d.packageVersion }}</div>
							</div>
						</el-tag>
					</div>
				</el-card>
			</el-col>
		</el-row>

		<el-row :gutter="8">
			<el-col :md="24" :sm="24">
				<el-card shadow="hover" header="磁盘信息" style="margin-top: 5px">
					<el-row>
						<el-col
							:span="4"
							:xs="(24 / state.machineDiskInfo?.diskInfos.length) * 2"
							:sm="24 / state.machineDiskInfo?.diskInfos.length"
							:md="24 / state.machineDiskInfo?.diskInfos.length"
							:lg="24 / state.machineDiskInfo?.diskInfos.length"
							:xl="24 / state.machineDiskInfo?.diskInfos.length"
							v-for="d in state.machineDiskInfo?.diskInfos"
							:key="d.diskName"
							style="text-align: center"
						>
							<el-progress type="circle" :percentage="d.availableRate" :color="'var(--el-color-primary)'">
								<template #default>
									<span>{{ d.availableRate }}%<br /></span>
									<span style="font-size: 10px">
										已用:{{ (d.usedSpace / 1024 / 1024 / 1024).toFixed(2) }}GB<br />
										剩余:{{ (d.freeSpace / 1024 / 1024 / 1024).toFixed(2) }}GB<br />
										{{ d.diskName }}
									</span>
								</template>
							</el-progress>
						</el-col>
					</el-row>
				</el-card>
			</el-col>
		</el-row>
	</div>
</template>

<script lang="ts" setup name="sysServer">
import { onActivated, onDeactivated, onMounted, reactive } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysServerApi } from '/@/api-services';

const state = reactive({
	machineBaseInfo: [] as any,
	machineUseInfo: [] as any,
	machineDiskInfo: [] as any,
	assemblyInfo: [] as any,
	timer: null as any,
});

onMounted(async () => {
	loadMachineBaseInfo();
	loadMachineUseInfo();
	loadMachineDiskInfo();
	loadAssemblyInfo();
});

// 服务器配置信息
const loadMachineBaseInfo = async () => {
	var res = await getAPI(SysServerApi).apiSysServerRuntimeInfoGet();
	state.machineBaseInfo = res.data.result;
	console.log(state.machineBaseInfo);
};

// 服务器内存信息
const loadMachineUseInfo = async () => {
	// var res = await getAPI(SysServerApi).apiSysServerHardwareInfoGet();
	// state.machineUseInfo = res.data.result;
};

// 服务器磁盘信息
const loadMachineDiskInfo = async () => {
	var res = await getAPI(SysServerApi).apiSysServerHardwareInfoGet();
	state.machineDiskInfo = res.data.result;
	console.log(state.machineDiskInfo);
};

// 框架程序集信息
const loadAssemblyInfo = async () => {
	var res = await getAPI(SysServerApi).apiSysServerNuGetPackageInfoGet();
	state.assemblyInfo = res.data.result;
};

// 实时刷新内存
const refreshData = () => {
	loadMachineUseInfo();
};

onActivated(() => {
	state.timer = setInterval(() => {
		refreshData();
	}, 10000);
});

onDeactivated(() => {
	clearInterval(state.timer);
});
</script>

<style lang="scss" scoped>
.sysInfo_table {
	width: 100%;
	min-height: 40px;
	line-height: 40px;
	text-align: center;
}

.sysInfo_td {
	border-bottom: 1px solid #e8e8e8;
	min-width: 100px;
}
</style>
