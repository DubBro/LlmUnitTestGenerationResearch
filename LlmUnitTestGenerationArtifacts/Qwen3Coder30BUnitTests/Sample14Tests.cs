using Dataset.Sample14;

namespace Qwen3Coder30BUnitTests
{
    public class RhombusGeneratorTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(2)]
        [InlineData(4)]
        public void GetRhombus_WithInvalidInput_ThrowsException(int count)
        {
            // Arrange
            // Act
            Action act = () => RhombusGenerator.GetRhombus(count);

            // Assert
            Assert.Throws<Exception>(act);
        }

        [Fact]
        public void GetRhombus_WithValidInput_ReturnsCorrectRhombus()
        {
            // Arrange
            int expectedSize = 5;
            int[,] expectedRhombus = new int[5, 5]
            {
                { 1, 1, 1, 2, 2 },
                { 1, 1, 0, 2, 2 },
                { 1, 0, 0, 0, 2 },
                { 3, 0, 0, 4, 4 },
                { 3, 3, 0, 4, 4 }
            };

            // Act
            int[,] result = RhombusGenerator.GetRhombus(5);

            // Assert
            Assert.Equal(expectedSize, result.GetLength(0));
            Assert.Equal(expectedSize, result.GetLength(1));

            for (int i = 0; i < expectedSize; i++)
            {
                for (int j = 0; j < expectedSize; j++)
                {
                    Assert.Equal(expectedRhombus[i, j], result[i, j]);
                }
            }
        }

        [Fact]
        public void GetRhombus_WithSize3_ReturnsCorrectRhombus()
        {
            // Arrange
            int expectedSize = 3;
            int[,] expectedRhombus = new int[3, 3]
            {
                { 1, 1, 2 },
                { 1, 0, 2 },
                { 3, 0, 4 }
            };

            // Act
            int[,] result = RhombusGenerator.GetRhombus(3);

            // Assert
            Assert.Equal(expectedSize, result.GetLength(0));
            Assert.Equal(expectedSize, result.GetLength(1));

            for (int i = 0; i < expectedSize; i++)
            {
                for (int j = 0; j < expectedSize; j++)
                {
                    Assert.Equal(expectedRhombus[i, j], result[i, j]);
                }
            }
        }
    }
}
