namespace LlmUnitTestGenerationResearchRunner.Models.OpenRouterApi;

public class ChatCompletionResult
{
    public ChatCompletionResult(string id, string content, TimeSpan elapsedTime)
    {
        Id = id;
        Content = content;
        ElapsedTime = elapsedTime;
    }

    public string Id { get; }

    /// <summary>
    /// `choices[0].message.content`.
    /// </summary>
    public string Content { get; }

    public TimeSpan ElapsedTime { get; }
}
