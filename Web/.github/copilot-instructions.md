# 项目指南

## 代码风格
- **Vue 3 组合式 API**：使用 `<script setup>` 语法糖编写组件。参考 `src/App.vue` 的设置模式。
- **TypeScript**：启用严格模式。为 props/emits 使用接口。参考 `src/types/global.d.ts` 的全局类型。
- **SCSS**：使用 `src/theme/common/` 中的变量和混入。参考 `src/theme/index.scss` 的主题结构。
- **格式化**：运行 `pnpm run format`（Prettier）和 `pnpm run lint-fix`（ESLint with Vue/TypeScript rules）。

## 架构
- **模块化 API**：核心模块的手写 API 在 `src/api/` 中，后端同步的自动生成 API 在 `src/api-services/` 中。基础类 `useBaseApi` 提供可重用的 CRUD。
- **路由**：双模式（前端/后端控制）。前端使用角色；后端从 API 获取。参考 `src/router/index.ts`。
- **状态管理**：主题、用户、路由的 Pinia 存储。参考 `src/stores/themeConfig.ts` 的广泛 UI 配置。
- **组件边界**：布局可定制（经典/列/水平）。`src/components/` 中的可重用组件带有插槽/props。
- **I18n**：动态加载语言文件。合并 Element Plus 语言包。参考 `src/i18n/index.ts`。

## 构建和测试
- 安装：`pnpm install`
- 开发：`pnpm run dev`
- 构建：`pnpm run build`
- 代码检查：`pnpm run lint-fix`
- 格式化：`pnpm run format`
- API 生成：`pnpm run build-api`（需要 Java 和运行中的后端）

## 约定
- **文件路径**：使用 `/@/` 别名表示 `src/`（例如 `/@/stores/index`）。
- **API URL**：基础 `/api/{module}/`；GET 参数作为查询字符串。
- **组件命名**：Vue 文件使用 PascalCase；插槽按 prop 命名。
- **存储使用**：`useStore(pinia)` 模式；使用 `storeToRefs` 的响应式引用。
- **I18n 键**：嵌套对象；使用 `$t('key')`。
- **表格配置**：如 `hideExport`、`isSelection` 的 props；格式化函数。
- **环境配置**：Vite 加载 `.env` 文件；构建时写入 `public/config.js`。键如 `VITE_API_URL`、`VITE_OPEN_CDN`。

请参阅 `README.md` 获取设置和使用信息。有关架构详情，请参考 [vue-next-admin-doc](https://lyt-top.gitee.io/vue-next-admin-doc-preview)。