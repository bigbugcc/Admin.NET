// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Magicodes.ExporterAndImporter.Core.Filters;
using Magicodes.ExporterAndImporter.Core.Models;

namespace Admin.NET.Core.Service;

/// <summary>
/// 基础存储过程输入类
/// </summary>
public class BaseProcInput
{
    /// <summary>
    /// ProcId
    /// </summary>
    public string ProcId { get; set; }

    /// <summary>
    /// 数据库配置Id
    /// </summary>
    public string ConfigId { get; set; } = SqlSugarConst.MainConfigId;

    /// <summary>
    /// 存储过程输入参数
    /// </summary>
    /// <example>{"id":"351060822794565"}</example>
    public Dictionary<string, object> ProcParams { get; set; }
}

/// <summary>
/// 带表头名称存储过程输入类
/// </summary>
public class ExportProcByTMPInput : BaseProcInput
{
    /// <summary>
    /// 模板名称
    /// </summary>
    public string Template { get; set; }
}

/// <summary>
/// 带表头名称存储过程输入类
/// </summary>
public class ExportProcInput : BaseProcInput
{
    public Dictionary<string, string> EHeader { get; set; }
}

/// <summary>
/// 指定导出类名（有排序）存储过程输入类
/// </summary>
public class ExportProcInput2 : BaseProcInput
{
    public List<string> EHeader { get; set; }
}

/// <summary>
/// 前端指定列
/// </summary>
public class ProcExporterHeaderFilter : IExporterHeaderFilter
{
    private Dictionary<string, Tuple<string, int>> _includeHeader;

    public ProcExporterHeaderFilter(Dictionary<string, Tuple<string, int>> includeHeader)
    {
        _includeHeader = includeHeader;
    }

    public ExporterHeaderInfo Filter(ExporterHeaderInfo exporterHeaderInfo)
    {
        if (_includeHeader != null && _includeHeader.Count > 0)
        {
            var key = exporterHeaderInfo.PropertyName.ToUpper();
            if (_includeHeader.ContainsKey(key))
            {
                exporterHeaderInfo.DisplayName = _includeHeader[key].Item1;
                return exporterHeaderInfo;
            }
            else
            {
                exporterHeaderInfo.ExporterHeaderAttribute.Hidden = true;
            }
        }
        return exporterHeaderInfo;
    }
}