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
import vitePluginsAutoI18n, { EmptyTranslator, YoudaoTranslator } from 'vite-auto-i18n-plugin';
const pathResolve = (dir: string) => {
	return resolve(__dirname, '.', dir);
};

const alias: Record<string, string> = {
	'/@': pathResolve('./src/'),
	'vue-i18n': 'vue-i18n/dist/vue-i18n.cjs.js',
	'ezuikit-js': pathResolve('node_modules/ezuikit-js/ezuikit.js'),
};

const viteConfig = defineConfig((mode: ConfigEnv) => {
	const env = loadEnv(mode.mode, process.cwd());
	fs.writeFileSync('./public/config.js', `window.__env__ = ${JSON.stringify(env, null, 2)} `);
	return {
		plugins: [
			visualizer({ open: false }), // 开启可视化分析页面
			CodeInspectorPlugin({
				bundler: 'vite',
				hotKeys: ['shiftKey'],
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
				// algorithm: 'gzip', // 压缩算法，可选[‘gzip’，‘brotliCompress’，‘deflate’，‘deflateRaw’]
				// exclude: [/\.(br)$/, /\.(gz)$/], // 排除指定文件
			}),
			JSON.parse(env.VITE_OPEN_CDN) ? buildConfig.cdn() : null,
			// 使用说明 https://github.com/auto-i18n/auto-i18n-translation-plugins
			vitePluginsAutoI18n({
				// 是否触发翻译
				enabled: false,
				originLang: 'zh-cn', //源语言，翻译以此语言为基础
				targetLangList: ['zh-hk', 'zh-tw', 'en', 'it'], // 目标语言列表，支持配置多个语言
				translator: new EmptyTranslator(), // 只生成Web\lang\index.json文件
				// translator: new YoudaoTranslator({ // 有道实时翻译
				// appId: '你申请的appId',
				// appKey: '你申请的appKey'
				// })
			}),
		],
		root: process.cwd(),
		resolve: { alias },
		base: mode.command === 'serve' ? './' : env.VITE_PUBLIC_PATH,
		optimizeDeps: { exclude: ['vue-demi'] },
		server: {
			host: '0.0.0.0',
			port: env.VITE_PORT as unknown as number,
			open: JSON.parse(env.VITE_OPEN),
			hmr: true,
			proxy: {
				'^/[Uu]pload': {
					target: env.VITE_API_URL,
					changeOrigin: true,
				},
			},
		},
		build: {
			outDir: 'dist',
			chunkSizeWarningLimit: 1500,
			assetsInlineLimit: 5000, // 小于此阈值的导入或引用资源将内联为 base64 编码
			sourcemap: false, // 构建后是否生成 source map 文件
			extractComments: false, // 移除注释
			minify: 'terser', // 启用后 terserOptions 配置才有效
			terserOptions: {
				compress: {
					drop_console: true, // 生产环境时移除console
					drop_debugger: true,
				},
			},
			rollupOptions: {
				output: {
					chunkFileNames: 'assets/js/[name]-[hash].js', // 引入文件名的名称
					entryFileNames: 'assets/js/[name]-[hash].js', // 包的入口文件名称
					assetFileNames: 'assets/[ext]/[name]-[hash].[ext]', // 资源文件像 字体，图片等
					manualChunks(id) {
						const normalizedId = id.toString().replace(/\\/g, '/');
						if (!normalizedId.includes('node_modules')) return;

						const includesPackage = (packageName: string) => normalizedId.includes(`/node_modules/${packageName}/`);

						// Vue 运行时及 Vue 组件库放在一起，避免 runtime helper 被拆到不同 chunk 后丢引用
						if (
							[
								'vue',
								'@vue',
								'@vueuse',
								'vue-demi',
								'vue-router',
								'pinia',
								'element-plus',
								'@element-plus',
								'vue-i18n',
								'@vue-office',
								'vue-json-pretty',
								'vue-plugin-hiprint',
								'vue-signature-pad',
								'vue3-tree-org',
								'vue-clipboard3',
								'vue-draggable-plus',
								'vue-grid-layout',
								'splitpanes',
								'vcrontab-3',
								'md-editor-v3',
								'json-editor-vue',
								'vform3-builds',
								'@wangeditor/editor-for-vue',
							].some(includesPackage)
						) {
							return 'vue-vendor';
						}

						if (['echarts', 'echarts-gl', 'echarts-wordcloud'].some(includesPackage)) {
							return 'charts';
						}

						if (includesPackage('monaco-editor')) {
							return 'monaco';
						}

						if (['@wangeditor/editor', 'cropperjs'].some(includesPackage)) {
							return 'editor';
						}

						if (['xlsx-js-style', 'js-table2excel', 'print-js', 'qrcodejs2-fixes'].some(includesPackage)) {
							return 'office';
						}

						if (['@logicflow', 'jsplumb', 'relation-graph'].some(includesPackage)) {
							return 'flow';
						}

						if (['mqtt', '@microsoft/signalr', 'axios'].some(includesPackage)) {
							return 'comm';
						}

						return 'vendor';
					},
				},
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
