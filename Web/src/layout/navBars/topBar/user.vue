<template>
	<div class="layout-navbars-breadcrumb-user pr15" :style="{ flex: layoutUserFlexNum }">
		<el-dropdown :show-timeout="70" :hide-timeout="50" trigger="click" @command="onComponentSizeChange">
			<div class="layout-navbars-breadcrumb-user-icon">
				<i class="iconfont icon-ziti" title="组件大小"></i>
			</div>
			<template #dropdown>
				<el-dropdown-menu>
					<el-dropdown-item command="large" :disabled="state.disabledSize === 'large'">大型</el-dropdown-item>
					<el-dropdown-item command="default" :disabled="state.disabledSize === 'default'">默认</el-dropdown-item>
					<el-dropdown-item command="small" :disabled="state.disabledSize === 'small'">小型</el-dropdown-item>
				</el-dropdown-menu>
			</template>
		</el-dropdown>
		<el-dropdown :show-timeout="70" :hide-timeout="50" trigger="click" @command="onLanguageChange">
			<div class="layout-navbars-breadcrumb-user-icon">
				<i class="iconfont" :class="state.disabledI18n === 'en' ? 'icon-fuhao-yingwen' : 'icon-fuhao-zhongwen'"
					title="语言切换"></i>
			</div>
			<template #dropdown>
				<el-dropdown-menu>
					<el-dropdown-item v-for="lang in state.languages" :key="lang.value" :command="lang.value"
						:disabled="lang.value === state.disabledI18n">
						{{ lang.label }}
					</el-dropdown-item>
				</el-dropdown-menu>
			</template>
		</el-dropdown>
		<div class="layout-navbars-breadcrumb-user-icon" @click="onSearchClick">
			<el-icon title="菜单搜索">
				<ele-Search />
			</el-icon>
		</div>
		<div class="layout-navbars-breadcrumb-user-icon" @click="onLayoutSetingClick">
			<i class="icon-skin iconfont" title="布局配置"></i>
		</div>
		<div class="layout-navbars-breadcrumb-user-icon">
			<el-popover placement="bottom" trigger="click" transition="el-zoom-in-top" :width="400" :persistent="false">
				<template #reference>
					<el-badge :is-dot="hasUnreadNotice">
						<el-icon title="消息">
							<ele-Bell />
						</el-icon>
					</el-badge>
				</template>
				<UserNews :noticeList="state.noticeList" />
			</el-popover>
		</div>
		<div class="layout-navbars-breadcrumb-user-icon" @click="onScreenfullClick">
			<i class="iconfont" :title="state.isScreenfull ? '关全屏' : '开全屏'"
				:class="!state.isScreenfull ? 'icon-fullscreen' : 'icon-tuichuquanping'"></i>
		</div>
		<div class="layout-navbars-breadcrumb-user-icon mr10" @click="onOnlineUserClick">
			<el-icon title="在线用户">
				<ele-User />
			</el-icon>
		</div>
		<el-dropdown :show-timeout="70" :hide-timeout="50" trigger="click" size="large" @command="onHandleCommandClick">
			<span class="layout-navbars-breadcrumb-user-link">
				<el-tooltip effect="dark" placement="left">
					<template #content>
						账号：{{ userInfos.account }}<br />
						姓名：{{ userInfos.realName }}<br />
						电话：{{ userInfos.phone }}<br />
						邮箱：{{ userInfos.email }}<br />
						部门：{{ userInfos.orgName }}<br />
						职位：{{ userInfos.posName }}<br />
					</template>
					<img :src="userInfos.avatar" class="layout-navbars-breadcrumb-user-link-photo mr5" />
				</el-tooltip>
				{{ userInfos.realName == '' ? userInfos.account : userInfos.realName }}
				<el-icon class="dropdown-icon">
					<ele-ArrowDown />
				</el-icon>
			</span>
			<template #dropdown>
				<el-dropdown-menu>
					<!-- <el-dropdown-item command="/dashboard/home">首页</el-dropdown-item> -->
					<el-dropdown-item :icon="Avatar" command="/system/userCenter">个人中心</el-dropdown-item>
					<el-dropdown-item :icon="Loading" command="clearCache">清理缓存</el-dropdown-item>
					<el-dropdown-item :icon="Switch" divided command="changeTenant"
						v-if="auth('sysTenant:changeTenant')">切换租户</el-dropdown-item>
					<el-dropdown-item :icon="Lock" divided command="lockScreen">开启锁屏</el-dropdown-item>
					<el-dropdown-item :icon="CircleCloseFilled" divided command="logOut">退出登录</el-dropdown-item>
				</el-dropdown-menu>
			</template>
		</el-dropdown>
		<Search ref="searchRef" />
		<OnlineUser ref="onlineUserRef" />
		<ChangeTenant ref="changeTenantRef" />
	</div>
</template>

