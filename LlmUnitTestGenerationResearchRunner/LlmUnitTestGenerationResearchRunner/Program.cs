namespace LlmUnitTestGenerationResearchRunner;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        await Runner.Start(RunnerType.Research);
        // await Runner.Start(RunnerType.ReadabilityIndexEvaluation);
    }
}
