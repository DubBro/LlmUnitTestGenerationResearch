using Dataset.Sample18;

namespace Gpt5MiniUnitTests
{
    public class CaesarEncoderTests
    {
        [Theory]
        [InlineData("abc", 1, "bcd")]
        [InlineData("abc", 27, "bcd")]
        [InlineData("abc", -1, "zab")]
        [InlineData("abc", 52, "abc")]
        public void Encode_Lowercase_KeyVariations_ReturnsExpected(string input, int key, string expected)
        {
            // Arrange

            // Act
            var actual = CaesarEncoder.Encode(input, key);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("ABC", 2, "CDE")]
        [InlineData("XYZ", 4, "BCD")]
        public void Encode_Uppercase_ShiftsAndWraps_ReturnsExpected(string input, int key, string expected)
        {
            // Arrange

            // Act
            var actual = CaesarEncoder.Encode(input, key);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Encode_PreservesNonLetterCharacters_ReturnsExpected()
        {
            // Arrange
            var input = "Hello, World! 123";
            var key = 5;
            var expected = "Mjqqt, Btwqi! 123";

            // Act
            var actual = CaesarEncoder.Encode(input, key);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Hello, World!", 1)]
        [InlineData("Hello, World!", 26)]
        [InlineData("Hello, World!", 52)]
        [InlineData("The quick brown fox.", -5)]
        [InlineData("SampleText123!", 27)]
        public void Decode_WhenApplied_AfterEncode_ReturnsOriginal(string original, int key)
        {
            // Arrange
            var encoded = CaesarEncoder.Encode(original, key);

            // Act
            var decoded = CaesarEncoder.Decode(encoded, key);

            // Assert
            Assert.Equal(original, decoded);
        }

        [Fact]
        public void Encode_EmptyString_ReturnsEmptyString()
        {
            // Arrange
            var input = string.Empty;
            var key = 10;

            // Act
            var result = CaesarEncoder.Encode(input, key);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Encode_Null_ThrowsNullReferenceException()
        {
            // Arrange
            string? input = null;
            var key = 3;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => CaesarEncoder.Encode(input!, key));
        }

        [Theory]
        [InlineData("abcdefghijklmnopqrstuvwxyz", 13, "nopqrstuvwxyzabcdefghijklm")]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 13, "NOPQRSTUVWXYZABCDEFGHIJKLM")]
        public void Encode_Rot13_WorksAsExpected(string input, int key, string expected)
        {
            // Arrange

            // Act
            var actual = CaesarEncoder.Encode(input, key);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
