using Dataset.Sample12;

namespace Gpt5MiniUnitTests
{
    public class LUPDecompositionTests
    {
        [Fact]
        public void Decompose_SingularMatrix_ThrowsInvalidOperationException()
        {
            // Arrange
            double[,] singular = new double[,]
            {
                { 0, 1 },
                { 0, 2 }
            };

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => LUPDecomposition.Decompose(singular, out _, out _, out _));

            // Assert
            Assert.Equal("Матриця є виродженою.", ex.Message);
        }

        [Fact]
        public void Decompose_OneByOneMatrix_ReturnsExpectedLAndUAndP()
        {
            // Arrange
            double[,] matrix = new double[,] { { 5.0 } };

            // Act
            LUPDecomposition.Decompose((double[,])matrix.Clone(), out double[,] L, out double[,] U, out int[] P);

            // Assert
            Assert.Single(P);
            Assert.Equal(0, P[0]);

            Assert.Equal(1, L[0, 0]);
            Assert.Equal(5, U[0, 0]);
        }

        [Fact]
        public void Decompose_KnownMatrix_ReconstructsPermutedOriginal()
        {
            // Arrange
            double[,] original = new double[,]
            {
                { 2.0, 3.0, 1.0 },
                { 4.0, 7.0, 7.0 },
                { 6.0, 18.0, 22.0 }
            };
            double[,] matrixToDecompose = (double[,])original.Clone();

            // Act
            LUPDecomposition.Decompose(matrixToDecompose, out double[,] L, out double[,] U, out int[] P);

            // Assert
            int n = original.GetLength(0);
            Assert.Equal(n, L.GetLength(0));
            Assert.Equal(n, U.GetLength(0));
            Assert.Equal(n, P.Length);

            // L should have ones on diagonal
            for (int i = 0; i < n; i++)
                Assert.Equal(1.0, L[i, i], 9);

            // U should be upper triangular (elements below diagonal zero)
            for (int i = 1; i < n; i++)
                for (int j = 0; j < i; j++)
                    Assert.Equal(0.0, U[i, j], 9);

            // Compute L*U
            double[,] LU = MultiplyMatrices(L, U);

            // Compute permuted original matrix PA where PA[i,j] = original[P[i], j]
            double[,] PA = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    PA[i, j] = original[P[i], j];

            // Compare LU and PA with tolerance
            double tolerance = 1e-9;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    Assert.InRange(Math.Abs(LU[i, j] - PA[i, j]), 0, tolerance);
        }

        [Fact]
        public void GetMatrix_ReturnsExpectedStringRepresentation()
        {
            // Arrange
            double[,] matrix = new double[,]
            {
                { 1.0, 2.0 },
                { 3.0, 4.0 }
            };

            // Act
            string result = LUPDecomposition.GetMatrix(matrix);

            // Assert
            string expected = "1\t2\t\n3\t4\t\n";
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(new int[] { 0, 1, 2 }, "1\t0\t0\t\n0\t1\t0\t\n0\t0\t1\t\n")]
        [InlineData(new int[] { 2, 0, 1 }, "0\t0\t1\t\n1\t0\t0\t\n0\t1\t0\t\n")]
        public void GetPermutationMatrix_ReturnsExpectedString(int[] permutation, string expected)
        {
            // Arrange & Act
            string result = LUPDecomposition.GetPermutationMatrix(permutation);

            // Assert
            Assert.Equal(expected, result);
        }

        private static double[,] MultiplyMatrices(double[,] A, double[,] B)
        {
            int n = A.GetLength(0);
            int m = B.GetLength(1);
            int common = A.GetLength(1);
            var result = new double[n, m];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < common; k++)
                        sum += A[i, k] * B[k, j];
                    result[i, j] = sum;
                }
            return result;
        }
    }
}
