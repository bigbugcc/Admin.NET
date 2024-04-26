<template>
	<el-popover placement="bottom" width="280" trigger="hover">
		<template #reference>
			<el-text type="primary" class="cursor-default">
				<el-icon><ele-InfoFilled /></el-icon>详情
			</el-text>
		</template>
		<el-descriptions direction="vertical" :column="2" border>
			<el-descriptions-item width="140">
				<template #label>
					<el-text>
						<el-icon><ele-UserFilled /></el-icon>创建者
					</el-text>
				</template>
				<el-tag>{{ props.data.createUserName ?? '无' }}</el-tag>
			</el-descriptions-item>
			<el-descriptions-item>
				<template #label>
					<el-text>
						<el-icon><ele-Calendar /></el-icon>创建时间
					</el-text>
				</template>
				<el-tag>{{ props.data.createTime ?? '无' }}</el-tag>
			</el-descriptions-item>
			<el-descriptions-item>
				<template #label>
					<el-text>
						<el-icon><ele-UserFilled /></el-icon>修改者
					</el-text>
				</template>
				<el-tag>{{ props.data.updateUserName ?? '无' }}</el-tag>
			</el-descriptions-item>
			<el-descriptions-item>
				<template #label>
					<el-text>
						<el-icon><ele-Calendar /></el-icon>修改时间
					</el-text>
				</template>
				<el-tag>{{ props.data.updateTime ?? '无' }}</el-tag>
			</el-descriptions-item>
			<el-descriptions-item>
				<template #label>
					<el-text>
						<el-icon><ele-Tickets /></el-icon>备注
					</el-text>
				</template>
				{{ props.data.remark ?? '无' }}
			</el-descriptions-item>
		</el-descriptions>
	</el-popover>
</template>

<script lang="ts" setup>
const props = defineProps<{
	data: ModifyRecord;
}>();
</script>
