<template>
	<div class="mqtt-box">
		<h1 class="header">MQTTX在线测试客户端</h1>
		<el-card :model="connection">
			<h1>连接参数(Configuration)</h1>
			<el-form label-position="top" :model="connection">
				<el-row :gutter="6">
					<el-col :span="8">
						<el-form-item prop="host" label="协议|主机|端口">
							<el-input v-model="connection.host" :disabled="connSuccess" type="password" show-password>
								<template #prepend>
									<el-select v-model="connection.protocol" class="w80" :disabled="connSuccess" @change="handleProtocolChange">
										<el-option label="ws://" value="ws"></el-option>
										<el-option label="wss://" value="wss"></el-option>
									</el-select>
								</template>
								<template #append>
									<el-input v-model.number="connection.port" type="number" class="w80" :disabled="connSuccess" placeholder="8083/8084"></el-input>
								</template>
							</el-input>
						</el-form-item>
					</el-col>
					<el-col :span="0">
						<el-form-item prop="clientId" label="标识(Client ID)唯一性">
							<el-input v-model="connection.clientId"> </el-input>
						</el-form-item>
					</el-col>
					<el-col :span="0">
						<el-form-item prop="username" label="账号(Username)">
							<el-input v-model="connection.username"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="0">
						<el-form-item prop="password" label="密码(Password)">
							<el-input v-model="connection.password" type="password" show-password></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="4">
						<el-form-item prop="regpacket" label="设备包名(Regpacket)">
							<el-input v-model="connection.repacket" :disabled="connSuccess" @input="syncdhtreg" @change="init_topic"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="4">
						<el-form-item prop="dhtRegpack" label="共享传感器(dhtRegpacket)">
							<el-input v-model="connection.dhtRegpack" :disabled="connSuccess" @change="init_topic"></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8" class="text-right">
						<el-button
							type="primary"
							:icon="Setting"
							class="sub-btn"
							:disabled="client.connected"
							@click="createConnection"
							:loading="btnLoadingType === 'connect'"
							:style="{ display: client.connected ? 'none' : '' }"
						>
							{{ client.connected ? '已连接(Connected)' : '连接(Connect)' }}
						</el-button>
						<el-button v-if="client.connected" class="sub-btn" type="warning" :icon="Discount" @click="destroyConnection" :loading="btnLoadingType === 'disconnect'"> 断开(Disconnect) </el-button>
					</el-col>
				</el-row>
			</el-form>
		</el-card>

		<el-card shadow="hover">
			<h1>订阅(Subscribe)</h1>
			<el-form label-position="top" :model="subscription">
				<el-row :gutter="6">
					<el-col :span="12">
						<el-form-item prop="topic" label="订阅主题(Topic)">
							<el-input v-model="connection.subTopics" :disabled="subscribedSuccess" type="password" show-password></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="4">
						<el-form-item prop="qos" label="订阅质量(QoS)">
							<el-select v-model="subscription.qos" :disabled="subscribedSuccess">
								<el-option v-for="qos in qosList" :key="qos" :label="qos == 0 ? '0 至多一次' : qos == 1 ? '1 至少一次' : '2 仅仅一次'" :value="qos"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8" class="text-right">
						<el-button
							type="primary"
							:icon="Connection"
							class="sub-btn"
							:style="{ display: subscribedSuccess ? 'none' : '' }"
							:loading="btnLoadingType === 'subscribe'"
							:disabled="!client.connected || subscribedSuccess"
							@click="doSubscribe"
						>
							{{ subscribedSuccess ? '已订阅(Subscribed)' : '订阅(Subscribe)' }}
						</el-button>
						<el-button v-if="subscribedSuccess" type="warning" :icon="Discount" class="sub-btn" :loading="btnLoadingType === 'unsubscribe'" :disabled="!client.connected" @click="doUnSubscribe">
							取消(Unsubscribe)
						</el-button>
					</el-col>
				</el-row>
			</el-form>
		</el-card>

		<el-card shadow="hover">
			<h1>发布(Publish)</h1>
			<el-form label-position="top" :model="publish">
				<el-row :gutter="6">
					<el-col :span="8">
						<el-form-item prop="topic" label="发布主题(Topic)">
							<el-input v-model="connection.pubTopic" type="password" show-password></el-input>
						</el-form-item>
					</el-col>
					<el-col :span="4">
						<el-form-item prop="qos" label="发布质量(QoS)">
							<el-select v-model="publish.qos">
								<el-option v-for="qos in qosList" :key="qos" :label="qos == 0 ? '0 至多一次' : qos == 1 ? '1 至少一次' : '2 仅仅一次'" :value="qos"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="4">
						<el-form-item prop="retain" label="发布保留(Retain)">
							<el-select v-model="publish.retain">
								<el-option value="false" label="false 不保留"></el-option>
								<el-option value="true" label="true 不保留"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
				</el-row>

				<el-row :gutter="6">
					<el-col :span="16">
						<el-form-item prop="payload" label="操作指令(Payload)">
							<el-input v-model="publish.payload" clearable maxlength="64" show-word-limit>
								<!--<template #prepend>
								<el-button :icon="Operation" />
								</template> -->
								<template #append>
									<el-select v-model="publish.payload" placeholder="选择指令" style="width: 115px">
										<el-option label="状态查询" value="55 AA AA AA AA 91 CF" />
										<el-option label="全部打开" value="55 AA AA AA AA 81 A4 01" />
										<el-option label="全部关闭" value="55 AA AA AA AA 81 A4 00" />
										<el-option label="一路开关" value="55 AA AA AA AA 81 BA 01" />
										<el-option label="二路开关" value="55 AA AA AA AA 81 BA 02" />
										<el-option label="三路开关" value="55 AA AA AA AA 81 BA 03" />
										<el-option label="四路开关" value="55 AA AA AA AA 81 BA 04" />
									</el-select>
								</template>
							</el-input>
						</el-form-item>
					</el-col>
					<el-col :span="8" class="text-right">
						<el-button type="success" :icon="Position" class="sub-btn" :loading="btnLoadingType === 'publish'" :disabled="!client.connected" @click="doPublish(publish.payload, connection.pubTopic)">
							发布(Publish)
						</el-button>
					</el-col>
				</el-row>
			</el-form>
		</el-card>

		<el-card shadow="hover">
			<h1>
				<el-button @click="clsmsg" type="success" :icon="Delete" title="点击清空历史记录">接收(Receive)</el-button>
				<el-tag title="接收次数">收 {{ recvnum }}</el-tag>
				<el-tag :title="dht_tm">{{ dht_wsd }}</el-tag>
				<el-tag title="设备已工作时长">{{ parseInt(runSeconds) }} 秒</el-tag>
				<el-button
					type="success"
					title="关闭一路"
					:disabled="!connection.onlineStatus || !client.connected"
					v-if="connection.ch1_Status"
					icon="ele-Check"
					id="ch1"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 01 00')"
					>关闭</el-button
				>
				<el-button
					type="warning"
					title="打开一路"
					:disabled="!connection.onlineStatus || !client.connected"
					v-else="!connection.ch1_Status"
					icon="ele-CloseBold"
					id="ch1"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 01 01')"
					>打开</el-button
				>
				<el-button
					type="success"
					title="关闭二路"
					:disabled="!connection.onlineStatus || !client.connected"
					v-if="connection.ch2_Status"
					icon="ele-Check"
					id="ch2"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 02 00')"
					>关闭</el-button
				>
				<el-button
					type="warning"
					title="打开二路"
					:disabled="!connection.onlineStatus || !client.connected"
					v-else="!connection.ch2_Status"
					icon="ele-CloseBold"
					id="ch2"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 02 01')"
					>打开</el-button
				>
				<el-button
					type="success"
					title="关闭三路"
					:disabled="!connection.onlineStatus || !client.connected"
					v-if="connection.ch3_Status"
					icon="ele-Check"
					id="ch3"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 03 00')"
					>关闭</el-button
				>
				<el-button
					type="warning"
					title="打开三路"
					:disabled="!connection.onlineStatus || !client.connected"
					v-else="!connection.ch3_Status"
					icon="ele-CloseBold"
					id="ch3"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 03 01')"
					>打开</el-button
				>
				<el-button
					type="success"
					title="关闭四路"
					:disabled="!connection.onlineStatus || !client.connected"
					v-if="connection.ch4_Status"
					icon="ele-Check"
					id="ch4"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 04 00')"
					>关闭</el-button
				>
				<el-button
					type="warning"
					title="打开四路"
					:disabled="!connection.onlineStatus || !client.connected"
					v-else="!connection.ch4_Status"
					icon="ele-CloseBold"
					id="ch4"
					v-reclick="2000"
					@click="switchLight('55 AA AA AA AA 81 04 01')"
					>打开</el-button
				>
				<el-button
					type="danger"
					title="四路全部关闭"
					:disabled="!connection.onlineStatus || !client.connected"
					v-if="connection.all_Status"
					icon="ele-SwitchButton"
					id="ch5"
					@click="switchLight('55 AA AA AA AA 81 A4 00')"
					>全关</el-button
				>
				<el-button
					type="success"
					title="四路全部打开"
					:disabled="!connection.onlineStatus || !client.connected"
					v-else="!connection.all_Status"
					icon="ele-Switch"
					id="ch5"
					@click="switchLight('55 AA AA AA AA 81 A4 01')"
					>全开</el-button
				>

				<el-alert v-if="!client.connected || !connection.onlineStatus" title="网络服务断开或设备离线!" center type="warning" effect="light" style="margin-top: 4px" />
			</h1>
			<!-- 绑定接收日志，只读 -->
			<el-col :span="24">
				<el-input type="textarea" :rows="8" id="recv" v-model="receivedMessages" readonly class="log"></el-input>
			</el-col>
		</el-card>
	</div>
