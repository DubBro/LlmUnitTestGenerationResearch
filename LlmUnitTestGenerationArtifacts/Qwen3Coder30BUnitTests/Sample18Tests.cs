using Dataset.Sample18;

namespace Qwen3Coder30BUnitTests
{
    public class CaesarEncoderTests
    {
        [Fact]
        public void Encode_WithValidTextAndKey_ReturnsEncodedText()
        {
            // Arrange
            var text = "Hello";
            var key = 3;

            // Act
            var result = CaesarEncoder.Encode(text, key);

            // Assert
            Assert.Equal("Khoor", result);
        }

        [Fact]
        public void Decode_WithValidTextAndKey_ReturnsDecodedText()
        {
            // Arrange
            var text = "Khoor";
            var key = 3;

            // Act
            var result = CaesarEncoder.Decode(text, key);

            // Assert
            Assert.Equal("Hello", result);
        }

        [Theory]
        [InlineData("ABC", 1, "BCD")]
        [InlineData("XYZ", 3, "ABC")]
        [InlineData("abc", 1, "bcd")]
        [InlineData("xyz", 3, "abc")]
        public void Encode_WithVariousInputs_ReturnsCorrectlyEncodedText(string input, int key, string expected)
        {
            // Arrange

            // Act
            var result = CaesarEncoder.Encode(input, key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("BCD", 1, "ABC")]
        [InlineData("ABC", 3, "XYZ")]
        [InlineData("bcd", 1, "abc")]
        [InlineData("abc", 3, "xyz")]
        public void Decode_WithVariousInputs_ReturnsCorrectlyDecodedText(string input, int key, string expected)
        {
            // Arrange

            // Act
            var result = CaesarEncoder.Decode(input, key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Encode_WithEmptyString_ReturnsEmptyString()
        {
            // Arrange
            var text = string.Empty;
            var key = 5;

            // Act
            var result = CaesarEncoder.Encode(text, key);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Decode_WithEmptyString_ReturnsEmptyString()
        {
            // Arrange
            var text = string.Empty;
            var key = 5;

            // Act
            var result = CaesarEncoder.Decode(text, key);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Encode_WithNonAlphabeticCharacters_ReturnsTextWithNonAlphabeticCharactersUnchanged()
        {
            // Arrange
            var text = "Hello, World!";
            var key = 3;

            // Act
            var result = CaesarEncoder.Encode(text, key);

            // Assert
            Assert.Equal("Khoor, Zruog!", result);
        }

        [Fact]
        public void Decode_WithNonAlphabeticCharacters_ReturnsTextWithNonAlphabeticCharactersUnchanged()
        {
            // Arrange
            var text = "Khoor, Zruog!";
            var key = 3;

            // Act
            var result = CaesarEncoder.Decode(text, key);

            // Assert
            Assert.Equal("Hello, World!", result);
        }

        [Fact]
        public void Encode_WithNegativeKey_ReturnsCorrectlyEncodedText()
        {
            // Arrange
            var text = "Hello";
            var key = -3;

            // Act
            var result = CaesarEncoder.Encode(text, key);

            // Assert
            Assert.Equal("Ebiil", result);
        }

        [Fact]
        public void Decode_WithNegativeKey_ReturnsCorrectlyDecodedText()
        {
            // Arrange
            var text = "Ebiil";
            var key = -3;

            // Act
            var result = CaesarEncoder.Decode(text, key);

            // Assert
            Assert.Equal("Hello", result);
        }

        [Theory]
        [InlineData("Hello", 26, "Hello")]
        [InlineData("Hello", 52, "Hello")]
        [InlineData("Hello", -26, "Hello")]
        public void Encode_WithKeyThatIsMultipleOf26_ReturnsOriginalText(string input, int key, string expected)
        {
            // Arrange

            // Act
            var result = CaesarEncoder.Encode(input, key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Hello", 26, "Hello")]
        [InlineData("Hello", 52, "Hello")]
        [InlineData("Hello", -26, "Hello")]
        public void Decode_WithKeyThatIsMultipleOf26_ReturnsOriginalText(string input, int key, string expected)
        {
            // Arrange

            // Act
            var result = CaesarEncoder.Decode(input, key);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Encode_WithKeyZero_ReturnsOriginalText()
        {
            // Arrange
            var text = "Hello";
            var key = 0;

            // Act
            var result = CaesarEncoder.Encode(text, key);

            // Assert
            Assert.Equal("Hello", result);
        }

        [Fact]
        public void Decode_WithKeyZero_ReturnsOriginalText()
        {
            // Arrange
            var text = "Hello";
            var key = 0;

            // Act
            var result = CaesarEncoder.Decode(text, key);

            // Assert
            Assert.Equal("Hello", result);
        }
    }
}
