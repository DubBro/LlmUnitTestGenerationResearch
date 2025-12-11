namespace LlmUnitTestGenerationResearchRunner.Models.OpenRouterApi;

public sealed class GenerationInfoResult
{
    public GenerationInfoResult(
        string id,
        int latency,
        int moderationLatency,
        int generationTime,
        int nativeTokensPrompt,
        int nativeTokensCompletion,
        decimal totalCost)
    {
        Id = id;
        Latency = latency;
        ModerationLatency = moderationLatency;
        GenerationTime = generationTime;
        NativeTokensPrompt = nativeTokensPrompt;
        NativeTokensCompletion = nativeTokensCompletion;
        TotalCost = totalCost;
    }

    public string Id { get; }

    public int Latency { get; }

    public int ModerationLatency { get; }

    public int GenerationTime { get; }

    public int NativeTokensPrompt { get; }

    public int NativeTokensCompletion { get; }

    public decimal TotalCost { get; }
}
