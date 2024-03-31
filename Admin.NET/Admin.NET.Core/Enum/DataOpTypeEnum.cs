// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Core;

/// <summary>
/// 数据操作类型枚举
/// </summary>
[Description("数据操作类型枚举")]
public enum DataOpTypeEnum
{
    /// <summary>
    /// 其它
    /// </summary>
    [Description("其它")]
    Other,

    /// <summary>
    /// 增加
    /// </summary>
    [Description("增加")]
    Add,

    /// <summary>
    /// 删除
    /// </summary>
    [Description("删除")]
    Delete,

    /// <summary>
    /// 编辑
    /// </summary>
    [Description("编辑")]
    Edit,

    /// <summary>
    /// 更新
    /// </summary>
    [Description("更新")]
    Update,

    /// <summary>
    /// 查询
    /// </summary>
    [Description("查询")]
    Query,

    /// <summary>
    /// 详情
    /// </summary>
    [Description("详情")]
    Detail,

    /// <summary>
    /// 树
    /// </summary>
    [Description("树")]
    Tree,

    /// <summary>
    /// 导入
    /// </summary>
    [Description("导入")]
    Import,

    /// <summary>
    /// 导出
    /// </summary>
    [Description("导出")]
    Export,

    /// <summary>
    /// 授权
    /// </summary>
    [Description("授权")]
    Grant,

    /// <summary>
    /// 强退
    /// </summary>
    [Description("强退")]
    Force,

    /// <summary>
    /// 清空
    /// </summary>
    [Description("清空")]
    Clean
}