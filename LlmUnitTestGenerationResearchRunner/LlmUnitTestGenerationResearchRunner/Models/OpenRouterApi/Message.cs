using Newtonsoft.Json;

namespace LlmUnitTestGenerationResearchRunner.Models.OpenRouterApi;

public class Message
{
    /// <summary>
    /// "user", "system", "assistant", "developer", etc.
    /// </summary>
    [JsonProperty("role")]
    public string Role { get; set; } = "system";

    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;
}
