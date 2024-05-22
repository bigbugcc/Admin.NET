namespace Admin.NET.Core.Service;

/// <summary>
/// 用户菜单快捷导航收藏
/// </summary>
public class UserFavoritesInput
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 收藏菜单Id集合
    /// </summary>
    public List<long> MenuIdList { get; set; }
}