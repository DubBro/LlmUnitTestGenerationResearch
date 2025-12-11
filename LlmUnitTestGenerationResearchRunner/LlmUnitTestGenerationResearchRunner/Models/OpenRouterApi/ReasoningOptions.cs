using Newtonsoft.Json;

namespace LlmUnitTestGenerationResearchRunner.Models.OpenRouterApi;

public class ReasoningOptions
{
    [JsonProperty("effort")]
    public string Effort { get; set; } = "high";

    [JsonProperty("enabled")]
    public bool Enabled { get; set; } = true;

    [JsonProperty("exclude")]
    public bool Exclude { get; set; } = true;
}
