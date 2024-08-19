<template>
	<el-tooltip :visible="state.capsLockVisible" effect="light" content="大写锁定已打开" placement="top">
		<el-form ref="ruleFormRef" :model="state.ruleForm" size="large" :rules="state.rules" class="login-content-form">
			<el-form-item class="login-animation1" prop="account">
				<el-input ref="accountRef" text placeholder="请输入账号" v-model="state.ruleForm.account" clearable autocomplete="off" @keyup.enter.native="handleSignIn">
					<template #prefix>
						<el-icon>
							<ele-User />
						</el-icon>
					</template>
				</el-input>
			</el-form-item>
			<el-form-item class="login-animation2" prop="password">
				<el-input ref="passwordRef" :type="state.isShowPassword ? 'text' : 'password'" placeholder="请输入密码" v-model="state.ruleForm.password" autocomplete="off" @keyup.enter.native="handleSignIn">
					<template #prefix>
						<el-icon>
							<ele-Unlock />
						</el-icon>
					</template>
					<template #suffix>
						<i class="iconfont el-input__icon login-content-password" :class="state.isShowPassword ? 'icon-yincangmima' : 'icon-xianshimima'" @click="state.isShowPassword = !state.isShowPassword">
						</i>
					</template>
				</el-input>
			</el-form-item>
			<el-form-item class="login-animation3" prop="captcha" v-if="state.captchaEnabled">
				<el-col :span="15">
					<el-input
						ref="codeRef"
						text
						maxlength="4"
						:placeholder="$t('message.account.accountPlaceholder3')"
						v-model="state.ruleForm.code"
						clearable
						autocomplete="off"
						@keyup.enter.native="handleSignIn"
					>
						<template #prefix>
							<el-icon>
								<ele-Position />
							</el-icon>
						</template>
					</el-input>
				</el-col>
				<el-col :span="1"></el-col>
				<el-col :span="8">
					<div :class="[state.expirySeconds > 0 ? 'login-content-code' : 'login-content-code-expired']" @click="getCaptcha">
						<img class="login-content-code-img" width="130px" height="38px" :src="state.captchaImage" style="cursor: pointer" />
					</div>
				</el-col>
			</el-form-item>
			<el-form-item class="login-animation4">
				<el-button type="primary" class="login-content-submit" round v-waves @click="handleSignIn" :loading="state.loading.signIn">
					<span>{{ $t('message.account.accountBtnText') }}</span>
				</el-button>
			</el-form-item>
			<div class="font12 mt30 login-animation4 login-msg">{{ $t('message.mobile.msgText') }}</div>
			<!-- <el-button type="primary" round v-waves @click="weixinSignIn" :loading="state.loading.signIn"></el-button> -->
		</el-form>
	</el-tooltip>
	<div class="dialog-header">
		<el-dialog v-model="state.rotateVerifyVisible" :show-close="false">
			<DragVerifyImgRotate
				ref="dragRef"
				:imgsrc="state.rotateVerifyImg"
				v-model:isPassing="state.isPassRotate"
				text="请按住滑块拖动"
				successText="验证通过"
				handlerIcon="fa fa-angle-double-right"
				successIcon="fa fa-hand-peace-o"
				@passcallback="passRotateVerify"
			/>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="loginAccount">
import { reactive, computed, ref, onMounted, defineAsyncComponent, onUnmounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ElMessage, InputInstance } from 'element-plus';
import { useI18n } from 'vue-i18n';
import { initBackEndControlRoutes } from '/@/router/backEnd';
import { Local, Session } from '/@/utils/storage';
import { formatAxis } from '/@/utils/formatTime';
import { NextLoading } from '/@/utils/loading';
import { sm2 } from 'sm-crypto-v2';
import { useThemeConfig } from '/@/stores/themeConfig';
import { storeToRefs } from 'pinia';

import { accessTokenKey, clearTokens, feature, getAPI } from '/@/utils/axios-utils';
import { SysAuthApi } from '/@/api-services/api';

