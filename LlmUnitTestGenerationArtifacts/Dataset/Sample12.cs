using System.Text;

namespace Dataset.Sample12;

public static class LUPDecomposition
{
    public static void Decompose(double[,] matrix, out double[,] L, out double[,] U, out int[] P)
    {
        int n = matrix.GetLength(0);
        L = new double[n, n];
        U = new double[n, n];
        P = new int[n];

        for (int i = 0; i < n; i++)
            P[i] = i;

        for (int i = 0; i < n; i++)
        {
            double max = 0;
            int row = i;
            for (int k = i; k < n; k++)
            {
                if (Math.Abs(matrix[k, i]) > max)
                {
                    max = Math.Abs(matrix[k, i]);
                    row = k;
                }
            }

            if (max == 0)
                throw new InvalidOperationException("Матриця є виродженою.");

            int temp = P[i];
            P[i] = P[row];
            P[row] = temp;

            for (int j = 0; j < n; j++)
            {
                double tmp = matrix[i, j];
                matrix[i, j] = matrix[row, j];
                matrix[row, j] = tmp;
            }

            for (int j = i; j < n; j++)
            {
                double sum = 0;
                for (int k = 0; k < i; k++)
                    sum += L[i, k] * U[k, j];

                U[i, j] = matrix[i, j] - sum;
            }

            for (int j = i + 1; j < n; j++)
            {
                double sum = 0;
                for (int k = 0; k < i; k++)
                    sum += L[j, k] * U[k, i];

                L[j, i] = (matrix[j, i] - sum) / U[i, i];
            }

            L[i, i] = 1;
        }
    }

    public static string GetMatrix(double[,] matrix)
    {
        StringBuilder result = new StringBuilder();

        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
                result.Append($"{matrix[i, j]}\t");
            result.Append('\n');
        }

        return result.ToString();
    }

    public static string GetPermutationMatrix(int[] P)
    {
        StringBuilder result = new StringBuilder();

        int n = P.Length;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                result.Append(j == P[i] ? "1\t" : "0\t");
            result.Append('\n');
        }

        return result.ToString();
    }
}
