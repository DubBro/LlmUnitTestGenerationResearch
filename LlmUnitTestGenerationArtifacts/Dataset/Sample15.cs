using System.Text;

namespace Dataset.Sample15;

public static class Base64Helpers
{
    public static string Encode(this string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
            return string.Empty;

        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        return Convert.ToBase64String(plainTextBytes);
    }

    public static string Decode(this string base64EncodedData)
    {
        if (string.IsNullOrWhiteSpace(base64EncodedData))
            return string.Empty;

        byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);

        return Encoding.UTF8.GetString(base64EncodedBytes);
    }
}
