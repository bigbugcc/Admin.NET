import { CircleNode, CircleNodeModel, h } from "@logicflow/core";

class EndNode extends CircleNode {
    getIconShape() {
        const { model } = this.props;
        const { x, y, width, height } = model;
        const stroke = '#404040';
        return h(
            'svg',
            {
                x: x - width / 2,
                y: y - height / 2,
                width: 40,
                height: 40,
                viewBox: '0 0 1024 1024'
            },
            h(
                'path',
                {
                    fill: stroke,
                    d: 'M212.992 526.336 212.992 526.336 212.992 526.336 215.04 526.336 212.992 526.336Z'
                }
            ),
            h(
                'path',
                {
                    fill: stroke,
                    d: 'M724.992 296.96 724.992 296.96 296.96 296.96 296.96 724.992 724.992 724.992 724.992 296.96Z'
                }
            )
        );
    }
    getShape() {
        const { model } = this.props
        const { x, y, r } = model
        const { fill, stroke, strokeWidth } = model.getNodeStyle()
        return h('g', {}, [h('circle', { cx: x, cy: y, r, fill, stroke, strokeWidth }), this.getIconShape()]);
    }
}

class EndNodeModel extends CircleNodeModel {
    initNodeData(data) {
        data.text = {
            value: (data.text && data.text.value) || '',
            x: data.x,
            y: data.y + 35
        }
        super.initNodeData(data)
        this.r = 20
    }
    // 自定义锚点样式
    getAnchorStyle() {
        const style = super.getAnchorStyle()
        style.hover.r = 8
        style.hover.fill = 'rgb(24, 125, 255)'
        style.hover.stroke = 'rgb(24, 125, 255)'
        return style
    }
    // 自定义节点outline
    getOutlineStyle() {
        const style = super.getOutlineStyle()
        style.stroke = '#88f'
        return style
    }
    getConnectedSourceRules() {
        const rules = super.getConnectedSourceRules()
        const notAsTarget = {
            message: '终止节点不能作为连线的起点',
            validate: () => false
        }
        rules.push(notAsTarget)
        return rules
    }
}
export default {
    type: 'end-node',
    view: EndNode,
    model: EndNodeModel
}
