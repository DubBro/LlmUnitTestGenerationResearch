using Newtonsoft.Json;

namespace LlmUnitTestGenerationResearchRunner.Models.OpenRouterApi;

public class ProviderOptions
{
    /// <summary>
    /// ["azure", "openai"], ["google-vertex"], etc.
    /// </summary>
    [JsonProperty("only")]
    public List<string> Only { get; set; } = [];
}
