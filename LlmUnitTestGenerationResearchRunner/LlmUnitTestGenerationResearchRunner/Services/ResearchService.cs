using LlmUnitTestGenerationResearchRunner.Models.OpenRouterApi;
using LlmUnitTestGenerationResearchRunner.Models.Research;

namespace LlmUnitTestGenerationResearchRunner.Services;

public class ResearchService
{
    private readonly OpenRouterService _openRouterApi;

    public ResearchService(string apiKey)
    {
        _openRouterApi = new OpenRouterService(apiKey);
    }

    public async Task DoResearch(ResearchRequest request)
    {
        try
        {
            var prompt = await File.ReadAllTextAsync(request.PromptFilePath);

            for (var i = 1; i <= request.DatasetCount; i++)
            {
                var sample = await File.ReadAllTextAsync(@$"{request.DatasetFilePath}\Sample{i}.cs");
                var finalPrompt = prompt.Replace("{{CODE}}", sample);

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
                    Only = { request.Provider }
                };

                Console.WriteLine($"---{i} completion request started---");
                var completion = await _openRouterApi.CreateChatCompletionAsync(
                    model: request.Model,
                    messages: messages,
                    provider: providerOptions);
                Console.WriteLine($"---{i} completion request completed---");

                await Task.Delay(50000);

                Console.WriteLine($"---{i} generation request started---");
                var generationInfo = await _openRouterApi.GetGenerationAsync(completion.Id);
                Console.WriteLine($"---{i} generation request completed---");

                var researchCaseResult = new ResearchCaseResult
                (
                    completion.Id,
                    completion.Content,
                    completion.ElapsedTime.TotalMilliseconds,
                    generationInfo.Latency + generationInfo.ModerationLatency + generationInfo.GenerationTime,
                    generationInfo.NativeTokensPrompt,
                    generationInfo.NativeTokensCompletion,
                    generationInfo.NativeTokensPrompt + generationInfo.NativeTokensCompletion,
                    generationInfo.TotalCost
                );

                await File.WriteAllTextAsync(@$"{request.ResultFilePath}\Sample{i}Result.txt", researchCaseResult.ToString());

                Console.WriteLine($"---{i} result saved---");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Console.ReadLine();
        }
    }
}
