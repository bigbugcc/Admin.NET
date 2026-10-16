// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// SQL 安全校验帮助类 🛡️
/// <para>
/// 数据库标识符（表名、列名、存储过程名）以及部分无法参数化的 SQL 片段（如 HAVING 字段表达式、
/// @column 中的函数表达式）只能拼接到 SQL 中，无法通过参数占位符传递。本类通过严格的白名单校验，
/// 确保这些内容只包含合法字符与结构，从根源上避免将用户输入拼接进 SQL 造成注入。
/// </para>
/// <para>
/// 标识符白名单为 Unicode 感知：允许中文等 Unicode 字母、数字、下划线与美元符号，
/// 但不含任何 SQL 元字符（引号、分号、注释符、运算符、空白等）。
/// </para>
/// <para>
/// 对无法参数化、但需进入字符串字面量的取值（表 / 列注释、默认值），另提供 <see cref="EscapeSqlLiteral"/> 转义与
/// <see cref="ValidateDefaultValue"/> / <see cref="ValidateSqlType"/> 白名单校验。
/// </para>
/// </summary>
public static partial class SqlSecurityHelper
{
    /// <summary>标识符最大长度（兼容主流数据库 64 字符限制）</summary>
    private const int MaxIdentifierLength = 64;

    /// <summary>SQL 表达式片段最大长度</summary>
    private const int MaxExpressionLength = 128;

    /// <summary>
    /// 数据库标识符：允许字母（含中文等 Unicode 字母）、数字、下划线和美元符号。
    /// <c>\w</c> 覆盖 Unicode 字母 / 数字 / 下划线，但不含引号、分号、连字符、括号、空白等 SQL 元字符，故无法用于注入。
    /// </summary>
    [GeneratedRegex(@"^[\w$]+$")]
    private static partial Regex IdentifierRegex();

    /// <summary>存储过程名：允许单层 schema 前缀（如 dbo.sp_user），同样支持中文等 Unicode 字母</summary>
    [GeneratedRegex(@"^[\w$]+(\.[\w$]+)?$")]
    private static partial Regex ProcedureNameRegex();

    /// <summary>
    /// SQL 表达式片段（字段名或函数表达式）：以字母、下划线或汉字等“非数字词字符”开头，
    /// 其后仅允许词字符、空白、圆括号、点号和星号。
    /// 该字符集不含引号、分号、注释符（--、/*）、比较运算符与逗号，因而无法构造出注入语句。
    /// </summary>
    [GeneratedRegex(@"^[^\W\d][\w\s().*]*$")]
    private static partial Regex SafeExpressionRegex();

    /// <summary>
    /// 危险 SQL 关键字（以单词边界匹配），用于对表达式片段做二次防御。
    /// 注意：<c>create</c>、<c>update</c> 等词不会误伤 <c>create_time</c> 之类的列名，因为单词边界要求其后为非词字符。
    /// </summary>
    [GeneratedRegex(@"\b(select|insert|update|delete|drop|alter|create|truncate|union|exec|execute|declare|from|where|having|join|sleep|benchmark|load_file|outfile|information_schema)\b", RegexOptions.IgnoreCase)]
    private static partial Regex SqlKeywordRegex();

    /// <summary>
    /// SQL 默认值表达式：允许不含引号的常量 / 函数（如 0、CURRENT_TIMESTAMP、now()）、空字符串字面量 <c>''</c>，
    /// 也兼容被单引号整体包裹的字面量写法（如 'abc'、'O''Brien'，其中内部引号须以 '' 转义）。
    /// 两种写法均拒绝分号、反斜杠与未配对引号；形如 <c>''''</c> 这类仅由引号组成的退化写法语义歧义，亦一并拒绝。
    /// </summary>
    [GeneratedRegex(@"^(?:[^'\\;]+|''|'(?=[^'])(?:[^'\\;]|'')*')$")]
    private static partial Regex DefaultValueRegex();