// 旋转图片滑块组件
// import verifyImg from '/@/assets/logo-mini.svg';
const DragVerifyImgRotate = defineAsyncComponent(() => import('/@/components/dragVerify/dragVerifyImgRotate.vue'));

const storesThemeConfig = useThemeConfig();
const { themeConfig } = storeToRefs(storesThemeConfig);

const { t } = useI18n();
const route = useRoute();
const router = useRouter();

const ruleFormRef = ref();
const accountRef = ref<InputInstance>();
const passwordRef = ref<InputInstance>();
const codeRef = ref<InputInstance>();

const dragRef: any = ref(null);
const state = reactive({
	isShowPassword: false,
	ruleForm: {
		account: window.__env__.VITE_DEFAULT_USER,
		password: window.__env__.VITE_DEFAULT_USER_PASSWORD,
		code: '',
		codeId: 0,
	},
	rules: {
		account: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
		password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
		// code: [{ required: true, message: '请输入验证码', trigger: 'blur' }],
	},
	loading: {
		signIn: false,
	},
	captchaImage: '',
	rotateVerifyVisible: false,
	// rotateVerifyImg: verifyImg,
	rotateVerifyImg: themeConfig.value.logoUrl,
	secondVerEnabled: false,
	captchaEnabled: false,
	isPassRotate: false,
	capsLockVisible: false,
	expirySeconds: 60, // 验证码过期时间
});

// 验证码过期计时器
let timer: any = null;

// 页面初始化
onMounted(async () => {
	// 若URL带有Token参数（第三方登录）
	var accessToken = route.query.token;
	if (accessToken != null && accessToken != undefined) {
		await saveTokenAndInitRoutes(accessToken);
	}

	// 获取登录配置
	state.secondVerEnabled = themeConfig.value.secondVer ?? true;
	state.captchaEnabled = themeConfig.value.captcha ?? true;

	// 获取验证码
	getCaptcha();

	// 注册验证码过期计时器
	if (state.captchaEnabled) {
		timer = setInterval(() => {
			if (state.expirySeconds > 0) state.expirySeconds -= 1;
		}, 1000);
	}

	// 检测大小写按键/CapsLK
	document.addEventListener('keyup', handleKeyPress);
});

// 页面卸载
onUnmounted(() => {
	// 销毁验证码过期计时器
	clearInterval(timer);
	timer = null;

	document.removeEventListener('keyup', handleKeyPress);
});

// 检测大小写按键
const handleKeyPress = (e: KeyboardEvent) => {
	const isCapsLockOn = e.getModifierState('CapsLock');
	state.capsLockVisible = isCapsLockOn;
};

// 获取验证码
const getCaptcha = async () => {
	if (!state.captchaEnabled) return;

	state.ruleForm.code = '';
	var res = await getAPI(SysAuthApi).apiSysAuthCaptchaGet();
	state.captchaImage = 'data:text/html;base64,' + res.data.result?.img;
	state.ruleForm.codeId = res.data.result?.id;
	state.expirySeconds = res.data.result?.expirySeconds;
};

// 获取时间
const currentTime = computed(() => {
	return formatAxis(new Date());
});

// 登录
const onSignIn = async () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return false;

		try {
			state.loading.signIn = true;

			// SM2加密密码
			// const keys = SM2.generateKeyPair();
			const publicKey = window.__env__.VITE_SM_PUBLIC_KEY;
			const password = sm2.doEncrypt(state.ruleForm.password, publicKey, 1);

			const [err, res] = await feature(getAPI(SysAuthApi).apiSysAuthLoginPost({ ...state.ruleForm, password: password }));
			if (err) {
				getCaptcha(); // 重新获取验证码
				return;
			}
			if (res.data.result?.accessToken == undefined) {
				getCaptcha(); // 重新获取验证码
				ElMessage.error('登录失败，请检查账号！');
				return;
			}
			await saveTokenAndInitRoutes(res.data.result?.accessToken);
		} finally {
			state.loading.signIn = false;
		}
	});
};

