/* tslint:disable */
/* eslint-disable */
/**
 * Admin.NET 通用权限开发平台
 * 让 .NET 开发更简单、更通用、更流行。整合最新技术，模块插件式开发，前后端分离，开箱即用。<br/><u><b><font color='FF0000'> 👮不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！</font></b></u>
 *
 * OpenAPI spec version: 1.0.0
 * 
 *
 * NOTE: This class is auto generated by the swagger code generator program.
 * https://github.com/swagger-api/swagger-codegen.git
 * Do not edit the class manually.
 */

import { Filter } from './filter';
import { Search } from './search';
 /**
 * 代码生成参数类
 *
 * @export
 * @interface CodeGenInput
 */
export interface CodeGenInput {

    /**
     * @type {Search}
     * @memberof CodeGenInput
     */
    search?: Search;

    /**
     * 模糊查询关键字
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    keyword?: string | null;

    /**
     * @type {Filter}
     * @memberof CodeGenInput
     */
    filter?: Filter;

    /**
     * 当前页码
     *
     * @type {number}
     * @memberof CodeGenInput
     */
    page?: number;

    /**
     * 页码容量
     *
     * @type {number}
     * @memberof CodeGenInput
     */
    pageSize?: number;

    /**
     * 排序字段
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    field?: string | null;

    /**
     * 排序方向
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    order?: string | null;

    /**
     * 降序排序
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    descStr?: string | null;

    /**
     * 数据库表名
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    tableName?: string | null;

    /**
     * 业务名（业务代码包名称）
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    busName?: string | null;

    /**
     * 命名空间
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    nameSpace?: string | null;

    /**
     * 作者姓名
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    authorName?: string | null;

    /**
     * 生成方式
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    generateType?: string | null;

    /**
     * 是否生成菜单
     *
     * @type {boolean}
     * @memberof CodeGenInput
     */
    generateMenu?: boolean;

    /**
     * 类名
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    className?: string | null;

    /**
     * 是否移除表前缀
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    tablePrefix?: string | null;

    /**
     * 库定位器名
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    configId?: string | null;

    /**
     * 数据库名(保留字段)
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    dbName?: string | null;

    /**
     * 数据库类型
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    dbType?: string | null;

    /**
     * 数据库链接
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    connectionString?: string | null;

    /**
     * 功能名（数据库表名称）
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    tableComment?: string | null;

    /**
     * 菜单应用分类（应用编码）
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    menuApplication?: string | null;

    /**
     * 菜单父级
     *
     * @type {number}
     * @memberof CodeGenInput
     */
    menuPid?: number | null;

    /**
     * 菜单图标
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    menuIcon?: string | null;

    /**
     * 页面目录
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    pagePath?: string | null;

    /**
     * 支持打印类型
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    printType?: string | null;

    /**
     * 打印模版名称
     *
     * @type {string}
     * @memberof CodeGenInput
     */
    printName?: string | null;
}