    /// <summary>SQL 数据类型：字母、数字、下划线与空格（如 int、varchar、double precision）</summary>
    [GeneratedRegex(@"^[A-Za-z0-9_ ]+$")]
    private static partial Regex SqlTypeRegex();

    /// <summary>
    /// 判断是否为合法的数据库标识符（表名/列名）
    /// </summary>
    /// <param name="identifier">待校验的标识符</param>
    /// <returns>合法返回 true，否则返回 false</returns>
    public static bool IsValidIdentifier(string identifier)
        => !string.IsNullOrWhiteSpace(identifier)
           && identifier.Length <= MaxIdentifierLength
           && IdentifierRegex().IsMatch(identifier);

    /// <summary>
    /// 校验数据库标识符（表名/列名），不合法则抛出异常
    /// </summary>
    /// <param name="identifier">待校验的标识符</param>
    /// <param name="paramName">参数名称（用于异常提示）</param>
    /// <returns>原样返回校验通过的标识符</returns>
    public static string ValidateIdentifier(string identifier, string paramName)
    {
        if (!IsValidIdentifier(identifier))
            throw Oops.Oh($"参数【{paramName}】不是合法的数据库标识符，仅允许字母（含中文）、数字、下划线和美元符号，且长度不超过 {MaxIdentifierLength} 个字符");

        return identifier;
    }

    /// <summary>
    /// 判断是否为合法的存储过程名（支持单层 schema 前缀）
    /// </summary>
    /// <param name="procedureName">待校验的存储过程名</param>
    /// <returns>合法返回 true，否则返回 false</returns>
    public static bool IsValidProcedureName(string procedureName)
        => !string.IsNullOrWhiteSpace(procedureName)
           && procedureName.Length <= MaxIdentifierLength * 2 + 1
           && ProcedureNameRegex().IsMatch(procedureName);

    /// <summary>
    /// 校验存储过程名，不合法则抛出异常
    /// </summary>
    /// <param name="procedureName">待校验的存储过程名</param>
    /// <param name="paramName">参数名称（用于异常提示）</param>
    /// <returns>原样返回校验通过的存储过程名</returns>
    public static string ValidateProcedureName(string procedureName, string paramName)
    {
        if (!IsValidProcedureName(procedureName))
            throw Oops.Oh($"参数【{paramName}】不是合法的存储过程名，仅允许字母（含中文）、数字、下划线、美元符号以及单层 schema 前缀");

        return procedureName;
    }

