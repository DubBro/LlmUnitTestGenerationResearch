using System.Text.RegularExpressions;

namespace Dataset.Sample16;

public static class AsciiHelpers
{
    private static readonly Regex NonAsciiRegex = new Regex(@"[^\u0000-\u007F]", RegexOptions.Compiled);

    public static string ReplaceNonAsciiCharsWith(this string s, char r)
    {
        return string.IsNullOrWhiteSpace(s)
            ? string.Empty
            : NonAsciiRegex.Replace(s, r.ToString());
    }

    public static string RemoveNonAsciiChars(this string s)
    {
        return string.IsNullOrWhiteSpace(s)
            ? string.Empty
            : NonAsciiRegex.Replace(s, string.Empty);
    }
}
