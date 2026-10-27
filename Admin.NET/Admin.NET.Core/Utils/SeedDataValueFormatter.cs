// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Globalization;
using System.Text.Json;

namespace Admin.NET.Core;

/// <summary>
/// 种子数据值格式化器 🧬
/// <para>
/// 将从数据库读出的 CLR 值渲染为可直接编译的 C# 字面量，供 <c>SeedData.cs.vm</c> 生成对象初始化器使用。
/// 以往实现只对字符串加了逐字引号、其余类型一律输出 <c>ToString()</c> 原始文本，遇到值里含双引号、
/// 未定义的枚举数值、<c>decimal</c>/<c>float</c> 后缀、<c>char</c>/<c>Guid</c>/<c>byte[]</c> 与 JSON 列时，
/// 生成的种子文件无法编译。
/// </para>
/// <para>
/// 兼容性约定：对以往已能正常编译的值保持输出形态不变——不含双引号与控制字符的字符串仍用逐字字符串
/// （值里的反斜杠不转义，与历史生成物一致）、已定义枚举成员仍输出 <c>T.Member</c>、日期仍输出
/// <c>DateTime.Parse("yyyy-MM-dd HH:mm:ss.fff")</c>、整数与布尔仍输出原样文本；只在原本就会生成非法代码
/// 的分支上补转义与类型包装。
/// </para>
/// </summary>
public static class SeedDataValueFormatter
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
    private const string DateTimeOffsetFormat = "yyyy-MM-dd HH:mm:ss.fff zzz";
    private const string DateOnlyFormat = "yyyy-MM-dd";
    private const string TimeOnlyFormat = "HH:mm:ss.fff";

    /// <summary>C# 关键字类型名（生成代码里直接写关键字，避免全名与别名混用）</summary>
    private static readonly Dictionary<Type, string> KeywordNames = new()
    {
        [typeof(bool)] = "bool",
        [typeof(byte)] = "byte",
        [typeof(sbyte)] = "sbyte",
        [typeof(short)] = "short",
        [typeof(ushort)] = "ushort",
        [typeof(int)] = "int",
        [typeof(uint)] = "uint",
        [typeof(long)] = "long",
        [typeof(ulong)] = "ulong",
        [typeof(float)] = "float",
        [typeof(double)] = "double",
        [typeof(decimal)] = "decimal",
        [typeof(char)] = "char",
        [typeof(string)] = "string"
    };

    /// <summary>
    /// JSON 列序列化选项：不做 HTML 转义，保证生成的中文内容可读（输出只写入 C# 源文件，不进入 HTML 上下文）
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// 将属性值格式化为 C# 字面量
    /// </summary>
    /// <param name="propertyType">属性声明类型（可空类型自动剥壳）</param>
    /// <param name="value">数据库读出的值</param>
    /// <returns>可直接写入对象初始化器右侧的字面量文本</returns>
    public static string Format(Type propertyType, object value)
    {
        if (value is null) return "null";

        var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        // 声明为 object 时按运行时类型渲染，否则输出的是裸文本、生成的代码无法编译
        if (type == typeof(object)) type = value.GetType();

        if (type == typeof(string)) return ToStringLiteral(value.ToString());
        if (type == typeof(char)) return $"'{Escape(value.ToString())}'";
        if (type == typeof(bool)) return (bool)value ? "true" : "false";
        if (type.IsEnum) return ToEnumLiteral(type, value);
        if (type == typeof(Guid)) return $"Guid.Parse(\"{((Guid)value).ToString("D", CultureInfo.InvariantCulture)}\")";
        if (type == typeof(DateTime)) return $"DateTime.Parse(\"{((DateTime)value).ToString(DateTimeFormat, CultureInfo.InvariantCulture)}\")";
        if (type == typeof(DateTimeOffset)) return $"DateTimeOffset.Parse(\"{((DateTimeOffset)value).ToString(DateTimeOffsetFormat, CultureInfo.InvariantCulture)}\")";
        if (type == typeof(DateOnly)) return $"DateOnly.Parse(\"{((DateOnly)value).ToString(DateOnlyFormat, CultureInfo.InvariantCulture)}\")";
        if (type == typeof(TimeOnly)) return $"TimeOnly.Parse(\"{((TimeOnly)value).ToString(TimeOnlyFormat, CultureInfo.InvariantCulture)}\")";
        if (type == typeof(TimeSpan)) return $"TimeSpan.Parse(\"{((TimeSpan)value).ToString("c", CultureInfo.InvariantCulture)}\")";
        if (type == typeof(byte[])) return $"System.Convert.FromBase64String(\"{Convert.ToBase64String((byte[])value)}\")";
        if (IsNumeric(type)) return ToNumberLiteral(type, value);

        // JSON 列（[SugarColumn(IsJson = true)]）等复杂值：以 JSON 往返方式还原为可编译的强类型表达式
        if (!type.IsValueType && GetTypeName(type) is { } typeName)
            return $"System.Text.Json.JsonSerializer.Deserialize<{typeName}>({ToStringLiteral(JsonSerializer.Serialize(value, type, JsonOptions))})";

        // 其余无法确定可编译形态的类型，保持历史输出形态（不臆测包装）
        return ToInvariantString(value);
    }

    /// <summary>
    /// 字符串字面量：无引号且无控制字符时沿用逐字字符串（保持历史生成物形态），否则输出转义字符串
    /// </summary>
    private static string ToStringLiteral(string text)
    {
        if (text is null) return "null";
        if (text.IndexOf('"') < 0 && !text.Any(u => u < ' ')) return $"@\"{text}\"";
        return $"\"{Escape(text)}\"";
    }

    /// <summary>
    /// 枚举字面量：已定义成员输出 <c>T.Member</c>，未定义数值（脏数据或 [Flags] 组合值）输出 <c>(T)数值</c>
    /// </summary>
    private static string ToEnumLiteral(Type type, object value)
    {
        var typeName = GetTypeName(type) ?? type.FullName;
        if (Enum.IsDefined(type, value)) return $"{typeName}.{value}";
        // 取底层数值而非 ToString()：[Flags] 组合值会输出 "Read, Write" 这类不可编译的名称列表
        var raw = ToInvariantString(Convert.ChangeType(value, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture));
        // 负数需再包一层括号，否则 (T)-3 触发 CS0075
        return $"({typeName}){(raw[0] == '-' ? "(" + raw + ")" : raw)}";
    }

    /// <summary>
    /// 数值字面量：统一用固定区域性输出（避免 1,5 之类的小数点本地化），并按目标类型补 m / f 后缀
    /// </summary>
    private static string ToNumberLiteral(Type type, object value)
    {
        var text = ToInvariantString(value);
        if (type != typeof(float) && type != typeof(double)) return type == typeof(decimal) ? text + "m" : text;

        var number = Convert.ToDouble(value, CultureInfo.InvariantCulture);
        var keyword = KeywordNames[type];
        if (double.IsNaN(number)) return $"{keyword}.NaN";
        if (double.IsPositiveInfinity(number)) return $"{keyword}.PositiveInfinity";
        if (double.IsNegativeInfinity(number)) return $"{keyword}.NegativeInfinity";

        // float 属性必须带 f 后缀（double 字面量到 float 无隐式转换），double 字面量本身即 double 无需后缀
        return type == typeof(float) ? text + "f" : text;
    }

    /// <summary>
    /// 转义字符串内容（逐字字符串分支不需要，故反斜杠仅在转义分支处理）
    /// </summary>
    private static string Escape(string text)
    {
        var sb = new StringBuilder(text.Length + 8);
        foreach (var c in text)
        {
            switch (c)
            {
                case '\\': sb.Append(@"\\"); break;
                case '"': sb.Append("\\\""); break;
                case '\'': sb.Append("\\'"); break;
                case '\0': sb.Append("\\0"); break;
                case '\a': sb.Append("\\a"); break;
                case '\b': sb.Append("\\b"); break;
                case '\f': sb.Append("\\f"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                case '\v': sb.Append("\\v"); break;
                // 其余控制字符走 \uXXXX：逐字字符串会把换行原样写进生成文件，值会随文件行尾（CRLF/LF）改变
                default: sb.Append(c < ' ' ? $"\\u{(int)c:X4}" : c.ToString()); break;
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// 渲染可写入生成代码的类型名；泛型参数、指针、匿名类型等无法书写时返回 null
    /// </summary>
    private static string GetTypeName(Type type)
    {
        if (type.IsGenericParameter || type.IsByRef || type.IsPointer) return null;
        if (KeywordNames.TryGetValue(type, out var keyword)) return keyword;

        if (type.IsArray)
        {
            if (type.GetArrayRank() != 1) return null;
            var elementType = type.GetElementType();
            var elementName = elementType is null ? null : GetTypeName(elementType);
            return elementName is null ? null : elementName + "[]";
        }

        if (type.IsGenericType)
        {
            var definitionName = type.GetGenericTypeDefinition().FullName;
            if (definitionName is null) return null;
            var tickIndex = definitionName.IndexOf('`');
            if (tickIndex >= 0) definitionName = definitionName[..tickIndex];
            var argumentNames = type.GetGenericArguments().Select(GetTypeName).ToArray();
            if (argumentNames.Any(u => u is null)) return null;
            return $"{definitionName.Replace('+', '.')}<{string.Join(",", argumentNames)}>";
        }

        var fullName = type.FullName;
        // 命名空间缺失或含尖括号的多泛型（编译器生成名）等形态无法书写
        return fullName is null || fullName.IndexOf('<') >= 0 ? null : fullName.Replace('+', '.');
    }

    private static bool IsNumeric(Type type) => type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) || type == typeof(ushort)
        || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong)
        || type == typeof(float) || type == typeof(double) || type == typeof(decimal);

    private static string ToInvariantString(object value) => value is IFormattable formattable ? formattable.ToString(null, CultureInfo.InvariantCulture) : value.ToString();
}
