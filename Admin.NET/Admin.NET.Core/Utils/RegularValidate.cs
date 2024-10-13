// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 正则校验
/// </summary>
public static class RegularValidate
{
    /// <summary>
    /// 验证密码规则
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static bool ValidatePassword(string password)
    {
        var regex = new Regex(@"
(?=.*[0-9])                     #必须包含数字
(?=.*[a-z])                     #必须包含小写
(?=.*[A-Z])                     #必须包含大写
(?=([\x21-\x7e]+)[^a-zA-Z0-9])  #必须包含特殊符号
.{8,30}                         #至少8个字符，最多30个字符
", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

        //如果要求必须包含小写、大写字母，则上面的(?=.*[a-zA-Z]) 要改为：
        /*
         * (?=.*[a-z])
         * (?=.*[A-Z])
         */
        return regex.IsMatch(password);
    }
}