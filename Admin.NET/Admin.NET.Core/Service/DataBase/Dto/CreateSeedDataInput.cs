// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

public class CreateSeedDataInput
{
    /// <summary>
    /// 库标识
    /// </summary>
    public string ConfigId { get; set; }

    /// <summary>
    /// 表名
    /// </summary>
    /// <example>student</example>
    public string TableName { get; set; }

    /// <summary>
    /// 实体名称
    /// </summary>
    /// <example>Student</example>
    public string EntityName { get; set; }

    /// <summary>
    /// 种子名称
    /// </summary>
    /// <example>Student</example>
    public string SeedDataName { get; set; }

    /// <summary>
    /// 导出位置
    /// </summary>
    /// <example>Web.Application</example>
    public string Position { get; set; }

    /// <summary>
    /// 后缀
    /// </summary>
    /// <example>Web.Application</example>
    public string Suffix { get; set; }

    /// <summary>
    /// 过滤已有数据
    /// </summary>
    /// <remarks>
    /// 如果数据在其它不同名的已有的种子类型的数据中出现过，就不生成这个数据
    /// 主要用于生成菜单功能，菜单功能往往与子项目绑定，如果生成完整数据就会导致菜单项多处理重复。
    /// </remarks>
    public bool FilterExistingData { get; set; }
}