using Newtonsoft.Json;

namespace LlmUnitTestGenerationResearchRunner.Models.OpenRouterApi;

public class ChatCompletionRequest
{
    [JsonProperty("model")]
    public string? Model { get; set; }

    [JsonProperty("messages")]
    public List<Message> Messages { get; set; } = [];

    [JsonProperty("reasoning")]
    public ReasoningOptions? Reasoning { get; set; }

    [JsonProperty("provider")]
    public ProviderOptions? Provider { get; set; }
}
