using Dataset.Sample8;

namespace DeepSeekR10528UnitTests;

public class LuTests
{
    private readonly Lu _lu = new Lu();

    [Fact]
    public void Decompose_MatrixIsNotSquare_ThrowsArgumentException()
    {
        // Arrange
        var nonSquareMatrix = new double[2, 3];

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _lu.Decompose(nonSquareMatrix));
    }

    [Fact]
    public void Decompose_1x1Matrix_ReturnsIdentityAndUpperMatrix()
    {
        // Arrange
        var matrix = new double[1, 1] { { 5 } };

        // Act
        var (l, u) = _lu.Decompose(matrix);

        // Assert
        Assert.Equal(1, l[0, 0]);
        Assert.Equal(5, u[0, 0]);
    }

    [Fact]
    public void Decompose_2x2Matrix_CorrectDecomposition()
    {
        // Arrange
        var matrix = new double[2, 2] { { 4, 3 }, { 6, 3 } };
        double[,] expectedL = { { 1, 0 }, { 1.5, 1 } };
        double[,] expectedU = { { 4, 3 }, { 0, -1.5 } };

        // Act
        var (l, u) = _lu.Decompose(matrix);

        // Assert
        Assert.Equal(expectedL[0, 0], l[0, 0]);
        Assert.Equal(expectedL[0, 1], l[0, 1]);
        Assert.Equal(expectedL[1, 0], l[1, 0]);
        Assert.Equal(expectedL[1, 1], l[1, 1]);
        Assert.Equal(expectedU[0, 0], u[0, 0]);
        Assert.Equal(expectedU[0, 1], u[0, 1]);
        Assert.Equal(expectedU[1, 0], u[1, 0]);
        Assert.Equal(expectedU[1, 1], u[1, 1]);
    }

    [Fact]
    public void Eliminate_MatrixIsNotSquare_ThrowsArgumentException()
    {
        // Arrange
        var nonSquareMatrix = new double[2, 3];
        var coefficients = new double[2];

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _lu.Eliminate(nonSquareMatrix, coefficients));
    }

    [Fact]
    public void Eliminate_1x1System_ReturnsCorrectSolution()
    {
        // Arrange
        var matrix = new double[1, 1] { { 2 } };
        var coefficients = new double[] { 8 };
        var expectedSolution = new double[] { 4 };

        // Act
        var solution = _lu.Eliminate(matrix, coefficients);

        // Assert
        Assert.Equal(expectedSolution[0], solution[0]);
    }

    [Fact]
    public void Eliminate_2x2System_ReturnsCorrectSolution()
    {
        // Arrange
        var matrix = new double[2, 2] { { 3, 1 }, { 1, 2 } };
        var coefficients = new double[] { 9, 8 };
        var expectedSolution = new double[] { 2, 3 };

        // Act
        var solution = _lu.Eliminate(matrix, coefficients);

        // Assert
        Assert.Equal(expectedSolution[0], solution[0], 5);
        Assert.Equal(expectedSolution[1], solution[1], 5);
    }

    [Fact]
    public void Eliminate_ZeroInUDiagonal_ThrowsDivideByZeroException()
    {
        // Arrange
        var matrix = new double[2, 2] { { 0, 0 }, { 0, 0 } };
        var coefficients = new double[] { 1, 2 };

        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => _lu.Eliminate(matrix, coefficients));
    }
}
