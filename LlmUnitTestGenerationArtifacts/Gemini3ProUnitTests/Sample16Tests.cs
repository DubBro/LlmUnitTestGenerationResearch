using Dataset.Sample16;

namespace Gemini3ProUnitTests;

public class AsciiHelpersTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\r\n")]
    public void ReplaceNonAsciiCharsWith_WhenInputIsNullOrWhiteSpace_ReturnsEmptyString(string? input)
    {
        // Arrange
        char replacement = '?';

        // Act
        var result = AsciiHelpers.ReplaceNonAsciiCharsWith(input!, replacement);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("Hello World", "Hello World")]
    [InlineData("1234567890", "1234567890")]
    [InlineData("!@#$%^&*()_+", "!@#$%^&*()_+")]
    public void ReplaceNonAsciiCharsWith_WhenInputContainsOnlyAscii_ReturnsOriginalString(string input, string expected)
    {
        // Arrange
        char replacement = '*';

        // Act
        var result = input.ReplaceNonAsciiCharsWith(replacement);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Héllo", '?', "H?llo")]
    [InlineData("Café", '*', "Caf*")]
    [InlineData("© 2023", '_', "_ 2023")]
    [InlineData("Zoë", 'e', "Zoe")]
    [InlineData("—", '-', "-")]
    public void ReplaceNonAsciiCharsWith_WhenInputContainsNonAscii_ReturnsStringWithReplacements(string input, char replacement, string expected)
    {
        // Arrange
        // Handled by InlineData

        // Act
        var result = input.ReplaceNonAsciiCharsWith(replacement);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ReplaceNonAsciiCharsWith_WhenInputIsOnlyNonAscii_ReplaceAllCharacters()
    {
        // Arrange
        string input = "äöü";
        char replacement = 'X';
        string expected = "XXX";

        // Act
        var result = input.ReplaceNonAsciiCharsWith(replacement);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void RemoveNonAsciiChars_WhenInputIsNullOrWhiteSpace_ReturnsEmptyString(string? input)
    {
        // Arrange
        // Handled by InlineData

        // Act
        var result = AsciiHelpers.RemoveNonAsciiChars(input!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("Hello World", "Hello World")]
    [InlineData("test_user@example.com", "test_user@example.com")]
    public void RemoveNonAsciiChars_WhenInputContainsOnlyAscii_ReturnsOriginalString(string input, string expected)
    {
        // Arrange
        // Handled by InlineData

        // Act
        var result = input.RemoveNonAsciiChars();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Héllo", "Hllo")]
    [InlineData("Café", "Caf")]
    [InlineData("© 2023", " 2023")]
    [InlineData("Smiley ☺", "Smiley ")]
    public void RemoveNonAsciiChars_WhenInputContainsNonAscii_RemovesNonAsciiCharacters(string input, string expected)
    {
        // Arrange
        // Handled by InlineData

        // Act
        var result = input.RemoveNonAsciiChars();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RemoveNonAsciiChars_WhenInputIsOnlyNonAsciiAndNotWhitespace_ReturnsEmptyString()
    {
        // Arrange
        // Usage of non-whitespace non-ASCII characters to bypass IsNullOrWhiteSpace check
        // but still be removed by the RegEx.
        string input = "µ§¶";

        // Act
        var result = input.RemoveNonAsciiChars();

        // Assert
        Assert.Equal(string.Empty, result);
    }
}
