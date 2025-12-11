namespace LlmUnitTestGenerationResearchRunner.Models.Research;

public class ResearchRequest
{
    public ResearchRequest(
        string model,
        string provider,
        string promptFilePath,
        string datasetFilePath,
        int datasetCount,
        string resultFilePath)
    {
        Model = model;
        Provider = provider;
        PromptFilePath = promptFilePath;
        DatasetFilePath = datasetFilePath;
        DatasetCount = datasetCount;
        ResultFilePath = resultFilePath;
    }

    public string Model { get; }

    public string Provider { get; }

    public string PromptFilePath { get; }

    public string DatasetFilePath { get; }

    public int DatasetCount { get; }

    public string ResultFilePath { get; }
}
