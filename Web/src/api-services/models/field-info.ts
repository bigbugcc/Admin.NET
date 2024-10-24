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

import { CustomAttributeData } from './custom-attribute-data';
import { FieldAttributes } from './field-attributes';
import { MemberTypes } from './member-types';
import { Module } from './module';
import { RuntimeFieldHandle } from './runtime-field-handle';
import { Type } from './type';
 /**
 * 
 *
 * @export
 * @interface FieldInfo
 */
export interface FieldInfo {

    /**
     * @type {string}
     * @memberof FieldInfo
     */
    name?: string | null;

    /**
     * @type {Type}
     * @memberof FieldInfo
     */
    declaringType?: Type;

    /**
     * @type {Type}
     * @memberof FieldInfo
     */
    reflectedType?: Type;

    /**
     * @type {Module}
     * @memberof FieldInfo
     */
    module?: Module;

    /**
     * @type {Array<CustomAttributeData>}
     * @memberof FieldInfo
     */
    customAttributes?: Array<CustomAttributeData> | null;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isCollectible?: boolean;

    /**
     * @type {number}
     * @memberof FieldInfo
     */
    metadataToken?: number;

    /**
     * @type {MemberTypes}
     * @memberof FieldInfo
     */
    memberType?: MemberTypes;

    /**
     * @type {FieldAttributes}
     * @memberof FieldInfo
     */
    attributes?: FieldAttributes;

    /**
     * @type {Type}
     * @memberof FieldInfo
     */
    fieldType?: Type;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isInitOnly?: boolean;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isLiteral?: boolean;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isNotSerialized?: boolean;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isPinvokeImpl?: boolean;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isSpecialName?: boolean;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isStatic?: boolean;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isAssembly?: boolean;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isFamily?: boolean;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isFamilyAndAssembly?: boolean;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isFamilyOrAssembly?: boolean;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isPrivate?: boolean;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isPublic?: boolean;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isSecurityCritical?: boolean;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isSecuritySafeCritical?: boolean;

    /**
     * @type {boolean}
     * @memberof FieldInfo
     */
    isSecurityTransparent?: boolean;

    /**
     * @type {RuntimeFieldHandle}
     * @memberof FieldInfo
     */
    fieldHandle?: RuntimeFieldHandle;
}
