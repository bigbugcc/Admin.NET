namespace Admin.NET.Plugin.K3Cloud.Service;
public class K3CloudLoginOutput
{
    public string Message { get; set; }
    public string MessageCode { get; set; }
    public ErpLoginResultType LoginResultType { get; set; }
}


public enum ErpLoginResultType
{
    /// <summary>
    /// 激活
    /// </summary>
    Activation = -7,
    /// <summary>
    /// 云通行证未绑定Cloud账号
    /// </summary>
    EntryCloudUnBind = -6,
    /// <summary>
    /// 需要表单处理
    /// </summary>
    DealWithForm = -5,
    /// <summary>
    /// 登录警告
    /// </summary>
    Wanning = -4,
    /// <summary>
    /// 密码验证不通过（强制的）
    /// </summary>
    PWInvalid_Required = -3,
    /// <summary>
    /// 密码验证不通过（可选的）
    /// </summary>
    PWInvalid_Optional = -2,
    /// <summary>
    /// 登录失败
    /// </summary>
    Failure = -1,
    /// <summary>
    /// 用户或密码错误
    /// </summary>
    PWError = 0,
    /// <summary>
    /// 登录成功
    /// </summary>
    Success = 1
}