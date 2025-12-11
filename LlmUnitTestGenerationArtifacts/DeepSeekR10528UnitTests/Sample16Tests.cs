using Dataset.Sample16;

namespace DeepSeekR10528UnitTests;

public class AsciiHelpersTests
{
    [Theory]
    [InlineData(null, '*', "")]
    [InlineData("", '*', "")]
    [InlineData("   ", '*', "")]
    [InlineData("Hello", '*', "Hello")]
    [InlineData("Café", '*', "Caf*")]
    [InlineData("Mötörhëäd", '*', "M*t*rh*d")]
    [InlineData("東京", '*', "**")]
    [InlineData("aβc", '*', "a*c")]
    public void ReplaceNonAsciiCharsWith_GivenVariousInputs_ReturnsExpectedResult(string input, char replacement, string expected)
    {
        // Act
        var result = AsciiHelpers.ReplaceNonAsciiCharsWith(input, replacement);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData("Hello", "Hello")]
    [InlineData("Café", "Caf")]
    [InlineData("Mötörhëäd", "Mtrhd")]
    [InlineData("東京", "")]
    [InlineData("aβc", "ac")]
    public void RemoveNonAsciiChars_GivenVariousInputs_ReturnsExpectedResult(string input, string expected)
    {
        // Act
        var result = AsciiHelpers.RemoveNonAsciiChars(input);

        // Assert
        Assert.Equal(expected, result);
    }
}
