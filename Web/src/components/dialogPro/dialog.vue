<template>
    <el-dialog v-bind="$attrs" body-class="dialogpro-body" 
        :modal="state.minimize ? false : $attrs.modal" 
        :modal-penetrable="state.minimize ? true : $attrs.modalPenetrable"
        :class="state.minimize ? 'dialogpro dialogpro-minimize' : 'dialogpro'"
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
import { reactive, useAttrs } from "vue";
defineProps({
    title: { type: String },
    prefixIcon: { type: String },
    height: { type: [String, Number, null], default: null }
});

const attrs = useAttrs();
const state = reactive({
    minimize: false,
    width: attrs.width || '600px'
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

// .el-overlay .el-overlay-dialog {
//     display: flex;
//     align-items:start;
//     justify-content:start;
//     position: unset !important;
//     width: 100%;
//     height: 100%;
// }

.dialogpro {
    width: v-bind('state.width');
    margin: 0;
    padding: 0;
    //position: fixed;
    //bottom: 200px;
    //left: calc(50% - (v-bind('state.width') / 2));

    transition-duration: 0.25s; /* 过渡时间 */
    transition-property: left, top, bottom, width; /* 过渡属性 */
    //transition-timing-function: ease, cubic-bezier(0.25, -0.55, 0.83, 0.13);

    .el-dialog__body { 
        transition-duration: 1s; /* 过渡时间 */
        transition-property: height; /* 过渡属性 */
    }


    //transform: translate(-205.188px, -111px);
}
.dialogpro-minimize {
    left: 10px;
    top: calc(100vh - 37px);
    
    .el-dialog__header { 
        border-radius: var(--el-dialog-border-radius); 
        //padding: 4px;
        //height: unset;
    }
    .el-dialog__body { 
        height: 0;
    }
    .el-dialog__footer { 
        display: none;
    }
}
</style>