<script setup lang="ts" name="layoutBreadcrumbUser">
import { defineAsyncComponent, ref, computed, reactive, onMounted } from 'vue';
import { useCssVar } from '@vueuse/core'
import { useRouter } from 'vue-router';
import { ElMessageBox, ElMessage, ElNotification } from 'element-plus';
import screenfull from 'screenfull';
import { storeToRefs } from 'pinia';
import { useUserInfo } from '/@/stores/userInfo';
import { useThemeConfig } from '/@/stores/themeConfig';
import other from '/@/utils/other';
import mittBus from '/@/utils/mitt';
import { Local, Session } from '/@/utils/storage';
import Push from 'push.js';
import { signalR } from '/@/views/system/onlineUser/signalR';
import { Avatar, CircleCloseFilled, Loading, Lock, Switch } from '@element-plus/icons-vue';
import { accessTokenKey, clearAccessAfterReload, getAPI } from '/@/utils/axios-utils';
import { SysAuthApi, SysNoticeApi, SysUserApi } from '/@/api-services/api';
import { auth } from '/@/utils/authFunction';
import { useLangStore } from '/@/stores/useLangStore';

const langStore = useLangStore();
// 引入组件
const UserNews = defineAsyncComponent(() => import('/@/layout/navBars/topBar/userNews.vue'));
const Search = defineAsyncComponent(() => import('/@/layout/navBars/topBar/search.vue'));
const OnlineUser = defineAsyncComponent(() => import('/@/views/system/onlineUser/index.vue'));
const ChangeTenant = defineAsyncComponent(() => import('./changeTenant.vue'));

// 定义变量内容
const router = useRouter();
const stores = useUserInfo();
const storesThemeConfig = useThemeConfig();
const { userInfos } = storeToRefs(stores);
const { themeConfig } = storeToRefs(storesThemeConfig);
const searchRef = ref();
const onlineUserRef = ref();
const changeTenantRef = ref();
const state = reactive({
	isScreenfull: false,
	disabledI18n: 'zh-cn',
	disabledSize: 'large',
	noticeList: [] as any, // 站内信列表
	languages: [] as any, // 语言列表
});
// 设置分割样式
const layoutUserFlexNum = computed(() => {
	let num: string | number;
	const { layout, isClassicSplitMenu } = themeConfig.value;
	const layoutArr: string[] = ['defaults', 'columns'];
	if (layoutArr.includes(layout) || (layout === 'classic' && !isClassicSplitMenu)) num = '1';
	else num = '';
	return num;
});
// 是否有未读消息
const hasUnreadNotice = computed(() => {
	return state.noticeList.some((r: any) => r.readStatus == undefined || r.readStatus == 0);
});
// 全屏点击时
const onScreenfullClick = () => {
	if (!screenfull.isEnabled) {
		ElMessage.warning('暂不不支持全屏');
		return false;
	}
	screenfull.toggle();
	screenfull.on('change', () => {
		if (screenfull.isFullscreen) state.isScreenfull = true;
		else state.isScreenfull = false;
	});
};
// 布局配置 icon 点击时
const onLayoutSetingClick = () => {
	mittBus.emit('openSettingsDrawer');
};
// 下拉菜单点击时
const onHandleCommandClick = (path: string) => {
	if (path === 'clearCache') {
		Local.clear();
		Session.clear();
		window.location.reload();
	} else if (path === 'lockScreen') {
		Local.remove('themeConfig');
		themeConfig.value.isLockScreen = true;
		themeConfig.value.lockScreenTime = 1;
		Local.set('themeConfig', themeConfig.value);
	} else if (path === 'logOut') {
		ElMessageBox({
			closeOnClickModal: false,
			closeOnPressEscape: false,
			title: '提示',
			message: '此操作将退出登录, 是否继续?',
			type: 'warning',
			showCancelButton: true,
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			buttonSize: 'default',
			beforeClose: async (action, instance, done) => {
				if (action === 'confirm') {
					instance.confirmButtonLoading = true;
					instance.confirmButtonText = '退出中';
					try {
						await getAPI(SysAuthApi).apiSysAuthLogoutPost();
					} catch (error) {
						console.error(error);
					}
					instance.confirmButtonLoading = false;
					done();
				} else {
					done();
				}
			},
		})
			.then(async () => {
				clearAccessAfterReload();
			})
			.catch(() => { });
	} else if (path === 'changeTenant') {
		changeTenantRef.value?.openDialog();
	} else {
		router.push(path);
	}
};
// 菜单搜索点击
const onSearchClick = () => {
	searchRef.value.openSearch();
};
// 在线用户列表
const onOnlineUserClick = () => {
	onlineUserRef.value.openDrawer();
};
// 组件大小改变
const onComponentSizeChange = (size: string) => {
	Local.remove('themeConfig');
	themeConfig.value.globalComponentSize = size;
	Local.set('themeConfig', themeConfig.value);
	initI18nOrSize('globalComponentSize', 'disabledSize');
	//window.location.reload();
};
// 语言切换
const onLanguageChange = async (lang: string) => {
	const langItem = state.languages.find((item: { value: string }) => item.value === lang);
	if (langItem) {
		await getAPI(SysUserApi).apiSysUserSetLangCodeLangCodePost(langItem.code);
		const accessToken = Local.get(accessTokenKey);
		await getAPI(SysAuthApi).apiSysAuthRefreshTokenGet(`${accessToken}`);
		window.location.reload();
	}
	Local.remove('themeConfig');
	themeConfig.value.globalI18n = lang;
	Local.set('themeConfig', themeConfig.value);
	window.$changeLang(lang)
	other.useTitle();
	initI18nOrSize('globalI18n', 'disabledI18n');
};
// 初始化组件大小/i18n
const initI18nOrSize = (value: string, attr: string) => {
	(<any>state)[attr] = Local.get('themeConfig')[value];

    // 设置菜单高度-横向
    useCssVar('--el-menu-horizontal-height').value = 'var(--el-menu-item-height-' + themeConfig.value.globalComponentSize + ')';
    useCssVar('--el-menu-horizontal-sub-item-height').value = 'var(--el-menu-item-height-' + themeConfig.value.globalComponentSize + ')';
    // 设置菜单高度-纵向
    useCssVar('--el-menu-item-height').value = 'var(--el-menu-item-height-' + themeConfig.value.globalComponentSize + ')';
    useCssVar('--el-menu-sub-item-height').value = 'var(--el-menu-item-height-' + themeConfig.value.globalComponentSize + ')';
};
// 页面加载时
onMounted(async () => {
	state.languages = langStore.languages;
	if (Local.get('themeConfig')) {
		initI18nOrSize('globalComponentSize', 'disabledSize');
		const userLangCode = userInfos.value.langCode;
		const langItem = state.languages.find((item: { code: string }) => item.code === userLangCode);
		if (langItem) {
			const themeConfig = Local.get('themeConfig');
			themeConfig.globalI18n = langItem.value;
			Local.set('themeConfig', themeConfig);
			initI18nOrSize('globalI18n', 'disabledI18n');
		} else {
			initI18nOrSize('globalI18n', 'disabledI18n');
		}
	}
	// 手动获取用户桌面通知权限
	if (Push.Permission.GRANTED) {
		// 判断当前是否有权限，没有则手动获取
		Push.Permission.request(undefined, undefined);
	}
	// 监听浏览器 当前系统是否在当前页
	document.addEventListener('visibilitychange', () => {
		if (!document.hidden) {
			// 清空关闭消息通知，
			Push.clear();
		}
	});
	// 加载未读的站内信
	var res = await getAPI(SysNoticeApi).apiSysNoticeUnReadListGet();
	state.noticeList = res.data.result ?? [];

	// 接收站内信
	signalR.on('PublicNotice', receiveNotice);

	// // 处理消息已读
	// mittBus.on('noticeRead', (id) => {
	// 	const notice = state.noticeList.find((r: any) => r.id == id);
	// 	if (notice == undefined) return;

	// 	// 设置已读
	// 	notice.readStatus = 1;
	// });
});
// // 页面卸载时
// onUnmounted(() => {
// 	mittBus.off('noticeRead', () => {});
// });

