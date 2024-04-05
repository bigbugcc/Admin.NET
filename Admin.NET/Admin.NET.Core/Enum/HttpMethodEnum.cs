// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core;

/// <summary>
/// HTTP请求方法
/// </summary>
[Description("HTTP请求方法")]
public enum HttpMethodEnum
{
    /// <summary>
    ///  HTTP "CONNECT" method.
    /// </summary>
    [Description("HTTP \"CONNECT\" method.")]
    Connect,
    /// <summary>
    /// HTTP "DELETE" method.
    /// </summary>
    [Description("HTTP \"DELETE\" method.")]
    Delete,
    /// <summary>
    ///  HTTP "GET" method.
    /// </summary>
    [Description("HTTP \"GET\" method.")]
    Get,
    /// <summary>
    /// HTTP "HEAD" method.
    /// </summary>
    [Description("HTTP \"HEAD\" method.")]
    Head,
    /// <summary>
    /// HTTP "OPTIONS" method.
    /// </summary>
    [Description("HTTP \"OPTIONS\" method.")]
    Options,
    /// <summary>
    /// HTTP "PATCH" method. 
    /// </summary>    
    [Description("HTTP \"PATCH\" method. ")]
    Patch,
    /// <summary>
    ///  HTTP "POST" method.
    /// </summary>    
    [Description("HTTP \"POST\" method.")]
    Post,
    /// <summary>
    /// HTTP "PUT" method.
    /// </summary>    
    [Description(" HTTP \"PUT\" method.")]
    Put,
    /// <summary>
    /// HTTP "TRACE" method.
    /// </summary>
    [Description(" HTTP \"TRACE\" method.")]
    Trace
}