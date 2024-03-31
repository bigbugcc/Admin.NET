// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

public class WechatPayOutput
{
    /// <summary>
    /// OpenId
    /// </summary>
    public string OpenId { get; set; }

    /// <summary>
    /// 订单金额
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 附加数据
    /// </summary>
    public string Attachment { get; set; }

    /// <summary>
    /// 优惠标记
    /// </summary>
    public string GoodsTag { get; set; }
}