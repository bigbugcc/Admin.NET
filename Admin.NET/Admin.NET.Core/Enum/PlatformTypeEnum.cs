// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core;

/// <summary>
/// 平台类型枚举
/// </summary>
[Description("平台类型枚举")]
public enum PlatformTypeEnum
{
    /// <summary>
    /// 微信公众号
    /// </summary>
    [Description("微信公众号")]
    微信公众号 = 1,

    /// <summary>
    /// 微信小程序
    /// </summary>
    [Description("微信小程序")]
    微信小程序 = 2,

    /// <summary>
    /// QQ
    /// </summary>
    [Description("QQ")]
    QQ = 3,

    /// <summary>
    /// 支付宝
    /// </summary>
    [Description("支付宝")]
    Alipay = 4,

    /// <summary>
    /// Gitee
    /// </summary>
    [Description("Gitee")]
    Gitee = 5,
}