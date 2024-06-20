<template>
	<div class="el-card box">
		<div class="card mb10">
			<h4 class="title">
				<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Postcard /> </el-icon>简介(About)
			</h4>
			<span class="text">
				基于 .NET6 (Furion/SqlSugar) 实现的通用权限开发框架，前端采用
				Vue3+Element-plus+Vite5，整合众多优秀技术和框架，模块插件式开发。集成多租户、缓存、数据校验、鉴权、事件总线、动态API、通讯、远程请求、任务调度、打印等众多黑科技。代码结构简单清晰，注释详尽，易于上手与二次开发，即便是复杂业务逻辑也能迅速实现，真正实现“开箱即用”。
			</span>
		</div>
		<div class="card mb10">
			<h4 class="title">
				<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Warning /> </el-icon>项目信息(Information)
			</h4>
			<el-descriptions :column="2" border>
				<el-descriptions-item label="名称及作者">
					<el-tag>{{ name }}</el-tag> <el-tag type="info">{{ author }}</el-tag>
				</el-descriptions-item>

				<el-descriptions-item label="框架描述">
					<el-tag>{{ description }}</el-tag>
				</el-descriptions-item>

				<el-descriptions-item label="版本号">
					<el-tag>{{ version }}</el-tag> <el-tag type="success">{{ license }}</el-tag>
				</el-descriptions-item>

				<el-descriptions-item label="发布时间">
					<el-tag>{{ lastBuildTime }}</el-tag>
				</el-descriptions-item>

				<el-descriptions-item label="Gitee">
					<el-link type="primary" href="https://gitee.com/zuohuaijun/Admin.NET" target="_blank"> Gitee </el-link>
				</el-descriptions-item>
				<el-descriptions-item label="Github">
					<el-link type="primary" href="https://github.com/zuohuaijun/Admin.NET.git" target="_blank"> Github </el-link>
				</el-descriptions-item>

				<el-descriptions-item label="文档地址">
					<el-link type="primary" href="http://101.43.53.74:5050/" target="_blank"> 文档地址 </el-link>
				</el-descriptions-item>
				<el-descriptions-item label="预览地址">
					<el-link type="primary" href="http://101.43.53.74:5005/dist/index.html" target="_blank"> 预览地址 </el-link>
				</el-descriptions-item>
			</el-descriptions>
		</div>
		<div class="card mb10">
			<h4 class="title">
				<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-SetUp /> </el-icon>生产环境依赖(Dependencies)
			</h4>
			<el-descriptions :column="3" border>
				<el-descriptions-item v-for="(value, key) in dependencies" :key="key" width="400px" :label="key">
					<el-tag type="success" effect="plain">
						{{ value }}
					</el-tag>
				</el-descriptions-item>
			</el-descriptions>
		</div>
		<div class="card">
			<h4 class="title">
				<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-SetUp /> </el-icon>开发环境依赖(devDependencies)
			</h4>
			<el-descriptions :column="3" border>
				<el-descriptions-item v-for="(value, key) in devDependencies" :key="key" width="400px" :label="key">
					<el-tag type="danger" effect="plain">
						{{ value }}
					</el-tag>
				</el-descriptions-item>
			</el-descriptions>
		</div>
		<div class="card">
			<h4 class="title">
				<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-SetUp /> </el-icon>关键词(Keywords)
			</h4>
			<el-descriptions :column="4" border>
				<el-descriptions-item v-for="(value, key) in keywords" :key="value" width="400px" :label="key + 1">
					<el-text type="primary">
						{{ value }}
					</el-text>
				</el-descriptions-item>
			</el-descriptions>
		</div>
	</div>
</template>

<script setup lang="ts" name="about">
import PackageJson from '/package.json';

const { dependencies, devDependencies, keywords, version, lastBuildTime, author, description, license, name } = PackageJson;
</script>

<style lang="scss" scoped>
.box {
	overflow-y: auto;
}
el-descriptions-item {
	width: 50%;
}
.card {
	padding: 10px;
	.title {
		margin: 5px 5px 10px;
		font-size: 17px;
		font-weight: bold;
		color: var(--el-text-color-primary);
	}
	.text {
		text-indent: 50px;
		font-size: 15px;
		line-height: 30px;
		padding: 10px 20px;
		color: var(--el-text-color-regular);
		.el-link {
			font-size: 15px;
		}
	}
}
</style>
