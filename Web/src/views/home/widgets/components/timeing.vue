<template>
	<el-card shadow="hover" header="时钟" class="item-background">
		<div class="time">
			<h2>{{ time }}</h2>
			<p>{{ day }}</p>
		</div>
	</el-card>
</template>

<script lang="ts">
export default {
	title: '时钟',
	icon: 'ele-Timer',
	description: '时钟原子组件演示'
}
</script>

<script setup lang="ts" name="timeing">
import { formatDate } from '/@/utils/formatTime'
import { ref, onMounted, onUnmounted } from 'vue'
const time = ref<string>('')
const day = ref<string>('')
const timer = ref<any>(null)

onMounted(() => {
	showTime()
	timer.value =setInterval(() => {
		showTime()
	}, 1000)
})

onUnmounted(() => {
	clearInterval(timer.value)
})

const showTime = () => {
	time.value = formatDate(new Date(), 'HH:MM:SS')
	day.value = formatDate(new Date(), 'YYYY年mm月dd日')
}

</script>

<style scoped>
.item-background {
	background: linear-gradient(to right, #8e54e9, #4776e6);
	color: #fff;
}
.time h2 {
	font-size: 40px;
}
.time p {
	font-size: 14px;
	margin-top: 13px;
	opacity: 0.7;
}
</style>
