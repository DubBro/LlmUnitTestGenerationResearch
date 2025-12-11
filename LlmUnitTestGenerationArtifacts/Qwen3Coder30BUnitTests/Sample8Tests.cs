using Dataset.Sample8;

namespace Qwen3Coder30BUnitTests
{
    public class LuTests
    {
        [Fact]
        public void Decompose_SourceMatrixIsNotSquare_ThrowsArgumentException()
        {
            // Arrange
            var lu = new Lu();
            var source = new double[2, 3];

            // Act
            Action act = () => lu.Decompose(source);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void Decompose_SourceMatrixIsSquare_ReturnsLAndU()
        {
            // Arrange
            var lu = new Lu();
            var source = new double[,]
            {
                { 2, 1 },
                { 4, 3 }
            };

            // Act
            var (L, U) = lu.Decompose(source);

            // Assert
            Assert.Equal(2, L.GetLength(0));
            Assert.Equal(2, L.GetLength(1));
            Assert.Equal(2, U.GetLength(0));
            Assert.Equal(2, U.GetLength(1));
        }

        [Fact]
        public void Eliminate_MatrixIsNotSquare_ThrowsArgumentException()
        {
            // Arrange
            var lu = new Lu();
            var matrix = new double[2, 3];
            var coefficients = new double[2];

            // Act
            Action act = () => lu.Eliminate(matrix, coefficients);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void Eliminate_MatrixAndCoefficientsAreValid_ReturnsSolution()
        {
            // Arrange
            var lu = new Lu();
            var matrix = new double[,]
            {
                { 2, 1 },
                { 4, 3 }
            };
            var coefficients = new double[] { 5, 11 };

            // Act
            var result = lu.Eliminate(matrix, coefficients);

            // Assert
            Assert.Equal(2, result.Length);
            Assert.Equal(1, result[0], 10);
            Assert.Equal(3, result[1], 10);
        }
    }
}
