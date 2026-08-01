// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// HTTP 请求作业处理程序
/// </summary>
[SuppressSniffer]
public class HttpJob : IJob
{
    /// <summary>
    /// 无效 HTTP 请求错误消息
    /// </summary>
    private const string INVALID_HTTP_ERROR_MESSAGE = "Invalid HTTP job request. (Parameter 'RequestUri')";

    /// <summary>
    /// <see cref="HttpClient"/> 创建工厂
    /// </summary>
    private readonly IHttpRemoteService _httpRemoteService;

    /// <summary>
    /// 作业调度器日志服务
    /// </summary>
    private readonly IScheduleLogger _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="httpRemoteService"><see cref="HttpClient"/> 创建工厂</param>
    /// <param name="logger">作业调度器日志服务</param>
    public HttpJob(IHttpRemoteService httpRemoteService
        , IScheduleLogger logger)
    {
        _httpRemoteService = httpRemoteService;
        _logger = logger;
    }

    /// <summary>
    /// 具体处理逻辑
    /// </summary>
    /// <param name="context">作业执行前上下文</param>
    /// <param name="stoppingToken">取消任务 Token</param>
    /// <returns><see cref="Task"/></returns>
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        var jobDetail = context.JobDetail;

        // 解析 HTTP 请求参数
        var httpJobMessage = JSON.Deserialize<HttpJobMessage>(jobDetail.GetProperty<string>(nameof(HttpJob)));

        // 空检查
        if (httpJobMessage == null || string.IsNullOrWhiteSpace(httpJobMessage.RequestUri))
        {
            _logger.LogWarning(INVALID_HTTP_ERROR_MESSAGE);
            context.Result = INVALID_HTTP_ERROR_MESSAGE;

            return;
        }

        // 创建 HttpRequestBuilder 构建器
        var httpRequestBuilder = HttpRequestBuilder.Create(httpJobMessage.HttpMethod, httpJobMessage.RequestUri)
            .SetHttpClientName(httpJobMessage.ClientName)
            .SetUserAgent(UserAgents.Chrome.PC);

        // 添加超时时间
        if (httpJobMessage.Timeout != null)
            httpRequestBuilder.SetTimeout(TimeSpan.FromMilliseconds(httpJobMessage.Timeout.Value));

        // 添加请求报文体，默认只支持发送 application/json 类型
        if (httpJobMessage.HttpMethod != HttpMethod.Get && httpJobMessage.HttpMethod != HttpMethod.Head && !string.IsNullOrWhiteSpace(httpJobMessage.Body))
            httpRequestBuilder.SetJsonContent(httpJobMessage.Body, Encoding.UTF8);

        // 添加请求头
        if (httpJobMessage.Headers is { Count: > 0 }) httpRequestBuilder.WithHeaders(httpJobMessage.Headers);

        // 发送请求
        var httpResponseMessage = await _httpRemoteService.SendAsync(httpRequestBuilder, stoppingToken);

        // 确保请求成功
        if (httpJobMessage.EnsureSuccessStatusCode)
        {
            httpResponseMessage = httpResponseMessage!.EnsureSuccessStatusCode();
        }

        // 是否解析返回值并打印
        string responseContent;
        if (httpJobMessage.PrintResponseContent)
        {
            // 解析返回值
            responseContent = await httpResponseMessage!.Content.ReadAsStringAsync(stoppingToken);

            // 输出日志
            _logger.LogInformation(
                $"Received HTTP response body with a length of <{responseContent.Length}> output as follows - {(int)httpResponseMessage.StatusCode}{Environment.NewLine}{responseContent}");
        }
        else responseContent = "COMPLETED";

        // 设置本次执行结果
        context.Result = JSON.Serialize(new
        {
            httpResponseMessage!.StatusCode,
            Body = responseContent
        });

        // 释放响应报文对象
        httpResponseMessage.Dispose();
    }
}