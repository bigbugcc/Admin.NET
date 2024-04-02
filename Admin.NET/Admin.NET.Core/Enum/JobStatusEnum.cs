// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core;

/// <summary>
/// 岗位状态枚举
/// </summary>
[Description("岗位状态枚举")]
public enum JobStatusEnum
{
    /// <summary>
    /// 在职
    /// </summary>
    [Description("在职")]
    On = 1,

    /// <summary>
    /// 离职
    /// </summary>
    [Description("离职")]
    Off = 2,

    /// <summary>
    /// 请假
    /// </summary>
    [Description("请假")]
    Leave = 3,

    /// <summary>
    /// 其他
    /// </summary>
    [Description("其他")]
    Other = 4,
}