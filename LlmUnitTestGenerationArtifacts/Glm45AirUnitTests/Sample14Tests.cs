using Dataset.Sample14;

namespace Glm45AirUnitTests;

public class RhombusGeneratorTests
{
    [Fact]
    public void GetRhombus_PassingValidOddCount_ReturnsCorrectRhombus()
    {
        // Arrange
        int count = 3;

        // Act
        var result = RhombusGenerator.GetRhombus(count);

        // Assert
        Assert.Equal(3, result.GetLength(0));
        Assert.Equal(3, result.GetLength(1));
        Assert.Equal(1, result[0, 0]);
        Assert.Equal(0, result[0, 1]);
        Assert.Equal(2, result[0, 2]);
        Assert.Equal(0, result[1, 0]);
        Assert.Equal(0, result[1, 1]);
        Assert.Equal(0, result[1, 2]);
        Assert.Equal(3, result[2, 0]);
        Assert.Equal(0, result[2, 1]);
        Assert.Equal(4, result[2, 2]);
    }

    [Fact]
    public void GetRhombus_PassingValidLargerOddCount_ReturnsCorrectRhombus()
    {
        // Arrange
        int count = 5;

        // Act
        var result = RhombusGenerator.GetRhombus(count);

        // Assert
        Assert.Equal(5, result.GetLength(0));
        Assert.Equal(5, result.GetLength(1));

        // First quarter
        Assert.Equal(1, result[0, 0]);
        Assert.Equal(1, result[0, 1]);
        Assert.Equal(0, result[0, 2]);
        Assert.Equal(0, result[0, 3]);
        Assert.Equal(0, result[0, 4]);
        Assert.Equal(0, result[1, 0]);
        Assert.Equal(1, result[1, 1]);
        Assert.Equal(0, result[1, 2]);
        Assert.Equal(0, result[1, 3]);
        Assert.Equal(0, result[1, 4]);
        Assert.Equal(0, result[2, 0]);
        Assert.Equal(0, result[2, 1]);
        Assert.Equal(0, result[2, 2]);
        Assert.Equal(0, result[2, 3]);
        Assert.Equal(0, result[2, 4]);
        Assert.Equal(0, result[3, 0]);
        Assert.Equal(0, result[3, 1]);
        Assert.Equal(0, result[3, 2]);
        Assert.Equal(0, result[3, 3]);
        Assert.Equal(0, result[3, 4]);
        Assert.Equal(0, result[4, 0]);
        Assert.Equal(0, result[4, 1]);
        Assert.Equal(0, result[4, 2]);
        Assert.Equal(0, result[4, 3]);
        Assert.Equal(0, result[4, 4]);

        // Second quarter
        Assert.Equal(2, result[0, 4]);
        Assert.Equal(2, result[1, 4]);
        Assert.Equal(0, result[2, 4]);
        Assert.Equal(0, result[3, 4]);
        Assert.Equal(0, result[4, 4]);

        // Third quarter
        Assert.Equal(3, result[4, 0]);
        Assert.Equal(3, result[4, 1]);
        Assert.Equal(0, result[4, 2]);
        Assert.Equal(0, result[4, 3]);
        Assert.Equal(0, result[4, 4]);

        // Fourth quarter
        Assert.Equal(4, result[4, 4]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(2)]
    [InlineData(4)]
    public void GetRhombus_PassingInvalidCount_ThrowsException(int count)
    {
        // Arrange
        // No additional arrangement needed.

        // Act & Assert
        Assert.Throws<Exception>(() => RhombusGenerator.GetRhombus(count));
    }
}