</template>

<script setup lang="ts" name="mqttx">
import { reactive, ref, onMounted, nextTick } from 'vue';
import { Search, ChatDotSquare, TopRight, Star, Operation, Setting, Connection, Discount, Open, Delete, Position } from '@element-plus/icons-vue';
import * as MQTT from 'mqtt/dist/mqtt.min'; // 针对4.3.7版本的引用方法。5.7.x会提示错误 (import * as MQTT from "mqtt")
import mittBus from '/@/utils/mitt'; // 事件总线mitt 解决打包后错误Uncaught (in promise) ReferenceError: Cannot access 'oe' before initialization

// vue 3 + vite use MQTT.js refer to https://github.com/mqttjs/MQTT.js/issues/1269
// https://github.com/mqttjs/MQTT.js#qos
const qosList = [0, 1, 2]; // 质量
const now = new Date();
const recvnum = ref(0);
const dht_wd = ref(0); // 温度、湿度
const dht_sd = ref(0);
const dht_tm = ref(''); // 同步时间
const dht_wsd = ref('温度0℃,湿度0%');
const runSeconds = ref(0); // 工作时长

// mqtt客户端变量 let或const
const client = ref({
	connected: false, //未连接
} as MQTT.MqttClient);

const receivedMessages = ref('');
const subscribedSuccess = ref(false); //订阅成功标志
const connSuccess = ref(false); //连接成功标志
const btnLoadingType = ref('');
const retryTimes = ref(0); //重连次数

