<template>
	<div class="sys-video-container">
		<el-container>
			<el-header>视频监控（萤石云云直播）</el-header>
			<el-container>
				<el-aside width="200px">
					<el-tree :data="data" :props="defaultProps" @node-click="handleNodeClick" />
				</el-aside>

				<el-main>
					<div class="updateToken" props="ezviz_video">
						<ul>
							<li>
								<el-span>密匙串：</el-span>
								<el-input
									placeholder="密匙"
									show-word-limit
									type="text"
									id="txt_token"
									title="每周更新（开放平台，云直播，轻应用，代码示例）"
									v-model="ezviz_video.ezvizToken"
									@keyup.enter="update_Token"
									class="token_input"
								/>
							</li>
							<li>
								<el-span>视频流：</el-span>
								<el-input
									placeholder="萤石云视频流地址"
									show-word-limit
									type="text"
									id="txt_url"
									title="密匙对应的视频流地址(高清后缀.h.live)"
									v-model="ezviz_video.ezvizUrl"
									@keyup.enter="update_Token"
									class="token_input"
								/>
							</li>
						</ul>
					</div>

					<div class="video">
						<div class="video-item">
							<div class="item">
								<div class="home" ref="viewtoolOne">
									<div id="video-container">等待加载...</div>
								</div>
							</div>
						</div>
					</div>
				</el-main>
			</el-container>
		</el-container>
	</div>
</template>

<!-- 
库：https://github.com/Ezviz-OpenBiz/EZUIKit-JavaScript-npm
安装：npm install ezuikit-js 或 pnpm add ezuikit-js
-->
<script lang="ts" setup name="video">
import { reactive, ref, onMounted, nextTick, beforeDestroy } from 'vue';
import EZUIKit from 'ezuikit-js'; //页面引用
//import { ElNotification } from 'element-plus';
//import { Search,ChatDotSquare,TopRight,Star,Operation,Setting,Connection,Discount,Open,Delete,Position,View,CopyDocument,DocumentChecked,VideoCamera} from '@element-plus/icons-vue';
import mittBus from '/@/utils/mitt'; //事件总线mitt 解决打包后错误Uncaught (in promise) ReferenceError: Cannot access 'oe' before initialization

let ezvizPlayOne = ref(null);
let ezvizPlayTwo = ref(null);
let ezvizPlayThree = ref(null);
let ezvizPlayFour = ref(null);
let viewtoolOne = ref();
let viewtoolTwo = ref();
let viewtoolThree = ref();
let viewtoolFour = ref();

interface Tree {
	label: string;
	children?: Tree[];
}

const defaultProps = {
	children: 'children',
	label: 'label',
};

const handleNodeClick = (data: Tree) => {
	console.log(data);
};

// 更新token
function update_Token(e) {
	//ezviz_video.ezvizToken=e.target.value;
	console.log(e.target.value);
	autoVideoOne('video-container');
}

onMounted(async () => {
	autoVideoOne('video-container');
	//console.log('https://open.ys7.com/console/ezuikit/template/detail.html?themeId=pcLive&editing=false');
});

// 测试 ezopen://open.ys7.com/G39444019/1.live 和 at.3bvmj4ycamlgdwgw1ig1jruma0wpohl6-48zifyb39c-13t5am6-yukyi86mz
// 备用 ezopen://open.ys7.com/AA2615287/1.live 和 ra.5k88qgc34vgr9yva7rlub985blo9ph7k-92q0bl2r4r-0aygaog-5cofhebpm
const ezviz_video = reactive({
	ezvizToken: 'ra.5k88qgc34vgr9yva7rlub985blo9ph7k-92q0bl2r4r-0aygaog-5cofhebpm', //需要修改每周（开放平台，云直播，轻应用，代码示例中找）演示设备
	ezvizUrl: 'ezopen://open.ys7.com/AA2615287/1.live', //高清直播拼接字符串   cosnt url = `ezopen://${item.identifyingCode}@open.ys7.com/${item.imei}/${item.channelNo}.hd.live`
	// 回放地址ezopen://open.ys7.com/AA2615287/1.rec
});

// beforeDestroy(()=>{
// 	ezvizPlayOne.value  && ezvizPlayOne.value.stop() //销毁并停止直播视频
// 	console.log('beforeDestroy');
// });

// 监控1，参数https://blog.csdn.net/weixin_53791978/article/details/126489296
function autoVideoOne(params) {
	// 获取父节点的宽高
	let divW = viewtoolOne.value.clientWidth;
	let divH = viewtoolOne.value.clientHeight;
	if (ezvizPlayOne.value != null) {
		return;
	}

	// 获取萤石token
	ezvizPlayOne.value = new EZUIKit.EZUIKitPlayer({
		autoplay: true, // 默认播放
		// 视频播放包括元素
		id: 'video-container', //DIV容器
		// 萤石token，https://open.ys7.com/console/ezuikit/template/detail.html?themeId=pcLive&editing=false中查询实例代码
		accessToken: ezviz_video.ezvizToken, //"ra.bl9n4hmb3c7w4fk6bbuumtmdcbbo66w0-3k7nal0q6y-0lp00m5-fi61isesz",
		// ezopen://open.ys7.com/${设备序列号}/{通道号}.live
		url: ezviz_video.ezvizUrl, //"ezopen://open.ys7.com/AA2615287/1.live", // 播放地址
		template: 'standard', // pcLive，simple - 极简版;standard-标准版;security - 安防版(预览回放);voice-语音版；theme-可配置主题；
		useHardDev: true, // 开启高性能模式 依赖需高于7.7.x 截止到2023.11.7 建议保持最新版本为7.7.6
		// header: ['capturePicture', 'zoom'], // 如果templete参数不为simple,该字段将被覆盖
		//plugin: ['talk'], // 加载插件，talk-对讲
		// 视频下方底部控件
		//footer: ["talk", "broadcast", "hd", "fullScreen"], // 如果template参数不为simple,该字段将被覆盖
		footer: ['talk', 'hd', 'fullScreen'], // 如果template参数不为simple,该字段将被覆盖
		//audio: 0, // 是否默认开启声音 0 - 关闭 1 - 开启
		// openSoundCallBack: data => console.log("开启声音回调", data),
		// closeSoundCallBack: data => console.log("关闭声音回调", data),
		// startSaveCallBack: data => console.log("开始录像回调", data),
		// stopSaveCallBack: data => console.log("录像回调", data),
		// capturePictureCallBack: data => console.log("截图成功回调", data),
		// fullScreenCallBack: data => console.log("全屏回调", data),
		// getOSDTimeCallBack: data => console.log("获取OSDTime回调", data),
		width: divW,
		height: divH,
		handleError: (err: any) => {
			if (err.type === 'handleRunTimeInfoError' && err.data.nErrorCode === 5) {
				console.log('加密设备密码错误');
			}
		},
	});
}

