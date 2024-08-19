import { createApp } from 'vue';
import pinia from '/@/stores/index';
import App from '/@/App.vue';
import router from '/@/router';
import { directive } from '/@/directive/index';
import { i18n } from '/@/i18n/index';
import other from '/@/utils/other';
import ElementPlus from 'element-plus';
import '/@/theme/index.scss';
// 动画库
import 'animate.css';
// 栅格布局
import VueGridLayout from 'vue-grid-layout';
// 电子签名
import VueSignaturePad from 'vue-signature-pad';
// 组织架构图
import vue3TreeOrg from 'vue3-tree-org';
import 'vue3-tree-org/lib/vue3-tree-org.css';
// VForm3 表单设计
import VForm3 from 'vform3-builds';
import 'vform3-builds/dist/designer.style.css';
// 关闭自动打印
import { disAutoConnect } from 'vue-plugin-hiprint';
disAutoConnect();

const app = createApp(App);

directive(app);
other.elSvg(app);

app.use(pinia).use(router).use(ElementPlus).use(i18n).use(VueGridLayout).use(VForm3).use(VueSignaturePad).use(vue3TreeOrg).mount('#app');
