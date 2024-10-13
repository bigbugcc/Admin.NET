// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Security.Cryptography;

namespace Admin.NET.Core;

/// <summary>
/// 3DES文件加解密
/// </summary>
public static class TripleDES
{
    /// <summary>
    /// 加密文件
    /// </summary>
    /// <param name="inputFile">待加密文件路径</param>
    /// <param name="outputFile">加密后的文件路径</param>
    /// <param name="password">密码 （24位长度）</param>
    [Obsolete]
    public static void EncryptFile(string inputFile, string outputFile, string password)
    {
        using var tdes = new TripleDESCryptoServiceProvider();
        tdes.Mode = CipherMode.ECB;
        tdes.Padding = PaddingMode.PKCS7;
        tdes.Key = Encoding.UTF8.GetBytes(password);
        using var inputFileStream = new FileStream(inputFile, FileMode.Open);
        using var encryptedFileStream = new FileStream(outputFile, FileMode.Create);
        using var cryptoStream = new CryptoStream(encryptedFileStream, tdes.CreateEncryptor(), CryptoStreamMode.Write);
        inputFileStream.CopyTo(cryptoStream);
    }

    /// <summary>
    /// 加密文件
    /// </summary>
    /// <param name="inputFile">加密的文件路径</param>
    /// <param name="outputFile">解密后的文件路径</param>
    /// <param name="password">密码 （24位长度）</param>
    [Obsolete]
    public static void DecryptFile(string inputFile, string outputFile, string password)
    {
        using var tdes = new TripleDESCryptoServiceProvider();
        tdes.Mode = CipherMode.ECB;
        tdes.Padding = PaddingMode.PKCS7;
        tdes.Key = Encoding.UTF8.GetBytes(password);
        using var encryptedFileStream = new FileStream(inputFile, FileMode.Open);
        using var decryptedFileStream = new FileStream(outputFile, FileMode.Create);
        using var cryptoStream = new CryptoStream(encryptedFileStream, tdes.CreateDecryptor(), CryptoStreamMode.Read);
        cryptoStream.CopyTo(decryptedFileStream);
    }
}