import LogicFlow from "@logicflow/core";
// 引入自定义的边
import edgeSql from './Edges/EdgeSql';
// 注册边
const Register = (lf: LogicFlow) => {
    lf.register(edgeSql);
};

export default { Register };