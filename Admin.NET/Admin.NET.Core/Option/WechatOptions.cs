// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core;

/// <summary>
/// 微信相关配置选项
/// </summary>
public sealed class WechatOptions : IConfigurableOptions
{
	//公众号
	public string WechatAppId { get; set; }

	public string WechatAppSecret { get; set; }

	/// <summary>
	/// 微信公众号服务器配置中的令牌(Token)
	/// </summary>
	public string WechatToken { get; set; }

	/// <summary>
	/// 微信公众号服务器配置中的消息加解密密钥(EncodingAESKey)
	/// </summary>
	public string WechatEncodingAESKey { get; set; }

	//小程序
	public string WxOpenAppId { get; set; }

	public string WxOpenAppSecret { get; set; }

	/// <summary>
	/// 小程序消息推送中的令牌(Token)
	/// </summary>
	public string WxToken { get; set; }

	/// <summary>
	/// 小程序消息推送中的消息加解密密钥(EncodingAESKey)
	/// </summary>
	public string WxEncodingAESKey { get; set; }
}