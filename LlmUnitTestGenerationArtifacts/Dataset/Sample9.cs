using System.Text;

namespace Dataset.Sample9;

public class LoremIpsumGenerator
{
    private static readonly string[] Words = new[] {
        "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
        "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
        "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

    private static readonly Random SharedRandom = new Random();

    public static string GenerateLoremIpsumString(
        int minWords,
        int maxWords,
        int minSentences,
        int maxSentences,
        int numParagraphs)
    {
        var numSentences = SharedRandom.Next(maxSentences - minSentences) + minSentences + 1;
        var numWords = SharedRandom.Next(maxWords - minWords) + minWords + 1;
        var result = new StringBuilder();

        for (var p = 0; p < numParagraphs; p++)
        {
            for (var s = 0; s < numSentences; s++)
            {
                for (var w = 0; w < numWords; w++)
                {
                    if (w > 0)
                        result.Append(' ');
                    result.Append(Words[SharedRandom.Next(Words.Length)]);
                }
                result.Append(". ");
            }
        }

        return result.ToString();
    }

    public static string GenerateLoremIpsumHtmlSafe(
        int minWords,
        int maxWords,
        int minSentences,
        int maxSentences,
        int numParagraphs)
    {
        var numSentences = SharedRandom.Next(maxSentences - minSentences) + minSentences + 1;
        var numWords = SharedRandom.Next(maxWords - minWords) + minWords + 1;
        var result = new StringBuilder();

        for (var p = 0; p < numParagraphs; p++)
        {
            result.Append("<p>");
            for (var s = 0; s < numSentences; s++)
            {
                for (var w = 0; w < numWords; w++)
                {
                    if (w > 0)
                        result.Append(' ');
                    result.Append(Words[SharedRandom.Next(Words.Length)]);
                }
                result.Append(". ");
            }
            result.Append("</p>");
        }

        return result.ToString();
    }
}
