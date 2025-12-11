using System.Text;
using Dataset.Sample12;

namespace Gemini3ProUnitTests;

public class LUPDecompositionTests
{
    [Fact]
    public void Decompose_Valid3x3Matrix_CalculatesLUPCorrectly()
    {
        // Arrange
        double[,] matrix =
        {
            { 1.0, 2.0, 4.0 },
            { 2.0, 8.0, 1.0 },
            { 5.0, 2.0, 1.0 }
        };
        // The Decompose method modifies the input matrix, so we store a copy for verification.
        double[,] originalMatrix = (double[,])matrix.Clone();
        int n = 3;

        // Act
        LUPDecomposition.Decompose(matrix, out double[,] L, out double[,] U, out int[] P);

        // Assert
        Assert.NotNull(L);
        Assert.NotNull(U);
        Assert.NotNull(P);
        Assert.Equal(n, L.GetLength(0));
        Assert.Equal(n, L.GetLength(1));
        Assert.Equal(n, U.GetLength(0));
        Assert.Equal(n, U.GetLength(1));
        Assert.Equal(n, P.Length);

        // Verify L is lower triangular with 1s on the diagonal
        for (int i = 0; i < n; i++)
        {
            Assert.Equal(1.0, L[i, i]);
            for (int j = i + 1; j < n; j++)
            {
                Assert.Equal(0.0, L[i, j]);
            }
        }

        // Verify U is upper triangular
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < i; j++)
            {
                Assert.Equal(0.0, U[i, j]);
            }
        }

        // Verify Decomposition: P * A = L * U
        // Meaning: The row i of (L*U) should equal the row P[i] of the original (unpermuted) A.
        double[,] luProduct = MultiplyMatrices(L, U);

        for (int i = 0; i < n; i++)
        {
            int originalRowIndex = P[i];
            for (int j = 0; j < n; j++)
            {
                Assert.Equal(originalMatrix[originalRowIndex, j], luProduct[i, j], 1e-9);
            }
        }
    }

    [Fact]
    public void Decompose_MatrixRequiringPivoting_SwapsRowsCorrectly()
    {
        // Arrange
        // Matrix with a zero on the main diagonal requires pivoting to avoid division by zero.
        double[,] matrix =
        {
            { 0.0, 1.0 },
            { 1.0, 0.0 }
        };
        double[,] originalMatrix = (double[,])matrix.Clone();
        int n = 2;

        // Act
        LUPDecomposition.Decompose(matrix, out double[,] L, out double[,] U, out int[] P);

        // Assert
        // Logic check: Since matrix[0,0] is 0 and matrix[1,0] is 1, a swap must occur.
        // P[0] should point to the original row index 1.
        Assert.Equal(1, P[0]);
        Assert.Equal(0, P[1]);

        // Verify mathematical correctness
        double[,] luProduct = MultiplyMatrices(L, U);
        for (int i = 0; i < n; i++)
        {
            int originalRowIndex = P[i];
            for (int j = 0; j < n; j++)
            {
                Assert.Equal(originalMatrix[originalRowIndex, j], luProduct[i, j], 1e-9);
            }
        }
    }

    [Fact]
    public void Decompose_SingularMatrix_ThrowsInvalidOperationException()
    {
        // Arrange
        // A singular matrix (rows are identical).
        double[,] matrix =
        {
            { 1.0, 2.0 },
            { 1.0, 2.0 }
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            LUPDecomposition.Decompose(matrix, out _, out _, out _));
    }

    [Fact]
    public void Decompose_ZeroMatrix_ThrowsInvalidOperationException()
    {
        // Arrange
        double[,] matrix =
        {
            { 0.0, 0.0 },
            { 0.0, 0.0 }
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            LUPDecomposition.Decompose(matrix, out _, out _, out _));
    }

    [Fact]
    public void Decompose_1x1Matrix_ReturnsIdentity()
    {
        // Arrange
        double[,] matrix = { { 5.0 } };

        // Act
        LUPDecomposition.Decompose(matrix, out double[,] L, out double[,] U, out int[] P);

        // Assert
        Assert.Equal(1.0, L[0, 0]);
        Assert.Equal(5.0, U[0, 0]);
        Assert.Equal(0, P[0]);
    }

    [Theory]
    [InlineData(1.5, 2.0, 3.0, 4.5)]
    [InlineData(-1.0, 0.0, 0.0, 1.0)]
    public void GetMatrix_ValidMatrix_ReturnsFormattedString(double d1, double d2, double d3, double d4)
    {
        // Arrange
        double[,] matrix = { { d1, d2 }, { d3, d4 } };

        // Build expected string using StringBuilder to mirror the implementation logic
        // and avoid locale specific hardcoding issues (e.g., comma vs dot).
        StringBuilder sb = new StringBuilder();
        sb.Append($"{d1}\t{d2}\t\n");
        sb.Append($"{d3}\t{d4}\t\n");
        string expected = sb.ToString();

        // Act
        string result = LUPDecomposition.GetMatrix(matrix);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetPermutationMatrix_IdentityPermutation_ReturnsTargetString()
    {
        // Arrange
        int[] P = { 0, 1 };
        string expected = "1\t0\t\n0\t1\t\n";

        // Act
        string result = LUPDecomposition.GetPermutationMatrix(P);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetPermutationMatrix_SwappedPermutation_ReturnsTargetString()
    {
        // Arrange
        int[] P = { 1, 0, 2 };
        // Row 0 has index 1 -> 0 1 0
        // Row 1 has index 0 -> 1 0 0
        // Row 2 has index 2 -> 0 0 1
        string expected = "0\t1\t0\t\n1\t0\t0\t\n0\t0\t1\t\n";

        // Act
        string result = LUPDecomposition.GetPermutationMatrix(P);

        // Assert
        Assert.Equal(expected, result);
    }

    private static double[,] MultiplyMatrices(double[,] A, double[,] B)
    {
        int n = A.GetLength(0);
        double[,] C = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                double sum = 0;
                for (int k = 0; k < n; k++)
                {
                    sum += A[i, k] * B[k, j];
                }
                C[i, j] = sum;
            }
        }
        return C;
    }
}
