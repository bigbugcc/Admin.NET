// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Globalization;

namespace Admin.NET.Core;

/// <summary>
/// DateTime 扩展方法
/// </summary>
public static class DateTimeFormatExtensions
{
    /// <summary>
    /// 获取 Unix 时间戳
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static long GetUnixTimeStamp(this DateTime dateTime)
    {
        return ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 获取 Unix 时间戳
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static long GetUnixTimeStamp(this DateTimeOffset dateTime)
    {
        return dateTime.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 获取当前时间的时间戳
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static long GetDateToTimeStamp(this DateTime dateTime)
    {
        var ts = dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }

    /// <summary>
    /// 获取日期天的最小时间
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime GetDayMinDate(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
    }

    /// <summary>
    /// 获取日期天的最大时间
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime GetDayMaxDate(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
    }

    /// <summary>
    /// 获取一天的范围
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static List<DateTime> GetDayDateRange(this DateTime dateTime)
    {
        return
        [
            dateTime.GetDayMinDate(),
            dateTime.GetDayMaxDate()
        ];
    }

    /// <summary>
    /// 获取日期开始时间
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="days"></param>
    /// <returns></returns>
    public static DateTime GetBeginTime(this DateTime? dateTime, int days = 0)
    {
        return dateTime == DateTime.MinValue || dateTime is null ? DateTime.Now.AddDays(days) : (DateTime)dateTime;
    }

    /// <summary>
    /// 获取星期几
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static string GetWeekByDate(this DateTime dateTime)
    {
        string[] day = ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"];
        return day[Convert.ToInt32(dateTime.DayOfWeek.ToString("d"))];
    }

    /// <summary>
    /// 获取这个月的第几周
    /// </summary>
    /// <param name="daytime"></param>
    /// <returns></returns>
    public static int GetWeekNumInMonth(this DateTime daytime)
    {
        var dayInMonth = daytime.Day;
        // 本月第一天
        var firstDay = daytime.AddDays(1 - daytime.Day);
        // 本月第一天是周几
        var weekday = firstDay.DayOfWeek == 0 ? 7 : (int)firstDay.DayOfWeek;
        // 本月第一周有几天
        var firstWeekEndDay = 7 - (weekday - 1);
        // 当前日期和第一周之差
        var diffDay = dayInMonth - firstWeekEndDay;
        diffDay = diffDay > 0 ? diffDay : 1;
        // 当前是第几周，若整除7就减一天
        return (diffDay % 7 == 0 ? (diffDay / 7) - 1 : diffDay / 7) + 1 + (dayInMonth > firstWeekEndDay ? 1 : 0);
    }

    /// <summary>
    /// 时间转换字符串
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static string FormatDateTimeToString(this DateTime dateTime)
    {
        return dateTime.ToString(dateTime.Year == DateTime.Now.Year ? "MM-dd HH:mm" : "yyyy-MM-dd HH:mm");
    }

    /// <summary>
    /// 时间转换字符串
    /// </summary>
    /// <param name="dateTimeBefore"></param>
    /// <param name="dateTimeAfter"></param>
    /// <returns></returns>
    public static string FormatDateTimeToString(this DateTime dateTimeBefore, DateTime dateTimeAfter)
    {
        if (dateTimeBefore >= dateTimeAfter)
        {
            throw new Exception("开始日期必须小于结束日期");
        }

        var timeSpan = dateTimeAfter - dateTimeBefore;
        return timeSpan.FormatTimeSpanToString();
    }

    /// <summary>
    /// 毫秒转换字符串
    /// </summary>
    /// <param name="milliseconds"></param>
    /// <returns></returns>
    public static string FormatMilliSecondsToString(this long milliseconds)
    {
        var timeSpan = TimeSpan.FromMilliseconds(milliseconds);
        return timeSpan.FormatTimeSpanToString();
    }

    /// <summary>
    /// 时刻转换字符串
    /// </summary>
    /// <param name="ticks"></param>
    /// <returns></returns>
    public static string FormatTimeTicksToString(this long ticks)
    {
        var timeSpan = TimeSpan.FromTicks(ticks);
        return timeSpan.FormatTimeSpanToString();
    }

    /// <summary>
    /// 毫秒转换字符串
    /// </summary>
    /// <param name="ms"></param>
    /// <returns></returns>
    public static string FormatTimeMilliSecondToString(this long ms)
    {
        const int Ss = 1000;
        const int Mi = Ss * 60;
        const int Hh = Mi * 60;
        const int Dd = Hh * 24;

        var day = ms / Dd;
        var hour = (ms - (day * Dd)) / Hh;
        var minute = (ms - (day * Dd) - (hour * Hh)) / Mi;
        var second = (ms - (day * Dd) - (hour * Hh) - (minute * Mi)) / Ss;
        var milliSecond = ms - (day * Dd) - (hour * Hh) - (minute * Mi) - (second * Ss);

        // 天
        var sDay = day < 10 ? "0" + day : string.Empty + day;
        // 小时
        var sHour = hour < 10 ? "0" + hour : string.Empty + hour;
        // 分钟
        var sMinute = minute < 10 ? "0" + minute : string.Empty + minute;
        // 秒
        var sSecond = second < 10 ? "0" + second : string.Empty + second;
        // 毫秒
        var sMilliSecond = milliSecond < 10 ? "0" + milliSecond : string.Empty + milliSecond;
        sMilliSecond = milliSecond < 100 ? "0" + sMilliSecond : string.Empty + sMilliSecond;

        return $"{sDay} 天 {sHour} 小时 {sMinute} 分 {sSecond} 秒 {sMilliSecond} 毫秒";
    }

    /// <summary>
    /// 时间跨度转换字符串
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    public static string FormatTimeSpanToString(this TimeSpan timeSpan)
    {
        var day = timeSpan.Days;
        var hour = timeSpan.Hours;
        var minute = timeSpan.Minutes;
        var second = timeSpan.Seconds;
        var milliSecond = timeSpan.Milliseconds;

        // 天
        var sDay = day < 10 ? "0" + day : string.Empty + day;
        // 小时
        var sHour = hour < 10 ? "0" + hour : string.Empty + hour;
        // 分钟
        var sMinute = minute < 10 ? "0" + minute : string.Empty + minute;
        // 秒
        var sSecond = second < 10 ? "0" + second : string.Empty + second;
        // 毫秒
        var sMilliSecond = milliSecond < 10 ? "0" + milliSecond : string.Empty + milliSecond;
        sMilliSecond = milliSecond < 100 ? "0" + sMilliSecond : string.Empty + sMilliSecond;

        return $"{sDay} 天 {sHour} 小时 {sMinute} 分 {sSecond} 秒 {sMilliSecond} 毫秒";
    }

    /// <summary>
    /// 将 TimeSpan 格式化为 y年M月d天h小时m分钟 的字符串，值为0的单位不显示
    /// 注意：此方法对年、月的计算是基于近似值（1年≈365.25天，1月≈30.44天），适用于显示目的，不保证绝对精确
    /// </summary>
    /// <param name="timeSpan">要格式化的 TimeSpan</param>
    /// <param name="maxUnits">保留的最大时间单位数量</param>
    /// <returns>格式化后的字符串</returns>
    public static string FormatTimeSpanText(this TimeSpan timeSpan, int maxUnits = int.MaxValue)
    {
        if (timeSpan < TimeSpan.Zero) timeSpan = TimeSpan.Zero; // 确保非负
        if (maxUnits <= 0) throw new ArgumentException("maxUnits 必须大于0", nameof(maxUnits));

        long totalMinutes = (long)timeSpan.TotalMinutes;
        if (totalMinutes == 0) return "0分钟";

        // 计算年、月、日、小时、分钟
        // 使用近似值进行计算
        const double minutesInYear = 365 * 24 * 60;
        const double minutesInMonth = 30 * 24 * 60; // 平均每月天数
        const long minutesInDay = 24 * 60;
        const long minutesInHour = 60;

        int years = (int)(totalMinutes / minutesInYear);
        totalMinutes %= (long)minutesInYear;

        int months = (int)(totalMinutes / minutesInMonth);
        totalMinutes %= (long)minutesInMonth;

        int days = (int)(totalMinutes / minutesInDay);
        totalMinutes %= minutesInDay;

        int hours = (int)(totalMinutes / minutesInHour);
        int minutes = (int)(totalMinutes % minutesInHour);

        var parts = new List<string>();
        if (years > 0) parts.Add($"{years}年");
        if (months > 0) parts.Add($"{months}个月");
        if (days > 0) parts.Add($"{days}天");
        if (hours > 0) parts.Add($"{hours}小时");
        if (minutes > 0) parts.Add($"{minutes}分钟");

        // 如果指定了最大单位数量，则截取前 maxUnits 个非零单位
        if (maxUnits < int.MaxValue && parts.Count > maxUnits)
        {
            parts = parts.Take(maxUnits).ToList();
        }

        return parts.Count == 0 ? "0分钟" : string.Join("", parts);
    }

    /// <summary>
    /// 时间转换简易字符串
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string FormatDateTimeToEasyString(this DateTime value)
    {
        var now = DateTime.Now;

        if (now < value)
        {
            var strDate = value.ToString("yyyy-MM-dd HH:mm:ss");
            return strDate;
        }

        var dep = now - value;

        return dep.TotalSeconds < 10
            ? "刚刚"
            : dep.TotalSeconds is >= 10 and < 60
                ? (int)dep.TotalSeconds + "秒前"
                : dep.TotalMinutes is >= 1 and < 60
                    ? (int)dep.TotalMinutes + "分钟前"
                    : dep.TotalHours < 24
                        ? (int)dep.TotalHours + "小时前"
                        : dep.TotalDays < 7
                            ? (int)dep.TotalDays + "天前"
                            : dep.TotalDays is >= 7 and < 30
                                ? ((int)dep.TotalDays / 7) + "周前"
                                : dep.TotalDays is >= 30 and < 365
                                    ? ((int)dep.TotalDays / 30) + "个月前"
                                    : now.Year - value.Year + "年前";
    }

    /// <summary>
    /// 字符串转日期
    /// </summary>
    /// <param name="thisValue"></param>
    /// <returns></returns>
    public static DateTime FormatStringToDate(this string thisValue)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(thisValue))
            {
                return DateTime.MinValue;
            }

            if (thisValue.Contains('-') || thisValue.Contains('/'))
            {
                return DateTime.Parse(thisValue);
            }

            var length = thisValue.Length;
            return length switch
            {
                4 => DateTime.ParseExact(thisValue, "yyyy", CultureInfo.CurrentCulture),
                6 => DateTime.ParseExact(thisValue, "yyyyMM", CultureInfo.CurrentCulture),
                8 => DateTime.ParseExact(thisValue, "yyyyMMdd", CultureInfo.CurrentCulture),
                10 => DateTime.ParseExact(thisValue, "yyyyMMddHH", CultureInfo.CurrentCulture),
                12 => DateTime.ParseExact(thisValue, "yyyyMMddHHmm", CultureInfo.CurrentCulture),
                _ => DateTime.ParseExact(thisValue, "yyyyMMddHHmmss", CultureInfo.CurrentCulture)
            };
        }
        catch
        {
            return DateTime.MinValue;
        }
    }
}