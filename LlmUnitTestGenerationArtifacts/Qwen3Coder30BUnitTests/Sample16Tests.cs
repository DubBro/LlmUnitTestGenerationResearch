using Dataset.Sample16;

namespace Qwen3Coder30BUnitTests;

public class AsciiHelpersTests
{
    [Fact]
    public void ReplaceNonAsciiCharsWith_InputIsNull_ReturnsEmptyString()
    {
        // Arrange
        string input = null;

        // Act
        var result = input.ReplaceNonAsciiCharsWith('x');

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ReplaceNonAsciiCharsWith_InputIsEmptyString_ReturnsEmptyString()
    {
        // Arrange
        string input = string.Empty;

        // Act
        var result = input.ReplaceNonAsciiCharsWith('x');

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ReplaceNonAsciiCharsWith_InputContainsOnlyAsciiChars_ReturnsOriginalString()
    {
        // Arrange
        string input = "Hello World";

        // Act
        var result = input.ReplaceNonAsciiCharsWith('x');

        // Assert
        Assert.Equal("Hello World", result);
    }

    [Fact]
    public void ReplaceNonAsciiCharsWith_InputContainsNonAsciiChars_ReplacesWithSpecifiedChar()
    {
        // Arrange
        string input = "Hello Wørld";

        // Act
        var result = input.ReplaceNonAsciiCharsWith('x');

        // Assert
        Assert.Equal("Hello Wxld", result);
    }

    [Fact]
    public void ReplaceNonAsciiCharsWith_InputContainsOnlyNonAsciiChars_ReplacesAllWithSpecifiedChar()
    {
        // Arrange
        string input = "Wørld";

        // Act
        var result = input.ReplaceNonAsciiCharsWith('x');

        // Assert
        Assert.Equal("xxx", result);
    }

    [Fact]
    public void RemoveNonAsciiChars_InputIsNull_ReturnsEmptyString()
    {
        // Arrange
        string input = null;

        // Act
        var result = input.RemoveNonAsciiChars();

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void RemoveNonAsciiChars_InputIsEmptyString_ReturnsEmptyString()
    {
        // Arrange
        string input = string.Empty;

        // Act
        var result = input.RemoveNonAsciiChars();

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void RemoveNonAsciiChars_InputContainsOnlyAsciiChars_ReturnsOriginalString()
    {
        // Arrange
        string input = "Hello World";

        // Act
        var result = input.RemoveNonAsciiChars();

        // Assert
        Assert.Equal("Hello World", result);
    }

    [Fact]
    public void RemoveNonAsciiChars_InputContainsNonAsciiChars_RemovesAllNonAsciiChars()
    {
        // Arrange
        string input = "Hello Wørld";

        // Act
        var result = input.RemoveNonAsciiChars();

        // Assert
        Assert.Equal("Hello Wld", result);
    }

    [Theory]
    [InlineData("Hello Wørld", "Hello Wld")]
    [InlineData("Café", "Caf")]
    [InlineData("naïve", "nave")]
    public void RemoveNonAsciiChars_InputWithVariousNonAsciiChars_RemovesAllNonAsciiChars(string input, string expected)
    {
        // Act
        var result = input.RemoveNonAsciiChars();

        // Assert
        Assert.Equal(expected, result);
    }
}