const receiveNotice = (msg: any) => {
	state.noticeList.unshift(msg);

	ElNotification({
		title: '提示',
		message: '您有一条新消息...',
		type: 'info',
		position: 'bottom-right',
	});
	Push.create('提示', {
		body: '你有一条新的消息',
		icon: 'logo.png', //public目录下的
		timeout: 4500, // 通知显示时间，单位为毫秒
	});
};
</script>

<style scoped lang="scss">
.layout-navbars-breadcrumb-user {
	display: flex;
	align-items: center;
	justify-content: flex-end;

	&-link {
		height: 100%;
		display: flex;
		align-items: center;
		white-space: nowrap;
        cursor: pointer;

		&-photo {
			width: 25px;
			height: 25px;
			border-radius: 100%;
		}

        .dropdown-icon {
            transition: transform 0.3s; /* 添加过渡效果 */
        }
        &:has(.dropdown-icon)[aria-expanded=true] {
            .dropdown-icon {
                transform: rotate(180deg);
            }
        }
	}

	&-icon {
		padding: 0 10px;
		cursor: pointer;
		color: var(--next-bg-topBarColor);
		height: 50px;
		line-height: 50px;
		display: flex;
		align-items: center;
        font-size: var(--el-font-size-medium);

		&:hover {
			background: var(--next-color-user-hover);

			i {
				display: inline-block;
				animation: logoAnimation 0.3s ease-in-out;
			}
		}
	}

	:deep(.el-dropdown) {
		color: var(--next-bg-topBarColor);
	}

	:deep(.el-badge) {
		height: 40px;
		line-height: 40px;
		display: flex;
		align-items: center;
	}

	:deep(.el-badge__content.is-fixed) {
		top: 12px;
	}
}
</style>
