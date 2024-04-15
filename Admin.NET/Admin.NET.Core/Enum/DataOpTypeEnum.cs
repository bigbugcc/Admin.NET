// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

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