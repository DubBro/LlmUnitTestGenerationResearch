using Dataset.Sample15;

namespace Qwen3Coder30BUnitTests
{
    public class Base64HelpersTests
    {
        [Fact]
        public void Encode_NullString_ReturnsEmptyString()
        {
            // Arrange
            string input = null;

            // Act
            var result = input.Encode();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Encode_EmptyString_ReturnsEmptyString()
        {
            // Arrange
            string input = string.Empty;

            // Act
            var result = input.Encode();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Encode_WhitespaceString_ReturnsEmptyString()
        {
            // Arrange
            string input = "   ";

            // Act
            var result = input.Encode();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Encode_ValidString_ReturnsBase64EncodedString()
        {
            // Arrange
            string input = "Hello, World!";

            // Act
            var result = input.Encode();

            // Assert
            Assert.Equal("SGVsbG8sIFdvcmxkIQ==", result);
        }

        [Fact]
        public void Decode_NullString_ReturnsEmptyString()
        {
            // Arrange
            string input = null;

            // Act
            var result = input.Decode();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Decode_EmptyString_ReturnsEmptyString()
        {
            // Arrange
            string input = string.Empty;

            // Act
            var result = input.Decode();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Decode_WhitespaceString_ReturnsEmptyString()
        {
            // Arrange
            string input = "   ";

            // Act
            var result = input.Decode();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Decode_ValidBase64String_ReturnsDecodedString()
        {
            // Arrange
            string input = "SGVsbG8sIFdvcmxkIQ==";

            // Act
            var result = input.Decode();

            // Assert
            Assert.Equal("Hello, World!", result);
        }

        [Theory]
        [InlineData("AQ==", "A")]
        [InlineData("SGVsbG8=", "Hello")]
        [InlineData("V29ybGQh", "World!")]
        public void Decode_ValidBase64Strings_ReturnsCorrectDecodedStrings(string base64String, string expected)
        {
            // Arrange

            // Act
            var result = base64String.Decode();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Encode_InputNullOrWhitespace_ReturnsEmptyString(string input)
        {
            // Arrange

            // Act
            var result = input.Encode();

            // Assert
            Assert.Equal(string.Empty, result);
        }
    }
}
