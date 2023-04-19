/* tslint:disable */
/* eslint-disable */
/**
 * Admin.NET
 * 让 .NET 开发更简单、更通用、更流行。前后端分离架构(.NET6/Vue3)，开箱即用紧随前沿技术。<br/><a href='https://gitee.com/zuohuaijun/Admin.NET/'>https://gitee.com/zuohuaijun/Admin.NET</a>
 *
 * OpenAPI spec version: 1.0.0
 * Contact: 515096995@qq.com
 *
 * NOTE: This class is auto generated by the swagger code generator program.
 * https://github.com/swagger-api/swagger-codegen.git
 * Do not edit the class manually.
 */
import { SysNotice } from './sys-notice';
/**
 * 分页泛型集合
 * @export
 * @interface SqlSugarPagedListSysNotice
 */
export interface SqlSugarPagedListSysNotice {
    /**
     * 页码
     * @type {number}
     * @memberof SqlSugarPagedListSysNotice
     */
    page?: number;
    /**
     * 页容量
     * @type {number}
     * @memberof SqlSugarPagedListSysNotice
     */
    pageSize?: number;
    /**
     * 总条数
     * @type {number}
     * @memberof SqlSugarPagedListSysNotice
     */
    total?: number;
    /**
     * 总页数
     * @type {number}
     * @memberof SqlSugarPagedListSysNotice
     */
    totalPages?: number;
    /**
     * 当前页集合
     * @type {Array<SysNotice>}
     * @memberof SqlSugarPagedListSysNotice
     */
    items?: Array<SysNotice> | null;
    /**
     * 是否有上一页
     * @type {boolean}
     * @memberof SqlSugarPagedListSysNotice
     */
    hasPrevPage?: boolean;
    /**
     * 是否有下一页
     * @type {boolean}
     * @memberof SqlSugarPagedListSysNotice
     */
    hasNextPage?: boolean;
}
