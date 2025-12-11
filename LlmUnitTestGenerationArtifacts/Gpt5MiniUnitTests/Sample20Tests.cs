using Dataset.Sample20;

namespace Gpt5MiniUnitTests
{
    public class FisherYatesShufflerTests
    {
        // Arrange helper to compare multisets
        private static void AssertSameMultiset<T>(T[] expected, T[] actual)
        {
            var expectedCounts = expected.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            var actualCounts = actual.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

            Assert.Equal(expectedCounts.Count, actualCounts.Count);

            foreach (var kv in expectedCounts)
            {
                Assert.True(actualCounts.TryGetValue(kv.Key, out var count), $"Missing key {kv.Key}");
                Assert.Equal(kv.Value, count);
            }
        }

        [Fact]
        public void Shuffle_NullArray_ThrowsNullReferenceException()
        {
            // Arrange
            var shuffler = new FisherYatesShuffler<int>();
            int[]? array = null;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => shuffler.Shuffle(array!));
        }

        [Fact]
        public void Shuffle_EmptyArray_RemainsUnchanged()
        {
            // Arrange
            var shuffler = new FisherYatesShuffler<int>();
            var array = Array.Empty<int>();
            var expected = array.ToArray();

            // Act
            shuffler.Shuffle(array);

            // Assert
            Assert.Equal(expected, array);
        }

        [Fact]
        public void Shuffle_SingleElementArray_RemainsUnchanged()
        {
            // Arrange
            var shuffler = new FisherYatesShuffler<string>();
            var array = new[] { "only" };
            var expected = array.ToArray();

            // Act
            shuffler.Shuffle(array);

            // Assert
            Assert.Equal(expected, array);
        }

        [Fact]
        public void Shuffle_WithSeed_IsDeterministicAcrossInstances()
        {
            // Arrange
            var shuffler1 = new FisherYatesShuffler<int>();
            var shuffler2 = new FisherYatesShuffler<int>();
            var seed = 12345;
            var original = Enumerable.Range(0, 20).ToArray();
            var array1 = original.ToArray();
            var array2 = original.ToArray();

            // Act
            shuffler1.Shuffle(array1, seed);
            shuffler2.Shuffle(array2, seed);

            // Assert
            Assert.Equal(array1, array2);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        public void Shuffle_WithoutSeed_PreservesElements(int size)
        {
            // Arrange
            var shuffler = new FisherYatesShuffler<int>();
            var original = Enumerable.Range(0, size).ToArray();
            var array = original.ToArray();

            // Act
            shuffler.Shuffle(array);

            // Assert
            Assert.Equal(original.Length, array.Length);
            AssertSameMultiset(original, array);
        }

        [Fact]
        public void Shuffle_WithSeed_ProducesExpectedPermutation()
        {
            // Arrange
            var shuffler = new FisherYatesShuffler<int>();
            var seed = 9876;
            var original = Enumerable.Range(1, 15).ToArray();
            var array = original.ToArray();

            // Build expected result by replicating Fisher-Yates using Random(seed)
            var expected = original.ToArray();
            var rnd = new Random(seed);
            for (var i = expected.Length - 1; i > 0; i--)
            {
                var j = rnd.Next(0, i + 1);
                (expected[i], expected[j]) = (expected[j], expected[i]);
            }

            // Act
            shuffler.Shuffle(array, seed);

            // Assert
            Assert.Equal(expected, array);
        }

        [Fact]
        public void Shuffle_WithDuplicates_PreservesCounts()
        {
            // Arrange
            var shuffler = new FisherYatesShuffler<int>();
            var original = new[] { 1, 2, 2, 3, 3, 3, 4, 4, 4, 4 };
            var array = original.ToArray();

            // Act
            shuffler.Shuffle(array);

            // Assert
            AssertSameMultiset(original, array);
        }
    }
}
