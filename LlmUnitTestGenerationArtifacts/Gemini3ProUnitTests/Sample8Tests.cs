using Dataset.Sample8;

namespace Gemini3ProUnitTests;

public class LuTests
{
    [Fact]
    public void Decompose_WhenMatrixIsNotSquare_ThrowsArgumentException()
    {
        // Arrange
        var lu = new Lu();
        var matrix = new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => lu.Decompose(matrix));
        Assert.Equal("Source matrix is not square shaped.", exception.Message);
    }

    [Fact]
    public void Decompose_WhenMatrixIsSquare_ReturnsCorrectLowerAndUpperMatrices()
    {
        // Arrange
        var lu = new Lu();
        var matrix = new double[,]
        {
            { 2.0, -1.0, -2.0 },
            { -4.0, 6.0, 3.0 },
            { -4.0, -2.0, 8.0 }
        };
        int pivot = 3;

        // Act
        var (l, u) = lu.Decompose(matrix);

        // Assert
        // Verify dimensions
        Assert.Equal(pivot, l.GetLength(0));
        Assert.Equal(pivot, l.GetLength(1));
        Assert.Equal(pivot, u.GetLength(0));
        Assert.Equal(pivot, u.GetLength(1));

        // Verify L is lower triangular with 1s on diagonal
        for (int i = 0; i < pivot; i++)
        {
            for (int j = 0; j < pivot; j++)
            {
                if (i == j)
                {
                    Assert.Equal(1.0, l[i, j]);
                }
                else if (j > i)
                {
                    Assert.Equal(0.0, l[i, j]);
                }
            }
        }

        // Verify U is upper triangular
        for (int i = 0; i < pivot; i++)
        {
            for (int j = 0; j < i; j++)
            {
                Assert.Equal(0.0, u[i, j]);
            }
        }

        // Verify L * U equals original matrix
        for (int i = 0; i < pivot; i++)
        {
            for (int j = 0; j < pivot; j++)
            {
                double sum = 0;
                for (int k = 0; k < pivot; k++)
                {
                    sum += l[i, k] * u[k, j];
                }
                Assert.Equal(matrix[i, j], sum, 8);
            }
        }
    }

    [Fact]
    public void Eliminate_WhenMatrixIsNotSquare_ThrowsArgumentException()
    {
        // Arrange
        var lu = new Lu();
        var matrix = new double[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 }
        };
        var coefficients = new double[] { 1, 2 };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => lu.Eliminate(matrix, coefficients));
        Assert.Equal("Matrix of equation coefficients is not square shaped.", exception.Message);
    }

    [Fact]
    public void Eliminate_WhenSystemIs1x1_ReturnsCorrectSolution()
    {
        // Arrange
        var lu = new Lu();
        // Equation: 4x = 8 => x = 2
        var matrix = new double[,] { { 4.0 } };
        var coefficients = new double[] { 8.0 };

        // Act
        var solution = lu.Eliminate(matrix, coefficients);

        // Assert
        Assert.NotNull(solution);
        Assert.Single(solution);
        Assert.Equal(2.0, solution[0], 8);
    }

    [Fact]
    public void Eliminate_WhenSystemIs2x2_ReturnsCorrectSolution()
    {
        // Arrange
        var lu = new Lu();
        // System:
        // 2x + y = 5
        // 3x - 2y = 4
        // Solution: x = 2, y = 1
        var matrix = new double[,]
        {
            { 2.0, 1.0 },
            { 3.0, -2.0 }
        };
        var coefficients = new double[] { 5.0, 4.0 };

        // Act
        var solution = lu.Eliminate(matrix, coefficients);

        // Assert
        Assert.Equal(2, solution.Length);
        Assert.Equal(2.0, solution[0], 8);
        Assert.Equal(1.0, solution[1], 8);
    }

    [Fact]
    public void Eliminate_WhenSystemIs3x3_ReturnsCorrectSolution()
    {
        // Arrange
        var lu = new Lu();
        // System:
        // x + y + z = 6
        // 2x + y - z = 1
        // 2x - y + z = 3
        // Solution: x = 1, y = 2, z = 3
        var matrix = new double[,]
        {
            { 1.0, 1.0, 1.0 },
            { 2.0, 1.0, -1.0 },
            { 2.0, -1.0, 1.0 }
        };
        var coefficients = new double[] { 6.0, 1.0, 3.0 };

        // Act
        var solution = lu.Eliminate(matrix, coefficients);

        // Assert
        Assert.Equal(3, solution.Length);
        Assert.Equal(1.0, solution[0], 8);
        Assert.Equal(2.0, solution[1], 8);
        Assert.Equal(3.0, solution[2], 8);
    }
}
