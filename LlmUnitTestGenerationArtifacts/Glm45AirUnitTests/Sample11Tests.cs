using Dataset.Sample11;

namespace Glm45AirUnitTests;

public class MatrixChainMultiplicationTests
{
    [Fact]
    public void MatrixChainOrder_ValidInput_ReturnsMinimumMultiplications()
    {
        // Arrange
        int[] p = { 10, 20, 30, 40, 30 };
        int n = p.Length;
        int[,] s;

        // Act
        int result = MatrixChainMultiplication.MatrixChainOrder(p, n, out s);

        // Assert
        Assert.Equal(30000, result);
    }

    [Fact]
    public void MatrixChainOrder_SingleMatrix_ReturnsZero()
    {
        // Arrange
        int[] p = { 10 };
        int n = p.Length;
        int[,] s;

        // Act
        int result = MatrixChainMultiplication.MatrixChainOrder(p, n, out s);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void MatrixChainOrder_TwoMatrices_ReturnsSingleMultiplication()
    {
        // Arrange
        int[] p = { 10, 20, 30 };
        int n = p.Length;
        int[,] s;

        // Act
        int result = MatrixChainMultiplication.MatrixChainOrder(p, n, out s);

        // Assert
        Assert.Equal(10 * 20 * 30, result);
    }

    [Fact]
    public void MatrixChainOrder_EmptyArray_ThrowsArgumentException()
    {
        // Arrange
        int[] p = Array.Empty<int>();
        int n = p.Length;
        int[,] s;

        // Act
        Action act = () => MatrixChainMultiplication.MatrixChainOrder(p, n, out s);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void GetOptimalParentheses_ValidInput_ReturnsCorrectParentheses()
    {
        // Arrange
        int[,] s = new int[5, 5];
        s[1, 2] = 1;
        s[1, 3] = 1;
        s[1, 4] = 3;
        s[2, 3] = 2;
        s[2, 4] = 3;
        s[3, 4] = 3;

        // Act
        string result = MatrixChainMultiplication.GetOptimalParentheses(s, 1, 4);

        // Assert
        Assert.Equal("((A1(A2A3))A4)", result);
    }

    [Fact]
    public void GetOptimalParentheses_SingleMatrix_ReturnsMatrixName()
    {
        // Arrange
        int[,] s = new int[2, 2];

        // Act
        string result = MatrixChainMultiplication.GetOptimalParentheses(s, 1, 1);

        // Assert
        Assert.Equal("A1", result);
    }

    [Fact]
    public void GetOptimalParentheses_TwoMatrices_ReturnsParenthesesAroundMatrices()
    {
        // Arrange
        int[,] s = new int[3, 3];
        s[1, 2] = 1;

        // Act
        string result = MatrixChainMultiplication.GetOptimalParentheses(s, 1, 2);

        // Assert
        Assert.Equal("(A1A2)", result);
    }
}
