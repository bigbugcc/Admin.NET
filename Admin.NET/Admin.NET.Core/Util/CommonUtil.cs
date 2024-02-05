// 麻省理工学院许可证
//
// 版权所有 (c) 2021-2023 zuohuaijun，大名科技（天津）有限公司  联系电话/微信：18020030720  QQ：515096995
//
// 特此免费授予获得本软件的任何人以处理本软件的权利，但须遵守以下条件：在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Magicodes.ExporterAndImporter.Core.Models;
using Nest;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.CgibinTagsMembersGetBlackListResponse.Types;

namespace Admin.NET.Core;

/// <summary>
/// 通用工具类
/// </summary>
public static class CommonUtil
{
    /// <summary>
    /// 生成百分数
    /// </summary>
    /// <param name="PassCount"></param>
    /// <param name="allCount"></param>
    /// <returns></returns>
    public static string ExecPercent(decimal PassCount, decimal allCount)
    {
        string res = "";
        if (allCount > 0)
        {
            var value = (double)Math.Round(PassCount / allCount * 100, 1);
            if (value < 0)
                res = Math.Round(value + 5 / Math.Pow(10, 0 + 1), 0, MidpointRounding.AwayFromZero).ToString();
            else
                res = Math.Round(value, 0, MidpointRounding.AwayFromZero).ToString();
        }
        if (res == "") res = "0";
        return res + "%";
    }

    /// <summary>
    /// 获取服务地址
    /// </summary>
    /// <returns></returns>
    public static string GetLocalhost()
    {
        string result = $"{App.HttpContext.Request.Scheme}://{App.HttpContext.Request.Host.Value}";

        // 代理模式：获取真正的本机地址
        // X-Original-Host=原始请求
        // X-Forwarded-Server=从哪里转发过来
        if (App.HttpContext.Request.Headers.ContainsKey("Origin")) // 配置成完整的路径如（结尾不要带"/"）,比如 https://www.abc.com
            result = $"{App.HttpContext.Request.Headers["Origin"]}";
        else if (App.HttpContext.Request.Headers.ContainsKey("X-Original")) // 配置成完整的路径如（结尾不要带"/"）,比如 https://www.abc.com
            result = $"{App.HttpContext.Request.Headers["X-Original"]}";
        else if (App.HttpContext.Request.Headers.ContainsKey("X-Original-Host"))
            result = $"{App.HttpContext.Request.Scheme}://{App.HttpContext.Request.Headers["X-Original-Host"]}";
        return result;
    }

