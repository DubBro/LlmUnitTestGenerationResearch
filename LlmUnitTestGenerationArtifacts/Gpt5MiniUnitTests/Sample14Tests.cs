using Dataset.Sample14;

namespace Gpt5MiniUnitTests
{
    public class RhombusGeneratorTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(-1)]
        [InlineData(4)]
        public void GetRhombus_InvalidCount_ThrowsException(int count)
        {
            // Arrange

            // Act
            var ex = Assert.Throws<Exception>(() => RhombusGenerator.GetRhombus(count));

            // Assert
            Assert.Equal("Error. Invalid input", ex.Message);
        }

        [Fact]
        public void GetRhombus_CountIs1_ReturnsSingleZero()
        {
            // Arrange
            int count = 1;

            // Act
            int[,] result = RhombusGenerator.GetRhombus(count);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.GetLength(0));
            Assert.Equal(1, result.GetLength(1));
            Assert.Equal(0, result[0, 0]);
        }

        [Fact]
        public void GetRhombus_CountIs3_ReturnsExpectedPattern()
        {
            // Arrange
            int count = 3;
            int[,] expected =
            {
                { 1, 0, 2 },
                { 0, 0, 0 },
                { 3, 0, 4 }
            };

            // Act
            int[,] result = RhombusGenerator.GetRhombus(count);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(count, result.GetLength(0));
            Assert.Equal(count, result.GetLength(1));
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    Assert.Equal(expected[i, j], result[i, j]);
                }
            }
        }

        [Fact]
        public void GetRhombus_CountIs5_ReturnsExpectedPattern()
        {
            // Arrange
            int count = 5;
            int[,] expected =
            {
                { 1, 1, 0, 2, 2 },
                { 1, 0, 0, 0, 2 },
                { 0, 0, 0, 0, 0 },
                { 3, 0, 0, 0, 4 },
                { 3, 3, 0, 4, 4 }
            };

            // Act
            int[,] result = RhombusGenerator.GetRhombus(count);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(count, result.GetLength(0));
            Assert.Equal(count, result.GetLength(1));
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    Assert.Equal(expected[i, j], result[i, j]);
                }
            }
        }
    }
}
