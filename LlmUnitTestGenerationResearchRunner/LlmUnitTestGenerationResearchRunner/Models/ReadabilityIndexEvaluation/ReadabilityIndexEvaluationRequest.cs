namespace LlmUnitTestGenerationResearchRunner.Models.ReadabilityIndexEvaluation;

public class ReadabilityIndexEvaluationRequest
{
    public ReadabilityIndexEvaluationRequest(
        IEnumerable<Agent> agents,
        string promptFilePath,
        string datasetFilePath,
        int datasetCount,
        string resultFilePath)
    {
        Agents = agents.ToList();
        PromptFilePath = promptFilePath;
        DatasetFilePath = datasetFilePath;
        DatasetCount = datasetCount;
        ResultFilePath = resultFilePath;
    }

    public List<Agent> Agents  { get; }

    public string PromptFilePath { get; }

    public string DatasetFilePath { get; }

    public int DatasetCount { get; }

    public string ResultFilePath { get; }
}
