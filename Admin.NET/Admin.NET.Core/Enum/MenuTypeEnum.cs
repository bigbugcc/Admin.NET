// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core;

/// <summary>
/// 系统菜单类型枚举
/// </summary>
[Description("系统菜单类型枚举")]
public enum MenuTypeEnum
{
    /// <summary>
    /// 目录
    /// </summary>
    [Description("目录")]
    Dir = 1,

    /// <summary>
    /// 菜单
    /// </summary>
    [Description("菜单")]
    Menu = 2,

    /// <summary>
    /// 按钮
    /// </summary>
    [Description("按钮")]
    Btn = 3
}