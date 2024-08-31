namespace Admin.NET.Plugin.K3Cloud.Service;
/// <summary>
/// ERP基础入参
/// </summary>
public class K3CloudBaeInput<T>
{
    /// <summary>
    /// 表单Id
    /// </summary>
    public string formid { get; set; }
    /// <summary>
    /// 数据包
    /// </summary>
    public T data { get; set; }

}