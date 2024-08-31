// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 微信小程序服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 240)]
public class SysWxOpenService : IDynamicApiController, ITransient
{
    private readonly SqlSugarRepository<SysWechatUser> _sysWechatUserRep;
    private readonly SysConfigService _sysConfigService;
    private readonly WechatApiClient _wechatApiClient;
    private readonly SysFileService _sysFileService;


    public SysWxOpenService(SqlSugarRepository<SysWechatUser> sysWechatUserRep,
        SysConfigService sysConfigService,
        WechatApiClientFactory wechatApiClientFactory,
        SysFileService sysFileService)
    {
        _sysWechatUserRep = sysWechatUserRep;
        _sysConfigService = sysConfigService;
        _wechatApiClient = wechatApiClientFactory.CreateWxOpenClient();
        _sysFileService = sysFileService;

    }

    /// <summary>
    /// 获取微信用户OpenId 🔖
    /// </summary>
    /// <param name="input"></param>
    [AllowAnonymous]
    [DisplayName("获取微信用户OpenId")]
    public async Task<WxOpenIdOutput> GetWxOpenId([FromQuery] JsCode2SessionInput input)
    {
        var reqJsCode2Session = new SnsJsCode2SessionRequest()
        {
            JsCode = input.JsCode,
        };
        var resCode2Session = await _wechatApiClient.ExecuteSnsJsCode2SessionAsync(reqJsCode2Session);
        if (resCode2Session.ErrorCode != (int)WechatReturnCodeEnum.请求成功)
            throw Oops.Oh(resCode2Session.ErrorMessage + " " + resCode2Session.ErrorCode);

        var wxUser = await _sysWechatUserRep.GetFirstAsync(p => p.OpenId == resCode2Session.OpenId);
        if (wxUser == null)
        {
            wxUser = new SysWechatUser
            {
                OpenId = resCode2Session.OpenId,
                UnionId = resCode2Session.UnionId,
                SessionKey = resCode2Session.SessionKey,
                PlatformType = PlatformTypeEnum.微信小程序
            };
            wxUser = await _sysWechatUserRep.AsInsertable(wxUser).ExecuteReturnEntityAsync();
        }
        else
        {
            await _sysWechatUserRep.AsUpdateable(wxUser).IgnoreColumns(true).ExecuteCommandAsync();
        }

        return new WxOpenIdOutput
        {
            OpenId = resCode2Session.OpenId
        };
    }

    /// <summary>
    /// 获取微信用户电话号码 🔖
    /// </summary>
    /// <param name="input"></param>
    [AllowAnonymous]
    [DisplayName("获取微信用户电话号码")]
    public async Task<WxPhoneOutput> GetWxPhone([FromQuery] WxPhoneInput input)
    {
        var accessToken = await GetCgibinToken();
        var reqUserPhoneNumber = new WxaBusinessGetUserPhoneNumberRequest()
        {
            Code = input.Code,
            AccessToken = accessToken,
        };
        var resUserPhoneNumber = await _wechatApiClient.ExecuteWxaBusinessGetUserPhoneNumberAsync(reqUserPhoneNumber);
        if (resUserPhoneNumber.ErrorCode != (int)WechatReturnCodeEnum.请求成功)
            throw Oops.Oh(resUserPhoneNumber.ErrorMessage + " " + resUserPhoneNumber.ErrorCode);

        var wxUser = await _sysWechatUserRep.GetFirstAsync(p => p.OpenId == input.OpenId);
        if (wxUser == null)
        {
            wxUser = new SysWechatUser
            {
                OpenId = input.OpenId,
                Mobile = resUserPhoneNumber.PhoneInfo?.PhoneNumber,
                PlatformType = PlatformTypeEnum.微信小程序
            };
            wxUser = await _sysWechatUserRep.AsInsertable(wxUser).ExecuteReturnEntityAsync();
        }
        else
        {
            wxUser.Mobile = resUserPhoneNumber.PhoneInfo?.PhoneNumber;
            await _sysWechatUserRep.AsUpdateable(wxUser).IgnoreColumns(true).ExecuteCommandAsync();
        }

        return new WxPhoneOutput
        {
            PhoneNumber = resUserPhoneNumber.PhoneInfo?.PhoneNumber
        };
    }

