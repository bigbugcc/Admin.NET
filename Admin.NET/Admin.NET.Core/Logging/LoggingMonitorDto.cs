// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 日志监控信息输出参数
/// </summary>
public class LoggingMonitorDto
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 控制器名称
    /// </summary>
    public string ControllerName { get; set; }

    /// <summary>
    /// 控制器类型名称
    /// </summary>
    public string ControllerTypeName { get; set; }

    /// <summary>
    /// 操作方法名称
    /// </summary>
    public string ActionName { get; set; }

    /// <summary>
    /// 操作方法类型名称
    /// </summary>
    public string ActionTypeName { get; set; }

    /// <summary>
    /// 区域名称（Area）
    /// </summary>
    public string AreaName { get; set; }

    /// <summary>
    /// 显示名称（全路径）
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// 显示标题
    /// </summary>
    public string DisplayTitle { get; set; }

    /// <summary>
    /// 本地IPv4地址
    /// </summary>
    public string LocalIPv4 { get; set; }

    /// <summary>
    /// 本地端口
    /// </summary>
    public int? LocalPort { get; set; }

    /// <summary>
    /// 远程IPv4地址
    /// </summary>
    public string RemoteIPv4 { get; set; }

    /// <summary>
    /// 远程端口
    /// </summary>
    public int? RemotePort { get; set; }

    /// <summary>
    /// HTTP请求方法（如GET、POST）
    /// </summary>
    public string HttpMethod { get; set; }

    /// <summary>
    /// 分布式追踪ID（TraceId）
    /// </summary>
    public string TraceId { get; set; }

    /// <summary>
    /// 线程ID
    /// </summary>
    public int? ThreadId { get; set; }

    /// <summary>
    /// 请求URL
    /// </summary>
    public string RequestUrl { get; set; }

    /// <summary>
    /// 协议版本（如HTTP/1.1）
    /// </summary>
    public string Protocol { get; set; }

    /// <summary>
    /// 引用页面URL（Referer）
    /// </summary>
    public string RefererUrl { get; set; }

    /// <summary>
    /// 用户代理（User-Agent）
    /// </summary>
    public string UserAgent { get; set; }

    /// <summary>
    /// 接受的语言（Accept-Language）
    /// </summary>
    public string AcceptLanguage { get; set; }

    /// <summary>
    /// 请求来源（client、server等）
    /// </summary>
    public string RequestFrom { get; set; }

    /// <summary>
    /// 请求头中的Cookies
    /// </summary>
    public string RequestHeaderCookies { get; set; }

    /// <summary>
    /// 操作耗时（毫秒）
    /// </summary>
    public long? TimeOperationElapsedMilliseconds { get; set; }

    /// <summary>
    /// 访问令牌（AccessToken）
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// 响应头中的Cookies
    /// </summary>
    public string ResponseHeaderCookies { get; set; }

    /// <summary>
    /// 操作系统描述
    /// </summary>
    public string OsDescription { get; set; }

    /// <summary>
    /// 操作系统架构（如X64）
    /// </summary>
    public string OsArchitecture { get; set; }

    /// <summary>
    /// 框架描述（如.NET 8.0.18）
    /// </summary>
    public string FrameworkDescription { get; set; }

    /// <summary>
    /// 基础框架名称（如Furion.Pure）
    /// </summary>
    public string BasicFramework { get; set; }

    /// <summary>
    /// 基础框架版本
    /// </summary>
    public string BasicFrameworkVersion { get; set; }

    /// <summary>
    /// 入口程序集名称
    /// </summary>
    public string EntryAssemblyName { get; set; }

    /// <summary>
    /// 进程名称
    /// </summary>
    public string ProcessName { get; set; }

    /// <summary>
    /// 部署服务器（如Kestrel）
    /// </summary>
    public string DeployServer { get; set; }

    /// <summary>
    /// 启动监听地址
    /// </summary>
    public string StartUrls { get; set; }

    /// <summary>
    /// 环境（如Development、Production）
    /// </summary>
    public string Environment { get; set; }

    /// <summary>
    /// 授权声明集合
    /// </summary>
    public List<LoggingAuthorizationClaimsDto> AuthorizationClaims { get; set; }

    /// <summary>
    /// 请求头集合
    /// </summary>
    public List<KeyValuePair<string, object>> RequestHeaders { get; set; }

    /// <summary>
    /// 请求参数集合
    /// </summary>
    public List<LoggingParametersDto> Parameters { get; set; }

    /// <summary>
    /// 返回信息
    /// </summary>
    public LoggingReturnInformationDto ReturnInformation { get; set; }

    /// <summary>
    /// 异常信息
    /// </summary>
    public object Exception { get; set; }

    /// <summary>
    /// 验证信息
    /// </summary>
    public object Validation { get; set; }
}

public class LoggingAuthorizationClaimsDto
{
    /// <summary>
    /// 类型名
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 值类型
    /// </summary>
    public string ValueType { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string Value { get; set; }
}

/// <summary>
/// 输入参数
/// </summary>
public class LoggingParametersDto
{
    /// <summary>
    /// 输入类型
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 输入类型
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 实际输入数据
    /// </summary>
    public object Value { get; set; }
}

/// <summary>
/// 返回信息详情
/// </summary>
public class LoggingReturnInformationDto
{
    /// <summary>
    /// 返回类型
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// HTTP状态码
    /// </summary>
    public int? HttpStatusCode { get; set; }

    /// <summary>
    /// 实际返回类型
    /// </summary>
    public string ActType { get; set; }

    /// <summary>
    /// 实际返回数据
    /// </summary>
    public object Value { get; set; }
}

/// <summary>
/// 用户信息
/// </summary>
public class LoggingUserInfo
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// 账号
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string RealName { get; set; }

    /// <summary>
    /// 租户Id
    /// </summary>
    public long? TenantId { get; set; }
}