// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core.Service;

/// <summary>
/// 自定义方法
/// </summary>
public class FuncList
{
    /// <summary>
    /// 字符串相加
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public string Merge(object a, object b)
    {
        return a.ToString() + b.ToString();
    }

    /// <summary>
    /// 对象合并
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public object MergeObj(object a, object b)
    {
        return new { a, b };
    }

    /// <summary>
    /// 是否包含
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool IsContain(object a, object b)
    {
        return a.ToString().Split(',').Contains(b);
    }

    /// <summary>
    /// 根据jtoken的实际类型来转换SugarParameter，避免全转成字符串
    /// </summary>
    /// <param name="jToken"></param>
    /// <returns></returns>
    public static dynamic TransJObjectToSugarPara(JToken jToken)
    {
        JTokenType jTokenType = jToken.Type;
        return jTokenType switch
        {
            JTokenType.Integer => jToken.ToObject(typeof(long)),
            JTokenType.Float => jToken.ToObject(typeof(decimal)),
            JTokenType.Boolean => jToken.ToObject(typeof(bool)),
            JTokenType.Date => jToken.ToObject(typeof(DateTime)),
            JTokenType.Bytes => jToken.ToObject(typeof(byte)),
            JTokenType.Guid => jToken.ToObject(typeof(Guid)),
            JTokenType.TimeSpan => jToken.ToObject(typeof(TimeSpan)),
            JTokenType.Array => TransJArrayToSugarPara(jToken),
            _ => jToken
        };
    }

    /// <summary>
    /// 根据jArray的实际类型来转换SugarParameter，避免全转成字符串
    /// </summary>
    /// <param name="jToken"></param>
    /// <returns></returns>
    public static dynamic TransJArrayToSugarPara(JToken jToken)
    {
        if (jToken is not JArray) return jToken;
        if (jToken.Any())
        {
            JTokenType jTokenType = jToken.First().Type;
            return jTokenType switch
            {
                JTokenType.Integer => jToken.ToObject<long[]>(),
                JTokenType.Float => jToken.ToObject<decimal[]>(),
                JTokenType.Boolean => jToken.ToObject<bool[]>(),
                JTokenType.Date => jToken.ToObject<DateTime[]>(),
                JTokenType.Bytes => jToken.ToObject<byte[]>(),
                JTokenType.Guid => jToken.ToObject<Guid[]>(),
                JTokenType.TimeSpan => jToken.ToObject<TimeSpan[]>(),
                _ => jToken.ToArray()
            };
        }

        return (JArray)jToken;
    }

    /// <summary>
    /// 获取字符串里的值的真正类型
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string GetValueCSharpType(string input)
    {
        if (DateTime.TryParse(input, out _))
            return "DateTime";
        else if (int.TryParse(input, out _))
            return "int";
        else if (long.TryParse(input, out _))
            return "long";
        else if (decimal.TryParse(input, out _))
            return "decimal";
        else if (bool.TryParse(input, out _))
            return "bool";
        else
            return "string";
    }
}