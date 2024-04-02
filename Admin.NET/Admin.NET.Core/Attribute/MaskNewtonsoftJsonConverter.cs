// 大名科技（天津）有限公司 版权所有
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动
//
// 任何基于本项目二次开发而产生的一切法律纠纷和责任，均与作者无关

using Newtonsoft.Json;

namespace Admin.NET.Core;

/// <summary>
/// 字符串掩码
/// </summary>
[SuppressSniffer]
public class MaskNewtonsoftJsonConverter : JsonConverter<string>
{
    public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return reader.Value.ToString();
    }

    public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
    {
        writer.WriteValue(value?.ToString().Mask());
    }
}

/// <summary>
/// 身份证掩码
/// </summary>
[SuppressSniffer]
public class MaskIdCardNewtonsoftJsonConverter : JsonConverter<string>
{
    public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return reader.Value.ToString();
    }

    public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
    {
        writer.WriteValue(value?.ToString().MaskIdCard());
    }
}

/// <summary>
/// 邮箱掩码
/// </summary>
[SuppressSniffer]
public class MaskEmailNewtonsoftJsonConverter : JsonConverter<string>
{
    public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return reader.Value.ToString();
    }

    public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
    {
        writer.WriteValue(value?.ToString().MaskEmail());
    }
}