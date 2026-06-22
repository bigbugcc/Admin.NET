<template>
    <el-dialog v-bind="$attrs" body-class="dialogpro-body" 
        :modal="state.minimize ? false : $attrs.modal" 
        :modal-penetrable="state.minimize ? true : $attrs.modalPenetrable"
        :class="state.minimize ? 'dialogpro-minimize' : ''"
    >
        <template #header v-if="$slots.prefix || prefixIcon">
            <div class="dialogpro-header">
                <div v-if="$slots.prefix || prefixIcon" class="dialogpro-header-prefix">
                    <slot v-if="$slots.prefix" name="prefix" />
                    <SvgIcon v-else-if="prefixIcon" :name="prefixIcon" />
                </div>
                <div class="dialogpro-header-title">
                    <span>{{ title }}</span>
                </div>
            </div>
            <button type="button" :class="['el-dialog__headerbtn']" style="right: 40px;" @click="() => state.minimize = !state.minimize">
                <SvgIcon :name="state.minimize ? 'ele-CopyDocument' : 'ele-Minus'" class="el-dialog__close" color="#fff" />
            </button>
        </template>
        
        <template v-for="(value, name) in $slots" #[name]="scopedData">
            <slot :name="name" v-bind="scopedData"></slot>
        </template>
    </el-dialog>
</template>

<script setup lang="ts" name="DialogPro">
import { reactive } from "vue";
defineProps({
    title: { type: String },
    prefixIcon: { type: String },
    height: { type: [String, Number, null], default: null }
});

const state = reactive({
    minimize: false
});
</script>

<style lang="scss">
.dialogpro-header {
    display: flex;
    align-items: center;

    &-prefix {
        height: 100%;
        line-height: 100%;
        margin-right: 5px;
    }
    &-title { 
        flex: 1; 
        overflow: hidden;
        white-space: nowrap;
        text-overflow: ellipsis;
    }
}
.dialogpro-body {
    height: calc(v-bind(height) - 37px);
}
.dialogpro-body:has(+ .el-dialog__footer) {
    height: calc(v-bind(height) - 37px - 47px - 18px);
}

.dialogpro-minimize {
    width: 300px;
    margin: 0;
    padding: 0;
    position: fixed;
    bottom: 0;
    transition-duration: 1s; /* 过渡时间 */
    transition-property: left, bottom; /* 过渡属性 */
    transition-timing-function: ease, cubic-bezier(0.25, -0.55, 0.83, 0.13);

    .el-dialog__header { border-radius: var(--el-dialog-border-radius); }
    .el-dialog__body { display: none; }
    .el-dialog__footer { display: none; }
}
</style>
