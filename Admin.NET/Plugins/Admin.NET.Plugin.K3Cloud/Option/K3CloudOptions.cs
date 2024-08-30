

using Furion.ConfigurableOptions;

namespace Admin.NET.Plugin.K3Cloud;
public sealed class K3CloudOptions : IConfigurableOptions
{

    /// <summary>
    /// ERP业务站点地址
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// 帐套Id(数据中心ID)
    /// </summary>
    public string AcctID { get; set; }
    /// <summary>
    /// 应用Id
    /// </summary>
    public string AppId { get; set; }
    /// <summary>
    /// 应用密钥
    /// </summary>
    public string AppKey { get; set; }
    /// <summary>
    /// 用户名称
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// 用户密码
    /// </summary>
    public string UserPassword { get; set; }
    /// <summary>
    /// 语言代码
    /// </summary>
    public string LanguageCode { get; set; }

}
