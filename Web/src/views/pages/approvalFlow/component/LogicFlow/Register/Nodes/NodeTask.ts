import { RectNode, RectNodeModel, h } from "@logicflow/core";

class TaskNode extends RectNode {
    getShare() {
        const { model } = this.props;
        const { width, height, x, y } = model;
        const position = {
            x: x - width / 2,
            y: y - height / 2,
        }
        const style = model.getNodeStyle();
        return h('rect', { ...style, ...position });
    }
}
class TaskNodeModel extends RectNodeModel {
    constructor(data, graphModel) {
        super(data, graphModel);
        this.radius = 20;
    }
}
export default {
    type: 'task-node',
    view: TaskNode,
    model: TaskNodeModel,
}