module.exports = {
	mode: 'jit',
	purge: ['./src/**/*.{vue,js,ts,jsx,tsx}', './index.html'],
	corePlugins: {
		preflight: false, // 防止和已有UI框架样式冲突
	},
	darkMode: false, // or 'media' or 'class'
	theme: {
		extend: {},
	},
	variants: {
		extend: {},
	},
	plugins: [],
};
