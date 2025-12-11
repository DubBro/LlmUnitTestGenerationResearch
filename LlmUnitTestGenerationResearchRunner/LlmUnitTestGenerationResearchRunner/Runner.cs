using LlmUnitTestGenerationResearchRunner.Models.ReadabilityIndexEvaluation;
using LlmUnitTestGenerationResearchRunner.Models.Research;
using LlmUnitTestGenerationResearchRunner.Services;

namespace LlmUnitTestGenerationResearchRunner;

internal static class Runner
{
    public static async Task Start(RunnerType runnerType)
    {
        switch (runnerType)
        {
            case RunnerType.Research:
                await StartResearchRunner();
                break;
            case RunnerType.ReadabilityIndexEvaluation:
                await StartReadabilityIndexEvaluationRunner();
                break;
            default:
                throw new ArgumentException($"'{runnerType}' runner is not implemented.");
        }
    }

    private static async Task StartResearchRunner()
    {
        Console.WriteLine("---Research started---");

        var researchService = new ResearchService(
            ""); // api key

        var researchRequest = new ResearchRequest
        (
            "", // model
            "", // provider
            @"", // prompt file path
            @"", // dataset file path
            0, // dataset count
            @"" // result file path
        );

        await researchService.DoResearch(researchRequest);

        Console.WriteLine("---Research completed---");
    }

    private static async Task StartReadabilityIndexEvaluationRunner()
    {
        Console.WriteLine("---Readability index evaluation started---");

        var readabilityIndexEvaluationService = new ReadabilityIndexEvaluationService(
            ""); // api key

        var evaluationRequest = new ReadabilityIndexEvaluationRequest
        (
            new List<Agent>
            {
                new Agent("", "", ""), // model name, model id, provider id
            },
            @"", // prompt file path
            @"", // dataset file path
            0, // dataset count
            @"" // result file path
        );

        await readabilityIndexEvaluationService.DoEvaluation(evaluationRequest);

        Console.WriteLine("---Readability index evaluation completed---");
    }
}
