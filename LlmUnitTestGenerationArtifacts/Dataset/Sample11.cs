using System.Text;

namespace Dataset.Sample11;

public static class MatrixChainMultiplication
{
    public static int MatrixChainOrder(int[] p, int n, out int[,] s)
    {
        int[,] m = new int[n, n];
        s = new int[n, n];

        for (int i = 1; i < n; i++)
            m[i, i] = 0;

        for (int l = 2; l < n; l++)
        {
            for (int i = 1; i < n - l + 1; i++)
            {
                int j = i + l - 1;
                m[i, j] = int.MaxValue;

                for (int k = i; k <= j - 1; k++)
                {
                    int q = m[i, k] + m[k + 1, j] + p[i - 1] * p[k] * p[j];
                    if (q < m[i, j])
                    {
                        m[i, j] = q;
                        s[i, j] = k;
                    }
                }
            }
        }

        return m[1, n - 1];
    }

    public static string GetOptimalParentheses(int[,] s, int i, int j)
    {
        StringBuilder result = new StringBuilder();

        if (i == j)
        {
            result.Append($"A{i}");
        }
        else
        {
            result.Append('(');
            result.Append(GetOptimalParentheses(s, i, s[i, j]));
            result.Append(GetOptimalParentheses(s, s[i, j] + 1, j));
            result.Append(')');
        }

        return result.ToString();
    }
}
