using Dataset.Sample17;

namespace Qwen3Coder30BUnitTests
{
    public class VectorExtensionsTests
    {
        [Fact]
        public void Copy_WithValidVector_ReturnsCopyOfVector()
        {
            // Arrange
            var vector = new double[] { 1.0, 2.0, 3.0 };

            // Act
            var result = vector.Copy();

            // Assert
            Assert.Equal(vector, result);
            Assert.NotSame(vector, result);
        }

        [Fact]
        public void OuterProduct_WithValidVectors_ReturnsCorrectOuterProduct()
        {
            // Arrange
            var lhs = new double[] { 1.0, 2.0 };
            var rhs = new double[] { 3.0, 4.0 };

            // Act
            var result = lhs.OuterProduct(rhs);

            // Assert
            Assert.Equal(2, result.GetLength(0));
            Assert.Equal(2, result.GetLength(1));
            Assert.Equal(3.0, result[0, 0]);
            Assert.Equal(4.0, result[0, 1]);
            Assert.Equal(6.0, result[1, 0]);
            Assert.Equal(8.0, result[1, 1]);
        }

        [Fact]
        public void Dot_WithEqualLengthVectors_ReturnsDotProduct()
        {
            // Arrange
            var lhs = new double[] { 1.0, 2.0, 3.0 };
            var rhs = new double[] { 4.0, 5.0, 6.0 };

            // Act
            var result = lhs.Dot(rhs);

            // Assert
            Assert.Equal(32.0, result);
        }

        [Fact]
        public void Dot_WithDifferentLengthVectors_ThrowsArgumentException()
        {
            // Arrange
            var lhs = new double[] { 1.0, 2.0 };
            var rhs = new double[] { 3.0, 4.0, 5.0 };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => lhs.Dot(rhs));
            Assert.Equal("Arguments must have same dimension.", exception.Message);
        }

        [Fact]
        public void Magnitude_WithValidVector_ReturnsCorrectMagnitude()
        {
            // Arrange
            var vector = new double[] { 3.0, 4.0 };

            // Act
            var result = vector.Magnitude();

            // Assert
            Assert.Equal(5.0, result);
        }

        [Fact]
        public void Scale_WithValidVectorAndFactor_ReturnsScaledVector()
        {
            // Arrange
            var vector = new double[] { 1.0, 2.0, 3.0 };
            const double factor = 2.0;

            // Act
            var result = vector.Scale(factor);

            // Assert
            Assert.Equal(new double[] { 2.0, 4.0, 6.0 }, result);
        }

        [Fact]
        public void ToColumnVector_WithValidVector_ReturnsColumnVector()
        {
            // Arrange
            var source = new double[] { 1.0, 2.0, 3.0 };

            // Act
            var result = source.ToColumnVector();

            // Assert
            Assert.Equal(3, result.GetLength(0));
            Assert.Equal(1, result.GetLength(1));
            Assert.Equal(1.0, result[0, 0]);
            Assert.Equal(2.0, result[1, 0]);
            Assert.Equal(3.0, result[2, 0]);
        }

        [Fact]
        public void ToRowVector_WithValidColumnVector_ReturnsRowVector()
        {
            // Arrange
            var source = new double[,] { { 1.0 }, { 2.0 }, { 3.0 } };

            // Act
            var result = source.ToRowVector();

            // Assert
            Assert.Equal(new double[] { 1.0, 2.0, 3.0 }, result);
        }

        [Fact]
        public void ToRowVector_WithInvalidColumnVector_ThrowsInvalidOperationException()
        {
            // Arrange
            var source = new double[,] { { 1.0, 2.0 }, { 3.0, 4.0 } };

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => source.ToRowVector());
            Assert.Equal("The column vector must have only 1 element in width.", exception.Message);
        }

        [Fact]
        public void ToDiagonalMatrix_WithValidVector_ReturnsDiagonalMatrix()
        {
            // Arrange
            var vector = new double[] { 1.0, 2.0, 3.0 };

            // Act
            var result = vector.ToDiagonalMatrix();

            // Assert
            Assert.Equal(3, result.GetLength(0));
            Assert.Equal(3, result.GetLength(1));
            Assert.Equal(1.0, result[0, 0]);
            Assert.Equal(2.0, result[1, 1]);
            Assert.Equal(3.0, result[2, 2]);
            Assert.Equal(0.0, result[0, 1]);
            Assert.Equal(0.0, result[1, 0]);
        }
    }
}
