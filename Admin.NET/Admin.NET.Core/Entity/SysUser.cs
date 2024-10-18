// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 系统用户表
/// </summary>
[SugarTable(null, "系统用户表")]
[SysTable]
[SugarIndex("index_{table}_A", nameof(Account), OrderByType.Asc)]
[SugarIndex("index_{table}_P", nameof(Phone), OrderByType.Asc)]
public partial class SysUser : EntityTenant
{
    /// <summary>
    /// 账号
    /// </summary>
    [SugarColumn(ColumnDescription = "账号", Length = 32)]
    [Required, MaxLength(32)]
    public virtual string Account { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [SugarColumn(ColumnDescription = "密码", Length = 512)]
    [MaxLength(512)]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual string Password { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "真实姓名", Length = 32)]
    [MaxLength(32)]
    public virtual string RealName { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [SugarColumn(ColumnDescription = "昵称", Length = 32)]
    [MaxLength(32)]
    public string? NickName { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    [SugarColumn(ColumnDescription = "头像", Length = 512)]
    [MaxLength(512)]
    public string? Avatar { get; set; }

    /// <summary>
    /// 性别-男_1、女_2
    /// </summary>
    [SugarColumn(ColumnDescription = "性别")]
    public GenderEnum Sex { get; set; } = GenderEnum.Male;

    /// <summary>
    /// 年龄
    /// </summary>
    [SugarColumn(ColumnDescription = "年龄")]
    public int Age { get; set; }

    /// <summary>
    /// 出生日期
    /// </summary>
    [SugarColumn(ColumnDescription = "出生日期")]
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// 民族
    /// </summary>
    [SugarColumn(ColumnDescription = "民族", Length = 32)]
    [MaxLength(32)]
    public string? Nation { get; set; }

    /// <summary>
    /// 手机号码
    /// </summary>
    [SugarColumn(ColumnDescription = "手机号码", Length = 16)]
    [MaxLength(16)]
    public string? Phone { get; set; }

    /// <summary>
    /// 证件类型
    /// </summary>
    [SugarColumn(ColumnDescription = "证件类型")]
    public CardTypeEnum CardType { get; set; }

    /// <summary>
    /// 身份证号
    /// </summary>
    [SugarColumn(ColumnDescription = "身份证号", Length = 32)]
    [MaxLength(32)]
    public string? IdCardNum { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(ColumnDescription = "邮箱", Length = 64)]
    [MaxLength(64)]
    public string? Email { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(ColumnDescription = "地址", Length = 256)]
    [MaxLength(256)]
    public string? Address { get; set; }

    /// <summary>
    /// 文化程度
    /// </summary>
    [SugarColumn(ColumnDescription = "文化程度")]
    public CultureLevelEnum CultureLevel { get; set; }

    /// <summary>
    /// 政治面貌
    /// </summary>
    [SugarColumn(ColumnDescription = "政治面貌", Length = 16)]
    [MaxLength(16)]
    public string? PoliticalOutlook { get; set; }

    /// <summary>
    /// 毕业院校
    /// </summary>COLLEGE
    [SugarColumn(ColumnDescription = "毕业院校", Length = 128)]
    [MaxLength(128)]
    public string? College { get; set; }

    /// <summary>
    /// 办公电话
    /// </summary>
    [SugarColumn(ColumnDescription = "办公电话", Length = 16)]
    [MaxLength(16)]
    public string? OfficePhone { get; set; }

    /// <summary>
    /// 紧急联系人
    /// </summary>
    [SugarColumn(ColumnDescription = "紧急联系人", Length = 32)]
    [MaxLength(32)]
    public string? EmergencyContact { get; set; }

    /// <summary>
    /// 紧急联系人电话
    /// </summary>
    [SugarColumn(ColumnDescription = "紧急联系人电话", Length = 16)]
    [MaxLength(16)]
    public string? EmergencyPhone { get; set; }

    /// <summary>
    /// 紧急联系人地址
    /// </summary>
    [SugarColumn(ColumnDescription = "紧急联系人地址", Length = 256)]
    [MaxLength(256)]
    public string? EmergencyAddress { get; set; }

    /// <summary>
    /// 个人简介
    /// </summary>
    [SugarColumn(ColumnDescription = "个人简介", Length = 512)]
    [MaxLength(512)]
    public string? Introduction { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int OrderNo { get; set; } = 100;

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public StatusEnum Status { get; set; } = StatusEnum.Enable;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 256)]
    [MaxLength(256)]
    public string? Remark { get; set; }

    /// <summary>
    /// 账号类型
    /// </summary>
    [SugarColumn(ColumnDescription = "账号类型")]
    public AccountTypeEnum AccountType { get; set; } = AccountTypeEnum.NormalUser;

    /// <summary>
    /// 直属机构Id
    /// </summary>
    [SugarColumn(ColumnDescription = "直属机构Id")]
    public long OrgId { get; set; }

    /// <summary>
    /// 直属机构
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(OrgId))]
    public SysOrg SysOrg { get; set; }

    /// <summary>
    /// 直属主管Id
    /// </summary>
    [SugarColumn(ColumnDescription = "直属主管Id")]
    public long? ManagerUserId { get; set; }

    /// <summary>
    /// 直属主管
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(ManagerUserId))]
    public SysUser ManagerUser { get; set; }

    /// <summary>
    /// 职位Id
    /// </summary>
    [SugarColumn(ColumnDescription = "职位Id")]
    public long PosId { get; set; }

    /// <summary>
    /// 职位
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Navigate(NavigateType.OneToOne, nameof(PosId))]
    public SysPos SysPos { get; set; }

    /// <summary>
    /// 工号
    /// </summary>
    [SugarColumn(ColumnDescription = "工号", Length = 32)]
    [MaxLength(32)]
    public string? JobNum { get; set; }

    /// <summary>
    /// 职级
    /// </summary>
    [SugarColumn(ColumnDescription = "职级", Length = 32)]
    [MaxLength(32)]
    public string? PosLevel { get; set; }

    /// <summary>
    /// 职称
    /// </summary>
    [SugarColumn(ColumnDescription = "职称", Length = 32)]
    [MaxLength(32)]
    public string? PosTitle { get; set; }

    /// <summary>
    /// 擅长领域
    /// </summary>
    [SugarColumn(ColumnDescription = "擅长领域", Length = 32)]
    [MaxLength(32)]
    public string? Expertise { get; set; }

    /// <summary>
    /// 办公区域
    /// </summary>
    [SugarColumn(ColumnDescription = "办公区域", Length = 32)]
    [MaxLength(32)]
    public string? OfficeZone { get; set; }

    /// <summary>
    /// 办公室
    /// </summary>
    [SugarColumn(ColumnDescription = "办公室", Length = 32)]
    [MaxLength(32)]
    public string? Office { get; set; }

    /// <summary>
    /// 入职日期
    /// </summary>
    [SugarColumn(ColumnDescription = "入职日期")]
    public DateTime? JoinDate { get; set; }

    /// <summary>
    /// 最新登录Ip
    /// </summary>
    [SugarColumn(ColumnDescription = "最新登录Ip", Length = 256)]
    [MaxLength(256)]
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// 最新登录地点
    /// </summary>
    [SugarColumn(ColumnDescription = "最新登录地点", Length = 128)]
    [MaxLength(128)]
    public string? LastLoginAddress { get; set; }

    /// <summary>
    /// 最新登录时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最新登录时间")]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 最新登录设备
    /// </summary>
    [SugarColumn(ColumnDescription = "最新登录设备", Length = 128)]
    [MaxLength(128)]
    public string? LastLoginDevice { get; set; }

    /// <summary>
    /// 电子签名
    /// </summary>
    [SugarColumn(ColumnDescription = "电子签名", Length = 512)]
    [MaxLength(512)]
    public string? Signature { get; set; }

    /// <summary>
    /// 验证超级管理员类型，若账号类型为超级管理员则报错
    /// </summary>
    /// <param name="errorMsg">自定义错误消息</param>
    public void ValidateIsSuperAdminAccountType(ErrorCodeEnum? errorMsg = ErrorCodeEnum.D1014)
    {
        if (AccountType == AccountTypeEnum.SuperAdmin)
        {
            throw Oops.Oh(errorMsg);
        }
    }

    /// <summary>
    /// 验证用户Id是否相同，若用户Id相同则报错
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <param name="errorMsg">自定义错误消息</param>
    public void ValidateIsUserId(long userId, ErrorCodeEnum? errorMsg = ErrorCodeEnum.D1001)
    {
        if (Id == userId)
        {
            throw Oops.Oh(errorMsg);
        }
    }
}