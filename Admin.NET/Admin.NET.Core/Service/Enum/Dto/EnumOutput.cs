// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

namespace Admin.NET.Core.Service;

/// <summary>
/// 枚举类型输出参数
/// </summary>
public class EnumTypeOutput
{
    /// <summary>
    /// 枚举类型描述
    /// </summary>
    public string TypeDescribe { get; set; }

    /// <summary>
    /// 枚举类型名称
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// 枚举类型备注
    /// </summary>
    public string TypeRemark { get; set; }

    /// <summary>
    /// 枚举实体
    /// </summary>
    public List<EnumEntity> EnumEntities { get; set; }
}