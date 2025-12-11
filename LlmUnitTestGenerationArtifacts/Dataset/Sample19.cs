namespace Dataset.Sample19;

public static class StringExtensions
{
    public static bool IsDigit(this char c) => c >= '0' && c <= '9';

    public static bool IsInteger(this string s) => !string.IsNullOrWhiteSpace(s) && int.TryParse(s, out _);

    public static bool IsNumber(this string s) => !string.IsNullOrWhiteSpace(s) && double.TryParse(s, out _);

    public static string Reverse(this string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return s;

        var chars = s.ToCharArray();
        Array.Reverse(chars);

        return new string(chars);
    }

    public static string ToCsvCompatible(this string s)
    {
        if (string.IsNullOrEmpty(s))
            return s;

        if (s.Contains('"'))
            s = s.Replace("\"", "\"\"");

        if (s.Contains(',') ||
            s.Contains(';') ||
            s.Contains('"') ||
            s.Contains('\n') ||
            s[0] == ' ' ||
            s[s.Length - 1] == ' ')
        {
            s = $"\"{s}\"";
        }

        return s;
    }

    public static string GetFileExtension(this string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return s;

        var pos = s.LastIndexOf('.');

        return pos < 0
            ? string.Empty
            : s.Substring(pos + 1).Trim();
    }
}