/**
 * this demo uses EMQX Public MQTT Broker (https://www.emqx.com/en/mqtt/public-mqtt5-broker), here are the details:
 * 参考https://github.com/emqx/MQTT-Client-Examples
 * 方法https://github.com/mqttjs/MQTT.js
 * Broker host: broker.emqx.io
 * WebSocket port: 8083
 * WebSocket over TLS/SSL port: 8084
 * ws -> 8083; wss -> 8084
 * By default, EMQX allows clients to connect without authentication.
 * https://docs.emqx.com/en/enterprise/v4.4/advanced/auth.html#anonymous-login

 * for more options and details, please refer to https://github.com/mqttjs/MQTT.js#mqttclientstreambuilder-options
 */
const connection = reactive({
	protocol: 'ws',
	host: 'broker.emqx.io',
	// ws -> 8083; wss -> 8084
	port: 8083,
	clientId: 'emqx_vue3_' + Math.random().toString(16).substring(2, 8),
	username: '',
	password: '',
	repacket: 'd1ca1ff51f04', //注册包（改为您的注册包）
	dhtRegpack: 'd1ca1ff51f04', //温度注册包（可以相同可以共享传感器）
	mqttToken: '0804d4c44c1f1bd11dea461481f19868', //授权TOKEN自己约定
	keepalive: 30,
	clean: true, //清除 clean session
	connectTimeout: 30 * 1000, // ms 超时毫秒
	reconnectPeriod: 5000, // ms 重连毫秒
	resubscribe: true, //重新订阅
	//定义您自己的主题
	subTopic: 'mqtt/admintnet/#0#/out',
	willTopic: 'mqtt/admintnet/#0#/will',
	dhtTopic: 'mqtt/admintnet/#0#/dht',
	pubTopic: 'mqtt/admintnet/#0#/into',
	subTopics: [],
	pubPayload: '{"msg":"hellow vue3 mqtt."}',
	onlineStatus: false,
	ch1_Status: false,
	ch2_Status: false,
	ch3_Status: false,
	ch4_Status: false,
	all_Status: false,
	isAC: null, //强电true
});
// 初始化主题
const init_topic = () => {
	let st = 'mqtt/admintnet/#0#/out'; //订阅主题
	let pt = 'mqtt/admintnet/#0#/into'; //发布主题
	let ptbody = '{"token":"{0}","cmd":"{1}","cmdpara":"{2}","clientid":"{3}"}';
	let wt = 'mqtt/admintnet/#0#/will'; //遗嘱主题
	let dh = 'mqtt/admintnet/#0#/dht'; //温湿度
	connection.subTopic = st.replace('#0#', connection.repacket);
	connection.willTopic = wt.replace('#0#', connection.repacket);
	connection.dhtTopic = dh.replace('#0#', connection.dhtRegpack); //温湿度
	connection.pubTopic = pt.replace('#0#', connection.repacket);
	connection.subTopics = [connection.subTopic, connection.willTopic, connection.dhtTopic];
	connection.pubPayload = ptbody;
	//console.log(connection.subTopics);
};

