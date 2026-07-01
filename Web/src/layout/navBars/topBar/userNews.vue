<template>
	<div class="user-news-container">
		<el-tabs stretch class="content-box">
			<el-tab-pane label="站内信">
				<template #label>
					<el-icon><ele-Bell /></el-icon>
					<span style="margin-left: 5px">站内信</span>
				</template>
				<div class="notice-box">
					<template v-if="noticeList.length > 0">
						<div class="notice-item" v-for="(v, k) in noticeList" :key="k" @click="viewNoticeDetail(v)" v-show="v.readStatus == 1 ? false : true">
							<div class="notice-item-icon">
                                <el-icon size="24" color="var(--el-color-primary)">
                                    <ele-Notification v-if="v.type == 1" />
                                    <ele-Message v-else />
                                </el-icon>
                            </div>
                            <div>
                                <div class="notice-title">{{ v.title }}</div>
                                <div class="notice-content">{{ removeHtmlSub(v.content) }}</div>
                                <div class="notice-time">{{ v.publicTime }}</div>
                            </div>
                            
						</div>
					</template>
					<el-empty description="没有新消息" v-else style="height: 85%;"></el-empty>
				</div>
				<div class="notice-foot" @click="goToNotice" v-if="noticeList.length > 0">前往通知中心</div>
			</el-tab-pane>
			<el-tab-pane label="我的">
				<template #label>
					<el-icon><ele-Position /></el-icon>
					<span style="margin-left: 5px">我的</span>
				</template>
				<div class="notice-box" style="height: 435px;">
					<el-empty description="没有新消息" style="height: 85%;"></el-empty>
				</div>
			</el-tab-pane>
		</el-tabs>
		<el-dialog v-model="state.dialogVisible" draggable width="769px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Bell /> </el-icon>
					<span> 消息详情 </span>
				</div>
			</template>
			<div class="w-e-text-container">
				<div v-html="state.content" data-slate-editor></div>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button type="primary" @click="state.dialogVisible = false">确认</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script setup lang="ts" name="layoutBreadcrumbUserNews">
import '@wangeditor-next/editor/dist/css/style.css';
import { reactive } from 'vue';
import router from '/@/router';
import commonFunction from '/@/utils/commonFunction';

import { getAPI } from '/@/utils/axios-utils';
import { SysNoticeApi } from '/@/api-services/api';

defineProps({
	noticeList: Array as any,
});
const { removeHtmlSub } = commonFunction();
const state = reactive({
	dialogVisible: false,
	content: '',
});
// 前往通知中心点击
const goToNotice = () => {
	router.push('/dashboard/notice');
};
// 查看消息详情
const viewNoticeDetail = async (notice: any) => {
	state.content = notice.content;
	state.dialogVisible = true;

	// 设置已读
	notice.readStatus = 1;
	await getAPI(SysNoticeApi).apiSysNoticeSetReadPost({ id: notice.id });
};
</script>

<style scoped lang="scss">
.user-news-container {
	.content-box {
		font-size: 12px;
		.notice-box {
			height: 400px;
			padding: 0 5px;
            overflow-y: auto;
		}
		.notice-item {
            display: flex;
            
            padding: 10px 0;
            border-bottom: 1px dashed var(--el-border-color);

            &-icon {
                margin: 0 15px;
                display: inline-flex;
                justify-content: center;
                align-items: center;
            }

			&:hover {
				background-color: var(--el-color-primary-light-9);
                cursor: pointer;
			}
			.notice-title {
                font-size: 14px;
                font-weight: 600;
				//color: var(--el-color-primary);
			}
			.notice-content {
				color: var(--el-text-color-secondary);
				margin: 10px 0;
			}
			.notice-time {
				color: var(--el-text-color-secondary);
				text-align: right;
			}
		}
	}
	.notice-foot {
		height: 35px;
		width: 100%;
		color: var(--el-color-primary);
		font-size: 14px;
		cursor: pointer;
		background-color: #fff;
		display: flex;
		align-items: center;
		justify-content: center;

        border-top: 1px solid #eee;
        padding-top: 9px;
	}
}
.el-tabs {
    :deep(.el-tabs__header) {
        margin: 0;
    }
    .el-tab-pane {
        display: flex;
        flex-direction: column;
    }
}
</style>
