using Newtonsoft.Json;

namespace LlmUnitTestGenerationResearchRunner.Models.OpenRouterApi;

public class GenerationInfoData
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("latency")]
    public int Latency { get; set; }

    [JsonProperty("moderation_latency")]
    public int? ModerationLatency { get; set; }

    [JsonProperty("generation_time")]
    public int GenerationTime { get; set; }

    [JsonProperty("native_tokens_prompt")]
    public int NativeTokensPrompt { get; set; }

    [JsonProperty("native_tokens_completion")]
    public int NativeTokensCompletion { get; set; }

    [JsonProperty("total_cost")]
    public decimal TotalCost { get; set; }
}
