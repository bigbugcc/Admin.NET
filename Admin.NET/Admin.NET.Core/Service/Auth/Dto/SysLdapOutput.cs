namespace Admin.NET.Core.Service;

/// <summary>
/// 系统域登录信息配置表输出参数
/// </summary>
public class SysLdapOutput
{
    /// <summary>
    /// 主键Id
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// 主机
    /// </summary>
    public string Host { get; set; }
    
    /// <summary>
    /// 端口
    /// </summary>
    public Int16 Port { get; set; }
    
    /// <summary>
    /// 用户搜索基准
    /// </summary>
    public string BaseDn { get; set; }
    
    /// <summary>
    /// 绑定DN
    /// </summary>
    public string BindDn { get; set; }
    
    /// <summary>
    /// 绑定密码
    /// </summary>
    public string BindPass { get; set; }
    
    /// <summary>
    /// 用户过滤规则
    /// </summary>
    public string AuthFilter { get; set; }
    
    /// <summary>
    /// Ldap版本
    /// </summary>
    public Int16 Version { get; set; }
    
    /// <summary>
    /// 状态
    /// </summary>
    public long Status { get; set; }
    
    /// <summary>
    /// 租户Id
    /// </summary>
    public long? TenantId { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
    
    /// <summary>
    /// 创建者Id
    /// </summary>
    public long? CreateUserId { get; set; }
    
    /// <summary>
    /// 创建者姓名
    /// </summary>
    public string? CreateUserName { get; set; }
    
    /// <summary>
    /// 修改者Id
    /// </summary>
    public long? UpdateUserId { get; set; }
    
    /// <summary>
    /// 修改者姓名
    /// </summary>
    public string? UpdateUserName { get; set; }
    
    /// <summary>
    /// 软删除
    /// </summary>
    public bool IsDelete { get; set; }
    
    }
 