    /// <summary>
    /// 对象序列化XML
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string SerializeObjectToXml<T>(T obj)
    {
        if (obj == null) return string.Empty;

        var xs = new XmlSerializer(obj.GetType());
        var stream = new MemoryStream();
        var setting = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false), // 不包含BOM
            Indent = true // 设置格式化缩进
        };
        using (var writer = XmlWriter.Create(stream, setting))
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", ""); // 去除默认命名空间
            xs.Serialize(writer, obj, ns);
        }
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>
    /// 字符串转XML格式
    /// </summary>
    /// <param name="xmlStr"></param>
    /// <returns></returns>
    public static XElement SerializeStringToXml(string xmlStr)
    {
        try
        {
            return XElement.Parse(xmlStr);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 导出模板Excel
    /// </summary>
    /// <returns></returns>
    public static async Task<IActionResult> ExportExcelTemplate<T>() where T : class,new()
    { 
        IImporter importer = new ExcelImporter();
        var res=await importer.GenerateTemplateBytes<T>();  

        return new FileContentResult(res, "application/octet-stream") { FileDownloadName = typeof(T).Name+".xlsx" };
    }

    /// <summary>
    /// 导出数据excel
    /// </summary>
    /// <returns></returns>
    public static async Task<IActionResult> ExportExcelData<T>(ICollection<T> data) where T : class, new()
    {
        var export = new ExcelExporter();
        var res = await export.ExportAsByteArray<T>(data);

        return new FileContentResult(res, "application/octet-stream") { FileDownloadName = typeof(T).Name + ".xlsx" };
    }

    /// <summary>
    /// 导出数据excel,包括字典转换
    /// </summary>
    /// <returns></returns>
    public static async Task<IActionResult> ExportExcelData<TSource, TTarget>(ISugarQueryable<TSource> query, Func<TSource, TTarget, TTarget> action = null) 
        where TSource : class, new() where TTarget : class, new ()
    {
        var PropMappings = GetExportPropertMap< TSource, TTarget >(); 
        var data = query.ToList();
        //相同属性复制值，字典值转换
        var result = new List<TTarget>();
        foreach (var item in data)
        {
            var newData = new TTarget();
            foreach (var dict in PropMappings)
            {
                var targeProp = dict.Value.Item3;
                if (targeProp != null)
                {
                    var propertyInfo = dict.Value.Item2;
                    var sourceVal = propertyInfo.GetValue(item, null);
                    if (sourceVal == null)
                    {
                        continue;
                    }

                    var map = dict.Value.Item1;
                    if (map != null && map.ContainsKey(sourceVal))
                    {
                        var newVal = map[sourceVal];
                        targeProp.SetValue(newData, newVal);
                    }
                    else
                    {
                        if (targeProp.PropertyType.FullName == propertyInfo.PropertyType.FullName)
                        {
                            targeProp.SetValue(newData, sourceVal);
                        }
                        else
                        {
                            var newVal = sourceVal.ToString().ParseTo(targeProp.PropertyType);
                            targeProp.SetValue(newData, newVal);
                        }
                    }
                }
                if (action != null)
                {
                    newData = action(item, newData);
                }
            } 
            result.Add(newData);
        } 
        var export = new ExcelExporter();
        var res = await export.ExportAsByteArray(result);

        return new FileContentResult(res, "application/octet-stream") { FileDownloadName = typeof(TTarget).Name + ".xlsx" };
    }

    /// <summary>
    /// 导入数据Excel
    /// </summary>
    /// <param name="file"></param> 
    /// <returns></returns>
    public static async Task<ICollection<T>> ImportExcelData<T>([Required] IFormFile file) where T : class, new()
    {  
        IImporter importer = new ExcelImporter();
        var res = await importer.Import<T>(file.OpenReadStream());
        string message = string.Empty;
        if (res.HasError)
        {
            if (res.Exception != null)
                message += $"\r\n{res.Exception.Message}";
            foreach (DataRowErrorInfo drErrorInfo in res.RowErrors)
            {
                int rowNum = drErrorInfo.RowIndex;
                foreach (var item in drErrorInfo.FieldErrors)
                    message += $"\r\n{item.Key}：{item.Value}（文件第{drErrorInfo.RowIndex}行）";
            }
            message += "字段缺失：" + string.Join("，", res.TemplateErrors.Select(m => m.RequireColumnName).ToList());

            throw Oops.Oh("导入异常:" + message);
        }
        return res.Data;
    }

    //例：List<Dm_ApplyDemo> ls = CommonUtil.ParseList<Dm_ApplyDemoInport, Dm_ApplyDemo>(importResult.Data);
    /// <summary>
    /// 对象转换 含字典转换
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="data"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static List<TTarget> ParseList<TSource, TTarget>(IEnumerable<TSource> data, Func<TSource, TTarget, TTarget> action=null) where TTarget : new()
    {
        Dictionary<string, Tuple<Dictionary<string, object>, PropertyInfo, PropertyInfo>> PropMappings = GetImportPropertMap<TSource, TTarget>();
        //相同属性复制值，字典值转换
        var result = new List<TTarget>();
        foreach (var item in data)
        {
            var newData = new TTarget();
            foreach (var dict in PropMappings)
            {
                var targeProp = dict.Value.Item3;
                if (targeProp != null)
                {
                    var propertyInfo = dict.Value.Item2;
                    var sourceVal = propertyInfo.GetValue(item, null);
                    if (sourceVal == null)
                    {
                        continue;
                    }

                    var map = dict.Value.Item1;
                    if (map != null && map.ContainsKey(sourceVal.ToString()))
                    {
                        var newVal = map[sourceVal.ToString()];
                        targeProp.SetValue(newData, newVal);
                    }
                    else
                    {
                        if (targeProp.PropertyType.FullName == propertyInfo.PropertyType.FullName)
                        {
                            targeProp.SetValue(newData, sourceVal);
                        }
                        else
                        {
                            var newVal = sourceVal.ToString().ParseTo(targeProp.PropertyType);
                            targeProp.SetValue(newData, newVal);
                        }
                    }
                }
            }
            if (action != null)
            {
                newData = action(item, newData);
            }
            if (newData != null)
                result.Add(newData);
        }
        return result;
    }


    /// <summary>
    /// 获取导入属性映射       
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <returns>整理导入对象的 属性名称， 字典数据，原属性信息，目标属性信息 </returns>
    private static Dictionary<string, Tuple<Dictionary<string, object>, PropertyInfo, PropertyInfo>> GetImportPropertMap<TSource, TTarget>() where TTarget : new()
    {
        var dictService = App.GetService<SqlSugarRepository<SysDictData>>();
        //整理导入对象的 属性名称，<字典数据，原属性信息，目标属性信息>
        Dictionary<string, Tuple<Dictionary<string, object>, PropertyInfo, PropertyInfo>> PropMappings =
        new Dictionary<string, Tuple<Dictionary<string, object>, PropertyInfo, PropertyInfo>>();

        var TSourceProps = typeof(TSource).GetProperties().ToList();
        var TTargetProps = typeof(TTarget).GetProperties().ToDictionary(m => m.Name);

        foreach (var propertyInfo in TSourceProps)
        {
            var attrs = propertyInfo.GetCustomAttribute<ImportDictAttribute>();
            if (attrs != null && !string.IsNullOrWhiteSpace(attrs.TypeCode))
            {
                var targetProp = TTargetProps[attrs.TargetPropName];

                var MappingValues = dictService.Context.Queryable<SysDictType, SysDictData>((a, b) =>
                    new JoinQueryInfos(JoinType.Inner, a.Id == b.DictTypeId))
                    .Where(a => a.Code == attrs.TypeCode)
                    .Where((a, b) => a.Status == StatusEnum.Enable && b.Status == StatusEnum.Enable)
                    .Select((a, b) => new
                    {
                        Label = b.Value,
                        Value = b.Code
                    }).ToList()
                    .ToDictionary(m => m.Label, m => m.Value.ParseTo(targetProp.PropertyType));
                PropMappings.Add(propertyInfo.Name, new Tuple<Dictionary<string, object>, PropertyInfo, PropertyInfo>(
                    MappingValues, propertyInfo, targetProp
                    ));
            }
            else
            {
                PropMappings.Add(propertyInfo.Name, new Tuple<Dictionary<string, object>, PropertyInfo, PropertyInfo>(
                    null, propertyInfo, TTargetProps.ContainsKey(propertyInfo.Name) ? TTargetProps[propertyInfo.Name] : null
                    ));
            }
        }

        return PropMappings;
    }



    /// <summary>
    /// 获取导出属性映射       
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <returns>整理导入对象的 属性名称， 字典数据，原属性信息，目标属性信息 </returns>
    private static Dictionary<string, Tuple<Dictionary<object,string>, PropertyInfo, PropertyInfo>> GetExportPropertMap<TSource, TTarget>() where TTarget : new()
    {
        var dictService = App.GetService<SqlSugarRepository<SysDictData>>();
        //整理导入对象的 属性名称，<字典数据，原属性信息，目标属性信息>
        Dictionary<string, Tuple<Dictionary<object,string>, PropertyInfo, PropertyInfo>> PropMappings =
        new Dictionary<string, Tuple<Dictionary<object,string>, PropertyInfo, PropertyInfo>>();

        var TargetProps = typeof(TTarget).GetProperties().ToList();
        var SourceProps = typeof(TSource).GetProperties().ToDictionary(m => m.Name);

        foreach (var propertyInfo in TargetProps)
        {
            var attrs = propertyInfo.GetCustomAttribute<ImportDictAttribute>();
            if (attrs != null && !string.IsNullOrWhiteSpace(attrs.TypeCode))
            {
                var targetProp = SourceProps[attrs.TargetPropName];

                var MappingValues = dictService.Context.Queryable<SysDictType, SysDictData>((a, b) =>
                    new JoinQueryInfos(JoinType.Inner, a.Id == b.DictTypeId))
                    .Where(a => a.Code == attrs.TypeCode)
                    .Where((a, b) => a.Status == StatusEnum.Enable && b.Status == StatusEnum.Enable)
                    .Select((a, b) => new
                    {
                        Label = b.Value,
                        Value = b.Code
                    }).ToList()
                    .ToDictionary(m => m.Value.ParseTo(targetProp.PropertyType), m => m.Label);
                PropMappings.Add(propertyInfo.Name, new Tuple<Dictionary<object,string>, PropertyInfo, PropertyInfo>(
                    MappingValues, targetProp, propertyInfo
                    ));
            }
            else
            {
                PropMappings.Add(propertyInfo.Name, new Tuple<Dictionary<object,string>, PropertyInfo, PropertyInfo>(
                    null, SourceProps.ContainsKey(propertyInfo.Name) ? SourceProps[propertyInfo.Name] : null, propertyInfo
                    ));
            }
        }

        return PropMappings;
    }


    /// <summary>
    /// 获取属性映射       
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <returns>整理导入对象的 属性名称， 字典数据，原属性信息，目标属性信息 </returns>
    private static Dictionary<string, Tuple<string, string>> GetExportDicttMap<  TTarget>() where TTarget : new()
    {
        var dictService = App.GetService<SqlSugarRepository<SysDictData>>();
        //整理导入对象的 属性名称，目标属性名，字典Code
        Dictionary<string, Tuple<string, string>> PropMappings = new Dictionary<string, Tuple<string, string>>(); 
        var TTargetProps = typeof(TTarget).GetProperties(); 
        foreach (var propertyInfo in TTargetProps)
        {
            var attrs = propertyInfo.GetCustomAttribute<ImportDictAttribute>();
            if (attrs != null && !string.IsNullOrWhiteSpace(attrs.TypeCode))
            { 
                PropMappings.Add(propertyInfo.Name, new Tuple<string, string>(  attrs.TargetPropName,attrs.TypeCode  )); 
            } 
        }

        return PropMappings;
    }
}