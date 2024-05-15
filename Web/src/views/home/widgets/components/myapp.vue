<template>
	<el-card shadow="hover" header="快捷入口">
		<ul class="myMods">
			<!-- <li v-for="mod in myMods" :key="mod.path" :style="{ background:mod.meta.color||'#eeeeee'}"> -->
			<li v-for="mod in myMods" :key="mod.path">
				<a v-if="mod.meta.type == 'link'" :href="mod.path" target="_blank">
					<SvgIcon :name="mod.meta.icon" style="font-size: 18px;" />
					<p>{{ mod.meta.title }}</p>
				</a>
				<router-link v-else :to="{ path: mod.path }">
					<SvgIcon :name="mod.meta.icon" style="font-size: 18px;" />
					<p>{{ mod.meta.title }}</p>
				</router-link>
			</li>
			<li class="modItem-add" @click="addMods">
				<a>
					<el-icon><ele-Plus :style="{ color: '#fff' }" /></el-icon>
				</a>
			</li>
		</ul>

		<el-drawer title="添加应用" v-model="modsDrawer" :size="520" destroy-on-close :before-close="beforeClose">
			<div class="setMods mt15">
				<h4>我的常用 ( {{ myMods.length }} )</h4>
				<draggable tag="ul"
					v-model="myMods"
					animation="200"
					item-key="path"
					group="app"
					class="draggable-box"
					force-fallback
					fallback-on-body
				>
					<template #item="{ element }">
						<li>
							<SvgIcon :name="element.meta.icon" style="font-size: 18px;" />
							<p>{{ element.meta.title }}</p>
						</li>
					</template>
				</draggable>
			</div>
			<div class="setMods">
				<h4>全部应用 ( {{ filterMods.length }} )</h4>
				<draggable
					tag="ul"
					v-model="filterMods"
					animation="200"
					item-key="path"
					group="app"
					class="draggable-box"
					force-fallback
					fallback-on-body
				>
					<template #item="{ element }">
						<li :style="{ background: element.meta.color || '#909399' }">
							<SvgIcon :name="element.meta.icon" style="font-size: 18px;" />
							<p>{{ element.meta.title }}</p>
						</li>
					</template>
				</draggable>
			</div>
			<template #footer>
				<el-button @click="beforeClose">取消</el-button>
				<el-button type="primary" @click="saveMods">保存</el-button>
			</template>
		</el-drawer>
	</el-card>
</template>

<script lang="ts">
export default {
	title: "快捷入口",
	icon: "ele-Monitor",
	description: "可以配置的快捷入口"
}
</script>

<script setup lang="ts" name="myapp">
import draggable from 'vuedraggable'
import { onMounted, ref } from 'vue'
import { Local } from '/@/utils/storage'
import { useRequestOldRoutes } from '/@/stores/requestOldRoutes'
import { ElMessage } from 'element-plus'

const mods = ref<any>([]) // 所有应用
const myMods = ref<any>([]) // 我的常用
const myModsName = ref<any>([]) // 我的常用
const filterMods = ref<any>([]) // 过滤我的常用后的应用
const modsDrawer = ref<boolean>(false)

onMounted(() => {
	getMods()
})

const addMods = () => {
	modsDrawer.value = true
}

const getMods = () => {
	myModsName.value = Local.get('my-mods') || []
	var menuTree = useRequestOldRoutes().requestOldRoutes || []
	filterMenu(menuTree)
	myMods.value = mods.value.filter((item: any) => {
		return myModsName.value.includes(item.name);
	})
	
	filterMods.value = mods.value.filter((item: any) => {
		return !myModsName.value.includes(item.name)
	})
}

// 递归拿到所有可显示非iframe的2级菜单
const filterMenu = (map: any) => {
	map.forEach((item: any) => {
		if (item.meta.isHide || item.type == 3 || item.status != 1) {
			return false
		}
		if (item.meta.isIframe) {
			item.path = `/i/${item.name}`
		}
		if (item.children && item.children.length > 0) {
			filterMenu(item.children)
		} else {
			mods.value.push(item)
		}
	})
}

// 保存我的常用
const saveMods = () => {
	const myModsName = myMods.value.map((v: any) => v.name)
	Local.set('my-mods', myModsName)
	ElMessage.success('设置常用成功')
	modsDrawer.value = false
}

// 取消
const beforeClose = () => {
	myModsName.value = Local.get('my-mods') || []
	myMods.value = mods.value.filter((item: any) => {
		return myModsName.value.includes(item.name);
	})
	modsDrawer.value = false
}
</script>

<style scoped lang="scss">
.myMods {
	list-style: none;
	margin: -10px;
}
.myMods li {
	display: inline-block;
	width: 100px;
	height: 100px;
	vertical-align: top;
	transition: all 0.3s ease;
	margin: 10px;
	border-radius: 5px;
	background: var(--el-color-primary);
}
.myMods li:hover {
	opacity: 0.5;
}
.myMods li a {
	width: 100%;
	height: 100%;
	padding: 10px;
	display: flex;
	flex-direction: column;
	align-items: center;
	justify-content: center;
	text-align: center;
	color: #fff;
}
.myMods li i {
	font-size: 26px;
	color: #fff;
}
.myMods li p {
	font-size: 14px;
	color: #fff;
	margin-top: 10px;
	width: 100%;
	white-space: nowrap;
	text-overflow: ellipsis;
	overflow: hidden;
}

.modItem-add {
	border: 1px dashed #ddd;
	cursor: pointer;
}
.modItem-add i {
	font-size: 30px;
	color: #999 !important;
}
.modItem-add:hover,
.modItem-add:hover i {
	border-color: #409eff;
	color: #409eff !important;
}

.draggable-box {
	border: 1px dashed var(--el-color-primary);
	padding: 15px
}

.setMods {
	padding: 0 20px;
}
.setMods h4 {
	font-size: 14px;
	font-weight: normal;
}
.setMods ul {
	margin: 20px -5px;
	min-height: 90px;
}
.setMods li {
	display: inline-block;
	width: 80px;
	height: 80px;
	text-align: center;
	margin: 5px;
	color: #fff;
	vertical-align: top;
	padding: 4px;
	padding-top: 15px;
	cursor: move;
	border-radius: 3px;
	background: var(--el-color-primary);
}
.setMods li i {
	font-size: 20px;
}
.setMods li p {
	font-size: 12px;
	margin-top: 10px;
}
.setMods li.sortable-ghost {
	opacity: 0.3;
}
</style>
