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

