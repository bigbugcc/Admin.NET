// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Security.Cryptography;

namespace Admin.NET.Core;

/// <summary>
/// 哈希生成辅助类
/// </summary>
/// <remarks>
/// 是一系列加密哈希函数，主要用于生成数据的固定长度散列值，以确保数据完整性和安全性。
/// </remarks>
public static class HashHelper
{
    /// <summary>
    /// 生成 SHA1 哈希值
    /// </summary>
    /// <param name="data">待加密的数据</param>
    /// <returns>生成的哈希值</returns>
    public static string Sha1(string data)
    {
        // 创建 SHA256 加密算法实例，将字符串数据转换为字节数组，并生成相应的哈希值
        var hashBytes = SHA1.HashData(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// 生成 SHA256 哈希值
    /// </summary>
    /// <param name="data">待加密的数据</param>
    /// <returns>生成的哈希值</returns>
    public static string Sha256(string data)
    {
        // 创建 SHA256 加密算法实例，将字符串数据转换为字节数组，并生成相应的哈希值
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// 生成 SHA384 哈希值
    /// </summary>
    /// <param name="data">待加密的数据</param>
    /// <returns>生成的哈希值</returns>
    public static string Sha384(string data)
    {
        // 创建 SHA384 加密算法实例，将字符串数据转换为字节数组，并生成相应的哈希值
        var hashBytes = SHA384.HashData(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// 生成 SHA512 哈希值
    /// </summary>
    /// <param name="data">待加密的数据</param>
    /// <returns>生成的哈希值</returns>
    public static string Sha512(string data)
    {
        // 创建 SHA512 加密算法实例，将字符串数据转换为字节数组，并生成相应的哈希值
        var hashBytes = SHA512.HashData(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// 对字符串进行 MD5 生成哈希
    /// </summary>
    /// <param name="input">待加密的明文字符串</param>
    /// <returns>生成的哈希值</returns>
    public static string Md5(string input)
    {
        var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// 对数据流进行 MD5 生成哈希
    /// </summary>
    /// <param name="inputPath">待加密的数据流路径</param>
    /// <returns>生成的哈希值</returns>
    public static string StreamMd5(string inputPath)
    {
        using FileStream stream = new(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return StreamMd5(stream);
    }

    /// <summary>
    /// 对数据流进行 MD5 生成哈希
    /// </summary>
    /// <param name="stream">待加密的数据流</param>
    /// <returns>生成的哈希值</returns>
    public static string StreamMd5(Stream stream)
    {
        var hashBytes = MD5.HashData(stream);
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// 对数据流进行 SHA256 生成哈希
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string StreamHash(Stream data)
    {
        var hashBytes = SHA256.HashData(data);
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// 对二进制数据进行 MD5 生成哈希
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string ByteMd5(byte[] data)
    {
        var hashBytes = MD5.HashData(data);
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// 对二进制数据进行 SHA256 生成哈希
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string ByteHash(byte[] data)
    {
        var hashBytes = SHA256.HashData(data);
        return Convert.ToHexString(hashBytes);
    }
}