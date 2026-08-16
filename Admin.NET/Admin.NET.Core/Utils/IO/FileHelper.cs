// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 文件帮助类
/// </summary>
public static class FileHelper
{
    #region 文件操作

    /// <summary>
    /// 读取文件内容到字符串
    /// </summary>
    /// <param name="filePath">要读取的文件路径</param>
    /// <returns>文件内容为字符串</returns>
    public static string ReadAllText(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    /// <summary>
    /// 打开一个文本文件，读取文件的所有行，然后关闭文件
    /// </summary>
    /// <param name="filePath">要打开以进行读取的文件路径</param>
    /// <returns>包含文件所有行的字符串</returns>
    public static async Task<string> ReadAllTextAsync(string filePath)
    {
        using var reader = File.OpenText(filePath);
        return await reader.ReadToEndAsync();
    }

    /// <summary>
    /// 打开一个文本文件，读取文件的所有字节，然后关闭文件
    /// </summary>
    /// <param name="filePath">要打开以进行读取的文件路径</param>
    /// <returns>包含文件所有字节的字节数组</returns>
    public static async Task<byte[]> ReadAllBytesAsync(string filePath)
    {
        await using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var result = new byte[stream.Length];
        _ = await stream.ReadAsync(result.AsMemory(0, (int)stream.Length));
        return result;
    }

    /// <summary>
    /// 打开一个文本文件，读取文件的所有行，然后关闭文件
    /// </summary>
    /// <param name="path">要打开以进行读取的文件</param>
    /// <param name="encoding">文件的编码默认为 UTF8</param>
    /// <param name="fileMode">指定操作系统应如何打开文件默认为 Open</param>
    /// <param name="fileAccess">定义对文件的读取、写入或读写访问的常量默认为 Read</param>
    /// <param name="fileShare">包含控制其他 FileStream 对象可以对同一文件拥有的访问类型的常量默认为 Read</param>
    /// <param name="bufferSize">StreamReader 缓冲区的长度默认为 4096</param>
    /// <param name="fileOptions">指示 FileStream 选项默认为 Asynchronous(文件将用于异步读取)和 SequentialScan(文件将从开始到末尾顺序访问)</param>
    /// <returns>包含文件所有行的字符串数组</returns>
    public static async Task<string[]> ReadAllLinesAsync(string path,
        Encoding? encoding = null,
        FileMode fileMode = FileMode.Open,
        FileAccess fileAccess = FileAccess.Read,
        FileShare fileShare = FileShare.Read,
        int bufferSize = 4096,
        FileOptions fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan)
    {
        encoding ??= Encoding.UTF8;
        List<string> lines = [];
        await using (FileStream stream = new(path, fileMode, fileAccess, fileShare, bufferSize, fileOptions))
        {
            using StreamReader reader = new(stream, encoding);
            while (await reader.ReadLineAsync() is { } line)
            {
                lines.Add(line);
            }
        }

        return [.. lines];
    }

    /// <summary>
    /// 打开一个文本文件，读取不包含 BOM 的内容
    /// </summary>
    /// <param name="path">要打开以进行读取的文件</param>
    /// <returns>包含文件所有行的字符串</returns>
    public static async Task<string> ReadWithoutBomAsync(string path)
    {
        var content = await ReadAllBytesAsync(path);
        return ConvertFromBytesWithoutBom(content)!;
    }

    /// <summary>
    /// 将字节数组 byte[]转换为不包含字节顺序标记(BOM)的字符串
    /// </summary>
    /// <param name="bytes">要转换为字符串的 byte[]数组</param>
    /// <param name="encoding">获取字符串的编码默认为 UTF8</param>
    /// <returns>转换得到的字符串</returns>
    private static string? ConvertFromBytesWithoutBom(byte[]? bytes, Encoding? encoding = null)
    {
        if (bytes is null)
        {
            return null;
        }

        encoding ??= Encoding.UTF8;

        var hasBom = bytes is [0xEF, 0xBB, 0xBF, ..];

        return hasBom ? encoding.GetString(bytes, 3, bytes.Length - 3) : encoding.GetString(bytes);
    }

    /// <summary>
    /// 将文本内容写入到文件
    /// </summary>
    /// <param name="filePath">要写入的文件路径</param>
    /// <param name="content">要写入的文本内容</param>
    public static void WriteAllText(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
    }

    /// <summary>
    /// 将文本内容追加到文件
    /// </summary>
    /// <param name="filePath">要追加的文件路径</param>
    /// <param name="content">要追加的文本内容</param>
    public static void AppendAllText(string filePath, string content)
    {
        File.AppendAllText(filePath, content);
    }

    /// <summary>
    /// 创建文件，如果文件不存在
    /// </summary>
    /// <param name="filePath">文件路径</param>
    public static void CreateIfNotExists(string filePath)
    {
        if (!File.Exists(filePath))
        {
            _ = File.Create(filePath);
        }
    }

    /// <summary>
    /// 删除一个文件，如果文件存在
    /// </summary>
    /// <param name="filePath">要删除的文件路径</param>
    public static void DeleteIfExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    /// <summary>
    /// 移动文件到另一个位置
    /// </summary>
    /// <param name="sourcePath">当前文件的路径</param>
    /// <param name="destinationPath">目标文件的路径</param>
    public static void Move(string sourcePath, string destinationPath)
    {
        File.Move(sourcePath, destinationPath);
    }

    /// <summary>
    /// 复制文件到另一个位置
    /// </summary>
    /// <param name="sourcePath">当前文件的路径</param>
    /// <param name="destinationPath">目标文件的路径</param>
    /// <param name="overwrite">如果目标位置已经存在同名文件，是否覆盖</param>
    public static void Copy(string sourcePath, string destinationPath, bool overwrite = false)
    {
        File.Copy(sourcePath, destinationPath, overwrite);
    }

    /// <summary>
    /// 清空文件内容
    /// </summary>
    /// <param name="filePath">文件的绝对路径</param>
    public static void Clean(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        // 删除文件
        File.Delete(filePath);
        // 重新创建该文件
        _ = File.Create(filePath);
    }

    /// <summary>
    /// 尝试删除文件/目录
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool TryDelete(string path)
    {
        try
        {
            if (string.IsNullOrEmpty(path)) return false;
            if (Directory.Exists(path)) Directory.Delete(path, recursive: true);
            else File.Delete(path);
            return true;
        }
        catch (Exception)
        {
            // ignored
            return false;
        }
    }

    /// <summary>
    /// 复制目录
    /// </summary>
    /// <param name="sourceDir"></param>
    /// <param name="destinationDir"></param>
    /// <param name="overwrite"></param>
    public static void CopyDirectory(string sourceDir, string destinationDir, bool overwrite = false)
    {
        // 检查源目录是否存在
        if (!Directory.Exists(sourceDir)) throw new DirectoryNotFoundException("Source directory not found: " + sourceDir);

        // 如果目标目录不存在，则创建它
        if (!Directory.Exists(destinationDir)) Directory.CreateDirectory(destinationDir!);

        // 获取源目录下的所有文件并复制它们
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string name = Path.GetFileName(file);
            string dest = Path.Combine(destinationDir, name);
            File.Copy(file, dest, overwrite);
        }

        // 递归复制所有子目录
        foreach (string directory in Directory.GetDirectories(sourceDir))
        {
            string name = Path.GetFileName(directory);
            string dest = Path.Combine(destinationDir, name);
            CopyDirectory(directory, dest, overwrite);
        }
    }