const data: Tree[] = [
	{
		label: '节点A',
		children: [
			{
				label: '菜单A-1',
				children: [
					{
						label: '菜单A-1-1',
					},
				],
			},
		],
	},
	{
		label: '节点B',
		children: [
			{
				label: '菜单B-1',
				children: [
					{
						label: '菜单B-1-1',
					},
				],
			},
			{
				label: '菜单B-2',
				children: [
					{
						label: '菜单B-2-1',
					},
				],
			},
		],
	},
	{
		label: '节点C',
		children: [
			{
				label: '菜单C-1',
				children: [
					{
						label: '菜单C-1-1',
					},
				],
			},
			{
				label: '菜单C-2',
				children: [
					{
						label: '菜单C-2-1',
					},
				],
			},
		],
	},
];
</script>

<style lang="scss" scoped>
.sys-video-container {
	overflow: hidden;
	height: 100vh;
}
.common-layout {
	background-color: #ecf5ff;
}

.el-header {
	text-align: center;
	height: 45px;
	line-height: 45px;
	font-size: 22px;
	background-color: #eee;
	padding: 5px auto;
}
.el-aside {
	text-align: center;
	padding: 4px auto;
	overflow: hidden;
}
.el-aside .el-form {
	text-align: center;
	padding: 2px auto;
	margin: 4px;
}
.el-aside .el-button {
	margin: 2px;
}
.el-aside .el-input {
	font-size: 14px;
	padding: 2px;
}
.el-aside .el-card {
	margin: 10px 0 10px auto;
}
.el-aside .el-card .el-button {
	width: 90px;
}
.el-form-item {
	font-weight: bold;
}

.el-main {
	background-color: #111;
	padding: 4px;
	width: 100%;
	overflow: hidden;
	color: #fff;
}

.recvs {
	overflow-y: auto;
	overflow-x: hidden;
	width: 100%;
	height: 800px;
}
.rev_title {
	width: 100%;
	display: block;
	font-style: italic;
	color: #999;
	font-size: 14px;
	background-color: #fafafa;
	padding: 2px 4px;
	line-height: 25px;
}
.rev_conts {
	width: 100%;
	word-wrap: break-word;
	line-height: 1.5em;
	padding: 4px;
	line-height: 30px;
} /*缩进text-indent:2em;*/
.recvfontsize {
	text-align: center;
	display: block;
	padding-top: 4px;
}
.el-color-picker {
	margin-left: 4px;
}
.recv_count {
	text-align: left;
}
.recv_count p {
	line-height: 30px;
}

.header {
	font-size: 24px;
	font-weight: bold;
	margin: -10px auto 10px auto;
}

h1 {
	font-size: 16px;
	margin-top: 10px auto 20px auto;
	padding: 5px 0px 5px 0;
}

.el-col {
	padding: 4px;
}

.el-input {
	font-size: 13px;
}
.el-card {
	margin-bottom: 12px;
}
.el-card__body {
	padding: 24px;
}

.el-select {
	width: 100%;
}

.text-right {
	text-align: right;
}

.sub-btn {
	margin-top: 30px;
}

.updateToken {
	display: block;
	line-height: 30px;
	align: left;
	background: rgb(250, 250, 250, 0.2);
	padding: 5px;
}

.token_input {
	width: 90%;
}
.hidden {
	display: none;
}
.w80 {
	width: 80px;
}
.w100 {
	width: 100px;
}
.log {
	font-size: 14px;
	color: #fff;
	background-color: black;
}
.center {
	text-align: center;
}
#ch1,
#ch2,
#ch3,
#ch4,
#ch5 {
	width: 120px;
}
.el-tag {
	padding: auto 4px;
	margin: 5px;
	min-width: 60px;
}

el-tree span {
	line-height: 50px;
}

.video {
	width: 100%;
	height: 100%;
	overflow: hidden;

	.video-item {
		display: flex;
		padding: 5px;
		overflow: hidden;

		.item {
			flex: 1;

			min-height: 40%;
			width: 100%;
			margin: 0 5px;
			background-color: #000000;
			color: #fff;
			border-radius: 2px;

			.home {
				width: 100%;
				height: 70vh;
				overflow: hidden;
				padding: 0;
				marigin: 0;
				aspect-ratio: 16/9; /* 设置任意宽高任意一项即可 然后使用aspect-ratio元素 动态设置比例 */
				text-align: center;
				justify-content: center;
			}
		}
	}
}
</style>
