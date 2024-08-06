import { createApp } from 'vue';
import pinia from '/@/stores/index';
import App from '/@/App.vue';
import router from '/@/router';
import { directive } from '/@/directive/index';
import { i18n } from '/@/i18n/index';
import other from '/@/utils/other';

import ElementPlus from 'element-plus';
import '/@/theme/index.scss';
import VueGridLayout from 'vue-grid-layout';

import VForm3 from 'vform3-builds'; // VForm3表单设计
import 'vform3-builds/dist/designer.style.css'; // VForm3表单设计样式
import VueSignaturePad from 'vue-signature-pad'; // 电子签名
import vue3TreeOrg from 'vue3-tree-org'; // 组织架构图
import 'vue3-tree-org/lib/vue3-tree-org.css'; // 组织架构图样式
import 'animate.css'; // 动画库

import { disAutoConnect } from 'vue-plugin-hiprint';
disAutoConnect();

const app = createApp(App);

directive(app);
other.elSvg(app);
//#region  FastCrud配置
import { FastCrud } from "@fast-crud/fast-crud";
import "@fast-crud/fast-crud/dist/style.css";
import ui from "@fast-crud/ui-element";
import { FsExtendsUploader, FsExtendsEditor } from "@fast-crud/fast-extends";
import "@fast-crud/fast-extends/dist/style.css";
import { getToken } from '/@/utils/axios-utils';
app.use(ui);
app.use(FastCrud, {
    i18n,
    commonOptions() {
        return {
            request: {
                transformQuery: ({ page, form, sort }) => {
                    const order = sort == null ? {} : { orderProp: sort.prop, orderAsc: sort.asc }
                    return { page: page.currentPage, pageSize: page.pageSize, ...form, ...order };
                },
                // page请求结果转换
                transformRes: ({ res }) => {
                    const records = res.data.result.items;
                    const total = res.data.result.total;
                    const currentPage = res.data.result.page;
                    const pageSize = res.data.result.pageSize;
                    return {
                        currentPage: currentPage, pageSize: pageSize, total: total, records
                    }
                },
                form: {
                    display: 'flex', // 表单布局
                    labelWidth: '120px' // 表单label宽度
                }
            },
        };
    },
});
const baseURL = import.meta.env.VITE_API_URL;
import request from '/@/utils/request';
//文件上传
app.use(FsExtendsUploader, {
    defaultType: 'form',
    form: {
        action: baseURL + '/api/sysFile/uploadFile',
        name: 'file',
        withCredentials: false,
        uploadRequest: async props => {
            const { action, file, onProgress } = props;
            const data = new FormData();
            data.append('file', file);
            const token = getToken();
            const Authorization = token ? `Bearer ${token}` : null;
            const result = await request({
                url: action,
                method: 'post',
                data,
                headers: {
                    'Content-Type': 'multipart/form-data',
                    "Authorization": Authorization
                },
                timeout: 60000,
                onUploadProgress(progress) {
                    onProgress({ percent: Math.round((progress.loaded / progress.total!) * 100) });
                }
            });
            if (result) {
                return result.data
            } else {
                throw new Error(result.message);
            }
        },
        async successHandle(ret: any) {
            return {
                url: baseURL + "/" + ret.result.filePath + "/" + ret.result.id + ret.result.suffix,
                key: ret.result.fileName
            };
        }
    }
});
//富文本编辑器
app.use(FsExtendsEditor, {
    wangEditor: {}
});
//#endregion
app.use(pinia).use(router).use(ElementPlus).use(i18n).use(VueGridLayout).use(VForm3).use(VueSignaturePad).use(vue3TreeOrg).mount('#app');
