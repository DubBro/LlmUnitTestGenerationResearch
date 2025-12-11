using Dataset.Sample14;

namespace DeepSeekR10528UnitTests;

public class RhombusGeneratorTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(2)]
    [InlineData(4)]
    public void GetRhombus_InvalidInput_ThrowsException(int count)
    {
        // Arrange

        // Act
        Action act = () => RhombusGenerator.GetRhombus(count);

        // Assert
        Exception ex = Assert.Throws<Exception>(act);
        Assert.Equal("Error. Invalid input", ex.Message);
    }

    [Fact]
    public void GetRhombus_Size1_ReturnsCorrectRhombus()
    {
        // Arrange
        int size = 1;
        int[,] expectedArray = { { 0 } };

        // Act
        var result = RhombusGenerator.GetRhombus(size);

        // Assert
        AssertArrayEqual(expectedArray, result);
    }

    [Fact]
    public void GetRhombus_Size3_ReturnsCorrectRhombus()
    {
        // Arrange
        int size = 3;
        int[,] expectedArray =
        {
            { 1, 0, 2 },
            { 0, 0, 0 },
            { 3, 0, 4 }
        };

        // Act
        var result = RhombusGenerator.GetRhombus(size);

        // Assert
        AssertArrayEqual(expectedArray, result);
    }

    [Fact]
    public void GetRhombus_Size5_ReturnsCorrectRhombus()
    {
        // Arrange
        int size = 5;
        int[,] expectedArray =
        {
            { 1, 1, 0, 2, 2 },
            { 1, 0, 0, 0, 2 },
            { 0, 0, 0, 0, 0 },
            { 3, 0, 0, 0, 4 },
            { 3, 3, 0, 4, 4 }
        };

        // Act
        var result = RhombusGenerator.GetRhombus(size);

        // Assert
        AssertArrayEqual(expectedArray, result);
    }

    private void AssertArrayEqual(int[,] expected, int[,] actual)
    {
        Assert.Equal(expected.GetLength(0), actual.GetLength(0));
        Assert.Equal(expected.GetLength(1), actual.GetLength(1));

        for (int i = 0; i < expected.GetLength(0); i++)
        {
            for (int j = 0; j < expected.GetLength(1); j++)
            {
                Assert.Equal(expected[i, j], actual[i, j]);
            }
        }
    }
}