// 默认注册包同步和传感器包名一致，反之不动
const syncdhtreg = () => {
	connection.dhtRegpack = connection.repacket;
};

// 字符串替换模拟  string.format(str,ar1,arn)
const stringFormat = (formatted, args) => {
	for (let i = 0; i < args.length; i++) {
		let regexp = new RegExp('\\{' + i + '\\}', 'gi');
		formatted = formatted.replace(regexp, args[i]);
	}
	return formatted;
};

onMounted(async () => {
	init_topic();
	nextTick(() => {});
});

// topic & QoS for MQTT subscribing 订阅主题(多个)
const subscription = ref({
	topic: `$(connection.subTopics.value)`,
	qos: 0 as MQTT.QoS,
});

// topic, QoS & payload for publishing message 发布主题
const publish = ref({
	topic: `${connection.pubTopic}`,
	qos: 0 as MQTT.QoS,
	retain: false, //保留否
	payload: '55 AA AA AA AA 91 CF', //'{ "msg": "Hello, I am browser." }',
});

const initData = () => {
	client.value = {
		connected: false,
	} as MQTT.MqttClient;
	retryTimes.value = 0;
	btnLoadingType.value = '';
	subscribedSuccess.value = false;
};

const handleOnReConnect = () => {
	retryTimes.value++;
	connection.clientId = 'emqx_vue3_' + Math.random().toString(16).substring(2, 8);
	console.log(retryTimes.value, '重试次数');
	if (retryTimes.value > 5) {
		try {
			client.value.end(); //重连超过5次断开
			initData();
			console.log('connection maxReconnectTimes limit, stop retry');
			appmessage(now.toLocaleString() + '|超出重连接次数，停止重试' + retryTimes.value);
		} catch (error) {
			console.log('handleOnReConnect catch error:', error);
		}
	}
};

