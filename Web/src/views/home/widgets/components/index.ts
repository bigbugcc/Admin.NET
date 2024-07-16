import { markRaw } from 'vue';
// 定义组件类型
type Component = any;

const resultComps: Record<string, Component> = {};

// 要排除的组件名称数组
const excludeComponents = ['scheduleEdit'];

// 使用 import.meta.glob 动态导入当前目录中的所有 .vue 文件，急切导入
const requireComponent = import.meta.glob('./*.vue', { eager: true });
// console.log(requireComponent);

Object.keys(requireComponent).forEach((fileName: string) => {
	// 处理文件名，去掉开头的 './' 和结尾的文件扩展名
	const componentName = fileName.replace(/^\.\/(.*)\.\w+$/, '$1');

	// 如果组件名称在排除数组中，跳过导入
	if (excludeComponents.includes(componentName)) {
		return;
	}

	// 确保模块导出存在并且是默认导出
	const componentModule = requireComponent[fileName] as { default: Component };

	// 将组件添加到 resultComps 中，使用处理后的文件名作为键
	resultComps[componentName] = componentModule.default;
});

// 标记 resultComps 为原始对象，避免其被设为响应式
export default markRaw(resultComps);
