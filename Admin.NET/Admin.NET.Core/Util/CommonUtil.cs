// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Magicodes.ExporterAndImporter.Core.Models;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

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
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static async Task<IActionResult> ExportExcelTemplate<T>(string fileName) where T : class, new()
    {
        fileName = $"{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
        IImporter importer = new ExcelImporter();
        var res = await importer.GenerateTemplate<T>(Path.Combine(App.WebHostEnvironment.WebRootPath, fileName));
        return new FileStreamResult(new FileStream(res.FileName, FileMode.Open), "application/octet-stream") { FileDownloadName = fileName };
    }



    /// <summary>
    /// 导出模板Excel
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="fileDto"></param>
    /// <returns></returns>
    public static async Task<IActionResult> ExportExcelTemplate(string fileName, dynamic fileDto)
    {
        MethodInfo generateTemplateMethod = typeof(CommonUtil).GetMethods().FirstOrDefault(p => p.Name == "ExportExcelTemplate" && p.IsGenericMethodDefinition);
        MethodInfo closedGenerateTemplateMethod = generateTemplateMethod.MakeGenericMethod(fileDto.GetType());
        return await (Task<IActionResult>)closedGenerateTemplateMethod.Invoke(null, new object[] { fileName });
    }


    /// <summary>
    /// 导入数据Excel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="file"></param>
    /// <returns></returns>
    public static async Task<ICollection<T>> ImportExcelData<T>([Required] IFormFile file) where T : class, new()
    {
        var newFile = await App.GetRequiredService<SysFileService>().UploadFile(file, "");
        var filePath = Path.Combine(App.WebHostEnvironment.WebRootPath, newFile.FilePath, newFile.Id.ToString() + newFile.Suffix);

        IImporter importer = new ExcelImporter();
        var res = await importer.Import<T>(filePath);
        if (res == null || res.Exception != null)
            throw Oops.Oh("导入异常:" + res.Exception);
        return res.Data;
    }



    /// <summary>
    /// 导入数据Excel
    /// </summary>
    /// <param name="file"></param>
    /// <param name="dataDto"></param>
    /// <returns></returns>
    public static async Task<dynamic> ImportExcelData([Required] IFormFile file, dynamic dataDto)
    {
        MethodInfo importMethod = typeof(CommonUtil).GetMethods().FirstOrDefault(p => p.Name == "ImportExcelData" && p.IsGenericMethodDefinition);
        MethodInfo closedImportMethod = importMethod.MakeGenericMethod(dataDto.GetType());
        var task = (Task)closedImportMethod.Invoke(null, new object[] { file });
        await task;
        return task.GetType().GetProperty("Result").GetValue(task);
    }
}