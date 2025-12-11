using Dataset.Sample20;

namespace Gemini3ProUnitTests;

public class FisherYatesShufflerTests
{
    [Fact]
    public void Shuffle_WhenArrayIsNull_ThrowsNullReferenceException()
    {
        // Arrange
        var shuffler = new FisherYatesShuffler<int>();
        int[]? array = null;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => shuffler.Shuffle(array!));
    }

    [Fact]
    public void Shuffle_WhenArrayIsEmpty_DoesNotChangeArray()
    {
        // Arrange
        var shuffler = new FisherYatesShuffler<int>();
        var array = Array.Empty<int>();

        // Act
        shuffler.Shuffle(array);

        // Assert
        Assert.Empty(array);
    }

    [Fact]
    public void Shuffle_WhenArrayHasOneElement_DoesNotChangeArray()
    {
        // Arrange
        var shuffler = new FisherYatesShuffler<string>();
        var array = new[] { "SingleItem" };
        var expected = new[] { "SingleItem" };

        // Act
        shuffler.Shuffle(array);

        // Assert
        Assert.Equal(expected, array);
    }

    [Fact]
    public void Shuffle_WhenSeedIsProvided_ProducesDeterministicResult()
    {
        // Arrange
        var shuffler = new FisherYatesShuffler<int>();
        var seed = 12345;
        var array1 = Enumerable.Range(0, 50).ToArray();
        var array2 = Enumerable.Range(0, 50).ToArray();

        // Act
        shuffler.Shuffle(array1, seed);
        shuffler.Shuffle(array2, seed);

        // Assert
        Assert.Equal(array1, array2);
    }

    [Fact]
    public void Shuffle_WhenSeedIsProvided_ModifiesOrderForLargeArray()
    {
        // Arrange
        var shuffler = new FisherYatesShuffler<int>();
        var seed = 42;
        var original = Enumerable.Range(0, 100).ToArray();
        var array = original.ToArray();

        // Act
        shuffler.Shuffle(array, seed);

        // Assert
        // Check that the order has changed
        Assert.NotEqual(original, array);
        // Check that elements are preserved (multiset equality)
        Assert.Equal(original.OrderBy(x => x), array.OrderBy(x => x));
    }

    [Theory]
    [InlineData(100)]
    [InlineData(0)]
    [InlineData(-5)]
    public void Shuffle_WhenSeedIsProvided_PreservesAllElements(int seed)
    {
        // Arrange
        var shuffler = new FisherYatesShuffler<char>();
        var original = new[] { 'a', 'b', 'c', 'd', 'e' };
        var array = original.ToArray();

        // Act
        shuffler.Shuffle(array, seed);

        // Assert
        Assert.Equal(original.Length, array.Length);
        Assert.Equal(original.OrderBy(x => x), array.OrderBy(x => x));
    }

    [Fact]
    public void Shuffle_WhenNoSeedIsProvided_PreservesAllElements()
    {
        // Arrange
        var shuffler = new FisherYatesShuffler<int>();
        var original = new[] { 1, 2, 3, 4, 5 };
        var array = original.ToArray();

        // Act
        shuffler.Shuffle(array); // Non-deterministic

        // Assert
        // We cannot guarantee the order, but we must guarantee the contents are preserved
        Assert.Equal(original.Length, array.Length);
        Assert.Equal(original.OrderBy(x => x), array.OrderBy(x => x));
    }

    [Fact]
    public void Shuffle_WhenCalledWithComplexObjects_ShufflesReferencesCorrectly()
    {
        // Arrange
        var shuffler = new FisherYatesShuffler<object>();
        var obj1 = new object();
        var obj2 = new object();
        var obj3 = new object();
        var original = new[] { obj1, obj2, obj3 };
        var array = original.ToArray();

        // Act
        shuffler.Shuffle(array, seed: 99);

        // Assert
        Assert.Equal(original.Length, array.Length);
        foreach (var item in original)
        {
            Assert.Contains(item, array);
        }
    }
}
