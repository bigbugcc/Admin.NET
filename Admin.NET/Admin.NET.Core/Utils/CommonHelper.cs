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

namespace Admin.NET.Core;

/// <summary>
/// 通用工具类
/// </summary>
public static class CommonHelper
{
    /// <summary>
    /// 根据字符串获取固定整型哈希值
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int GetFixedHashCode(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) return 0;
        unchecked
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;
            for (int i = 0; i < str.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }
            return Math.Abs(hash1 + (hash2 * 1566083941));
        }
    }

    /// <summary>
    /// 将版本号转换为长整型，版本格式要求 x.xx.xxxxxx，比如 v1.2.5=>102000005 V3.10.42=>310000042
    /// </summary>
    /// <param name="version">x.xx.xxxxxx</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static long ConvertVersionToLong(string version)
    {
        // 1. 移除所有字母（不区分大小写）
        string noLetters = Regex.Replace(version, "[a-zA-Z]", "");

        // 2. 按 '.' 分割版本号
        string[] parts = noLetters.Split('.', StringSplitOptions.RemoveEmptyEntries);

        // 3. 确保至少有3部分，不足则补 "0"
        if (parts.Length < 3)
        {
            Array.Resize(ref parts, 3);
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = string.IsNullOrEmpty(parts[i]) ? "0" : parts[i];
            }
        }

        // 4. 格式化各部分：
        //    - part1: 至少1位（直接取）
        //    - part2: 补齐到2位（如 "2" → "02"）
        //    - part3: 补齐到6位（如 "5" → "000005"）
        string part1 = parts[0];
        string part2 = parts[1].PadLeft(2, '0');
        string part3 = parts[2].PadLeft(6, '0');

        // 5. 拼接所有部分并转换为 long
        string combined = $"{part1}{part2}{part3}";
        if (long.TryParse(combined, out long result))
        {
            return result;
        }
        else
        {
            throw new ArgumentException("版本号转换失败，结果超出 long 范围。");
        }
    }

    /// <summary>
    /// 生成百分数
    /// </summary>
    /// <param name="passCount"></param>
    /// <param name="allCount"></param>
    /// <returns></returns>
    public static string ExecPercent(decimal passCount, decimal allCount)
    {
        string res = "";
        if (allCount > 0)
        {
            var value = (double)Math.Round(passCount / allCount * 100, 1);
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
        if (App.HttpContext.Request.Headers.TryGetValue("Origin", out Microsoft.Extensions.Primitives.StringValues value1)) // 配置成完整的路径如（结尾不要带"/"）,比如 https://www.abc.com
            result = $"{value1}";
        else if (App.HttpContext.Request.Headers.TryGetValue("X-Original", out Microsoft.Extensions.Primitives.StringValues value2)) // 配置成完整的路径如（结尾不要带"/"）,比如 https://www.abc.com
            result = $"{value2}";
        else if (App.HttpContext.Request.Headers.TryGetValue("X-Original-Host", out Microsoft.Extensions.Primitives.StringValues value3))
            result = $"{App.HttpContext.Request.Scheme}://{value3}";
        return result;
    }

    /// <summary>
    /// 获取请求地址源
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static string GetOrigin(this HttpRequest request)
    {
        string scheme = request.Scheme;
        string host = request.Host.Host;
        int port = request.Host.Port ?? (-1);

        string url = $"{scheme}://{host}";
        if (port != 80 && port != 443 && port != -1) url += $":{port}";

        return url;
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
        var res = await new ExcelImporter().GenerateTemplateBytes<T>();
        return new FileContentResult(res, "application/octet-stream") { FileDownloadName = $"{(string.IsNullOrEmpty(fileName) ? typeof(T).Name : fileName)}.xlsx" };
    }

    /// <summary>
    /// 导出数据excel
    /// </summary>
    /// <returns></returns>
    public static async Task<IActionResult> ExportExcelData<T>(ICollection<T> data, string fileName = null) where T : class, new()
    {
        var res = await new ExcelExporter().ExportAsByteArray<T>(data);
        return new FileContentResult(res, "application/octet-stream") { FileDownloadName = $"{(string.IsNullOrEmpty(fileName) ? typeof(T).Name : fileName)}.xlsx" };
    }

    /// <summary>
    /// 导出数据excel,包括字典转换
    /// </summary>
    /// <returns></returns>
    public static async Task<IActionResult> ExportExcelData<TSource, TTarget>(ISugarQueryable<TSource> query, Func<TSource, TTarget, TTarget> action = null)
        where TSource : class, new() where TTarget : class, new()
    {
        var propMappings = GetExportPropertMap<TSource, TTarget>();
        var data = query.ToList();
        // 相同属性复制值，字典值转换
        var result = new List<TTarget>();
        foreach (var item in data)
        {
            var newData = new TTarget();
            foreach (var dict in propMappings)
            {
                var targetProp = dict.Value.Item3;
                if (targetProp != null)
                {
                    var propertyInfo = dict.Value.Item2;
                    var sourceVal = propertyInfo.GetValue(item, null);
                    if (sourceVal == null)
                    {
                        continue;
                    }

                    var map = dict.Value.Item1;
                    if (map != null && map.TryGetValue(sourceVal, out string newVal1))
                    {
                        targetProp.SetValue(newData, newVal1);
                    }
                    else
                    {
                        if (targetProp.PropertyType.FullName == propertyInfo.PropertyType.FullName)
                        {
                            targetProp.SetValue(newData, sourceVal);
                        }
                        else
                        {
                            var newVal = sourceVal.ToString().ParseTo(targetProp.PropertyType);
                            targetProp.SetValue(newData, newVal);
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

        var res = await new ExcelExporter().ExportAsByteArray(result);
        return new FileContentResult(res, "application/octet-stream") { FileDownloadName = typeof(TTarget).Name + ".xlsx" };
    }

    /// <summary>
    /// 导入数据Excel
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static async Task<ICollection<T>> ImportExcelData<T>([Required] IFormFile file) where T : class, new()
    {
        var res = await new ExcelImporter().Import<T>(file.OpenReadStream());
        if (!res.HasError) return res.Data;

        var message = string.Empty;
        if (res.Exception != null)
            message += $"\r\n{res.Exception.Message}";
        foreach (DataRowErrorInfo drErrorInfo in res.RowErrors)
        {
            int rowNum = drErrorInfo.RowIndex;
            foreach (var item in drErrorInfo.FieldErrors)
                message += $"\r\n{item.Key}：{item.Value}（文件第{drErrorInfo.RowIndex}行）";
        }
        message += "\r\n字段缺失：" + string.Join("，", res.TemplateErrors.Select(m => m.RequireColumnName).ToList());
        throw Oops.Oh("导入异常:" + message);
    }

    /// <summary>
    /// 导入Excel数据并错误标记
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="file"></param>
    /// <param name="importResultCallback"></param>
    /// <returns></returns>
    public static async Task<ICollection<T>> ImportExcelData<T>([Required] IFormFile file, Func<ImportResult<T>, ImportResult<T>> importResultCallback = null) where T : class, new()
    {
        var resultStream = new MemoryStream();
        var res = await new ExcelImporter().Import<T>(file.OpenReadStream(), resultStream, importResultCallback);
        resultStream.Seek(0, SeekOrigin.Begin);
        if (!res.HasError) return res.Data;

        var message = string.Empty;
        if (res.Exception != null)
            message += $"\r\n{res.Exception.Message}";

        var userId = App.User?.FindFirst(ClaimConst.UserId)?.Value;
        var sysCacheService = App.GetRequiredService<SysCacheService>();
        sysCacheService.Remove(CacheConst.KeyExcelTemp + userId);
        sysCacheService.Set(CacheConst.KeyExcelTemp + userId, resultStream, TimeSpan.FromMinutes(5));

        foreach (DataRowErrorInfo drErrorInfo in res.RowErrors)
        {
            message = drErrorInfo.FieldErrors.Aggregate(message, (current, item) => current + $"\r\n{item.Key}：{item.Value}（文件第{drErrorInfo.RowIndex}行）");
        }
        if (res.TemplateErrors.Count > 0)
            message += "\r\n字段缺失：" + string.Join("，", res.TemplateErrors.Select(m => m.RequireColumnName).ToList());

        if (message.Length > 200)
            message = string.Concat(message.AsSpan(0, 200), "...\r\n异常过多，建议下载错误标记文件查看详细错误信息并重新导入。");
        throw Oops.Oh("导入异常:" + message);
    }

    /// <summary>
    /// 导入数据Excel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="file"></param>
    /// <returns></returns>
    public static async Task<List<T>> ImportExcelDataAsync<T>([Required] IFormFile file) where T : class, new()
    {
        using MemoryStream stream = new();
        await file.CopyToAsync(stream);

        var res = await ((IImporter)new ExcelImporter()).Import<T>(stream);
        if (res == null)
            throw Oops.Oh("导入数据为空");
        if (res.Exception != null)
            throw Oops.Oh("导入异常:" + res.Exception);
        if (res.TemplateErrors?.Count > 0)
            throw Oops.Oh("模板异常:" + res.TemplateErrors.Select(x => $"[{x.RequireColumnName}]{x.Message}").Join("\n"));

        return [.. res.Data];
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

        var dictService = App.GetRequiredService<SqlSugarRepository<SysDictData>>();
        var tSourceProps = typeof(TSource).GetProperties().ToList();
        var tTargetProps = typeof(TTarget).GetProperties().ToDictionary(u => u.Name);
        foreach (var propertyInfo in tSourceProps)
        {
            var attrs = propertyInfo.GetCustomAttribute<ImportDictAttribute>();
            if (attrs != null && !string.IsNullOrWhiteSpace(attrs.TypeCode))
            {
                var targetProp = tTargetProps[attrs.TargetPropName];
                var mappingValues = dictService.Context.Queryable<SysDictType, SysDictData>((u, a) =>
                    new JoinQueryInfos(JoinType.Inner, u.Id == a.DictTypeId))
                    .Where(u => u.Code == attrs.TypeCode)
                    .Where((u, a) => u.Status == StatusEnum.Enable && a.Status == StatusEnum.Enable)
                    .Select((u, a) => new
                    {
                        a.Label,
                        a.Value
                    }).ToList()
                    .ToDictionary(u => u.Label, u => u.Value.ParseTo(targetProp.PropertyType));
                propMappings.Add(propertyInfo.Name, new Tuple<Dictionary<string, object>, PropertyInfo, PropertyInfo>(mappingValues, propertyInfo, targetProp));
            }
            else
            {
                propMappings.Add(propertyInfo.Name, new Tuple<Dictionary<string, object>, PropertyInfo, PropertyInfo>(
                    null, propertyInfo, tTargetProps.TryGetValue(propertyInfo.Name, out PropertyInfo value) ? value : null));
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

        var dictService = App.GetRequiredService<SqlSugarRepository<SysDictData>>();
        var targetProps = typeof(TTarget).GetProperties().ToList();
        var sourceProps = typeof(TSource).GetProperties().ToDictionary(u => u.Name);
        foreach (var propertyInfo in targetProps)
        {
            var attrs = propertyInfo.GetCustomAttribute<ImportDictAttribute>();
            if (attrs != null && !string.IsNullOrWhiteSpace(attrs.TypeCode))
            {
                var targetProp = sourceProps[attrs.TargetPropName];
                var mappingValues = dictService.Context.Queryable<SysDictType, SysDictData>((u, a) =>
                    new JoinQueryInfos(JoinType.Inner, u.Id == a.DictTypeId))
                    .Where(u => u.Code == attrs.TypeCode)
                    .Where((u, a) => u.Status == StatusEnum.Enable && a.Status == StatusEnum.Enable)
                    .Select((u, a) => new
                    {
                        a.Label,
                        a.Value
                    }).ToList()
                    .ToDictionary(u => u.Value.ParseTo(targetProp.PropertyType), u => u.Label);
                propMappings.Add(propertyInfo.Name, new Tuple<Dictionary<object, string>, PropertyInfo, PropertyInfo>(mappingValues, targetProp, propertyInfo));
            }
            else
            {
                propMappings.Add(propertyInfo.Name, new Tuple<Dictionary<object, string>, PropertyInfo, PropertyInfo>(
                    null, sourceProps.TryGetValue(propertyInfo.Name, out PropertyInfo prop) ? prop : null, propertyInfo));
            }
        }

        return propMappings;
    }

    /// <summary>
    /// 获取属性映射
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <returns>整理导入对象的 属性名称， 字典数据，原属性信息，目标属性信息 </returns>
    private static Dictionary<string, Tuple<string, string>> GetExportDictMap<TTarget>() where TTarget : new()
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
            var location = string.Join(" ", addressList.Where(u => u != "0" && !string.IsNullOrWhiteSpace(u)).ToList()); // 去掉0及空并用空格连接
            if (string.IsNullOrWhiteSpace(location)) location = "未知";
            return (location, ipInfo.Longitude, ipInfo.Latitude);
        }
        catch
        {
            // 不做处理
        }
        return ("未知", 0, 0);
    }

    /// <summary>
    /// 获取客户端设备信息（操作系统和浏览器）
    /// </summary>
    /// <param name="userAgent">User-Agent字符串</param>
    /// <returns>(操作系统, 浏览器)</returns>
    public static (string os, string browser) GetClientDeviceInfo(string userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent)) return ("Unknown", "Unknown");

        try
        {
            var client = Parser.GetDefault().Parse(userAgent);

            // 爬虫检测
            if (client.Device.IsSpider) return ("Spider", "Spider");

            // 获取操作系统信息
            var os = $"{client.OS.Family} {client.OS.Major} {client.OS.Minor}".Trim();
            // 获取浏览器信息
            var browser = $"{client.UA.Family} {client.UA.Major}.{client.UA.Minor} / {client.Device.Family}".Trim();
            return (os, browser);
        }
        catch
        {
            return ("Unknown", "Unknown");
        }
    }

    /// <summary>
    /// 从日志消息中获取用户信息
    /// </summary>
    /// <param name="loggingMonitor"></param>
    /// <returns></returns>
    public static LoggingUserInfo GetUserInfo(LoggingMonitorDto loggingMonitor)
    {
        LoggingUserInfo result = new();

        // 从授权声明中获取用户信息
        if (loggingMonitor.AuthorizationClaims != null)
        {
            var authDict = loggingMonitor.AuthorizationClaims.ToDictionary(u => u.Type, u => u.Value);
            result.UserId = long.TryParse(authDict?.GetValueOrDefault(ClaimConst.UserId) ?? "", out var userId) ? userId : null;
            result.Account = authDict?.GetValueOrDefault(ClaimConst.Account);
            result.RealName = authDict?.GetValueOrDefault(ClaimConst.RealName);
            result.TenantId = long.TryParse(authDict?.GetValueOrDefault(ClaimConst.TenantId) ?? "", out var tenantId) ? tenantId : null;
        }

        // 登录时从请求参数获取用户信息
        if (result.UserId == null && loggingMonitor.ActionName == "login" && loggingMonitor.Parameters?.FirstOrDefault()?.Value is JObject jObject)
        {
            result.Account = jObject.GetValue("account")?.ToString();
            if (string.IsNullOrEmpty(result.Account)) return result;

            var db = SqlSugarSetup.ITenant.GetConnectionScope(SqlSugarConst.MainConfigId);
            var user = db.Queryable<SysUser>().First(u => u.Account == result.Account);
            if (user != null)
            {
                result.UserId = user.Id;
                result.RealName = user.RealName;
                result.TenantId = user.TenantId;
            }
        }
        return result;
    }

    /// <summary>
    /// 判断是否为移动端UA
    /// </summary>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public static bool IsMobile(string userAgent)
    {
        var mobilePatterns = new[] { "android.*mobile", "iphone", "ipod", "windows phone", "blackberry", "nokia", "mobile", "opera mini", "opera mobi", "palm", "webos", "bb\\d+", "meego" };
        return mobilePatterns.Any(pattern => Regex.IsMatch(userAgent ?? "", pattern, RegexOptions.IgnoreCase));
    }

    /// <summary>
    /// 获取对象属性变更的字典集合
    /// </summary>
    /// <param name="oldEntity">旧实体</param>
    /// <param name="newEntity">新实体</param>
    /// <param name="ignoreNull">忽略空值</param>
    /// <returns></returns>
    public static (Dictionary<string, object> oldValues, Dictionary<string, object> newValues) GetChangedDictionary<T>(T oldEntity, T newEntity, bool ignoreNull = false) where T : class
    {
        var newValues = new Dictionary<string, object>();
        var oldValues = new Dictionary<string, object>();
        var properties = typeof(T).GetProperties();
        foreach (var prop in properties)
        {
            var oldValue = prop.GetValue(oldEntity);
            var newValue = prop.GetValue(newEntity);
            if (newValue == null && ignoreNull) continue;

            // 排除通用字段
            if (CodeGenHelper.IsCommonColumn(prop.Name)) continue;

            // 排除引用类型
            if (prop.PropertyType.IsClass) continue;

            if (!Equals(oldValue, newValue))
            {
                oldValues[prop.Name] = oldValue;
                newValues[prop.Name] = newValue;
            }
        }
        return (oldValues, newValues);
    }
}