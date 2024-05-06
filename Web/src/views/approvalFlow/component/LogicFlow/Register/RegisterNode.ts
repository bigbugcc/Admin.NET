import LogicFlow from "@logicflow/core";
// 引入自定义节点
import nodeStart from './Nodes/NodeStart';
import nodeEnd from './Nodes/NodeEnd';
import nodeTask from './Nodes/NodeTask';
import nodeUser from './Nodes/NodeUser';
import nodeSql from './Nodes/NodeSql';
// 注册节点
const Register = (lf: LogicFlow) => {
    lf.register(nodeStart);
    lf.register(nodeEnd);
    lf.register(nodeTask);
    lf.register(nodeUser);
    lf.register(nodeSql);
};

export default { Register };