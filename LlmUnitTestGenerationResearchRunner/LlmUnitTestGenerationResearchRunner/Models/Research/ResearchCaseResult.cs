namespace LlmUnitTestGenerationResearchRunner.Models.Research;

public class ResearchCaseResult
{
    public ResearchCaseResult(
        string generationId,
        string content,
        double elapsedTimeMilliseconds,
        double openRouterElapsedTimeMilliseconds,
        int promptTokens,
        int completionTokens,
        int totalTokens,
        decimal totalCost)
    {
        GenerationId = generationId;
        Content = content;
        ElapsedTimeMilliseconds = elapsedTimeMilliseconds;
        OpenRouterElapsedTimeMilliseconds = openRouterElapsedTimeMilliseconds;
        PromptTokens = promptTokens;
        CompletionTokens = completionTokens;
        TotalTokens = totalTokens;
        TotalCost = totalCost;
    }

    /// <summary>
    /// generation_id
    /// </summary>
    public string GenerationId { get; }

    public string Content { get; }

    public double ElapsedTimeMilliseconds { get; }

    /// <summary>
    /// latency + moderation_latency + generation_time
    /// </summary>
    public double OpenRouterElapsedTimeMilliseconds { get; }

    /// <summary>
    /// native_tokens_prompt
    /// </summary>
    public int PromptTokens { get; }

    /// <summary>
    /// native_tokens_completion
    /// </summary>
    public int CompletionTokens { get; }

    /// <summary>
    /// native_tokens_prompt + native_tokens_completion
    /// </summary>
    public int TotalTokens { get; }

    /// <summary>
    /// total_cost
    /// </summary>
    public decimal TotalCost { get; }

    public override string ToString()
    {
        return $"""
                {nameof(GenerationId)}:
                {GenerationId}
                
                {nameof(Content)}:
                {Content}
                
                {nameof(ElapsedTimeMilliseconds)}:
                {ElapsedTimeMilliseconds}
                
                {nameof(OpenRouterElapsedTimeMilliseconds)}:
                {OpenRouterElapsedTimeMilliseconds}
                
                {nameof(PromptTokens)}:
                {PromptTokens}
                
                {nameof(CompletionTokens)}:
                {CompletionTokens}
                
                {nameof(TotalTokens)}:
                {TotalTokens}
                
                {nameof(TotalCost)}:
                {TotalCost}
                """;
    }
}
