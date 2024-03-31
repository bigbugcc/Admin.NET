// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core.Service;

public class PageNoticeInput : BasePageInput
{
    /// <summary>
    /// 标题
    /// </summary>
    public virtual string Title { get; set; }

    /// <summary>
    /// 类型（1通知 2公告）
    /// </summary>
    public virtual NoticeTypeEnum? Type { get; set; }
}

public class AddNoticeInput : SysNotice
{
}

public class UpdateNoticeInput : AddNoticeInput
{
}

public class DeleteNoticeInput : BaseIdInput
{
}

public class NoticeInput : BaseIdInput
{
}