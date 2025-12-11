namespace Dataset.Sample14;

public static class RhombusGenerator
{
    public static int[,] GetRhombus(int count)
    {
        if ((count <= 0) || (count % 2 == 0))
        {
            throw new Exception("Error. Invalid input");
        }

        int[,] array = new int[count, count];

        array = FirstQuarter(array);
        array = SecondQuarter(array);
        array = ThirdQuarter(array);
        array = FourthQuarter(array);

        return array;
    }

    private static int[,] FirstQuarter(int[,] arr)
    {
        int dividedRows = (int)Math.Sqrt(arr.Length) / 2;
        int dividedColumns = (int)Math.Sqrt(arr.Length) / 2;

        for (int i = 0; i < dividedRows; i++)
        {
            for (int j = 0; j < dividedColumns; j++)
            {
                arr[i, j] = 1;
            }
            dividedColumns--;
        }

        return arr;
    }

    private static int[,] SecondQuarter(int[,] arr)
    {
        int dividedRows = (int)Math.Sqrt(arr.Length) / 2;
        int columns = (int)Math.Sqrt(arr.Length);
        int dividedColumns = columns / 2;

        for (int i = 0; i < dividedRows; i++)
        {
            for (int j = columns - 1; j > dividedColumns; j--)
            {
                arr[i, j] = 2;
            }
            dividedColumns++;
        }

        return arr;
    }

    private static int[,] ThirdQuarter(int[,] arr)
    {
        int rows = (int)Math.Sqrt(arr.Length);
        int dividedColumns = (int)Math.Sqrt(arr.Length) / 2;
        int dividedRows = rows / 2;

        for (int i = rows - 1; i > dividedRows; i--)
        {
            for (int j = 0; j < dividedColumns; j++)
            {
                arr[i, j] = 3;
            }
            dividedColumns--;
        }

        return arr;
    }

    private static int[,] FourthQuarter(int[,] arr)
    {
        int rows = (int)Math.Sqrt(arr.Length);
        int columns = (int)Math.Sqrt(arr.Length);
        int dividedRows = rows / 2;
        int dividedColumns = columns / 2;

        for (int i = rows - 1; i > dividedRows; i--)
        {
            for (int j = columns - 1; j > dividedColumns; j--)
            {
                arr[i, j] = 4;
            }
            dividedColumns++;
        }

        return arr;
    }
}
