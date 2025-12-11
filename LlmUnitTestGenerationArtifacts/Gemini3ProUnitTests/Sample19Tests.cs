using Dataset.Sample19;

namespace Gemini3ProUnitTests;

public class StringExtensionsTests
{
    #region IsDigit

    [Theory]
    [InlineData('0')]
    [InlineData('1')]
    [InlineData('5')]
    [InlineData('9')]
    public void IsDigit_PassingDigitChar_ReturnsTrue(char c)
    {
        // Arrange
        // (Input provided via parameter)

        // Act
        var result = c.IsDigit();

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData('a')]
    [InlineData('Z')]
    [InlineData('/')] // ASCII 47 (before '0')
    [InlineData(':')] // ASCII 58 (after '9')
    [InlineData(' ')]
    [InlineData('$')]
    public void IsDigit_PassingNonDigitChar_ReturnsFalse(char c)
    {
        // Arrange
        // (Input provided via parameter)

        // Act
        var result = c.IsDigit();

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsInteger

    [Theory]
    [InlineData("0")]
    [InlineData("123")]
    [InlineData("-456")]
    [InlineData("2147483647")] // int.MaxValue
    [InlineData("-2147483648")] // int.MinValue
    [InlineData("  123  ")] // int.TryParse allows whitespace
    public void IsInteger_PassingValidIntegerString_ReturnsTrue(string s)
    {
        // Arrange
        // (Input provided via parameter)

        // Act
        var result = s.IsInteger();

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("12.34")]
    [InlineData("2147483648")] // int.MaxValue + 1
    public void IsInteger_PassingInvalidIntegerString_ReturnsFalse(string s)
    {
        // Arrange
        // (Input provided via parameter)

        // Act
        var result = s.IsInteger();

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsNumber

    [Theory]
    [InlineData("0")]
    [InlineData("123")]
    [InlineData("-456")]
    [InlineData("  789  ")] // double.TryParse allows whitespace
    public void IsNumber_PassingValidNumberString_ReturnsTrue(string s)
    {
        // Arrange
        // (Input provided via parameter)

        // Act
        var result = s.IsNumber();

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("12.34.56")]
    public void IsNumber_PassingInvalidNumberString_ReturnsFalse(string s)
    {
        // Arrange
        // (Input provided via parameter)

        // Act
        var result = s.IsNumber();

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Reverse

    [Theory]
    [InlineData("abc", "cba")]
    [InlineData("Hello", "olleH")]
    [InlineData("a", "a")]
    [InlineData("123456", "654321")]
    public void Reverse_PassingValidString_ReturnsReversedString(string input, string expected)
    {
        // Arrange
        // (Input provided via parameters)

        // Act
        var result = input.Reverse();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Reverse_PassingNullOrWhitespace_ReturnsOriginalString(string input)
    {
        // Arrange
        // (Input provided via parameters)

        // Act
        var result = input.Reverse();

        // Assert
        Assert.Same(input, result);
    }

    #endregion

    #region ToCsvCompatible

    [Theory]
    [InlineData("NormalString", "NormalString")]
    [InlineData("123", "123")]
    [InlineData("NoSpecialChars", "NoSpecialChars")]
    public void ToCsvCompatible_PassingSimpleString_ReturnsOriginalString(string input, string expected)
    {
        // Arrange
        // (Input provided via parameters)

        // Act
        var result = input.ToCsvCompatible();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Value with comma, inside", "\"Value with comma, inside\"")]
    [InlineData("Value with semicolon; inside", "\"Value with semicolon; inside\"")]
    [InlineData("Line\nBreak", "\"Line\nBreak\"")]
    [InlineData(" LeadingSpace", "\" LeadingSpace\"")]
    [InlineData("TrailingSpace ", "\"TrailingSpace \"")]
    [InlineData("Quote\"Inside", "\"Quote\"\"Inside\"")] // Replaces " with "" and wraps in ""
    [InlineData("A,B;C\"D", "\"A,B;C\"\"D\"")] // Mixed special characters
    public void ToCsvCompatible_PassingStringRequiringEscaping_ReturnsEscapedString(string input, string expected)
    {
        // Arrange
        // (Input provided via parameters)

        // Act
        var result = input.ToCsvCompatible();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ToCsvCompatible_PassingNullOrEmpty_ReturnsOriginalString(string input)
    {
        // Arrange
        // (Input provided via parameters)

        // Act
        var result = input.ToCsvCompatible();

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void ToCsvCompatible_PassingSingleSpace_ReturnsWrappedString()
    {
        // Arrange
        // " " -> Has leading/trailing space -> Wraps -> " "
        string input = " ";
        string expected = "\" \"";

        // Act
        var result = input.ToCsvCompatible();

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region GetFileExtension

    [Theory]
    [InlineData("file.txt", "txt")]
    [InlineData("archive.tar.gz", "gz")]
    [InlineData(".config", "config")]
    [InlineData("image.JPEG", "JPEG")]
    [InlineData("path/to/file.xml", "xml")]
    public void GetFileExtension_PassingValidStringWithExtension_ReturnsExtension(string input, string expected)
    {
        // Arrange
        // (Input provided via parameters)

        // Act
        var result = input.GetFileExtension();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("file.   txt   ", "txt")] // result is trimmed
    [InlineData("file.", "")] // dot is last char, substring is empty
    public void GetFileExtension_PassingStringWithWhitespaceAroundExtension_ReturnsTrimmedExtension(string input, string expected)
    {
        // Arrange
        // (Input provided via parameters)

        // Act
        var result = input.GetFileExtension();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetFileExtension_PassingStringWithoutExtension_ReturnsEmptyString()
    {
        // Arrange
        string input = "makefile";

        // Act
        var result = input.GetFileExtension();

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetFileExtension_PassingNullOrWhitespace_ReturnsOriginalString(string input)
    {
        // Arrange
        // (Input provided via parameters)

        // Act
        var result = input.GetFileExtension();

        // Assert
        Assert.Equal(input, result);
    }

    #endregion
}
