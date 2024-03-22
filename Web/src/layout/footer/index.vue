<template>
	<div class="layout-footer pb15">
		<div class="layout-footer-warp">
			<div>{{ themeConfig.globalTitle }}</div>
			<div class="mt5">Copyright © 2023 Daming All rights reserved.</div>
		</div>
	</div>
</template>

<script setup lang="ts" name="layoutFooter">
import { storeToRefs } from 'pinia';
import { useThemeConfig } from '/@/stores/themeConfig';

const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);
</script>

<style scoped lang="scss">
.layout-footer {
	width: 100%;
	display: flex;
	&-warp {
		margin: auto;
		color: var(--el-text-color-secondary);
		text-align: center;
		animation: error-num 0.3s ease;
	}
}
</style>
