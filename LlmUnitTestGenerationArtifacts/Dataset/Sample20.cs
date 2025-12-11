namespace Dataset.Sample20;

public class FisherYatesShuffler<T>
{
    public void Shuffle(T[] array, int? seed = null)
    {
        var random = seed is null
            ? new Random()
            : new Random(seed.Value);

        for (var i = array.Length - 1; i > 0; i--)
        {
            var j = random.Next(0, i + 1);

            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
