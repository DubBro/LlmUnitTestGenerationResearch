using Dataset.Sample15;

namespace DeepSeekR10528UnitTests;

public class Base64HelpersTests
{
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    public void Encode_WithNullOrEmptyOrWhitespace_ReturnsEmptyString(string input, string expected)
    {
        // Arrange

        // Act
        var result = input.Encode();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Hello", "SGVsbG8=")]
    [InlineData("World", "V29ybGQ=")]
    [InlineData("Hello, World!", "SGVsbG8sIFdvcmxkIQ==")]
    [InlineData("12345", "MTIzNDU=")]
    [InlineData("αβγ", "zrHOss6z")]
    public void Encode_ValidInput_ReturnsBase64EncodedString(string input, string expected)
    {
        // Arrange

        // Act
        var result = input.Encode();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    public void Decode_WithNullOrEmptyOrWhitespace_ReturnsEmptyString(string input, string expected)
    {
        // Arrange

        // Act
        var result = input.Decode();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("SGVsbG8=", "Hello")]
    [InlineData("V29ybGQ=", "World")]
    [InlineData("SGVsbG8sIFdvcmxkIQ==", "Hello, World!")]
    [InlineData("MTIzNDU=", "12345")]
    [InlineData("zrHOss6z", "αβγ")]
    public void Decode_ValidBase64Input_ReturnsDecodedString(string input, string expected)
    {
        // Arrange

        // Act
        var result = input.Decode();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("InvalidBase64!@#$")]
    [InlineData("SGVsbG8sIFdvcmxkIQ")] // Missing padding
    public void Decode_WithInvalidBase64Input_ThrowsFormatException(string input)
    {
        // Arrange

        // Act & Assert
        Assert.Throws<FormatException>(() => input.Decode());
    }
}
