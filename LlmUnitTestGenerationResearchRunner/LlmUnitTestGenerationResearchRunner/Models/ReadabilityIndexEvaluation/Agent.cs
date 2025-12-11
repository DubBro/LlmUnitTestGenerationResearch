namespace LlmUnitTestGenerationResearchRunner.Models.ReadabilityIndexEvaluation;

public class Agent
{
    public Agent(string modelName, string modelId, string providerId)
    {
        ModelName = modelName;
        ModelId = modelId;
        ProviderId = providerId;
    }

    public string ModelName { get; }

    public string ModelId { get; }

    public string ProviderId { get; }
}
