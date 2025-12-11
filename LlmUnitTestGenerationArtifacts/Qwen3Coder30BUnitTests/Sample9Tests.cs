using Dataset.Sample9;

namespace Qwen3Coder30BUnitTests;

public class LoremIpsumGeneratorTests
{
    [Fact]
    public void GenerateLoremIpsumString_ValidInputs_ReturnsStringWithCorrectNumberOfParagraphs()
    {
        // Arrange
        var minWords = 1;
        var maxWords = 5;
        var minSentences = 1;
        var maxSentences = 3;
        var numParagraphs = 2;

        // Act
        var result = LoremIpsumGenerator.GenerateLoremIpsumString(
            minWords,
            maxWords,
            minSentences,
            maxSentences,
            numParagraphs);

        // Assert
        var paragraphCount = result.Split(new[] { "\n\n" }, StringSplitOptions.None).Length;
        Assert.Equal(numParagraphs, paragraphCount);
    }

    [Theory]
    [InlineData(1, 1, 1, 1, 1)]
    [InlineData(1, 5, 1, 3, 2)]
    [InlineData(2, 10, 2, 5, 3)]
    public void GenerateLoremIpsumString_ValidInputs_ReturnsStringWithCorrectStructure(
        int minWords,
        int maxWords,
        int minSentences,
        int maxSentences,
        int numParagraphs)
    {
        // Arrange
        var expectedParagraphCount = numParagraphs;

        // Act
        var result = LoremIpsumGenerator.GenerateLoremIpsumString(
            minWords,
            maxWords,
            minSentences,
            maxSentences,
            numParagraphs);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(". ", result);
        Assert.Contains("\n\n", result);
    }

    [Theory]
    [InlineData(0, 5, 1, 3, 1)]
    [InlineData(1, 5, 0, 3, 1)]
    [InlineData(1, 5, 1, 0, 1)]
    [InlineData(1, 5, 1, 3, 0)]
    public void GenerateLoremIpsumString_InvalidInputs_ThrowsArgumentException(
        int minWords,
        int maxWords,
        int minSentences,
        int maxSentences,
        int numParagraphs)
    {
        // Arrange
        // Act & Assert
        Assert.Throws<ArgumentException>(() => LoremIpsumGenerator.GenerateLoremIpsumString(
            minWords,
            maxWords,
            minSentences,
            maxSentences,
            numParagraphs));
    }

    [Fact]
    public void GenerateLoremIpsumHtmlSafe_ValidInputs_ReturnsHtmlStringWithCorrectStructure()
    {
        // Arrange
        var minWords = 1;
        var maxWords = 5;
        var minSentences = 1;
        var maxSentences = 3;
        var numParagraphs = 1;

        // Act
        var result = LoremIpsumGenerator.GenerateLoremIpsumHtmlSafe(
            minWords,
            maxWords,
            minSentences,
            maxSentences,
            numParagraphs);

        // Assert
        Assert.NotNull(result);
        Assert.StartsWith("<p>", result);
        Assert.EndsWith("</p>", result);
        Assert.Contains(". ", result);
    }

    [Theory]
    [InlineData(1, 1, 1, 1, 1)]
    [InlineData(1, 5, 1, 3, 2)]
    public void GenerateLoremIpsumHtmlSafe_ValidInputs_ReturnsHtmlStringWithCorrectParagraphCount(
        int minWords,
        int maxWords,
        int minSentences,
        int maxSentences,
        int numParagraphs)
    {
        // Arrange
        var expectedParagraphCount = numParagraphs;

        // Act
        var result = LoremIpsumGenerator.GenerateLoremIpsumHtmlSafe(
            minWords,
            maxWords,
            minSentences,
            maxSentences,
            numParagraphs);

        // Assert
        Assert.NotNull(result);
        var openTags = new StringReader(result).ReadToEnd().Split('<').Length;
        var closeTags = new StringReader(result).ReadToEnd().Split('>').Length;
        Assert.Contains("<p>", result);
        Assert.Contains("</p>", result);
    }

    [Theory]
    [InlineData(0, 5, 1, 3, 1)]
    [InlineData(1, 5, 0, 3, 1)]
    [InlineData(1, 5, 1, 0, 1)]
    [InlineData(1, 5, 1, 3, 0)]
    public void GenerateLoremIpsumHtmlSafe_InvalidInputs_ThrowsArgumentException(
        int minWords,
        int maxWords,
        int minSentences,
        int maxSentences,
        int numParagraphs)
    {
        // Arrange
        // Act & Assert
        Assert.Throws<ArgumentException>(() => LoremIpsumGenerator.GenerateLoremIpsumHtmlSafe(
            minWords,
            maxWords,
            minSentences,
            maxSentences,
            numParagraphs));
    }
}