/**
 * if protocol is "ws", connectUrl = "ws://broker.emqx.io:8083/mqtt"
 * if protocol is "wss", connectUrl = "wss://broker.emqx.io:8084/mqtt"
 *
 * /mqtt: MQTT-WebSocket uniformly uses /path as the connection path,
 * which should be specified when connecting, and the path used on EMQX is /mqtt.
 *
 * for more details about "mqtt.connect" method & options,
 * please refer to https://github.com/mqttjs/MQTT.js#mqttconnecturl-options
 */
// create MQTT connection 创建连接
const createConnection = () => {
	try {
		btnLoadingType.value = 'connect';
		const { protocol, host, port, ...options } = connection;
		const connectUrl = `${protocol}://${host}:${port}/mqtt`; //组成新的连接字符串
		console.log(connectUrl, '连接地址');
		client.value = MQTT.connect(connectUrl, options);
		if (client.value.on) {
			// https://github.com/mqttjs/MQTT.js#event-connect
			client.value.on('connect', () => {
				//v5.x  reconnect
				btnLoadingType.value = '';
				connSuccess.value = true; //client.value.connected;
				console.log('connection successful', client.value.connected);
				appmessage(now.toLocaleString() + '|连接服务成功');
			});

			// https://github.com/mqttjs/MQTT.js#event-reconnect 重连回调
			client.value.on('reconnect', handleOnReConnect);
			// https://github.com/mqttjs/MQTT.js#event-error
			client.value.on('error', (error) => {
				console.log('connection error:', error);
				appmessage(now.toLocaleString() + '|发生错误：' + error);
			});

			// https://github.com/mqttjs/MQTT.js#event-message 接收消息，处理方法单独定义
			client.value.on('message', (topic: string, message) => {
				//处理方法
				recvnum.value++; //接收次数累计
				doAction(topic, message); //处理
				receivedMessages.value = receivedMessages.value.concat(
					//拼接字符串输出
					now.toLocaleString() + ' ' + `${topic}\r\n` + message.toString() + '\r\n'
				);
				// console.log(now.toLocaleString()+`收到消息: ${message} from topic: ${topic}`);
				//滚动此方法可行
				nextTick(() => {
					setTimeout(() => {
						syncBottom(); //滚动到底部
					}, 50);
				});
			});
		}
	} catch (error) {
		btnLoadingType.value = '';
		console.log('mqtt.connect error:', error);
	}
};

