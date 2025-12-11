using Dataset.Sample17;

namespace Gpt5MiniUnitTests
{
    public class VectorExtensionsTests
    {
        [Fact]
        public void Copy_WhenCalled_ReturnsNewArrayWithSameElements()
        {
            // Arrange
            var original = new double[] { 1.0, 2.0, 3.0 };

            // Act
            var copy = original.Copy();

            // Assert
            Assert.False(ReferenceEquals(original, copy));
            Assert.Equal(original, copy);
            original[0] = 99.0;
            Assert.Equal(1.0, copy[0]);
        }

        [Fact]
        public void Copy_EmptyArray_ReturnsEmptyArray()
        {
            // Arrange
            var original = Array.Empty<double>();

            // Act
            var copy = original.Copy();

            // Assert
            Assert.False(ReferenceEquals(original, copy));
            Assert.Empty(copy);
        }

        [Fact]
        public void OuterProduct_WhenCalled_ReturnsCorrectMatrix()
        {
            // Arrange
            var lhs = new double[] { 1.0, 2.0 };
            var rhs = new double[] { 3.0, 4.0, 5.0 };

            // Act
            var result = lhs.OuterProduct(rhs);

            // Assert
            Assert.Equal(2, result.GetLength(0));
            Assert.Equal(3, result.GetLength(1));
            Assert.Equal(3.0, result[0, 0]);
            Assert.Equal(4.0, result[0, 1]);
            Assert.Equal(5.0, result[0, 2]);
            Assert.Equal(6.0, result[1, 0]);
            Assert.Equal(8.0, result[1, 1]);
            Assert.Equal(10.0, result[1, 2]);
        }

        [Fact]
        public void OuterProduct_WithEmptyRight_ReturnsZeroWidthMatrix()
        {
            // Arrange
            var lhs = new double[] { 1.0, 2.0 };
            var rhs = Array.Empty<double>();

            // Act
            var result = lhs.OuterProduct(rhs);

            // Assert
            Assert.Equal(2, result.GetLength(0));
            Assert.Equal(0, result.GetLength(1));
        }

        [Fact]
        public void Dot_WhenSameLength_ReturnsCorrectResult()
        {
            // Arrange
            var a = new double[] { 1.0, 2.0, 3.0 };
            var b = new double[] { 4.0, 5.0, 6.0 };

            // Act
            var result = a.Dot(b);

            // Assert
            Assert.Equal(32.0, result);
        }

        [Fact]
        public void Dot_DifferentLengths_ThrowsArgumentException()
        {
            // Arrange
            var a = new double[] { 1.0, 2.0 };
            var b = new double[] { 3.0 };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => a.Dot(b));
        }

        [Fact]
        public void Dot_EmptyArrays_ReturnsZero()
        {
            // Arrange
            var a = Array.Empty<double>();
            var b = Array.Empty<double>();

            // Act
            var result = a.Dot(b);

            // Assert
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void Magnitude_WhenCalled_ReturnsSqrtOfDotProduct()
        {
            // Arrange
            var vector = new double[] { 3.0, 4.0 };

            // Act
            var result = vector.Magnitude();

            // Assert
            Assert.Equal(5.0, result);
        }

        [Fact]
        public void Magnitude_EmptyArray_ReturnsZero()
        {
            // Arrange
            var vector = Array.Empty<double>();

            // Act
            var result = vector.Magnitude();

            // Assert
            Assert.Equal(0.0, result);
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(1.0)]
        [InlineData(-2.0)]
        public void Scale_WhenCalled_ReturnsScaledArray(double factor)
        {
            // Arrange
            var input = new double[] { 1.0, -2.0, 3.0 };

            // Act
            var result = input.Scale(factor);

            // Assert
            var expected = new double[] { 1.0 * factor, -2.0 * factor, 3.0 * factor };
            Assert.Equal(expected, result);
            Assert.False(ReferenceEquals(input, result));
        }

        [Fact]
        public void ToColumnVector_WhenCalled_ReturnsMatrixWithSingleColumn()
        {
            // Arrange
            var source = new double[] { 1.0, 2.0, 3.0 };

            // Act
            var column = source.ToColumnVector();

            // Assert
            Assert.Equal(3, column.GetLength(0));
            Assert.Equal(1, column.GetLength(1));
            Assert.Equal(1.0, column[0, 0]);
            Assert.Equal(2.0, column[1, 0]);
            Assert.Equal(3.0, column[2, 0]);
        }

        [Fact]
        public void ToRowVector_WhenMatrixHasSingleColumn_ReturnsArray()
        {
            // Arrange
            var column = new double[3, 1];
            column[0, 0] = 7.0;
            column[1, 0] = 8.0;
            column[2, 0] = 9.0;

            // Act
            var row = column.ToRowVector();

            // Assert
            var expected = new double[] { 7.0, 8.0, 9.0 };
            Assert.Equal(expected, row);
        }

        [Fact]
        public void ToRowVector_WhenWidthGreaterThanOne_ThrowsInvalidOperationException()
        {
            // Arrange
            var matrix = new double[2, 2];

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => matrix.ToRowVector());
        }

        [Fact]
        public void ToDiagonalMatrix_WhenCalled_ReturnsSquareMatrixWithDiagonalSet()
        {
            // Arrange
            var vector = new double[] { 1.0, 2.0, 3.0 };

            // Act
            var matrix = vector.ToDiagonalMatrix();

            // Assert
            Assert.Equal(3, matrix.GetLength(0));
            Assert.Equal(3, matrix.GetLength(1));
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == j)
                    {
                        Assert.Equal(vector[i], matrix[i, j]);
                    }
                    else
                    {
                        Assert.Equal(0.0, matrix[i, j]);
                    }
                }
            }
        }

        [Fact]
        public void ToDiagonalMatrix_EmptyArray_ReturnsZeroByZeroMatrix()
        {
            // Arrange
            var vector = Array.Empty<double>();

            // Act
            var matrix = vector.ToDiagonalMatrix();

            // Assert
            Assert.Equal(0, matrix.GetLength(0));
            Assert.Equal(0, matrix.GetLength(1));
        }
    }
}
