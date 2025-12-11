using LlmUnitTestGenerationResearchRunner.Models.OpenRouterApi;
using LlmUnitTestGenerationResearchRunner.Models.ReadabilityIndexEvaluation;

namespace LlmUnitTestGenerationResearchRunner.Services;

public class ReadabilityIndexEvaluationService
{
    private readonly OpenRouterService _openRouterApi;

    public ReadabilityIndexEvaluationService(string apiKey)
    {
        _openRouterApi = new OpenRouterService(apiKey);
    }

    public async Task DoEvaluation(ReadabilityIndexEvaluationRequest request)
    {
        try
        {
            var prompt = await File.ReadAllTextAsync(request.PromptFilePath);
            var resultsDirectory = $@"{request.ResultFilePath}\ReadabilityIndexEvaluationResults";

            if (!Directory.Exists(resultsDirectory))
                Directory.CreateDirectory(resultsDirectory);

            for (var i = 1; i <= request.DatasetCount; i++)
            {
                var sampleResultsDirectory = $@"{resultsDirectory}\Sample{i}";

                if (!Directory.Exists(sampleResultsDirectory))
                    Directory.CreateDirectory(sampleResultsDirectory);

                var sample = await File.ReadAllTextAsync(@$"{request.DatasetFilePath}\Sample{i}Tests.cs");
                var finalPrompt = prompt.Replace("{{CODE}}", sample);

                foreach (var agent in request.Agents)
                {
                    var messages = new[]
                    {
                        new Message
                        {
                            Role = "user",
                            Content = finalPrompt
                        }
                    };

                    var providerOptions = new ProviderOptions
                    {
                        Only = { agent.ProviderId }
                    };

                    Console.WriteLine($"---{i} completion request started---");
                    var completion = await _openRouterApi.CreateChatCompletionAsync(
                        model: agent.ModelId,
                        messages: messages,
                        provider: providerOptions);
                    Console.WriteLine($"---{i} completion request completed---");

                    await File.WriteAllTextAsync(
                        @$"{sampleResultsDirectory}\{agent.ModelName}ReadabilityIndexEvaluation.txt",
                        $"GenerationId:\n{completion.Id}\n\nContent:\n{completion.Content}");

                    Console.WriteLine($"---{i} result saved---");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Console.ReadLine();
        }
    }
}