// 处理事件
const doAction = (t, msg) => {
	let res = JSON.parse(msg.toString()); //必须规范的json格式否则出错，双引号不能是单引号；；；后不安全但强大 eval('(' + message.toString() + ')'); //JSON.parse(message.toString());//json对象

	// 消息不能带''否则错误
	let regp = res.regpacket; // 接收的注册包
	let regs = connection.repacket; // 订阅的注册包
	let isOK = regp == regs ? true : false; // 是不是本设备的消息
	if (!isOK || regp == null) {
		return; // 不是丢弃
	}

	if (t == connection.dhtTopic) {
		// 温湿度
		let rp = res.regpacket;
		let wd = res.temperature;
		let sd = res.humidity;
		let sj = res.time;
		let sc = res.runsec;
		if (rp != connection.dhtRegpack) {
			// 来自订阅的温湿度包
			return;
		}
		if (rp != null) {
			dht_wd.value = wd; // 实际应用时替换此3个变量即可
			dht_sd.value = sd;
			dht_tm.value = '更新时间:' + sj;
			runSeconds.value = sc;
			dht_wsd.value = '温度:' + dht_wd.value + '℃,湿度:' + dht_sd.value + '%';
			//state.option.title.text="实时温湿度变化趋势图(运行"+parseInt(sc)+"秒)";
			//updatechart(false);//实时数据(这种方法是实时推送，如果用 定时器 是定时显示的)updatewsd_time(false)
		}
	}
	if (t == connection.willTopic) {
		// 遗嘱
		if (res.redata == 'offline') {
			connection.onlineStatus = false;
		} else {
			connection.onlineStatus = true;
		}
	}
	if (t == connection.subTopic) {
		let rp0 = res.regpacket;
		if (rp0 != undefined) {
			if (rp0 == regs) {
				op(res.redata); // 该设备执行指令其他放弃
			}
		}
	}
};

// 处理开关状态(自定义的指令，需要修改为您自己的指令)
const op = (cmd: any) => {
	if (cmd == '55 AA AA AA AA 82 01 01') {
		connection.ch1_Status = true;
	}
	if (cmd == '55 AA AA AA AA 82 01 00') {
		connection.ch1_Status = false;
	}
	if (cmd == '55 AA AA AA AA 82 02 01') {
		connection.ch2_Status = true;
	}
	if (cmd == '55 AA AA AA AA 82 02 00') {
		connection.ch2_Status = false;
	}
	if (cmd == '55 AA AA AA AA 82 03 01') {
		connection.ch3_Status = true;
	}
	if (cmd == '55 AA AA AA AA 82 03 00') {
		connection.ch3_Status = false;
	}
	if (cmd == '55 AA AA AA AA 82 04 01') {
		connection.ch4_Status = true;
	}
	if (cmd == '55 AA AA AA AA 82 04 00') {
		connection.ch4_Status = false;
	}
	if (cmd == '55 AA AA AA AA 82 A4 01') {
		connection.ch1_Status = true;
		connection.ch2_Status = true;
		connection.ch3_Status = true;
		connection.ch4_Status = true;
	}
	if (cmd == '55 AA AA AA AA 82 A4 00') {
		connection.ch1_Status = false;
		connection.ch2_Status = false;
		connection.ch3_Status = false;
		connection.ch4_Status = false;
	}
	if (cmd == '55 AA AA AA AA 84 AC 01') {
		connection.isAC = true;
	}
	if (cmd == '55 AA AA AA AA 84 AC 00') {
		connection.isAC = false;
	}
	if (connection.ch1_Status && connection.ch2_Status && connection.ch3_Status && connection.ch4_Status) {
		connection.all_Status = true;
	}
	if (!connection.ch1_Status && !connection.ch2_Status && !connection.ch3_Status && !connection.ch4_Status) {
		connection.all_Status = false;
	}
	if (cmd == '55 AA AA AA AA 84 AC 01') {
		connection.isAC = true;
	}
	if (cmd == '55 AA AA AA AA 84 AC 00') {
		connection.isAC = false;
	}
};

// 自动同步滚动（建议延时执行）textarea:any=null
const syncBottom = () => {
	const textarea = document.getElementById('recv');
	if (textarea) {
		textarea.scrollTop = textarea.scrollHeight - 30;
	}
};

// subscribe topic 开始订阅
// https://github.com/mqttjs/MQTT.js#mqttclientsubscribetopictopic-arraytopic-object-options-callback
const doSubscribe = () => {
	btnLoadingType.value = 'subscribe';
	const { topic, qos } = subscription.value;
	console.log(connection.subTopics, '订阅主题');
	client.value.subscribe(connection.subTopics, { qos }, (error: Error, granted: mqtt.ISubscriptionGrant[]) => {
		btnLoadingType.value = '';
		if (error) {
			console.log('subscribe error:', error);
			return;
		}
		subscribedSuccess.value = true; //订阅成功
		// 连接成功，发布首个问询指令
		switchLight('55 AA AA AA AA 91 CF'); //发送首页问询指令
		console.log('订阅成功subscribe successfully:', granted);
	});
};

