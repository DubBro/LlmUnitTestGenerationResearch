using Dataset.Sample17;

namespace Gemini3ProUnitTests;

public class VectorExtensionsTests
{
    [Fact]
    public void Copy_WhenCalledWithPopulatedVector_ReturnsNewArrayWithSameValues()
    {
        // Arrange
        double[] vector = { 1.5, 2.5, 3.5 };

        // Act
        var result = vector.Copy();

        // Assert
        Assert.NotSame(vector, result);
        Assert.Equal(vector, result);
    }

    [Fact]
    public void Copy_WhenCalledWithEmptyVector_ReturnsEmptyArray()
    {
        // Arrange
        double[] vector = Array.Empty<double>();

        // Act
        var result = vector.Copy();

        // Assert
        Assert.NotSame(vector, result);
        Assert.Empty(result);
    }

    [Fact]
    public void OuterProduct_WhenCalledWithValidVectors_ReturnsCorrectResult()
    {
        // Arrange
        double[] lhs = { 1.0, 2.0, 3.0 };
        double[] rhs = { 4.0, 5.0 };

        // Act
        var result = lhs.OuterProduct(rhs);

        // Assert
        Assert.Equal(3, result.GetLength(0));
        Assert.Equal(2, result.GetLength(1));
        Assert.Equal(4.0, result[0, 0]);
        Assert.Equal(5.0, result[0, 1]);
        Assert.Equal(8.0, result[1, 0]);
        Assert.Equal(10.0, result[1, 1]);
        Assert.Equal(12.0, result[2, 0]);
        Assert.Equal(15.0, result[2, 1]);
    }

    [Fact]
    public void Dot_WhenCalledWithVectorsOfSameDimension_ReturnsDotProduct()
    {
        // Arrange
        double[] lhs = { 1.0, 3.0, -5.0 };
        double[] rhs = { 4.0, -2.0, -1.0 };

        // Act
        var result = lhs.Dot(rhs);

        // Assert
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void Dot_WhenCalledWithVectorsOfDifferentDimension_ThrowsArgumentException()
    {
        // Arrange
        double[] lhs = { 1.0, 2.0 };
        double[] rhs = { 1.0, 2.0, 3.0 };

        // Act
        Action act = () => lhs.Dot(rhs);

        // Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("Arguments must have same dimension.", exception.Message);
    }

    [Fact]
    public void Dot_WhenCalledWithEmptyVectors_ReturnsZero()
    {
        // Arrange
        double[] lhs = Array.Empty<double>();
        double[] rhs = Array.Empty<double>();

        // Act
        var result = lhs.Dot(rhs);

        // Assert
        Assert.Equal(0.0, result);
    }

    [Theory]
    [InlineData(new[] { 3.0, 4.0 }, 5.0)]
    [InlineData(new[] { 1.0 }, 1.0)]
    [InlineData(new[] { -5.0 }, 5.0)]
    [InlineData(new double[] { }, 0.0)]
    public void Magnitude_WhenCalled_ReturnsEuclideanMagnitude(double[] vector, double expected)
    {
        // Arrange
        // Input provided via Theory

        // Act
        var result = vector.Magnitude();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Scale_WhenCalledWithFactor_ReturnsScaledVector()
    {
        // Arrange
        double[] vector = { 1.0, -2.0, 0.0 };
        double factor = 2.5;
        double[] expected = { 2.5, -5.0, 0.0 };

        // Act
        var result = vector.Scale(factor);

        // Assert
        Assert.NotSame(vector, result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToColumnVector_WhenCalledWithVector_ReturnsColumnMatrix()
    {
        // Arrange
        double[] vector = { 10.0, 20.0 };

        // Act
        var result = vector.ToColumnVector();

        // Assert
        Assert.Equal(2, result.GetLength(0));
        Assert.Equal(1, result.GetLength(1));
        Assert.Equal(10.0, result[0, 0]);
        Assert.Equal(20.0, result[1, 0]);
    }

    [Fact]
    public void ToRowVector_WhenSourceIsSingleColumn_ReturnsVector()
    {
        // Arrange
        double[,] source = new double[3, 1] { { 1.0 }, { 2.0 }, { 3.0 } };

        // Act
        var result = source.ToRowVector();

        // Assert
        Assert.Equal(new[] { 1.0, 2.0, 3.0 }, result);
    }

    [Fact]
    public void ToRowVector_WhenSourceIsNotSingleColumn_ThrowsInvalidOperationException()
    {
        // Arrange
        double[,] source = new double[2, 2] { { 1.0, 2.0 }, { 3.0, 4.0 } };

        // Act
        Action act = () => source.ToRowVector();

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(act);
        Assert.Equal("The column vector must have only 1 element in width.", exception.Message);
    }

    [Fact]
    public void ToDiagonalMatrix_WhenCalledWithVector_ReturnsSquareMatrixWithValuesOnDiagonal()
    {
        // Arrange
        double[] vector = { 1.0, 2.0 };

        // Act
        var result = vector.ToDiagonalMatrix();

        // Assert
        Assert.Equal(2, result.GetLength(0));
        Assert.Equal(2, result.GetLength(1));
        Assert.Equal(1.0, result[0, 0]);
        Assert.Equal(0.0, result[0, 1]);
        Assert.Equal(0.0, result[1, 0]);
        Assert.Equal(2.0, result[1, 1]);
    }
}
