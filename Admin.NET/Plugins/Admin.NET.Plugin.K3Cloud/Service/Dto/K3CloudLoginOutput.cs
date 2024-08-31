// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

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