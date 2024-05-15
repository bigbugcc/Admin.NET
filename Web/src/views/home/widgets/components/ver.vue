<template>
	<el-card shadow="hover" header="版本信息">
		<div style="height: 210px; text-align: center">
			<img :src="verSvg" style="height: 140px" />
			<h2 style="margin-top: 15px">Admin.Net</h2>
			<p style="margin-top: 5px">最新版本 {{ ver }}</p>
		</div>
		<div style="margin-top: 20px">
			<el-button type="primary" plain round @click="golog">更新日志</el-button>
			<el-button type="primary" plain round @click="gogit">gitee</el-button>
		</div>
	</el-card>
</template>

<script lang="ts">
export default {
	title: '版本信息',
	icon: 'ele-Monitor',
	description: '版本信息原子组件演示'
}
</script>

<script setup lang="ts" name="ver">
import { ref, onMounted } from 'vue'
import verSvg from '/@/assets/img/ver.svg'

const ver = ref<string>('loading...')

onMounted(() => {
	ver.value = '11'
})

const getVer = () => {
	ver.value = '11'
}

const golog = () => {
	window.open('https://gitee.com/zuohuaijun/Admin.NET/issues')
}

const gogit = () => {
	window.open('https://gitee.com/zuohuaijun/Admin.NET.git')
}

</script>