    /// <summary>
    /// 微信小程序登录OpenId 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("微信小程序登录OpenId")]
    public async Task<dynamic> WxOpenIdLogin(WxOpenIdLoginInput input)
    {
        var wxUser = await _sysWechatUserRep.GetFirstAsync(p => p.OpenId == input.OpenId);
        if (wxUser == null)
            throw Oops.Oh("微信小程序登录失败");

        var tokenExpire = await _sysConfigService.GetTokenExpire();
        return new
        {
            wxUser.Avatar,
            accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>
            {
                { ClaimConst.UserId, wxUser.Id },
                { ClaimConst.RealName, wxUser.NickName },
                { ClaimConst.LoginMode, LoginModeEnum.APP },
            }, tokenExpire)
        };
    }

    /// <summary>
    /// 上传小程序头像
    /// </summary> 
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("上传小程序头像")]
    public async Task<SysFile> UploadAvatar([FromForm] UploadAvatarInput input)
    {
        var wxUser = await _sysWechatUserRep.GetFirstAsync(p => p.OpenId == input.OpenId);
        if (wxUser == null)
            throw Oops.Oh("未找到用户上传失败");

        var res = await _sysFileService.UploadFile(new FileUploadInput { File = input.File, FileType = input.FileType, Path = input.Path });
        wxUser.Avatar = res.Url;
        await _sysWechatUserRep.AsUpdateable(wxUser).IgnoreColumns(true).ExecuteCommandAsync();

        return res;
    }

    /// <summary>
    /// 设置小程序用户昵称
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    public async Task SetNickName(SetNickNameInput input)
    {
        var wxUser = await _sysWechatUserRep.GetFirstAsync(p => p.OpenId == input.OpenId);
        if (wxUser == null)
            throw Oops.Oh("未找到用户信息设置失败");
        wxUser.NickName = input.NickName;
        await _sysWechatUserRep.AsUpdateable(wxUser).IgnoreColumns(true).ExecuteCommandAsync();
        return;
    }

    /// <summary>
    /// 获取小程序用户信息
    /// </summary>
    /// <param name="openid"></param>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<dynamic> GetUserInfo(string openid)
    {
        var wxUser = await _sysWechatUserRep.GetFirstAsync(p => p.OpenId == openid);
        if (wxUser == null)
            throw Oops.Oh("未找到用户信息获取失败");
        return new { nickName = wxUser.NickName, avator = wxUser.Avatar };
    }



    /// <summary>
    /// 获取订阅消息模板列表 🔖
    /// </summary>
    [DisplayName("获取订阅消息模板列表")]
    public async Task<dynamic> GetMessageTemplateList()
    {
        var accessToken = await GetCgibinToken();
        var reqTemplate = new WxaApiNewTemplateGetTemplateRequest()
        {
            AccessToken = accessToken
        };
        var resTemplate = await _wechatApiClient.ExecuteWxaApiNewTemplateGetTemplateAsync(reqTemplate);
        if (resTemplate.ErrorCode != (int)WechatReturnCodeEnum.请求成功)
            throw Oops.Oh(resTemplate.ErrorMessage + " " + resTemplate.ErrorCode);

        return resTemplate.TemplateList;
    }

    /// <summary>
    /// 发送订阅消息 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("发送订阅消息")]
    public async Task<dynamic> SendSubscribeMessage(SendSubscribeMessageInput input)
    {
        var accessToken = await GetCgibinToken();
        var reqMessage = new CgibinMessageSubscribeSendRequest()
        {
            AccessToken = accessToken,
            TemplateId = input.TemplateId,
            ToUserOpenId = input.ToUserOpenId,
            Data = input.Data,
            MiniProgramState = input.MiniprogramState,
            Language = input.Language,
            MiniProgramPagePath = input.MiniProgramPagePath
        };
        var resMessage = await _wechatApiClient.ExecuteCgibinMessageSubscribeSendAsync(reqMessage);
        return resMessage;
    }

    /// <summary>
    /// 增加订阅消息模板 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "AddSubscribeMessageTemplate"), HttpPost]
    [DisplayName("增加订阅消息模板")]
    public async Task<dynamic> AddSubscribeMessageTemplate(AddSubscribeMessageTemplateInput input)
    {
        var accessToken = await GetCgibinToken();
        var reqMessage = new WxaApiNewTemplateAddTemplateRequest()
        {
            AccessToken = accessToken,
            TemplateTitleId = input.TemplateTitleId,
            KeyworkIdList = input.KeyworkIdList,
            SceneDescription = input.SceneDescription
        };
        var resTemplate = await _wechatApiClient.ExecuteWxaApiNewTemplateAddTemplateAsync(reqMessage);
        return resTemplate;
    }

    /// <summary>
    /// 生成二维码
    /// </summary>
    /// <param name="input"> 扫码进入的小程序页面路径，最大长度 128 个字符，不能为空； eg: pages / index ? id = AY000001 </param>
    /// <returns></returns>
    [DisplayName("生成小程序二维码")]
    [ApiDescriptionSettings(Name = "GenerateQRImage")]
    public async Task<GenerateQRImageOutput> GenerateQRImageAsync(GenerateQRImageInput input)
    {
        GenerateQRImageOutput generateQRImageOutInput = new GenerateQRImageOutput();
        if (input.PagePath.IsNullOrEmpty())
        {
            generateQRImageOutInput.Success = false;
            generateQRImageOutInput.ImgPath = "";
            generateQRImageOutInput.Message = $"生成失败 页面路径不能为空";
            return generateQRImageOutInput;
        }

        if (input.ImageName.IsNullOrEmpty())
        {
            input.ImageName = DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        var accessToken = await GetCgibinToken();
        var request = new CgibinWxaappCreateWxaQrcodeRequest
        {
            AccessToken = accessToken,
            Path = input.PagePath,
            Width = input.Width
        };
        var response = await _wechatApiClient.ExecuteCgibinWxaappCreateWxaQrcodeAsync(request);

        if (response.IsSuccessful())
        {
            var QRImagePath = App.GetConfig<string>("Wechat:QRImagePath");
            //判断文件存放路径是否存在
            if (!Directory.Exists(QRImagePath))
            {
                Directory.CreateDirectory(QRImagePath);
            }
            // 将二维码图片数据保存为文件
            var filePath = QRImagePath + $"\\{input.ImageName.ToUpper()}.png";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.WriteAllBytes(filePath, response.GetRawBytes());

            generateQRImageOutInput.Success = true;
            generateQRImageOutInput.ImgPath = filePath;
            generateQRImageOutInput.Message = "生成成功";
        }
        else
        {
            // 处理错误情况
            generateQRImageOutInput.Success = false;
            generateQRImageOutInput.ImgPath = "";
            generateQRImageOutInput.Message = $"生成失败 错误代码：{response.ErrorCode}  错误描述：{response.ErrorMessage}";
        }
        return generateQRImageOutInput;
    }

    /// <summary>
    /// 获取Access_token
    /// </summary>
    private async Task<string> GetCgibinToken()
    {
        var reqCgibinToken = new CgibinTokenRequest();
        var resCgibinToken = await _wechatApiClient.ExecuteCgibinTokenAsync(reqCgibinToken);
        if (resCgibinToken.ErrorCode != (int)WechatReturnCodeEnum.请求成功)
            throw Oops.Oh(resCgibinToken.ErrorMessage + " " + resCgibinToken.ErrorCode);
        return resCgibinToken.AccessToken;
    }
}