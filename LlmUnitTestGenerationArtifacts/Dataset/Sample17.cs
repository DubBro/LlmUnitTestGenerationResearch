namespace Dataset.Sample17;

public static class VectorExtensions
{
    public static double[] Copy(this double[] vector)
    {
        var result = new double[vector.Length];

        for (var i = 0; i < vector.Length; i++)
        {
            result[i] = vector[i];
        }

        return result;
    }

    public static double[,] OuterProduct(this double[] lhs, double[] rhs)
    {
        var result = new double[lhs.Length, rhs.Length];

        for (var i = 0; i < lhs.Length; i++)
        {
            for (var j = 0; j < rhs.Length; j++)
            {
                result[i, j] = lhs[i] * rhs[j];
            }
        }

        return result;
    }

    public static double Dot(this double[] lhs, double[] rhs)
    {
        if (lhs.Length != rhs.Length)
        {
            throw new ArgumentException("Arguments must have same dimension.");
        }

        double result = 0;

        for (var i = 0; i < lhs.Length; i++)
        {
            result += lhs[i] * rhs[i];
        }

        return result;
    }

    public static double Magnitude(this double[] vector)
    {
        return Math.Sqrt(Dot(vector, vector));
    }

    public static double[] Scale(this double[] vector, double factor)
    {
        var result = new double[vector.Length];

        for (var i = 0; i < vector.Length; i++)
        {
            result[i] = vector[i] * factor;
        }

        return result;
    }

    public static double[,] ToColumnVector(this double[] source)
    {
        var columnVector = new double[source.Length, 1];

        for (var i = 0; i < source.Length; i++)
        {
            columnVector[i, 0] = source[i];
        }

        return columnVector;
    }

    public static double[] ToRowVector(this double[,] source)
    {
        if (source.GetLength(1) != 1)
        {
            throw new InvalidOperationException("The column vector must have only 1 element in width.");
        }

        var rowVector = new double[source.Length];

        for (var i = 0; i < rowVector.Length; i++)
        {
            rowVector[i] = source[i, 0];
        }

        return rowVector;
    }

    public static double[,] ToDiagonalMatrix(this double[] vector)
    {
        var len = vector.Length;
        var result = new double[len, len];

        for (var i = 0; i < len; i++)
        {
            result[i, i] = vector[i];
        }

        return result;
    }
}
