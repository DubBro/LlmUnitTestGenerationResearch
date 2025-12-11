using Newtonsoft.Json;

namespace LlmUnitTestGenerationResearchRunner.Models.OpenRouterApi;

public class GenerationInfoResponse
{
    [JsonProperty("data")]
    public GenerationInfoData? Data { get; set; }
}
