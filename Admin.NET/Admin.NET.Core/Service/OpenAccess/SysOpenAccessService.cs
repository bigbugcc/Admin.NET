// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Security.Claims;
using System.Security.Cryptography;

namespace Admin.NET.Core.Service;

/// <summary>
/// 开放接口身份服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 244)]
public class SysOpenAccessService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysOpenAccess> _sysOpenAccessRep;
    private readonly SysCacheService _sysCacheService;

    /// <summary>
    /// 开放接口身份服务构造函数
    /// </summary>
    public SysOpenAccessService(SqlSugarRepository<SysOpenAccess> sysOpenAccessRep,
        SysCacheService sysCacheService)
    {
        _sysOpenAccessRep = sysOpenAccessRep;
        _sysCacheService = sysCacheService;
    }

    /// <summary>
    /// 获取生成的签名
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取生成的签名")]
    public string GetGenerateSignature([FromQuery] GenerateSignatureInput input)
    {
        // 密钥
        var appSecretByte = Encoding.UTF8.GetBytes(input.AppSecret);
        // 时间戳，精确到秒
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;
        var timestamp = currentTime.ToUnixTimeSeconds();
        // 唯一随机数，可以使用guid或者雪花id，以下是sqlsugar提供获取雪花id的方法
        var nonce = YitIdHelper.NextId();
        //// 请求方式
        //var sMethod = method.ToString();
        // 拼接参数
        var parameter = $"{input.Method}&{input.Url}&{input.AccessKey}&{timestamp}&{nonce}";
        // 使用 HMAC-SHA256 协议创建基于哈希的消息身份验证代码 (HMAC)，以appSecretByte 作为密钥，对上面拼接的参数进行计算签名，所得签名进行 Base-64 编码
        using HMAC hmac = new HMACSHA256();
        hmac.Key = appSecretByte;
        var sign = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(parameter)));
        return sign;
    }

    /// <summary>
    /// 获取开放接口身份分页列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取开放接口身份分页列表")]
    public async Task<SqlSugarPagedList<OpenAccessOutput>> Page(OpenAccessInput input)
    {
        return await _sysOpenAccessRep.AsQueryable()
            .LeftJoin<SysUser>((u, a) => u.BindUserId == a.Id)
            .LeftJoin<SysTenant>((u, a, b) => u.BindTenantId == b.Id)
            .LeftJoin<SysOrg>((u, a, b, c) => b.OrgId == c.Id)
            .WhereIF(!string.IsNullOrWhiteSpace(input.AccessKey?.Trim()), (u, a, b, c) => u.AccessKey.Contains(input.AccessKey))
            .Select((u, a, b, c) => new OpenAccessOutput
            {
                BindUserAccount = a.Account,
                BindTenantName = c.Name,
            }, true)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 增加开放接口身份 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Add"), HttpPost]
    [DisplayName("增加开放接口身份")]
    public async Task AddOpenAccess(AddOpenAccessInput input)
    {
        if (await _sysOpenAccessRep.AsQueryable().AnyAsync(u => u.AccessKey == input.AccessKey && u.Id != input.Id))
            throw Oops.Oh(ErrorCodeEnum.O1000);

        var openAccess = input.Adapt<SysOpenAccess>();
        await _sysOpenAccessRep.InsertAsync(openAccess);
    }

    /// <summary>
    /// 更新开放接口身份 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新开放接口身份")]
    public async Task UpdateOpenAccess(UpdateOpenAccessInput input)
    {
        if (await _sysOpenAccessRep.AsQueryable().AnyAsync(u => u.AccessKey == input.AccessKey && u.Id != input.Id))
            throw Oops.Oh(ErrorCodeEnum.O1000);

        var openAccess = input.Adapt<SysOpenAccess>();
        _sysCacheService.Remove(CacheConst.KeyOpenAccess + openAccess.AccessKey);

        await _sysOpenAccessRep.UpdateAsync(openAccess);
    }

    /// <summary>
    /// 删除开放接口身份 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除开放接口身份")]
    public async Task DeleteOpenAccess(DeleteOpenAccessInput input)
    {
        var openAccess = await _sysOpenAccessRep.GetFirstAsync(u => u.Id == input.Id);
        if (openAccess != null)
            _sysCacheService.Remove(CacheConst.KeyOpenAccess + openAccess.AccessKey);

        await _sysOpenAccessRep.DeleteAsync(u => u.Id == input.Id);
    }

    /// <summary>
    /// 创建密钥 🔖
    /// </summary>
    /// <returns></returns>
    [DisplayName("创建密钥")]
    public async Task<string> CreateSecret()
    {
        return await Task.FromResult(Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..^2]);
    }

    /// <summary>
    /// 根据 Key 获取对象
    /// </summary>
    /// <param name="accessKey"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<SysOpenAccess> GetByKey(string accessKey)
    {
        return await Task.FromResult(
            _sysCacheService.GetOrAdd(CacheConst.KeyOpenAccess + accessKey, _ =>
            {
                return _sysOpenAccessRep.AsQueryable()
                    .Includes(u => u.BindUser)
                    .Includes(u => u.BindUser, p => p.SysOrg)
                    .First(u => u.AccessKey == accessKey);
            })
        );
    }

    /// <summary>
    /// Signature 身份验证事件默认实现
    /// </summary>
    [NonAction]
    public static SignatureAuthenticationEvent GetSignatureAuthenticationEventImpl()
    {
        return new SignatureAuthenticationEvent
        {
            OnGetAccessSecret = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<SysOpenAccessService>>();
                try
                {
                    var openAccessService = context.HttpContext.RequestServices.GetRequiredService<SysOpenAccessService>();
                    var openAccess = openAccessService.GetByKey(context.AccessKey).GetAwaiter().GetResult();
                    return Task.FromResult(openAccess == null ? "" : openAccess.AccessSecret);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "开发接口身份验证");
                    return Task.FromResult("");
                }
            },
            OnValidated = context =>
            {
                var openAccessService = context.HttpContext.RequestServices.GetRequiredService<SysOpenAccessService>();
                var openAccess = openAccessService.GetByKey(context.AccessKey).GetAwaiter().GetResult();
                var identity = ((ClaimsIdentity)context.Principal!.Identity!);

                identity.AddClaims(new[]
                {
                    new Claim(ClaimConst.UserId, openAccess.BindUserId + ""),
                    new Claim(ClaimConst.TenantId, openAccess.BindTenantId + ""),
                    new Claim(ClaimConst.Account, openAccess.BindUser.Account + ""),
                    new Claim(ClaimConst.RealName, openAccess.BindUser.RealName),
                    new Claim(ClaimConst.AccountType, ((int)openAccess.BindUser.AccountType).ToString()),
                    new Claim(ClaimConst.OrgId, openAccess.BindUser.OrgId + ""),
                    new Claim(ClaimConst.OrgName, openAccess.BindUser.SysOrg?.Name + ""),
                    new Claim(ClaimConst.OrgType, openAccess.BindUser.SysOrg?.Type + ""),
                });
                return Task.CompletedTask;
            }
        };
    }
}