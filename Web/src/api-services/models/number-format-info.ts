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

import { DigitShapes } from './digit-shapes';
 /**
 * 
 *
 * @export
 * @interface NumberFormatInfo
 */
export interface NumberFormatInfo {

    /**
     * @type {number}
     * @memberof NumberFormatInfo
     */
    currencyDecimalDigits?: number;

    /**
     * @type {string}
     * @memberof NumberFormatInfo
     */
    currencyDecimalSeparator?: string | null;

    /**
     * @type {boolean}
     * @memberof NumberFormatInfo
     */
    isReadOnly?: boolean;

    /**
     * @type {Array<number>}
     * @memberof NumberFormatInfo
     */
    currencyGroupSizes?: Array<number> | null;

    /**
     * @type {Array<number>}
     * @memberof NumberFormatInfo
     */
    numberGroupSizes?: Array<number> | null;

    /**
     * @type {Array<number>}
     * @memberof NumberFormatInfo
     */
    percentGroupSizes?: Array<number> | null;

    /**
     * @type {string}
     * @memberof NumberFormatInfo
     */
    currencyGroupSeparator?: string | null;

    /**
     * @type {string}
     * @memberof NumberFormatInfo
     */
    currencySymbol?: string | null;

    /**
     * @type {string}
     * @memberof NumberFormatInfo
     */
    naNSymbol?: string | null;

    /**
     * @type {number}
     * @memberof NumberFormatInfo
     */
    currencyNegativePattern?: number;

    /**
     * @type {number}
     * @memberof NumberFormatInfo
     */
    numberNegativePattern?: number;

    /**
     * @type {number}
     * @memberof NumberFormatInfo
     */
    percentPositivePattern?: number;

    /**
     * @type {number}
     * @memberof NumberFormatInfo
     */
    percentNegativePattern?: number;

    /**
     * @type {string}
     * @memberof NumberFormatInfo
     */
    negativeInfinitySymbol?: string | null;

    /**
     * @type {string}
     * @memberof NumberFormatInfo
     */
    negativeSign?: string | null;

    /**
     * @type {number}
     * @memberof NumberFormatInfo
     */
    numberDecimalDigits?: number;

    /**
     * @type {string}
     * @memberof NumberFormatInfo
     */
    numberDecimalSeparator?: string | null;

    /**
     * @type {string}
     * @memberof NumberFormatInfo
     */
    numberGroupSeparator?: string | null;

    /**
     * @type {number}
     * @memberof NumberFormatInfo
     */
    currencyPositivePattern?: number;

    /**
     * @type {string}
     * @memberof NumberFormatInfo
     */
    positiveInfinitySymbol?: string | null;

    /**
     * @type {string}
     * @memberof NumberFormatInfo
     */
    positiveSign?: string | null;

    /**
     * @type {number}
     * @memberof NumberFormatInfo
     */
    percentDecimalDigits?: number;

    /**
     * @type {string}
     * @memberof NumberFormatInfo
     */
    percentDecimalSeparator?: string | null;

    /**
     * @type {string}
     * @memberof NumberFormatInfo
     */
    percentGroupSeparator?: string | null;

    /**
     * @type {string}
     * @memberof NumberFormatInfo
     */
    percentSymbol?: string | null;

    /**
     * @type {string}
     * @memberof NumberFormatInfo
     */
    perMilleSymbol?: string | null;

    /**
     * @type {Array<string>}
     * @memberof NumberFormatInfo
     */
    nativeDigits?: Array<string> | null;

    /**
     * @type {DigitShapes}
     * @memberof NumberFormatInfo
     */
    digitSubstitution?: DigitShapes;
}