// unsubscribe topic 取消订阅
// https://github.com/mqttjs/MQTT.js#mqttclientunsubscribetopictopic-array-options-callback
const doUnSubscribe = () => {
	btnLoadingType.value = 'unsubscribe';
	const { topic, qos } = subscription.value;
	client.value.unsubscribe(connection.subTopics, { qos }, (error) => {
		btnLoadingType.value = '';
		subscribedSuccess.value = false;
		if (error) {
			console.log('unsubscribe error:', error);
			return;
		}
		console.log(`unsubscribed topic: ${topic}`);
	});
};

// publish message发布消息
// https://github.com/mqttjs/MQTT.js#mqttclientpublishtopic-message-options-callback
const doPublish = (b, t) => {
	//btnLoadingType.value = "publish";
	const { topic, qos, payload, retain } = publish.value;
	//console.log(t+b,"发布内容")
	let paybody = stringFormat(connection.pubPayload, [connection.mqttToken, b ?? publish.value.payload, '', connection.clientId]); //标准格式payload
	client.value.publish(t ?? connection.pubTopic, paybody, { qos }, (error) => {
		nextTick(() => {
			// 测试延时
			setTimeout(() => {
				btnLoadingType.value = '';
			}, 50);
		});
		if (error) {
			appmessage(now.toLocaleString() + '|发布消息错误.' + error);
			console.log('publish error:', error);
			return;
		}
	});
};

// 消息追加消息框
const appmessage = (msg) => {
	receivedMessages.value = receivedMessages.value.concat(
		// 拼接字符串输出
		msg + '\r\n'
	);
};

// 开关
const switchLight = (cmd) => {
	if (!client.value.connected) {
		appmessage('尚未连接到服务!');
		return;
	}
	let paybody = stringFormat(connection.pubPayload, [connection.mqttToken, cmd ?? publish.value.payload, '', connection.clientId]);
	const { topic, qos, payload, retain } = publish.value;
	//console.log(t+b,"发布内容")
	client.value.publish(connection.pubTopic, paybody, { qos }, retain, (error) => {
		btnLoadingType.value = '';
		if (error) {
			console.log('publish error:', error);
			return;
		}
	});
};

// disconnect 端口连接
// https://github.com/mqttjs/MQTT.js#mqttclientendforce-options-callback
const destroyConnection = () => {
	if (client.value.connected) {
		btnLoadingType.value = 'disconnect';
		try {
			client.value.end(false, () => {
				initData();
				connSuccess.value = false;
				//console.log("断开成功 disconnected successfully");
				appmessage(now.toLocaleString() + '|连接已断开.');
			});
		} catch (error) {
			btnLoadingType.value = '';
			console.log('断开错误 disconnect error:', error);
		}
	}
};

// 端口随协议而改变
const handleProtocolChange = (value: string) => {
	connection.port = value === 'wss' ? 8084 : 8083;
};

// 清空消息框
const clsmsg = () => {
	receivedMessages.value = '';
};
</script>

<style lang="scss" scoped>
.mqtt-box {
	max-width: 100%;
	margin: 0 auto;
}

.header {
	font-size: 24px;
	font-weight: bold;
	margin: -6px auto 0px auto;
}

h1 {
	font-size: 16px;
	margin-top: 10px auto 20px auto;
	padding: 6px 0px 6px 0;
}

.el-col {
	padding: 4px;
}

.el-input {
	font-size: 13px;
}
.el-card {
	margin-bottom: 6px;
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
	margin-top: 20px;
	width: 160px;
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
</style>
