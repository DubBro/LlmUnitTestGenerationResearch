using Dataset.Sample9;

namespace DeepSeekR10528UnitTests;

public class LoremIpsumGeneratorTests
{
    private static readonly string[] ExpectedWords =
    {
        "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
        "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
        "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"
    };

    [Fact]
    public void GenerateLoremIpsumString_InvalidWordsRange_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        int minWords = 5;
        int maxWords = 3;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            LoremIpsumGenerator.GenerateLoremIpsumString(minWords, maxWords, 1, 2, 1));
    }

    [Fact]
    public void GenerateLoremIpsumString_InvalidSentencesRange_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        int minSentences = 5;
        int maxSentences = 3;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            LoremIpsumGenerator.GenerateLoremIpsumString(1, 2, minSentences, maxSentences, 1));
    }

    [Fact]
    public void GenerateLoremIpsumHtmlSafe_InvalidWordsRange_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        int minWords = 5;
        int maxWords = 3;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            LoremIpsumGenerator.GenerateLoremIpsumHtmlSafe(minWords, maxWords, 1, 2, 1));
    }

    [Fact]
    public void GenerateLoremIpsumHtmlSafe_InvalidSentencesRange_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        int minSentences = 5;
        int maxSentences = 3;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            LoremIpsumGenerator.GenerateLoremIpsumHtmlSafe(1, 2, minSentences, maxSentences, 1));
    }

    [Fact]
    public void GenerateLoremIpsumString_WithinBoundsParameters_ReturnsValidStructure()
    {
        // Arrange
        int minWords = 1;
        int maxWords = 10;
        int minSentences = 1;
        int maxSentences = 5;
        int numParagraphs = 2;

        // Act
        string result = LoremIpsumGenerator.GenerateLoremIpsumString(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        Assert.NotNull(result);
        string[] words = result.Split(new[] { ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string word in words)
            Assert.Contains(word, ExpectedWords);
    }

    [Fact]
    public void GenerateLoremIpsumString_AllWordsFromDictionary()
    {
        // Arrange
        int minWords = 1;
        int maxWords = 10;
        int minSentences = 1;
        int maxSentences = 5;
        int numParagraphs = 2;

        // Act
        string result = LoremIpsumGenerator.GenerateLoremIpsumString(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        string[] words = result.Split(new[] { ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string word in words)
            Assert.Contains(word, ExpectedWords);
    }

    [Fact]
    public void GenerateLoremIpsumString_SentenceEndsWithDotSpace()
    {
        // Arrange
        int minWords = 1;
        int maxWords = 10;
        int minSentences = 1;
        int maxSentences = 5;
        int numParagraphs = 2;

        // Act
        string result = LoremIpsumGenerator.GenerateLoremIpsumString(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        string[] sentences = result.Split('.', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < sentences.Length - 1; i++)
            Assert.True(sentences[i].TrimEnd().EndsWith(' '));
    }

    [Fact]
    public void GenerateLoremIpsumString_DoesNotContainHtmlTags()
    {
        // Arrange
        int minWords = 1;
        int maxWords = 10;
        int minSentences = 1;
        int maxSentences = 5;
        int numParagraphs = 2;

        // Act
        string result = LoremIpsumGenerator.GenerateLoremIpsumString(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        Assert.DoesNotContain("<p>", result);
        Assert.DoesNotContain("</p>", result);
    }

    [Fact]
    public void GenerateLoremIpsumHtmlSafe_AllWordsFromDictionary()
    {
        // Arrange
        int minWords = 1;
        int maxWords = 10;
        int minSentences = 1;
        int maxSentences = 5;
        int numParagraphs = 2;

        // Act
        string result = LoremIpsumGenerator.GenerateLoremIpsumHtmlSafe(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        string withoutTags = result.Replace("<p>", "").Replace("</p>", "");
        string[] words = withoutTags.Split(new[] { ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string word in words)
            Assert.Contains(word, ExpectedWords);
    }

    [Fact]
    public void GenerateLoremIpsumHtmlSafe_StartsWithParagraphTag()
    {
        // Arrange
        int minWords = 1;
        int maxWords = 10;
        int minSentences = 1;
        int maxSentences = 5;
        int numParagraphs = 1;

        // Act
        string result = LoremIpsumGenerator.GenerateLoremIpsumHtmlSafe(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        Assert.StartsWith("<p>", result);
    }

    [Fact]
    public void GenerateLoremIpsumHtmlSafe_EndsWithClosingParagraphTag()
    {
        // Arrange
        int minWords = 1;
        int maxWords = 10;
        int minSentences = 1;
        int maxSentences = 5;
        int numParagraphs = 1;

        // Act
        string result = LoremIpsumGenerator.GenerateLoremIpsumHtmlSafe(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        Assert.EndsWith("</p>", result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    public void GenerateLoremIpsumHtmlSafe_CorrectNumberOfParagraphs(int numParagraphs)
    {
        // Arrange
        int minWords = 1;
        int maxWords = 10;
        int minSentences = 1;
        int maxSentences = 5;

        // Act
        string result = LoremIpsumGenerator.GenerateLoremIpsumHtmlSafe(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        int openParagraphCount = result.Split(new[] { "<p>" }, StringSplitOptions.None).Length - 1;
        int closeParagraphCount = result.Split(new[] { "</p>" }, StringSplitOptions.None).Length - 1;
        Assert.Equal(numParagraphs, openParagraphCount);
        Assert.Equal(numParagraphs, closeParagraphCount);
    }
}
