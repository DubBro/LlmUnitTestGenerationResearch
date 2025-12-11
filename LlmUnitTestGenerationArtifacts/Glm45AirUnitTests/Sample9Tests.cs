using Dataset.Sample9;

namespace Glm45AirUnitTests;

public class LoremIpsumGeneratorTests
{
    [Fact]
    public void GenerateLoremIpsumString_DefaultParameters_ReturnsString()
    {
        // Arrange
        int minWords = 5;
        int maxWords = 10;
        int minSentences = 2;
        int maxSentences = 5;
        int numParagraphs = 1;

        // Act
        var result = LoremIpsumGenerator.GenerateLoremIpsumString(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        Assert.False(string.IsNullOrEmpty(result));
    }

    [Theory]
    [InlineData(1, 1, 1, 1, 1)]
    [InlineData(10, 20, 3, 6, 2)]
    [InlineData(100, 100, 5, 5, 3)]
    public void GenerateLoremIpsumString_WithValidParameters_ReturnsString(
        int minWords, int maxWords, int minSentences, int maxSentences, int numParagraphs)
    {
        // Arrange
        // Parameters provided by InlineData

        // Act
        var result = LoremIpsumGenerator.GenerateLoremIpsumString(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        Assert.False(string.IsNullOrEmpty(result));
    }

    [Fact]
    public void GenerateLoremIpsumString_MinWordsEqualsMaxWords_UsesExactWordCount()
    {
        // Arrange
        int exactWordCount = 5;
        int exactSentenceCount = 2;
        int minWords = exactWordCount;
        int maxWords = exactWordCount;
        int minSentences = exactSentenceCount;
        int maxSentences = exactSentenceCount;
        int numParagraphs = 1;

        // Act
        var result = LoremIpsumGenerator.GenerateLoremIpsumString(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        var wordCount = result.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Count();
        Assert.Equal(exactWordCount * exactSentenceCount, wordCount);
    }

    [Fact]
    public void GenerateLoremIpsumString_MinSentencesEqualsMaxSentences_UsesExactSentenceCount()
    {
        // Arrange
        int exactWordCount = 3;
        int exactSentenceCount = 4;
        int minWords = exactWordCount;
        int maxWords = exactWordCount;
        int minSentences = exactSentenceCount;
        int maxSentences = exactSentenceCount;
        int numParagraphs = 1;

        // Act
        var result = LoremIpsumGenerator.GenerateLoremIpsumString(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        var sentenceCount = result.Split(new[] { ". " }, StringSplitOptions.RemoveEmptyEntries).Count();
        Assert.Equal(exactSentenceCount, sentenceCount);
    }

    [Fact]
    public void GenerateLoremIpsumString_NumParagraphsZero_ReturnsEmptyString()
    {
        // Arrange
        int minWords = 1;
        int maxWords = 5;
        int minSentences = 1;
        int maxSentences = 3;
        int numParagraphs = 0;

        // Act
        var result = LoremIpsumGenerator.GenerateLoremIpsumString(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GenerateLoremIpsumHtmlSafe_WithValidParameters_ReturnsHtmlString()
    {
        // Arrange
        int minWords = 3;
        int maxWords = 7;
        int minSentences = 1;
        int maxSentences = 2;
        int numParagraphs = 2;

        // Act
        var result = LoremIpsumGenerator.GenerateLoremIpsumHtmlSafe(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        Assert.False(string.IsNullOrEmpty(result));
        Assert.StartsWith("<p>", result);
        Assert.EndsWith("</p>", result);
    }

    [Theory]
    [InlineData(1, 1, 1, 1, 1)]
    [InlineData(5, 10, 2, 4, 3)]
    [InlineData(20, 25, 3, 5, 1)]
    public void GenerateLoremIpsumHtmlSafe_WithValidParameters_ReturnsCorrectHtmlStructure(
        int minWords, int maxWords, int minSentences, int maxSentences, int numParagraphs)
    {
        // Arrange
        // Parameters provided by InlineData

        // Act
        var result = LoremIpsumGenerator.GenerateLoremIpsumHtmlSafe(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        Assert.Equal(numParagraphs, result.Split(new[] { "<p>" }, StringSplitOptions.RemoveEmptyEntries).Length - 1);
        Assert.EndsWith("</p>", result);
    }

    [Fact]
    public void GenerateLoremIpsumHtmlSafe_NumParagraphsZero_ReturnsEmptyString()
    {
        // Arrange
        int minWords = 1;
        int maxWords = 5;
        int minSentences = 1;
        int maxSentences = 3;
        int numParagraphs = 0;

        // Act
        var result = LoremIpsumGenerator.GenerateLoremIpsumHtmlSafe(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}