// 保持Token并初始化路由
const saveTokenAndInitRoutes = async (accessToken: string | any) => {
	// 缓存token
	Local.set(accessTokenKey, accessToken);
	// Local.set(refreshAccessTokenKey, refreshAccessToken);
	Session.set('token', accessToken);

	// 添加完动态路由再进行router跳转，否则可能报错 No match found for location with path "/"
	const isNoPower = await initBackEndControlRoutes();
	signInSuccess(isNoPower); // 再执行 signInSuccess
};

// 登录成功后的跳转
const signInSuccess = (isNoPower: boolean | undefined) => {
	if (isNoPower) {
		ElMessage.warning('抱歉，您没有登录权限');
		clearTokens(); // 清空Token缓存
	} else {
		// 初始化登录成功时间问候语
		let currentTimeInfo = currentTime.value;
		// 登录成功，跳到转首页 如果是复制粘贴的路径，非首页/登录页，那么登录成功后重定向到对应的路径中
		if (route.query?.redirect) {
			router.push({
				path: <string>route.query?.redirect,
				query: Object.keys(<string>route.query?.params).length > 0 ? JSON.parse(<string>route.query?.params) : '',
			});
		} else {
			router.push('/');
		}

		// 登录成功提示
		const signInText = t('message.signInText');
		ElMessage.success(`${currentTimeInfo}，${signInText}`);
		// 添加 loading，防止第一次进入界面时出现短暂空白
		NextLoading.start();
	}
};

// 打开旋转验证
const openRotateVerify = () => {
	state.rotateVerifyVisible = true;
	state.isPassRotate = false;
	dragRef.value?.reset();
};

// 通过旋转验证
const passRotateVerify = () => {
	state.rotateVerifyVisible = false;
	state.isPassRotate = true;
	onSignIn();
};

// 登录处理
const handleSignIn = () => {
	if (!state.ruleForm.account) {
		accountRef.value?.focus();
	} else if (!state.ruleForm.password) {
		passwordRef.value?.focus();
	} else if (state.captchaEnabled && !state.ruleForm.code) {
		codeRef.value?.focus();
	} else {
		state.secondVerEnabled ? openRotateVerify() : onSignIn();
	}
};

// // 微信登录
// const weixinSignIn = () => {
// 	window.open('http://localhost:5005/api/sysoauth/signin?provider=Gitee&redirectUrl=http://localhost:8888');
// };

// 导出对象
defineExpose({ saveTokenAndInitRoutes });
</script>

<style lang="scss" scoped>
.dialog-header {
	:deep(.el-dialog) {
		width: unset !important;

		.el-dialog__header {
			display: none;
		}

		.el-dialog__wrapper {
			position: absolute !important;
		}

		.v-modal {
			position: absolute !important;
		}
	}
}

.login-content-form {
	margin-top: 20px;

	@for $i from 0 through 4 {
		.login-animation#{$i} {
			opacity: 0;
			animation-name: error-num;
			animation-duration: 0.5s;
			animation-fill-mode: forwards;
			animation-delay: calc($i/10) + s;
		}
	}

	.login-content-password {
		display: inline-block;
		width: 20px;
		cursor: pointer;

		&:hover {
			color: #909399;
		}
	}

	.login-content-code {
		display: flex;
		align-items: center;
		justify-content: space-around;
		position: relative;

		.login-content-code-img {
			width: 100%;
			height: 40px;
			line-height: 40px;
			background-color: #ffffff;
			border: 1px solid rgb(220, 223, 230);
			cursor: pointer;
			transition: all ease 0.2s;
			border-radius: 4px;
			user-select: none;

			&:hover {
				border-color: #c0c4cc;
				transition: all ease 0.2s;
			}
		}
	}

	.login-content-code-expired {
		@extend .login-content-code;
		&::before {
			content: '验证码已过期';
			position: absolute;
			top: 0;
			left: 0;
			right: 0;
			bottom: 0;
			border-radius: 4px;
			background-color: rgba(0, 0, 0, 0.5);
			color: #ffffff;
			text-align: center;
		}
	}

	.login-content-submit {
		width: 100%;
		letter-spacing: 2px;
		font-weight: 300;
		margin-top: 15px;
	}

	.login-msg {
		color: var(--el-text-color-placeholder);
	}
}
</style>