    /// <summary>
    /// 判断是否为安全的 SQL 表达式片段（字段名或函数表达式，如 count(*)、sum(amount)、tb.age）
    /// </summary>
    /// <param name="expression">待校验的表达式</param>
    /// <returns>安全返回 true，否则返回 false</returns>
    public static bool IsSafeSqlExpression(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression) || expression.Length > MaxExpressionLength)
            return false;

        var trimmed = expression.Trim();
        return SafeExpressionRegex().IsMatch(trimmed)
               && HasBalancedParentheses(trimmed)
               && !SqlKeywordRegex().IsMatch(trimmed);
    }

    /// <summary>
    /// 校验 SQL 表达式片段（字段名或函数表达式），不合法则抛出异常
    /// </summary>
    /// <param name="expression">待校验的表达式</param>
    /// <param name="paramName">参数名称（用于异常提示）</param>
    /// <returns>原样返回校验通过的表达式</returns>
    public static string ValidateSqlExpression(string expression, string paramName)
    {
        if (!IsSafeSqlExpression(expression))
            throw Oops.Oh($"参数【{paramName}】不是合法的 SQL 字段或函数表达式，仅允许字段名或聚合函数（如 count(*)、sum(amount)）");

        return expression;
    }

    /// <summary>
    /// 校验 SQL 默认值表达式，不合法则抛出异常。
    /// <para>
    /// 默认值在部分数据库（如 PostgreSQL、Oscar）的 DDL 模板中不会被引号包裹，直接拼接到语句末尾，
    /// 故拒绝分号、反斜杠与未配对引号等可截断语句或越出字面量的字符。
    /// </para>
    /// <para>
    /// 兼容两种写法：直接填内容（<c>abc</c>），或按 SQL 字面量习惯加单引号（<c>'abc'</c>）。
    /// 由于各数据库的 DDL 模板会自行决定是否补引号，这里统一剥离外层单引号后再返回，
    /// 使 <c>'abc'</c> 与 <c>abc</c> 的最终效果一致，避免在 MySQL / Oracle 等库上拼出 <c>''abc''</c>。
    /// 空字符串字面量 <c>''</c> 属于 SqlSugar 约定的写法，保持原样不剥离。
    /// </para>
    /// </summary>
    /// <param name="value">待校验的默认值</param>
    /// <param name="paramName">参数名称（用于异常提示）</param>
    /// <returns>去除首尾空白（并剥离外层单引号）后的默认值；入参为空时原样返回</returns>
    public static string ValidateDefaultValue(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        var trimmed = value.Trim();
        if (trimmed.Length > MaxExpressionLength || !DefaultValueRegex().IsMatch(trimmed))
            throw Oops.Oh($"参数【{paramName}】不是合法的默认值，仅允许常量或函数（如 0、CURRENT_TIMESTAMP、abc）、空字符串 ''，以及被单引号包裹的写法（如 'abc'）");

        // 兼容已带单引号包裹的写法：剥离外层引号，交由各数据库 DDL 模板自行补引号；
        // 但空字符串字面量 '' 是 SqlSugar 约定的写法，须原样保留，否则会被上游当成“未填默认值”而跳过。
        if (trimmed.Length >= 2 && trimmed[0] == '\'' && trimmed[^1] == '\'')
            return trimmed.Length == 2 ? trimmed : trimmed[1..^1];

        return trimmed;
    }

    /// <summary>
    /// 校验 SQL 数据类型，不合法则抛出异常（如 int、varchar、decimal、double precision）
    /// </summary>
    /// <param name="dataType">待校验的数据类型</param>
    /// <param name="paramName">参数名称（用于异常提示）</param>
    /// <returns>去除首尾空白后的数据类型；入参为空时原样返回</returns>
    public static string ValidateSqlType(string dataType, string paramName)
    {
        if (string.IsNullOrWhiteSpace(dataType))
            return dataType;

        var trimmed = dataType.Trim();
        if (trimmed.Length > MaxIdentifierLength || !SqlTypeRegex().IsMatch(trimmed))
            throw Oops.Oh($"参数【{paramName}】不是合法的数据类型，仅允许字母、数字、下划线与空格（如 int、varchar）");

        return trimmed;
    }

    /// <summary>
    /// 将字符串转义为可安全嵌入单引号字符串字面量的形式（如表 / 列注释、默认值），防止越出引号造成注入。
    /// </summary>
    /// <param name="value">原始字符串</param>
    /// <param name="dbType">目标数据库类型；MySQL 默认启用反斜杠转义，需额外转义反斜杠</param>
    /// <returns>转义后的字符串；入参为空时原样返回</returns>
    public static string EscapeSqlLiteral(string value, SqlSugar.DbType dbType)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var escaped = value.Replace("'", "''");
        if (dbType is SqlSugar.DbType.MySql or SqlSugar.DbType.MySqlConnector)
            escaped = escaped.Replace("\\", "\\\\");

        return escaped;
    }

    /// <summary>
    /// 判断字符串中的圆括号是否成对且顺序正确
    /// </summary>
    /// <param name="input">待校验字符串</param>
    /// <returns>配对返回 true，否则返回 false</returns>
    private static bool HasBalancedParentheses(string input)
    {
        var depth = 0;
        foreach (var c in input)
        {
            if (c == '(')
            {
                depth++;
            }
            else if (c == ')')
            {
                depth--;
                if (depth < 0) return false;
            }
        }

        return depth == 0;
    }
}
