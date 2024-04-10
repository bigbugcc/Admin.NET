namespace Admin.NET.Core.Service;

/// <summary>
/// 系统域登录信息配置表基础输入参数
/// </summary>
public class SysLdapBaseInput
    {
        /// <summary>
        /// 主机
        /// </summary>
        public virtual string Host { get; set; }
        
        /// <summary>
        /// 端口
        /// </summary>
        public virtual Int16 Port { get; set; }
        
        /// <summary>
        /// 用户搜索基准
        /// </summary>
        public virtual string BaseDn { get; set; }
        
        /// <summary>
        /// 绑定DN
        /// </summary>
        public virtual string BindDn { get; set; }
        
        /// <summary>
        /// 绑定密码
        /// </summary>
        public virtual string BindPass { get; set; }
        
        /// <summary>
        /// 用户过滤规则
        /// </summary>
        public virtual string AuthFilter { get; set; }
        
        /// <summary>
        /// Ldap版本
        /// </summary>
        public virtual Int16 Version { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        public virtual long Status { get; set; }
        
        /// <summary>
        /// 租户Id
        /// </summary>
        public virtual long? TenantId { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime? UpdateTime { get; set; }
        
        /// <summary>
        /// 创建者Id
        /// </summary>
        public virtual long? CreateUserId { get; set; }
        
        /// <summary>
        /// 创建者姓名
        /// </summary>
        public virtual string? CreateUserName { get; set; }
        
        /// <summary>
        /// 修改者Id
        /// </summary>
        public virtual long? UpdateUserId { get; set; }
        
        /// <summary>
        /// 修改者姓名
        /// </summary>
        public virtual string? UpdateUserName { get; set; }
        
        /// <summary>
        /// 软删除
        /// </summary>
        public virtual bool IsDelete { get; set; }
        
    }

    /// <summary>
    /// 系统域登录信息配置表分页查询输入参数
    /// </summary>
    public class SysLdapInput : BasePageInput
    {
        /// <summary>
        /// 关键字查询
        /// </summary>
        public string? SearchKey { get; set; }

        /// <summary>
        /// 主机
        /// </summary>
        public string? Host { get; set; }
        
    }

    /// <summary>
    /// 系统域登录信息配置表增加输入参数
    /// </summary>
    public class AddSysLdapInput : SysLdapBaseInput
    {
        /// <summary>
        /// 主机
        /// </summary>
        [Required(ErrorMessage = "主机不能为空")]
        public override string Host { get; set; }
        
        /// <summary>
        /// 端口
        /// </summary>
        [Required(ErrorMessage = "端口不能为空")]
        public override Int16 Port { get; set; }
        
        /// <summary>
        /// 用户搜索基准
        /// </summary>
        [Required(ErrorMessage = "用户搜索基准不能为空")]
        public override string BaseDn { get; set; }
        
        /// <summary>
        /// 绑定DN
        /// </summary>
        [Required(ErrorMessage = "绑定DN不能为空")]
        public override string BindDn { get; set; }
        
        /// <summary>
        /// 绑定密码
        /// </summary>
        [Required(ErrorMessage = "绑定密码不能为空")]
        public override string BindPass { get; set; }
        
        /// <summary>
        /// 用户过滤规则
        /// </summary>
        [Required(ErrorMessage = "用户过滤规则不能为空")]
        public override string AuthFilter { get; set; }
        
        /// <summary>
        /// Ldap版本
        /// </summary>
        [Required(ErrorMessage = "Ldap版本不能为空")]
        public override Int16 Version { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        [Required(ErrorMessage = "状态不能为空")]
        public override long Status { get; set; }
        
        /// <summary>
        /// 软删除
        /// </summary>
        [Required(ErrorMessage = "软删除不能为空")]
        public override bool IsDelete { get; set; }
        
    }

    /// <summary>
    /// 系统域登录信息配置表删除输入参数
    /// </summary>
    public class DeleteSysLdapInput : BaseIdInput
    {
    }

    /// <summary>
    /// 系统域登录信息配置表更新输入参数
    /// </summary>
    public class UpdateSysLdapInput : SysLdapBaseInput
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Required(ErrorMessage = "主键Id不能为空")]
        public long Id { get; set; }
        
    }

    /// <summary>
    /// 系统域登录信息配置表主键查询输入参数
    /// </summary>
    public class QueryByIdSysLdapInput : DeleteSysLdapInput
    {

    }
