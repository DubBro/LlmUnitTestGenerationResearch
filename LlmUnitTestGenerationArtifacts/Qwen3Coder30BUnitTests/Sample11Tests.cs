using Dataset.Sample11;

namespace Qwen3Coder30BUnitTests;

public class MatrixChainMultiplicationTests
{
    [Fact]
    public void MatrixChainOrder_ValidInput_ReturnsCorrectMinimumMultiplications()
    {
        // Arrange
        int[] p = { 1, 2, 3, 4 };
        int n = 4;
        int[,] expectedS = new int[4, 4];

        // Act
        int result = MatrixChainMultiplication.MatrixChainOrder(p, n, out int[,] s);

        // Assert
        Assert.Equal(18, result);
    }

    [Fact]
    public void MatrixChainOrder_ValidInput_SetsCorrectSplitMatrix()
    {
        // Arrange
        int[] p = { 1, 2, 3, 4 };
        int n = 4;

        // Act
        MatrixChainMultiplication.MatrixChainOrder(p, n, out int[,] s);

        // Assert
        Assert.Equal(1, s[1, 3]);
    }

    [Theory]
    [InlineData(new int[] { 1, 2, 3 }, 3, "A1A2A3")]
    [InlineData(new int[] { 1, 2, 3, 4 }, 4, "((A1A2)A3)A4")]
    public void GetOptimalParentheses_ValidInput_ReturnsCorrectParenthesesString(int[] p, int n, string expected)
    {
        // Arrange
        MatrixChainMultiplication.MatrixChainOrder(p, n, out int[,] s);

        // Act
        string result = MatrixChainMultiplication.GetOptimalParentheses(s, 1, n - 1);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetOptimalParentheses_SingleMatrix_ReturnsMatrixName()
    {
        // Arrange
        int[,] s = new int[3, 3];
        int i = 2;
        int j = 2;

        // Act
        string result = MatrixChainMultiplication.GetOptimalParentheses(s, i, j);

        // Assert
        Assert.Equal("A2", result);
    }
}
