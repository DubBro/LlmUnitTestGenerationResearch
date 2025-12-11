using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using LlmUnitTestGenerationResearchRunner.Models.OpenRouterApi;
using Newtonsoft.Json;

namespace LlmUnitTestGenerationResearchRunner.Services;

public sealed class OpenRouterService : IDisposable
{
    private const string BaseUrl = "https://openrouter.ai/api/v1/";

    private readonly HttpClient _httpClient;
    private bool _disposed;

    public OpenRouterService(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key must be provided.", nameof(apiKey));

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(600)
        };

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<ChatCompletionResult> CreateChatCompletionAsync(
        string model,
        IEnumerable<Message> messages,
        ReasoningOptions? reasoning = null,
        ProviderOptions? provider = null)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model must be provided.", nameof(model));

        if (messages is null)
            throw new ArgumentNullException(nameof(messages));

        var messagesList = messages.ToList();

        if (messagesList.Count == 0)
            throw new ArgumentException("At least one message must be provided.", nameof(messages));

        var request = new ChatCompletionRequest
        {
            Model = model,
            Messages = messagesList,
            Reasoning = reasoning,
            Provider = provider
        };

        using var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var startTimestamp = Stopwatch.GetTimestamp();
        using var response = await _httpClient.PostAsync("chat/completions", content);
        var elapsedTime = Stopwatch.GetElapsedTime(startTimestamp);

        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"OpenRouter's `chat/completions` returned {(int)response.StatusCode} ({response.StatusCode}). Body: {responseJson}");
        }

        var apiResponse = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseJson)
                          ?? throw new InvalidOperationException("Empty response from OpenRouter.");

        var firstChoice = apiResponse.Choices?.FirstOrDefault()
                          ?? throw new InvalidOperationException("OpenRouter response contains no choices.");

        var contentText = firstChoice.Message?.Content ?? string.Empty;

        return new ChatCompletionResult(apiResponse.Id, contentText, elapsedTime);
    }

    public async Task<GenerationInfoResult> GetGenerationAsync(string generationId)
    {
        ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(generationId))
            throw new ArgumentException("Generation Id must be provided.", nameof(generationId));

        var url = $"generation?id={Uri.EscapeDataString(generationId)}";

        using var response = await _httpClient.GetAsync(url);

        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"OpenRouter's `generation` returned {(int)response.StatusCode} ({response.StatusCode}). Body: {responseJson}");
        }

        var apiResponse = JsonConvert.DeserializeObject<GenerationInfoResponse>(responseJson)
                          ?? throw new InvalidOperationException("Empty generation response from OpenRouter.");

        var data = apiResponse.Data
                   ?? throw new InvalidOperationException("Generation response has no data.");

        return new GenerationInfoResult(
            data.Id,
            data.Latency,
            data.ModerationLatency ?? 0,
            data.GenerationTime,
            data.NativeTokensPrompt,
            data.NativeTokensCompletion,
            data.TotalCost
        );
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(OpenRouterService));
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _httpClient.Dispose();
        _disposed = true;
    }
}
