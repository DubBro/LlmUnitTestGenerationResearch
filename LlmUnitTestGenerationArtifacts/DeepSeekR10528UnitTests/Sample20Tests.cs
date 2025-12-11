using Dataset.Sample20;

namespace DeepSeekR10528UnitTests;

public class FisherYatesShufflerTests
{
    [Fact]
    public void Shuffle_ArrayIsNull_ThrowsNullReferenceException()
    {
        // Arrange
        var shuffler = new FisherYatesShuffler<int>();
        int[] array = null;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => shuffler.Shuffle(array));
    }

    [Fact]
    public void Shuffle_WithFixedSeedOnTwoElementArray_ShufflesToExpected()
    {
        // Arrange
        var array = new int[] { 1, 2 };
        var expected = new int[] { 2, 1 };
        var shuffler = new FisherYatesShuffler<int>();

        // Act
        shuffler.Shuffle(array, seed: 1);

        // Assert
        Assert.Equal(expected, array);
    }

    [Fact]
    public void Shuffle_WithFixedSeedOnThreeElementArray_ShufflesToExpected()
    {
        // Arrange
        var array = new int[] { 1, 2, 3 };
        var expected = new int[] { 3, 2, 1 };
        var shuffler = new FisherYatesShuffler<int>();

        // Act
        shuffler.Shuffle(array, seed: 1);

        // Assert
        Assert.Equal(expected, array);
    }

    [Theory]
    [InlineData(new int[] {})]
    [InlineData(new int[] { 1 })]
    [InlineData(new int[] { 1, 2 })]
    [InlineData(new int[] { 1, 2, 3, 4 })]
    [InlineData(new int[] { 1, 1, 2, 2, 3, 3 })]
    public void Shuffle_AnyArray_PreservesElements(int[] array)
    {
        // Arrange
        var original = (int[])array.Clone();
        var shuffler = new FisherYatesShuffler<int>();

        // Act
        shuffler.Shuffle(array);

        // Assert
        Array.Sort(original);
        var shuffledSorted = array.ToArray();
        Array.Sort(shuffledSorted);
        Assert.Equal(original, shuffledSorted);
    }
}
