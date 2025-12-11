using Dataset.Sample20;

namespace Qwen3Coder30BUnitTests
{
    public class FisherYatesShufflerTests
    {
        [Fact]
        public void Shuffle_ArrayIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var shuffler = new FisherYatesShuffler<int>();
            int[]? array = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => shuffler.Shuffle(array));
        }

        [Fact]
        public void Shuffle_ArrayIsEmpty_DoesNotThrow()
        {
            // Arrange
            var shuffler = new FisherYatesShuffler<int>();
            var array = new int[0];

            // Act
            shuffler.Shuffle(array);

            // Assert
            // No exception thrown, and array remains empty
            Assert.Empty(array);
        }

        [Fact]
        public void Shuffle_ArrayHasOneElement_ArrayUnchanged()
        {
            // Arrange
            var shuffler = new FisherYatesShuffler<int>();
            var array = new[] { 42 };

            // Act
            shuffler.Shuffle(array);

            // Assert
            Assert.Equal(42, array[0]);
        }

        [Theory]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 1)]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 100)]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 999)]
        public void Shuffle_ArrayWithMultipleElements_ShufflesWithSeed(int[] original, int seed)
        {
            // Arrange
            var shuffler = new FisherYatesShuffler<int>();
            var array = new int[original.Length];
            Array.Copy(original, array, original.Length);

            // Act
            shuffler.Shuffle(array, seed);

            // Assert
            // Elements are still present but order has changed (probabilistically)
            Assert.Equal(original.Length, array.Length);
            Assert.True(array.Contains(original[0]));
            Assert.True(array.Contains(original[1]));
            Assert.True(array.Contains(original[2]));
            Assert.True(array.Contains(original[3]));
            Assert.True(array.Contains(original[4]));
        }

        [Theory]
        [InlineData(new[] { "a", "b", "c" }, 0)]
        [InlineData(new[] { "a", "b", "c" }, 42)]
        [InlineData(new[] { "a", "b", "c" }, 999999)]
        public void Shuffle_StringArrayWithSeed_SharesSameElements(string[] original, int seed)
        {
            // Arrange
            var shuffler = new FisherYatesShuffler<string>();
            var array = new string[original.Length];
            Array.Copy(original, array, original.Length);

            // Act
            shuffler.Shuffle(array, seed);

            // Assert
            Assert.Contains("a", array);
            Assert.Contains("b", array);
            Assert.Contains("c", array);
            Assert.Equal(3, array.Length);
        }

        [Fact]
        public void Shuffle_ArrayWithSeed_NullSeedUsesDefaultRandom()
        {
            // Arrange
            var shuffler = new FisherYatesShuffler<int>();
            var array = new[] { 1, 2, 3, 4, 5 };

            // Act
            shuffler.Shuffle(array, null);

            // Assert
            // Should shuffle without throwing; behavior depends on Random() behavior
            // We check the structure only
            Assert.Equal(5, array.Length);
        }
    }
}