    /// <summary>
    /// 在文件倒数第lastIndex个identifier前插入内容（备份原文件）
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="insertContent">要插入的内容</param>
    /// <param name="identifier">标识符号</param>
    /// <param name="lastIndex">倒数第几个标识符</param>
    /// <param name="createBackup">是否创建备份文件</param>
    public static async Task InsertsStringAtSpecifiedLocationInFile(string filePath, string insertContent, char identifier, int lastIndex, bool createBackup = false)
    {
        // 参数校验
        if (lastIndex < 1) throw new ArgumentOutOfRangeException(nameof(lastIndex));
        if (identifier == 0) throw new ArgumentException("标识符不能为空字符");

        if (!File.Exists(filePath))
            throw new FileNotFoundException("目标文件不存在", filePath);

        // 创建备份文件
        if (createBackup)
        {
            string backupPath = $"{filePath}.bak_{DateTime.Now:yyyyMMddHHmmss}";
            File.Copy(filePath, backupPath, true);
        }

        using var reader = new StreamReader(filePath, Encoding.UTF8);
        var content = await reader.ReadToEndAsync();
        reader.Close();
        // 逆向查找算法
        int index = content.LastIndexOf(identifier);
        if (index == -1)
        {
            throw new ArgumentException($"文件中未包含{identifier}");
        }

        int resIndex = content.LastIndexOf(identifier, index - lastIndex);
        if (resIndex == -1)
        {
            throw new ArgumentException($"文件中{identifier}不足{lastIndex}个");
        }

        StringBuilder sb = new StringBuilder(content);
        sb = sb.Insert(resIndex, insertContent);
        await WriteToFileAsync(filePath, sb);
    }

