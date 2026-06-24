import vue from '@vitejs/plugin-vue';
import { resolve } from 'path';
import { defineConfig, loadEnv, ConfigEnv } from 'vite';
import vueSetupExtend from 'vite-plugin-vue-setup-extend';
import compression from 'vite-plugin-compression2';
import { buildConfig } from './src/utils/build';
import vueJsx from '@vitejs/plugin-vue-jsx';
import { CodeInspectorPlugin } from 'code-inspector-plugin';
import fs from 'fs';
import { visualizer } from 'rollup-plugin-visualizer';
import { webUpdateNotice } from '@plugin-web-update-notification/vite';
import vitePluginsAutoI18n, { EmptyTranslator } from 'vite-auto-i18n-plugin';

const pathResolve = (dir: string) => {
	return resolve(__dirname, '.', dir);
};

const alias: Record<string, string> = {
	'/@': pathResolve('./src/'),
	'vue-i18n': 'vue-i18n/dist/vue-i18n.cjs.js',
};

const viteConfig = defineConfig((mode: ConfigEnv) => {
	const env = loadEnv(mode.mode, process.cwd());
	env.VITE_BUILD_TIMESTAMP = new Date()
		.toLocaleString('zh-CN', {
			hour12: false,
			timeZone: 'Asia/Shanghai',
			year: 'numeric',
			month: '2-digit',
			day: '2-digit',
			hour: '2-digit',
			minute: '2-digit',
			second: '2-digit',
		})
		.replace(/\//g, '-');
	fs.writeFileSync('./public/config.js', `window.__env__ = ${JSON.stringify(env, null, 2)} `);
	return {
		plugins: [
			visualizer({ open: false }), // 开启可视化分析页面
			CodeInspectorPlugin({
				bundler: 'vite',
				// hotKeys: ['shiftKey'],
			}),
			vue(),
			vueJsx(),
			webUpdateNotice({
				versionType: 'build_timestamp',
				notificationConfig: {
					placement: 'topLeft',
				},
				notificationProps: {
					title: '📢 系统更新',
					description: '系统更新啦，请刷新页面！',
					buttonText: '刷新',
					dismissButtonText: '忽略',
				},
			}),
			vueSetupExtend(),
			compression({
				deleteOriginalAssets: false, // 是否删除源文件
				threshold: 5120, // 对大于 5KB 文件进行 gzip 压缩，单位Bytes
				skipIfLargerOrEqual: true, // 如果压缩后的文件大小等于或大于原始文件，则跳过压缩
				algorithms: ['gzip', 'brotliCompress'], // 压缩算法，可选[‘gzip’，‘brotliCompress’，‘deflate’，‘deflateRaw’]
				// exclude: [/\.(br)$/, /\.(gz)$/], // 排除指定文件
			}),
			JSON.parse(env.VITE_OPEN_CDN) ? buildConfig.cdn() : null,
			vitePluginsAutoI18n({
				// 是否触发翻译
				enabled: false,
				// 源语言
				originLang: 'zh-cn',
				// 目标语言列表
				targetLangList: ['en'], // 'zh-hk', 'zh-tw',
				// 翻译文件配置生成路径
				globalPath: './lang',
				// 指定只翻译某些目录路径（白名单），默认为src
				includePath: [/src\/views\//],
				// 翻译调用函数名称，例如$t 表示翻译调用时的函数名
				translateKey: '$tr',
				// 是否清除已经不在上下文中的内容（清除项目中不再使用到的源语言键值对）
				isClear: true,
				// 翻译器
				translator: new EmptyTranslator(),
				// 火山引擎AI翻译器 https://www.volcengine.com/docs/82379/1330310
				// translator: new VolcengineTranslator({
				// 	apiKey: '',
				// 	model: 'deepseek-r1-250528',
				// }),
				// translator: new YoudaoTranslator({
				// 	appId: '',
				// 	appKey: '',
				// }),
				// translator: new GoogleTranslator({
				// 	proxyOption: {
				// 		host: '127.0.0.1',
				// 		port: 7890,
				// 		headers: {
				// 			'User-Agent': 'Node',
				// 		},
				// 	},
				// }),
			}),
		],
		root: process.cwd(),
		resolve: { alias },
		base: mode.command === 'serve' ? './' : env.VITE_PUBLIC_PATH,
		optimizeDeps: {
			include: ['vue', 'vue-router', 'pinia', 'element-plus', '@vueuse/core'],
			exclude: ['vue-demi', 'vue-json-pretty'],
		},
		server: {
			host: '0.0.0.0',
			port: env.VITE_PORT as unknown as number,
			open: JSON.parse(env.VITE_OPEN),
			hmr: true,
			allowedHosts: true,
			proxy: {
				'^/api': {
					target: env.VITE_API_URL,
					changeOrigin: true,
				},
				'^/[Uu]pload': {
					target: env.VITE_API_URL,
					changeOrigin: true,
				},
				'^/[Ss]se': {
					target: env.VITE_API_URL,
					changeOrigin: true,
				},
			},
		},
		build: {
			outDir: 'dist', // 打包输出目录
			chunkSizeWarningLimit: 2000, // chunk 大小超过 1MB 时发出警告（单位 KB）
			assetsInlineLimit: 4096, // 小于 4KB 的静态资源内联为 base64，减少请求数
			sourcemap: false, // 是否生成 sourcemap 文件
			extractComments: false, // 移除所有注释
			minify: 'terser', // 使用 terser 进行代码压缩（比 esbuild 压缩率更高）
			terserOptions: {
				compress: {
					drop_console: true, // 移除所有 console.log、console.info 等调试语句
					drop_debugger: true, // 移除所有 debugger 断点语句
				},
				format: {
					comments: false, // 移除代码中的所有注释（包括版权注释、行内注释等）
				},
			},
			modulePreload: {
				polyfill: false,
			},
			reportCompressedSize: true, // 显示 gzip 压缩后的大小
			rollupOptions: {
				output: {
					chunkFileNames: 'assets/js/[name]-[hash].js', // 引入文件名的名称
					entryFileNames: 'assets/js/[name]-[hash].js', // 包的入口文件名称
					assetFileNames: 'assets/[ext]/[name]-[hash].[ext]', // 资源文件像 字体，图片等
					manualChunks(id) {
						if (!id.includes('node_modules')) return;

						const normalizedId = id.replace(/\\/g, '/');
						if (normalizedId.includes('monaco-editor') || normalizedId.includes('vue-element-plus-x') || normalizedId.includes('@flyfish-group/file-viewer3')) return;

						if (
							[
								'/node_modules/vue/',
								'/node_modules/@vue/',
								'/node_modules/@vueuse/',
								'/node_modules/vue-demi/',
								'/node_modules/vue-json-pretty/',
								'/node_modules/element-plus/',
								'/node_modules/@element-plus/',
							].some((packagePath) => normalizedId.includes(packagePath))
						)
							return 'vue-vendor';

						return normalizedId.match(/\/node_modules\/(?:\.pnpm\/[^/]+\/node_modules\/)?((?:@[^/]+\/)?[^/]+)/)?.[1].replace('/', '-') ?? 'vendor';
					},
				},
				// codeSplitting: false, // 禁用代码分割
				...(JSON.parse(env.VITE_OPEN_CDN) ? { external: buildConfig.external } : {}),
			},
		},
		css: { preprocessorOptions: { css: { charset: false }, scss: { silenceDeprecations: ['legacy-js-api', 'global-builtin', 'fs-importer-cwd', 'import'] } } },
		define: {
			__VUE_I18N_LEGACY_API__: JSON.stringify(false),
			__VUE_I18N_FULL_INSTALL__: JSON.stringify(false),
			__INTLIFY_PROD_DEVTOOLS__: JSON.stringify(false),
			__NEXT_VERSION__: JSON.stringify(process.env.npm_package_version),
			__NEXT_NAME__: JSON.stringify(process.env.npm_package_name),
		},
	};
});

export default viteConfig;
