// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 验证文件类型
/// </summary>
public static class VerifyFileExtensionName
{
    private static readonly IDictionary<string, string> dics_ext = new Dictionary<string, string>();
    private static readonly IDictionary<string, HashSet<int>> ext_dics = new Dictionary<string, HashSet<int>>();

    static VerifyFileExtensionName()
    {
        dics_ext.Add("FFD8FFE0", ".jpg");
        dics_ext.Add("FFD8FFE1", ".jpg");
        dics_ext.Add("89504E47", ".png");
        dics_ext.Add("47494638", ".gif");
        dics_ext.Add("49492A00", ".tif");
        dics_ext.Add("424D", ".bmp");

        // PS和CAD
        dics_ext.Add("38425053", ".psd");
        dics_ext.Add("41433130", ".dwg"); // CAD
        dics_ext.Add("252150532D41646F6265", ".ps");

        // 办公文档类
        dics_ext.Add("D0CF11E0", ".ppt,.doc,.xls"); // ppt、doc、xls
        dics_ext.Add("504B0304", ".pptx,.docx,.xlsx"); // pptx、docx、xlsx

        /* 注意由于文本文档录入内容过多，则读取文件头时较为多变-START */
        dics_ext.Add("0D0A0D0A", ".txt"); // txt
        dics_ext.Add("0D0A2D2D", ".txt"); // txt
        dics_ext.Add("0D0AB4B4", ".txt"); // txt
        dics_ext.Add("B4B4BDA8", ".txt"); // 文件头部为汉字
        dics_ext.Add("73646673", ".txt"); // txt,文件头部为英文字母
        dics_ext.Add("32323232", ".txt"); // txt,文件头部内容为数字
        dics_ext.Add("0D0A09B4", ".txt"); // txt,文件头部内容为数字
        dics_ext.Add("3132330D", ".txt"); // txt,文件头部内容为数字
        /* 注意由于文本文档录入内容过多，则读取文件头时较为多变-END */

        dics_ext.Add("7B5C727466", ".rtf"); // 日记本

        dics_ext.Add("255044462D312E", ".pdf");

        // 视频或音频类
        dics_ext.Add("3026B275", ".wma");
        dics_ext.Add("57415645", ".wav");
        dics_ext.Add("41564920", ".avi");
        dics_ext.Add("4D546864", ".mid");
        dics_ext.Add("2E524D46", ".rm");
        dics_ext.Add("000001BA", ".mpg");
        dics_ext.Add("000001B3", ".mpg");
        dics_ext.Add("6D6F6F76", ".mov");
        dics_ext.Add("3026B2758E66CF11", ".asf");

        // 压缩包
        dics_ext.Add("52617221", ".rar");
        dics_ext.Add("504B03040A000000", ".zip");
        dics_ext.Add("504B030414000000", ".zip");
        dics_ext.Add("1F8B08", ".gz");

        // 程序文件
        dics_ext.Add("3C3F786D6C", ".xml");
        dics_ext.Add("68746D6C3E", ".html");
		dics_ext.Add("04034b50", ".apk");
        //dics_ext.Add("7061636B", ".java");
        //dics_ext.Add("3C254020", ".jsp");
        //dics_ext.Add("4D5A9000", ".exe");

        dics_ext.Add("44656C69766572792D646174653A", ".eml"); // 邮件
        dics_ext.Add("5374616E64617264204A", ".mdb"); // Access数据库文件

        dics_ext.Add("46726F6D", ".mht");
        dics_ext.Add("4D494D45", ".mhtml");

        foreach (var dics in dics_ext)
        {
            foreach (var ext in dics.Value.Split(","))
            {
                if (!ext_dics.ContainsKey(ext))
                    ext_dics.Add(ext, new HashSet<int> { dics.Key.Length / 2 });
                else
                    ext_dics[ext].Add(dics.Key.Length / 2);
            }
        }
    }

    /// <summary>
    /// 文件格式和文件内容格式是否一致
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="suffix"></param>
    /// <returns></returns>
    public static bool IsSameType(Stream stream, string suffix = ".jpg")
    {
        if (stream == null)
            return false;

        suffix = suffix.ToLower();
        if (!ext_dics.ContainsKey(suffix))
            return false;

        try
        {
            foreach (var Len in ext_dics[suffix])
            {
                byte[] b = new byte[Len];
                stream.Read(b, 0, b.Length);
                // string fileType = System.Text.Encoding.UTF8.GetString(b);
                string fileKey = GetFileHeader(b);
                if (dics_ext.ContainsKey(fileKey))
                    return true;
            }
        }
        catch (IOException)
        {
        }
        return false;
    }

    /**
     * 根据文件转换成的字节数组获取文件头信息
     * @param 文件路径
     * @return 文件头信息
     */

    private static string GetFileHeader(byte[] b)
    {
        string value = BytesToHexString(b);
        return value;
    }

    /**
     * 将要读取文件头信息的文件的byte数组转换成string类型表示
     * 下面这段代码就是用来对文件类型作验证的方法，
     * 将字节数组的前四位转换成16进制字符串，并且转换的时候，要先和0xFF做一次与运算。
     * 这是因为，整个文件流的字节数组中，有很多是负数，进行了与运算后，可以将前面的符号位都去掉，
     * 这样转换成的16进制字符串最多保留两位，如果是正数又小于10，那么转换后只有一位，
     * 需要在前面补0，这样做的目的是方便比较，取完前四位这个循环就可以终止了
     * @param src要读取文件头信息的文件的byte数组
     * @return 文件头信息
     */

    private static string BytesToHexString(byte[] src)
    {
        var builder = new StringBuilder();
        if (src == null || src.Length <= 0)
            return null;

        string hv;
        for (int i = 0; i < src.Length; i++)
        {
            // 以十六进制（基数 16）无符号整数形式返回一个整数参数的字符串表示形式，并转换为大写
            hv = Convert.ToString(src[i] & 0xFF, 16).ToUpper();
            if (hv.Length < 2)
                builder.Append(0);
            builder.Append(hv);
        }
        return builder.ToString();
    }
}