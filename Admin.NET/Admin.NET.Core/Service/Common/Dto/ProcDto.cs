// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Magicodes.ExporterAndImporter.Core.Filters;
using Magicodes.ExporterAndImporter.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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