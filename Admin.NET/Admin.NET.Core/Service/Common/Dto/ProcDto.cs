// 大名科技（天津）有限公司版权所有  电话：18020030720  QQ：515096995
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证

using Magicodes.ExporterAndImporter.Core.Filters;
using Magicodes.ExporterAndImporter.Core.Models;

namespace Admin.NET.Core.Service;

/// <summary>
/// 基础存储过程输入
/// </summary>
public class BaseProcInput
{
    /// <summary>
    /// ProcId
    /// </summary>
    public string ProcId { get; set; }

    /// <summary>
    /// 数据库配置ID
    /// </summary>
    public string ConfigId { get; set; }

    /// <summary>
    /// 存储过程输入参数
    /// </summary>
    /// <example>{"id":"351060822794565"}</example>
    public Dictionary<string, object> ProcParams { get; set; }
}

/// <summary>
/// 带表头名称存储过程输入
/// </summary>
public class ExportProcByTempInput : BaseProcInput
{
    /// <summary>
    /// 模板名称
    /// </summary>
    public string Template { get; set; }
}

/// <summary>
/// 带表头名称存储过程输入
/// </summary>
public class ExportProcInput : BaseProcInput
{
    public Dictionary<string, string> EHeader { get; set; }
}

/// <summary>
/// 指定导出类名（有排序）存储过程输入
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
    private readonly Dictionary<string, Tuple<string, int>> _includeHeader;

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