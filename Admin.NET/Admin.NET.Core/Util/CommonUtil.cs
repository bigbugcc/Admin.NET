// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using IPTools.Core;
using Magicodes.ExporterAndImporter.Core.Models;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using UAParser;

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
    public static async Task<IActionResult> ExportExcelTemplate<T>(string fileName = null) where T : class, new()
    {
        IImporter importer = new ExcelImporter();
        var res = await importer.GenerateTemplateBytes<T>();

        return new FileContentResult(res, "application/octet-stream") { FileDownloadName = $"{(string.IsNullOrEmpty(fileName) ? typeof(T).Name : fileName)}.xlsx" };
    }

    /// <summary>
    /// 导出数据excel
    /// </summary>
    /// <returns></returns>
    public static async Task<IActionResult> ExportExcelData<T>(ICollection<T> data, string fileName = null) where T : class, new()
    {
        var export = new ExcelExporter();
        var res = await export.ExportAsByteArray<T>(data);

        return new FileContentResult(res, "application/octet-stream") { FileDownloadName = $"{(string.IsNullOrEmpty(fileName) ? typeof(T).Name : fileName)}.xlsx" };
    }

    /// <summary>
    /// 导出数据excel,包括字典转换
    /// </summary>
    /// <returns></returns>
    public static async Task<IActionResult> ExportExcelData<TSource, TTarget>(ISugarQueryable<TSource> query, Func<TSource, TTarget, TTarget> action = null)
        where TSource : class, new() where TTarget : class, new()
    {
        var PropMappings = GetExportPropertMap<TSource, TTarget>();
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
        var message = string.Empty;
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

    // 例：List<Dm_ApplyDemo> ls = CommonUtil.ParseList<Dm_ApplyDemoInport, Dm_ApplyDemo>(importResult.Data);
    /// <summary>
    /// 对象转换 含字典转换
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="data"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static List<TTarget> ParseList<TSource, TTarget>(IEnumerable<TSource> data, Func<TSource, TTarget, TTarget> action = null) where TTarget : new()
    {
        var propMappings = GetImportPropertMap<TSource, TTarget>();
        // 相同属性复制值，字典值转换
        var result = new List<TTarget>();
        foreach (var item in data)
        {
            var newData = new TTarget();
            foreach (var dict in propMappings)
            {
                var targeProp = dict.Value.Item3;
                if (targeProp != null)
                {
                    var propertyInfo = dict.Value.Item2;
                    var sourceVal = propertyInfo.GetValue(item, null);
                    if (sourceVal == null)
                        continue;

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
                newData = action(item, newData);

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
        // 整理导入对象的属性名称，<字典数据，原属性信息，目标属性信息>
        var propMappings = new Dictionary<string, Tuple<Dictionary<string, object>, PropertyInfo, PropertyInfo>>();

        var dictService = App.GetService<SqlSugarRepository<SysDictData>>();
        var tSourceProps = typeof(TSource).GetProperties().ToList();
        var tTargetProps = typeof(TTarget).GetProperties().ToDictionary(m => m.Name);
        foreach (var propertyInfo in tSourceProps)
        {
            var attrs = propertyInfo.GetCustomAttribute<ImportDictAttribute>();
            if (attrs != null && !string.IsNullOrWhiteSpace(attrs.TypeCode))
            {
                var targetProp = tTargetProps[attrs.TargetPropName];
                var mappingValues = dictService.Context.Queryable<SysDictType, SysDictData>((a, b) =>
                    new JoinQueryInfos(JoinType.Inner, a.Id == b.DictTypeId))
                    .Where(a => a.Code == attrs.TypeCode)
                    .Where((a, b) => a.Status == StatusEnum.Enable && b.Status == StatusEnum.Enable)
                    .Select((a, b) => new
                    {
                        Label = b.Value,
                        Value = b.Code
                    }).ToList()
                    .ToDictionary(m => m.Label, m => m.Value.ParseTo(targetProp.PropertyType));
                propMappings.Add(propertyInfo.Name, new Tuple<Dictionary<string, object>, PropertyInfo, PropertyInfo>(mappingValues, propertyInfo, targetProp));
            }
            else
            {
                propMappings.Add(propertyInfo.Name, new Tuple<Dictionary<string, object>, PropertyInfo, PropertyInfo>(
                    null, propertyInfo, tTargetProps.ContainsKey(propertyInfo.Name) ? tTargetProps[propertyInfo.Name] : null));
            }
        }

        return propMappings;
    }

    /// <summary>
    /// 获取导出属性映射
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    /// <returns>整理导入对象的 属性名称， 字典数据，原属性信息，目标属性信息 </returns>
    private static Dictionary<string, Tuple<Dictionary<object, string>, PropertyInfo, PropertyInfo>> GetExportPropertMap<TSource, TTarget>() where TTarget : new()
    {
        // 整理导入对象的属性名称，<字典数据，原属性信息，目标属性信息>
        var propMappings = new Dictionary<string, Tuple<Dictionary<object, string>, PropertyInfo, PropertyInfo>>();

        var dictService = App.GetService<SqlSugarRepository<SysDictData>>();
        var targetProps = typeof(TTarget).GetProperties().ToList();
        var sourceProps = typeof(TSource).GetProperties().ToDictionary(m => m.Name);
        foreach (var propertyInfo in targetProps)
        {
            var attrs = propertyInfo.GetCustomAttribute<ImportDictAttribute>();
            if (attrs != null && !string.IsNullOrWhiteSpace(attrs.TypeCode))
            {
                var targetProp = sourceProps[attrs.TargetPropName];
                var mappingValues = dictService.Context.Queryable<SysDictType, SysDictData>((a, b) =>
                    new JoinQueryInfos(JoinType.Inner, a.Id == b.DictTypeId))
                    .Where(a => a.Code == attrs.TypeCode)
                    .Where((a, b) => a.Status == StatusEnum.Enable && b.Status == StatusEnum.Enable)
                    .Select((a, b) => new
                    {
                        Label = b.Value,
                        Value = b.Code
                    }).ToList()
                    .ToDictionary(m => m.Value.ParseTo(targetProp.PropertyType), m => m.Label);
                propMappings.Add(propertyInfo.Name, new Tuple<Dictionary<object, string>, PropertyInfo, PropertyInfo>(mappingValues, targetProp, propertyInfo));
            }
            else
            {
                propMappings.Add(propertyInfo.Name, new Tuple<Dictionary<object, string>, PropertyInfo, PropertyInfo>(
                    null, sourceProps.ContainsKey(propertyInfo.Name) ? sourceProps[propertyInfo.Name] : null, propertyInfo));
            }
        }

        return propMappings;
    }

    /// <summary>
    /// 获取属性映射
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <returns>整理导入对象的 属性名称， 字典数据，原属性信息，目标属性信息 </returns>
    private static Dictionary<string, Tuple<string, string>> GetExportDicttMap<TTarget>() where TTarget : new()
    {
        // 整理导入对象的属性名称，目标属性名，字典Code
        var propMappings = new Dictionary<string, Tuple<string, string>>();
        var tTargetProps = typeof(TTarget).GetProperties();
        foreach (var propertyInfo in tTargetProps)
        {
            var attrs = propertyInfo.GetCustomAttribute<ImportDictAttribute>();
            if (attrs != null && !string.IsNullOrWhiteSpace(attrs.TypeCode))
            {
                propMappings.Add(propertyInfo.Name, new Tuple<string, string>(attrs.TargetPropName, attrs.TypeCode));
            }
        }

        return propMappings;
    }

    /// <summary>
    /// 解析IP地址
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public static (string ipLocation, double? longitude, double? latitude) GetIpAddress(string ip)
    {
        try
        {
            var ipInfo = IpTool.SearchWithI18N(ip); // 国际化查询，默认中文 中文zh-CN、英文en
            var addressList = new List<string>() { ipInfo.Country, ipInfo.Province, ipInfo.City, ipInfo.NetworkOperator };
            return (string.Join(" ", addressList.Where(u => u != "0" && !string.IsNullOrWhiteSpace(u)).ToList()), ipInfo.Longitude, ipInfo.Latitude); // 去掉0及空并用空格连接
        }
        catch
        {
            // 不做处理
        }
        return ("未知", 0, 0);
    }

    /// <summary>
    /// 获取客户端设备信息（操作系统+浏览器）
    /// </summary>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public static string GetClientDeviceInfo(string userAgent)
    {
        try
        {
            if (userAgent != null)
            {
                var client = Parser.GetDefault().Parse(userAgent);
                if (client.Device.IsSpider)
                    return "爬虫";
                return $"{client.OS.Family} {client.OS.Major} {client.OS.Minor}" +
                    $"|{client.UA.Family} {client.UA.Major}.{client.UA.Minor} / {client.Device.Family}";
            }
        }
        catch
        { }
        return "未知";
    }
}