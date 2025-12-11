using Newtonsoft.Json;

namespace LlmUnitTestGenerationResearchRunner.Models.OpenRouterApi;

public class Choice
{
    [JsonProperty("message")]
    public Message? Message { get; set; }
}
