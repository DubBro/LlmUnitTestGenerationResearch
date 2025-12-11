using Dataset.Sample9;

namespace Gemini3ProUnitTests;

public class LoremIpsumGeneratorTests
{
    [Fact]
    public void GenerateLoremIpsumString_WhenParagraphsIsZero_ReturnsEmptyString()
    {
        // Arrange
        int minWords = 5;
        int maxWords = 10;
        int minSentences = 2;
        int maxSentences = 5;
        int numParagraphs = 0;

        // Act
        string result = LoremIpsumGenerator.GenerateLoremIpsumString(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GenerateLoremIpsumString_WhenCalledWithValidArgs_ReturnsStringWithCorrectStructure()
    {
        // Arrange
        // By setting min and max equal, we force the Random logic to be deterministic regarding counts.
        // Logic in SUT: count = Random(max - min) + min + 1.
        // If min=1, max=1 -> Random(0) + 1 + 1 = 2.
        int wordParam = 1;      // Expecting 2 words per sentence
        int sentenceParam = 1;  // Expecting 2 sentences per paragraph
        int paragraphs = 2;     // Expecting 2 paragraphs

        // Act
        string result = LoremIpsumGenerator.GenerateLoremIpsumString(
            wordParam, wordParam, sentenceParam, sentenceParam, paragraphs);

        // Assert
        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result));

        // Logic check:
        // 2 paragraphs * 2 sentences = 4 total sentences.
        // Sentences end with ". ", so we count the occurrences of ". ".
        int sentenceEndingsCount = result.Split(new[] { ". " }, StringSplitOptions.RemoveEmptyEntries).Length;
        // Due to split behavior, if the string ends with the delimiter, the last entry might be empty or behavior varies slightly.
        // A manual count is safer for exact verification.
        int dotSpaceCount = (result.Length - result.Replace(". ", "").Length) / 2;

        Assert.Equal(4, dotSpaceCount);

        // Word count check:
        // 2 words per sentence * 4 sentences = 8 words total.
        // The SUT appends a space after every word except the last one in a sentence?
        // No, check code: (w > 0) adds space. Ends with ". ".
        // "Words[i] Words[j]. " -> 2 words, 2 spaces (one internal, one trailing after dot).
        // Total spaces in output = (para * sent * words).
        // 2 * 2 * 2 = 8 spaces.
        int spaceCount = result.Count(c => c == ' ');
        Assert.Equal(8, spaceCount);
    }

    [Theory]
    [InlineData(10, 5, 2, 5)]  // minWords > maxWords
    [InlineData(5, 10, 5, 2)]  // minSentences > maxSentences
    public void GenerateLoremIpsumString_WhenMaxLessThanMin_ThrowsArgumentOutOfRangeException(
        int minWords, int maxWords, int minSentences, int maxSentences)
    {
        // Arrange
        int numParagraphs = 1;

        // Act & Assert
        // The Random.Next(int maxValue) throws if maxValue is negative.
        // SUT calls Next(max - min). If max < min, arg is negative.
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            LoremIpsumGenerator.GenerateLoremIpsumString(
                minWords, maxWords, minSentences, maxSentences, numParagraphs));
    }

    [Fact]
    public void GenerateLoremIpsumHtmlSafe_WhenParagraphsIsZero_ReturnsEmptyString()
    {
        // Arrange
        int minWords = 5;
        int maxWords = 10;
        int minSentences = 2;
        int maxSentences = 5;
        int numParagraphs = 0;

        // Act
        string result = LoremIpsumGenerator.GenerateLoremIpsumHtmlSafe(
            minWords, maxWords, minSentences, maxSentences, numParagraphs);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GenerateLoremIpsumHtmlSafe_WhenCalledWithValidArgs_ReturnsWrappedHtmlParagraphs()
    {
        // Arrange
        int wordParam = 1;
        int sentenceParam = 1;
        int paragraphs = 3;

        // Act
        string result = LoremIpsumGenerator.GenerateLoremIpsumHtmlSafe(
            wordParam, wordParam, sentenceParam, sentenceParam, paragraphs);

        // Assert
        Assert.NotNull(result);

        // Verify HTML tags
        int openTagCount = (result.Length - result.Replace("<p>", "").Length) / 3;
        int closeTagCount = (result.Length - result.Replace("</p>", "").Length) / 4;

        Assert.Equal(paragraphs, openTagCount);
        Assert.Equal(paragraphs, closeTagCount);

        // Verify logical structure
        Assert.StartsWith("<p>", result);
        Assert.EndsWith("</p>", result);
    }

    [Fact]
    public void GenerateLoremIpsumHtmlSafe_WhenMaxLessThanMin_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        int minWords = 10;
        int maxWords = 5; // Invalid
        int minSentences = 2;
        int maxSentences = 5;
        int numParagraphs = 1;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            LoremIpsumGenerator.GenerateLoremIpsumHtmlSafe(
                minWords, maxWords, minSentences, maxSentences, numParagraphs));
    }

    [Fact]
    public void GenerateLoremIpsumString_WhenCalled_ContentContainsOnlyExpectedCharacters()
    {
        // Arrange
        // We verify that the SUT only uses the predefined dictionary words, spaces, and dots.
        // It should not generate numbers or special symbols other than correct punctuation.
        int minW = 1, maxW = 5, minS = 1, maxS = 2, paras = 1;

        // Act
        string result = LoremIpsumGenerator.GenerateLoremIpsumString(minW, maxW, minS, maxS, paras);

        // Assert
        // Allow a-z (case insensitive due to dictionary), space, dot.
        // Dictionary is lowercase, but standard checking ensures no weird ascii garbage.
        bool allCharsValid = result.All(c => char.IsLetter(c) || c == ' ' || c == '.');
        Assert.True(allCharsValid, "Output contained unexpected characters.");
    }
}
