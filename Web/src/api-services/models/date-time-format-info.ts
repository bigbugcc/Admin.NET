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

import { Calendar } from './calendar';
import { CalendarWeekRule } from './calendar-week-rule';
import { DayOfWeek } from './day-of-week';
 /**
 * 
 *
 * @export
 * @interface DateTimeFormatInfo
 */
export interface DateTimeFormatInfo {

    /**
     * @type {string}
     * @memberof DateTimeFormatInfo
     */
    amDesignator?: string | null;

    /**
     * @type {Calendar}
     * @memberof DateTimeFormatInfo
     */
    calendar?: Calendar;

    /**
     * @type {string}
     * @memberof DateTimeFormatInfo
     */
    dateSeparator?: string | null;

    /**
     * @type {DayOfWeek}
     * @memberof DateTimeFormatInfo
     */
    firstDayOfWeek?: DayOfWeek;

    /**
     * @type {CalendarWeekRule}
     * @memberof DateTimeFormatInfo
     */
    calendarWeekRule?: CalendarWeekRule;

    /**
     * @type {string}
     * @memberof DateTimeFormatInfo
     */
    fullDateTimePattern?: string | null;

    /**
     * @type {string}
     * @memberof DateTimeFormatInfo
     */
    longDatePattern?: string | null;

    /**
     * @type {string}
     * @memberof DateTimeFormatInfo
     */
    longTimePattern?: string | null;

    /**
     * @type {string}
     * @memberof DateTimeFormatInfo
     */
    monthDayPattern?: string | null;

    /**
     * @type {string}
     * @memberof DateTimeFormatInfo
     */
    pmDesignator?: string | null;

    /**
     * @type {string}
     * @memberof DateTimeFormatInfo
     */
    rfC1123Pattern?: string | null;

    /**
     * @type {string}
     * @memberof DateTimeFormatInfo
     */
    shortDatePattern?: string | null;

    /**
     * @type {string}
     * @memberof DateTimeFormatInfo
     */
    shortTimePattern?: string | null;

    /**
     * @type {string}
     * @memberof DateTimeFormatInfo
     */
    sortableDateTimePattern?: string | null;

    /**
     * @type {string}
     * @memberof DateTimeFormatInfo
     */
    timeSeparator?: string | null;

    /**
     * @type {string}
     * @memberof DateTimeFormatInfo
     */
    universalSortableDateTimePattern?: string | null;

    /**
     * @type {string}
     * @memberof DateTimeFormatInfo
     */
    yearMonthPattern?: string | null;

    /**
     * @type {Array<string>}
     * @memberof DateTimeFormatInfo
     */
    abbreviatedDayNames?: Array<string> | null;

    /**
     * @type {Array<string>}
     * @memberof DateTimeFormatInfo
     */
    shortestDayNames?: Array<string> | null;

    /**
     * @type {Array<string>}
     * @memberof DateTimeFormatInfo
     */
    dayNames?: Array<string> | null;

    /**
     * @type {Array<string>}
     * @memberof DateTimeFormatInfo
     */
    abbreviatedMonthNames?: Array<string> | null;

    /**
     * @type {Array<string>}
     * @memberof DateTimeFormatInfo
     */
    monthNames?: Array<string> | null;

    /**
     * @type {boolean}
     * @memberof DateTimeFormatInfo
     */
    isReadOnly?: boolean;

    /**
     * @type {string}
     * @memberof DateTimeFormatInfo
     */
    nativeCalendarName?: string | null;

    /**
     * @type {Array<string>}
     * @memberof DateTimeFormatInfo
     */
    abbreviatedMonthGenitiveNames?: Array<string> | null;

    /**
     * @type {Array<string>}
     * @memberof DateTimeFormatInfo
     */
    monthGenitiveNames?: Array<string> | null;
}
