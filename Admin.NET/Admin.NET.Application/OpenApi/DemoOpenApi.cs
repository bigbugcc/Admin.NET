// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

namespace Admin.NET.Application;

///// <summary>
///// 示例开放接口
///// </summary>
//[ApiDescriptionSettings("开放接口", Name = "Demo", Order = 100)]
//[Authorize(AuthenticationSchemes = SignatureAuthenticationDefaults.AuthenticationScheme)]
//public class DemoOpenApi : IDynamicApiController
//{
//    private readonly UserManager _userManager;

//    public DemoOpenApi(UserManager userManager)
//    {
//        _userManager = userManager;
//    }

//    [HttpGet("helloWord")]
//    public Task<string> HelloWord()
//    {
//        return Task.FromResult($"Hello word. {_userManager.Account}");
//    }
//}