// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core;

/// <summary>
/// 邮件配置选项
/// </summary>
public sealed class EmailOptions : IConfigurableOptions
{
    /// <summary>
    /// 主机
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 默认发件者邮箱
    /// </summary>
    public string DefaultFromEmail { get; set; }

    /// <summary>
    /// 默认接收人邮箱
    /// </summary>
    public string DefaultToEmail { get; set; }

    /// <summary>
    /// 启用SSL
    /// </summary>
    public bool EnableSsl { get; set; }

    ///// <summary>
    ///// 是否使用默认凭据
    ///// </summary>
    //public bool UseDefaultCredentials { get; set; }

    /// <summary>
    /// 邮箱账号
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 邮箱密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 默认邮件标题
    /// </summary>
    public string DefaultFromName { get; set; }
}