    /// <summary>
    /// 写入文件内容
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="sb"></param>
    public static async Task WriteToFileAsync(string filePath, StringBuilder sb)
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        await using var writer = new StreamWriter(filePath, false, new UTF8Encoding(false)); // 无BOM
        await writer.WriteAsync(sb.ToString());
        writer.Close();
        Console.WriteLine($"文件【{filePath}】写入完成");
        Console.ResetColor();
    }


    #endregion 文件操作

    #region 文件信息

    /// <summary>
    /// 获取文件的哈希值
    /// </summary>
    /// <param name="filePath">要计算哈希值的文件路径</param>
    /// <returns>文件的哈希值</returns>
    public static string GetHash(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        return HashHelper.StreamMd5(stream);
    }

    /// <summary>
    /// 获取指定文件大小
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static long GetSize(string filePath)
    {
        return new FileInfo(filePath).Length;
    }

    /// <summary>
    /// 从文件的绝对路径中获取文件名(包含扩展方法名)
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetName(string filePath)
    {
        return Path.GetFileName(filePath);
    }

    /// <summary>
    /// 获取随机文件名
    /// </summary>
    /// <returns></returns>
    public static string GetRandomName()
    {
        return Path.GetRandomFileName();
    }

    /// <summary>
    /// 根据时间得到文件名
    /// yyyyMMddHHmmssfff
    /// </summary>
    /// <returns></returns>
    public static string GetDateName()
    {
        return DateTime.Now.ToString("yyyyMMddHHmmssfff");
    }

    /// <summary>
    /// 从文件的绝对路径中获取扩展方法名
    /// 文件扩展方法名是包含点(.)的
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetExtension(string filePath)
    {
        return Path.GetExtension(filePath);
    }

    /// <summary>
    /// 从文件的绝对路径中获取文件名(不包含扩展方法名)
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetNameWithoutExtension(string filePath)
    {
        return Path.GetFileNameWithoutExtension(filePath);
    }

    /// <summary>
    /// 生成唯一的文件名，上传文件使用
    /// </summary>
    /// <param name="fileName">包含扩展方法名的源文件名</param>
    /// <returns></returns>
    public static string GetUniqueName(string fileName)
    {
        var fileNameWithoutExtension = GetNameWithoutExtension(fileName);
        var fileExtension = GetExtension(fileName);
        var uniqueFileName = $"{fileNameWithoutExtension}_{GetDateName()}_{GetRandomName()}";
        return uniqueFileName + fileExtension;
    }

    /// <summary>
    /// 获取文本文件的行数
    /// </summary>
    /// <param name="filePath">文件的绝对路径</param>
    /// <returns></returns>
    public static int GetTextLineCount(string filePath)
    {
        // 将文本文件的各行读到一个字符串数组中
        var rows = File.ReadAllLines(filePath);
        // 返回行数
        return rows.Length;
    }

    #endregion 文件信息

    #region 文件检查

    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    /// <param name="filePath">要检查的文件路径</param>
    /// <returns>如果文件存在返回 true，否则返回 false</returns>
    public static bool Exists(string filePath)
    {
        return File.Exists(filePath);
    }

    /// <summary>
    /// 检查文件是否被锁定
    /// </summary>
    /// <param name="filePath">要检查的文件路径</param>
    /// <returns>true 如果文件没有被锁定，可以进行读写操作，否则 false</returns>
    public static bool IsUnlocked(string filePath)
    {
        try
        {
            using var fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            // 如果没有异常，文件没有被锁定
            return true;
        }
        catch (IOException)
        {
            // 如果发生异常，文件可能被锁定
            return false;
        }
    }

    #endregion 文件检查
}