using Dataset.Sample16;

namespace Glm45AirUnitTests;

public class AsciiHelpersTests
{
    public class ReplaceNonAsciiCharsWith
    {
        [Fact]
        public void ReplaceNonAsciiCharsWith_NullInput_ReturnsEmptyString()
        {
            // Arrange
            string input = null;
            char replacement = ' ';

            // Act
            string result = input.ReplaceNonAsciiCharsWith(replacement);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ReplaceNonAsciiCharsWith_EmptyInput_ReturnsEmptyString()
        {
            // Arrange
            string input = string.Empty;
            char replacement = ' ';

            // Act
            string result = input.ReplaceNonAsciiCharsWith(replacement);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ReplaceNonAsciiCharsWith_WhitespaceInput_ReturnsEmptyString()
        {
            // Arrange
            string input = "   ";
            char replacement = ' ';

            // Act
            string result = input.ReplaceNonAsciiCharsWith(replacement);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ReplaceNonAsciiCharsWith_InputWithNoNonAsciiChars_ReturnsSameString()
        {
            // Arrange
            string input = "Hello World";
            char replacement = ' ';

            // Act
            string result = input.ReplaceNonAsciiCharsWith(replacement);

            // Assert
            Assert.Equal(input, result);
        }

        [Fact]
        public void ReplaceNonAsciiCharsWith_InputWithNonAsciiChars_ReturnsStringWithReplacedChars()
        {
            // Arrange
            string input = "Héllo Wörld";
            char replacement = ' ';

            // Act
            string result = input.ReplaceNonAsciiCharsWith(replacement);

            // Assert
            Assert.Equal("H llo W rld", result);
        }

        [Theory]
        [InlineData(' ')]
        [InlineData('_')]
        [InlineData('*')]
        public void ReplaceNonAsciiCharsWith_DifferentReplacementChars_ReplacesCorrectly(char replacement)
        {
            // Arrange
            string input = "Héllo Wörld";

            // Act
            string result = input.ReplaceNonAsciiCharsWith(replacement);

            // Assert
            string expected = $"H{replacement}llo W{replacement}rld";
            Assert.Equal(expected, result);
        }
    }

    public class RemoveNonAsciiChars
    {
        [Fact]
        public void RemoveNonAsciiChars_NullInput_ReturnsEmptyString()
        {
            // Arrange
            string input = null;

            // Act
            string result = input.RemoveNonAsciiChars();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void RemoveNonAsciiChars_EmptyInput_ReturnsEmptyString()
        {
            // Arrange
            string input = string.Empty;

            // Act
            string result = input.RemoveNonAsciiChars();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void RemoveNonAsciiChars_WhitespaceInput_ReturnsEmptyString()
        {
            // Arrange
            string input = "   ";

            // Act
            string result = input.RemoveNonAsciiChars();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void RemoveNonAsciiChars_InputWithNoNonAsciiChars_ReturnsSameString()
        {
            // Arrange
            string input = "Hello World";

            // Act
            string result = input.RemoveNonAsciiChars();

            // Assert
            Assert.Equal(input, result);
        }

        [Fact]
        public void RemoveNonAsciiChars_InputWithNonAsciiChars_ReturnsStringWithRemovedChars()
        {
            // Arrange
            string input = "Héllo Wörld";

            // Act
            string result = input.RemoveNonAsciiChars();

            // Assert
            Assert.Equal("Hllo Wrld", result);
        }
    }
}
