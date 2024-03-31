// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。
//
// 必须在法律法规允许的范围内正确使用，严禁将其用于非法、欺诈、恶意或侵犯他人合法权益的目的。

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Admin.NET.Core;

/// <summary>
/// 字符串掩码
/// </summary>
[SuppressSniffer]
public class MaskSystemTextJsonConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString();
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString().Mask());
    }
}

/// <summary>
/// 身份证掩码
/// </summary>
[SuppressSniffer]
public class MaskIdCardSystemTextJsonConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString();
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString().MaskIdCard());
    }
}

/// <summary>
/// 邮箱掩码
/// </summary>
[SuppressSniffer]
public class MaskEmailSystemTextJsonConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString();
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString().MaskEmail());
    }
}