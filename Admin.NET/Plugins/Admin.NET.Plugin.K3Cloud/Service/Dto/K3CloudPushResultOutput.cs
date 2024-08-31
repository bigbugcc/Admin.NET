// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Plugin.K3Cloud.Service;

public class K3CloudPushResultOutput
{
    public ErpPushResultInfo Result { get; set; }
}

public class ErpPushResultInfo
{
    /// <summary>
    /// Id
    /// </summary>
    public object? Id { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    public string? Number { get; set; }

    public ErpPushResultInfo_ResponseStatus ResponseStatus { get; set; }
}

public class ErpPushResultInfo_ResponseStatus
{
    public bool IsSuccess { get; set; }
    public int? ErrorCode { get; set; }

    /// <summary>
    /// 错误代码MsgCode说明
    ///0：默认
    ///1：上下文丢失 会话过期
    ///2：没有权限
    ///3：操作标识为空
    ///4：异常
    ///5：单据标识为空
    ///6：数据库操作失败
    ///7：许可错误
    ///8：参数错误
    ///9：指定字段/值不存在
    ///10：未找到对应数据
    ///11：验证失败
    ///12：不可操作
    ///13：网控冲突
    ///14：调用限制
    ///15：禁止管理员登录
    /// </summary>
    public int? MsgCode { get; set; }

    /// <summary>
    /// 如果失败，具体失败原因
    /// </summary>
    public List<ErpPushResultInfo_Errors> Errors { get; set; }
}

public class ErpPushResultInfo_Errors
{
    public string FieldName { get; set; }
    public string Message { get; set; }
    public int DIndex { get; set; }
}