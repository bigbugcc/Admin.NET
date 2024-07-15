<template>
	<el-card shadow="hover" header="关于项目" class="item-background">
		<template #header>
			<el-icon style="display: inline; vertical-align: middle"> <ele-QuestionFilled /> </el-icon>
			<span style=""> 关于项目 </span>
		</template>
		<p>
			基于 .NET6 (Furion/SqlSugar) 实现的通用权限开发框架，前端采用
			Vue3+Element-plus+Vite5，整合众多优秀技术和框架，模块插件式开发。集成多租户、缓存、数据校验、鉴权、事件总线、动态API、通讯、远程请求、任务调度、打印等众多黑科技。代码结构简单清晰，注释详尽，易于上手与二次开发，即便是复杂业务逻辑也能迅速实现，真正实现“开箱即用”。
		</p>
		<p>
			<a href="https://gitee.com/zuohuaijun/Admin.NET.git" target="_blank">
				<img src="https://gitee.com/zuohuaijun/Admin.NET/badge/star.svg?theme=dark" alt="star" style="vertical-align: middle" />
				<img src="https://gitee.com/zuohuaijun/Admin.NET/badge/fork.svg?theme=dark" alt="fork" style="vertical-align: middle" />
				<img src="https://img.shields.io/badge/license-MIT-yellow" alt="license" style="vertical-align: middle" />
			</a>
		</p>
	</el-card>
</template>

<script lang="ts">
export default {
	title: '关于项目',
	icon: 'ele-Setting',
	description: '点个星星支持一下',
};
</script>
<script setup lang="ts" name="about"></script>

<style scoped>
.item-background p {
	color: #999;
	margin-top: 10px;
	line-height: 1.8;
	/* height: 100px; */
}
</style>
