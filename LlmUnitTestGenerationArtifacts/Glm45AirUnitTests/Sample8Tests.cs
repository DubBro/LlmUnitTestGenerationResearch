using Dataset.Sample8;

namespace Glm45AirUnitTests;

public class LuTests
{
    public class DecomposeTests
    {
        [Fact]
        public void Decompose_NonSquareMatrix_ThrowsArgumentException()
        {
            // Arrange
            var lu = new Lu();
            var nonSquareMatrix = new double[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };

            // Act
            var exception = Record.Exception(() => lu.Decompose(nonSquareMatrix));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Source matrix is not square shaped.", exception.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Decompose_SquareMatrix_ReturnsLUMatrices(int size)
        {
            // Arrange
            var lu = new Lu();
            var sourceMatrix = CreateSquareMatrix(size);

            // Act
            var result = lu.Decompose(sourceMatrix);

            // Assert
            Assert.NotNull(result.L);
            Assert.NotNull(result.U);
            Assert.Equal(size, result.L.GetLength(0));
            Assert.Equal(size, result.L.GetLength(1));
            Assert.Equal(size, result.U.GetLength(0));
            Assert.Equal(size, result.U.GetLength(1));
            Assert.Equal(1, result.L[0, 0]);
            Assert.Equal(1, result.L[1, 1]);
            Assert.Equal(1, result.L[size - 1, size - 1]);
        }

        [Fact]
        public void Decompose_IdentityMatrix_ReturnsLUAsIdentityMatrices()
        {
            // Arrange
            var lu = new Lu();
            var identityMatrix = new double[,] { { 1, 0 }, { 0, 1 } };

            // Act
            var result = lu.Decompose(identityMatrix);

            // Assert
            Assert.Equal(identityMatrix, result.L);
            Assert.Equal(identityMatrix, result.U);
        }

        private static double[,] CreateSquareMatrix(int size)
        {
            var matrix = new double[size, size];
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    matrix[i, j] = i * size + j + 1;
                }
            }
            return matrix;
        }
    }

    public class EliminateTests
    {
        [Fact]
        public void Eliminate_NonSquareMatrix_ThrowsArgumentException()
        {
            // Arrange
            var lu = new Lu();
            var nonSquareMatrix = new double[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            var coefficients = new double[] { 1, 2, 3 };

            // Act
            var exception = Record.Exception(() => lu.Eliminate(nonSquareMatrix, coefficients));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Matrix of equation coefficients is not square shaped.", exception.Message);
        }

        [Theory]
        [InlineData(1, new double[] { 1 }, new double[] { 1 })]
        [InlineData(2, new double[] { 1, 2 }, new double[] { 1, 1 })]
        [InlineData(3, new double[] { 1, 2, 3 }, new double[] { 1, 1, 1 })]
        public void Eliminate_SquareMatrix_ReturnsSolution(int size, double[] coefficients, double[] expectedSolution)
        {
            // Arrange
            var lu = new Lu();
            var matrix = CreateSquareMatrix(size);

            // Act
            var solution = lu.Eliminate(matrix, coefficients);

            // Assert
            Assert.NotNull(solution);
            Assert.Equal(size, solution.Length);
            for (var i = 0; i < size; i++)
            {
                Assert.Equal(expectedSolution[i], solution[i], 6);
            }
        }

        [Fact]
        public void Eliminate_IdentityMatrixAndCoefficients_ReturnsSameCoefficientsAsSolution()
        {
            // Arrange
            var lu = new Lu();
            var identityMatrix = new double[,] { { 1, 0 }, { 0, 1 } };
            var coefficients = new[] { 1.5, 2.5 };

            // Act
            var solution = lu.Eliminate(identityMatrix, coefficients);

            // Assert
            Assert.Equal(coefficients, solution);
        }

        private static double[,] CreateSquareMatrix(int size)
        {
            var matrix = new double[size, size];
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    matrix[i, j] = i * size + j + 1;
                }
            }
            return matrix;
        }
    }
}
