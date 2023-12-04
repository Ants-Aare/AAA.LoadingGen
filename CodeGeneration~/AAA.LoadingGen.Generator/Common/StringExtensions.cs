using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AAA.SourceGenerators.Common;

public static class StringExtensions
{
    public static string FirstCharToUpper(this string input)
        => input[0].ToString().ToUpper() + input.Substring(1);
    public static string FirstCharToLower(this string input)
        => input[0].ToString().ToLower() + input.Substring(1);

    public static string CamelCaseToSpaced(this string input)
        => Regex.Replace(input, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");

    static readonly char[] NamespaceSeparatorToken = { '.' };

    public static string GetTypeNameIdentifier(this string input, int index = 0)
    {
        var strings = input.Split(NamespaceSeparatorToken);
        if (strings is { Length: 0 })
            return string.Empty;
        return index switch
        {
            < 0 when -index <= strings.Length => strings[strings.Length + index],
            >= 0 when index <= strings.Length => strings[index],
            _ => string.Empty
        };
    }

    public static int GetStableHashCode(this string str)
    {
        unchecked
        {
            var hash1 = 5381;
            var hash2 = hash1;

            for (var i = 0; i < str.Length && str[i] != '\0'; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1 || str[i + 1] == '\0')
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
            }

            return hash1 + (hash2 * 1566083941);
        }
    }
    
}