// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 代码生成参数类
/// </summary>
public class CodeGenInput : BasePageInput
{
    /// <summary>
    /// 作者姓名
    /// </summary>
    public virtual string AuthorName { get; set; }

    /// <summary>
    /// 类名
    /// </summary>
    public virtual string ClassName { get; set; }

    /// <summary>
    /// 是否移除表前缀
    /// </summary>
    public virtual string TablePrefix { get; set; }

    /// <summary>
    /// 库定位器名
    /// </summary>
    public virtual string ConfigId { get; set; }

    /// <summary>
    /// 数据库名(保留字段)
    /// </summary>
    public virtual string DbName { get; set; }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public virtual string DbType { get; set; }

    /// <summary>
    /// 数据库链接
    /// </summary>
    public virtual string ConnectionString { get; set; }

    /// <summary>
    /// 生成方式
    /// </summary>
    public virtual string GenerateType { get; set; }

    /// <summary>
    /// 数据库表名
    /// </summary>
    public virtual string TableName { get; set; }

    /// <summary>
    /// 命名空间
    /// </summary>
    public virtual string NameSpace { get; set; }

    /// <summary>
    /// 业务名（业务代码包名称）
    /// </summary>
    public virtual string BusName { get; set; }

    /// <summary>
    /// 功能名（数据库表名称）
    /// </summary>
    public virtual string TableComment { get; set; }

    /// <summary>
    /// 菜单应用分类（应用编码）
    /// </summary>
    public virtual string MenuApplication { get; set; }

    /// <summary>
    /// 菜单父级
    /// </summary>
    public virtual long MenuPid { get; set; }

    /// <summary>
    /// 支持打印类型
    /// </summary>
    public virtual string PrintType { get; set; }

    /// <summary>
    /// 打印模版名称
    /// </summary>
    public virtual string PrintName { get; set; }
}

public class AddCodeGenInput : CodeGenInput
{
    /// <summary>
    /// 数据库表名
    /// </summary>
    [Required(ErrorMessage = "数据库表名不能为空")]
    public override string TableName { get; set; }

    /// <summary>
    /// 业务名（业务代码包名称）
    /// </summary>
    [Required(ErrorMessage = "业务名不能为空")]
    public override string BusName { get; set; }

    /// <summary>
    /// 命名空间
    /// </summary>
    [Required(ErrorMessage = "命名空间不能为空")]
    public override string NameSpace { get; set; }

    /// <summary>
    /// 作者姓名
    /// </summary>
    [Required(ErrorMessage = "作者姓名不能为空")]
    public override string AuthorName { get; set; }

    ///// <summary>
    ///// 类名
    ///// </summary>
    //[Required(ErrorMessage = "类名不能为空")]
    //public override string ClassName { get; set; }

    ///// <summary>
    ///// 是否移除表前缀
    ///// </summary>
    //[Required(ErrorMessage = "是否移除表前缀不能为空")]
    //public override string TablePrefix { get; set; }

    /// <summary>
    /// 生成方式
    /// </summary>
    [Required(ErrorMessage = "生成方式不能为空")]
    public override string GenerateType { get; set; }

    ///// <summary>
    ///// 功能名（数据库表名称）
    ///// </summary>
    //[Required(ErrorMessage = "数据库表名不能为空")]
    //public override string TableComment { get; set; }

    /// <summary>
    /// 菜单父级
    /// </summary>
    [Required(ErrorMessage = "菜单父级不能为空")]
    public override long MenuPid { get; set; }
}

public class DeleteCodeGenInput
{
    /// <summary>
    /// 代码生成器Id
    /// </summary>
    [Required(ErrorMessage = "代码生成器Id不能为空")]
    public long Id { get; set; }
}

public class UpdateCodeGenInput : CodeGenInput
{
    /// <summary>
    /// 代码生成器Id
    /// </summary>
    [Required(ErrorMessage = "代码生成器Id不能为空")]
    public long Id { get; set; }
}

public class QueryCodeGenInput : DeleteCodeGenInput
{
}