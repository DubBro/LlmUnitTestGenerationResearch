using Dataset.Sample20;

namespace Glm45AirUnitTests;

public class FisherYatesShufflerTests
{
    [Fact]
    public void Shuffle_WhenCalledAndSeedIsNull_ShufflesArray()
    {
        // Arrange
        var shuffler = new FisherYatesShuffler<int>();
        var array = new int[] { 1, 2, 3, 4, 5 };
        var originalArray = (int[])array.Clone();

        // Act
        shuffler.Shuffle(array);

        // Assert
        Assert.NotEqual(originalArray, array);
    }

    [Fact]
    public void Shuffle_WhenCalledAndSeedIsNotNull_ShufflesArrayDeterministically()
    {
        // Arrange
        var shuffler = new FisherYatesShuffler<int>();
        var array1 = new int[] { 1, 2, 3, 4, 5 };
        var array2 = new int[] { 1, 2, 3, 4, 5 };
        const int seed = 123;

        // Act
        shuffler.Shuffle(array1, seed);
        shuffler.Shuffle(array2, seed);

        // Assert
        Assert.Equal(array1, array2);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Shuffle_WhenCalledWithSingleElementArray_DoesNotChangeArray(int element)
    {
        // Arrange
        var shuffler = new FisherYatesShuffler<int>();
        var array = new int[] { element };

        // Act
        shuffler.Shuffle(array);

        // Assert
        Assert.Equal(new int[] { element }, array);
    }

    [Fact]
    public void Shuffle_WhenCalledWithNullArray_ThrowsArgumentNullException()
    {
        // Arrange
        var shuffler = new FisherYatesShuffler<int>();
        int[] array = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => shuffler.Shuffle(array));
    }
}
