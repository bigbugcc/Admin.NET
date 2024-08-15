// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// 民族枚举
/// </summary>
[Description("民族枚举")]
public enum NationEnum
{
    /// <summary>
    /// 汉族
    /// </summary>
    [Description("汉族")]
    HanZu = 01,

    /// <summary>
    /// 壮族
    /// </summary>
    [Description("壮族")]
    ZhuangZu = 02,

    /// <summary>
    /// 满族
    /// </summary>
    [Description("满族")]
    ManZu = 03,

    /// <summary>
    /// 回族
    /// </summary>
    [Description("回族")]
    HuiZu = 04,

    /// <summary>
    /// 苗族
    /// </summary>
    [Description("苗族")]
    MiaoZu = 05,

    /// <summary>
    /// 维吾尔族
    /// </summary>
    [Description("维吾尔族")]
    WeiWuErZu = 06,

    /// <summary>
    /// 土家族
    /// </summary>
    [Description("土家族")]
    TuJiaZu = 07,

    /// <summary>
    /// 彝族
    /// </summary>
    [Description("彝族")]
    YiZu = 08,

    /// <summary>
    /// 蒙古族
    /// </summary>
    [Description("蒙古族")]
    MengGuZu = 09,

    /// <summary>
    /// 藏族
    /// </summary>
    [Description("藏族")]
    ZangZu = 10,

    /// <summary>
    /// 布依族
    /// </summary>
    [Description("布依族")]
    BuYiZu = 11,

    /// <summary>
    /// 侗族
    /// </summary>
    [Description("侗族")]
    DongZu = 12,

    /// <summary>
    /// 瑶族
    /// </summary>
    [Description("瑶族")]
    YaoZu = 13,

    /// <summary>
    /// 朝鲜族
    /// </summary>
    [Description("朝鲜族")]
    ChaoXianZu = 14,

    /// <summary>
    /// 白族
    /// </summary>
    [Description("白族")]
    BaiZu = 15,

    /// <summary>
    /// 哈尼族
    /// </summary>
    [Description("哈尼族")]
    HaNiZu = 16,

    /// <summary>
    /// 哈萨克族
    /// </summary>
    [Description("哈萨克族")]
    HaSaKeZu = 17,

    /// <summary>
    /// 黎族
    /// </summary>
    [Description("黎族")]
    LiZu = 18,

    /// <summary>
    /// 傣族
    /// </summary>
    [Description("傣族")]
    DaiZu = 19,

    /// <summary>
    /// 畲族
    /// </summary>
    [Description("畲族")]
    SheZu = 20,

    /// <summary>
    /// 傈僳族
    /// </summary>
    [Description("傈僳族")]
    LiSuZu = 21,

    /// <summary>
    /// 仡佬族
    /// </summary>
    [Description("仡佬族")]
    GeLaoZu = 22,

    /// <summary>
    /// 拉祜族
    /// </summary>
    [Description("拉祜族")]
    LaHuZu = 23,

    /// <summary>
    /// 东乡族
    /// </summary>
    [Description("东乡族")]
    DongXiangZu = 24,

    /// <summary>
    /// 纳西族
    /// </summary>
    [Description("纳西族")]
    NaXiZu = 25,

    /// <summary>
    /// 景颇族
    /// </summary>
    [Description("景颇族")]
    JingPoZu = 26,

    /// <summary>
    /// 柯尔克孜族
    /// </summary>
    [Description("柯尔克孜族")]
    KeErKeZiZu = 27,

    /// <summary>
    /// 土族
    /// </summary>
    [Description("土族")]
    TuZu = 28,

    /// <summary>
    /// 达斡尔族
    /// </summary>
    [Description("达斡尔族")]
    DaWoErZu = 29,

    /// <summary>
    /// 仫佬族
    /// </summary>
    [Description("仫佬族")]
    MuLaoZu = 30,

    /// <summary>
    /// 羌族
    /// </summary>
    [Description("羌族")]
    QiangZu = 31,

    /// <summary>
    /// 布朗族
    /// </summary>
    [Description("布朗族")]
    BuLangZu = 32,

    /// <summary>
    /// 撒拉族
    /// </summary>
    [Description("撒拉族")]
    SaLaZu = 33,

    /// <summary>
    /// 毛南族
    /// </summary>
    [Description("毛南族")]
    MaoNanZu = 34,

    /// <summary>
    /// 仡族
    /// </summary>
    [Description("仡族")]
    GeZu = 35,

    /// <summary>
    /// 锡伯族
    /// </summary>
    [Description("锡伯族")]
    XiBoZu = 36,

    /// <summary>
    /// 阿昌族
    /// </summary>
    [Description("阿昌族")]
    AChangZu = 37,

    /// <summary>
    /// 普米族
    /// </summary>
    [Description("普米族")]
    PuMiZu = 38,

    /// <summary>
    /// 塔吉克族
    /// </summary>
    [Description("塔吉克族")]
    TaJiKeZu = 39,

    /// <summary>
    /// 怒族
    /// </summary>
    [Description("怒族")]
    NuZu = 40,

    /// <summary>
    /// 乌孜别克族
    /// </summary>
    [Description("乌孜别克族")]
    WuZiBieKeZu = 41,

    /// <summary>
    /// 俄罗斯族
    /// </summary>
    [Description("俄罗斯族")]
    ELuoSiZu = 42,

    /// <summary>
    /// 鄂温克族
    /// </summary>
    [Description("鄂温克族")]
    EwenKeZu = 43,

    /// <summary>
    /// 德昂族
    /// </summary>
    [Description("德昂族")]
    DeAngZu = 44,

    /// <summary>
    /// 保安族
    /// </summary>
    [Description("保安族")]
    BaoAnZu = 45,

    /// <summary>
    /// 裕固族
    /// </summary>
    [Description("裕固族")]
    YuGuZu = 46,

    /// <summary>
    /// 京族
    /// </summary>
    [Description("京族")]
    JingZu = 47,

    /// <summary>
    /// 塔塔尔族
    /// </summary>
    [Description("塔塔尔族")]
    TaTaErZu = 48,

    /// <summary>
    /// 独龙族
    /// </summary>
    [Description("独龙族")]
    DuLongZu = 49,

    /// <summary>
    /// 鄂伦春族
    /// </summary>
    [Description("鄂伦春族")]
    ELunChunZu = 50,

    /// <summary>
    /// 赫哲族
    /// </summary>
    [Description("赫哲族")]
    HeZheZu = 51,

    /// <summary>
    /// 门巴族
    /// </summary>
    [Description("门巴族")]
    MenBaZu = 52,

    /// <summary>
    /// 珞巴族
    /// </summary>
    [Description("珞巴族")]
    LuoBaZu = 53,

    /// <summary>
    /// 高山族
    /// </summary>
    [Description("高山族")]
    GaoShanZu = 54,

    /// <summary>
    /// 佤族
    /// </summary>
    [Description("佤族")]
    WaZu = 55,

    /// <summary>
    /// 基诺族
    /// </summary>
    [Description("基诺族")]
    JiNuoZu = 56
}