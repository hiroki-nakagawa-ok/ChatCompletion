using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

namespace ChatCompletion;

internal class ChatCompletionService
{
    private static readonly string API_KEY = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";

    private static readonly string MODEL = "gemini-2.0-flash-lite";

    private readonly Kernel kernel;

    public ChatCompletionService()
    {
        kernel = Kernel.CreateBuilder()
            .AddGoogleAIGeminiChatCompletion(MODEL, API_KEY)
            .Build();
    }

    public ChatTimeLime StartChat()
    {
        return new ChatTimeLime(kernel);
    }

    public class ChatTimeLime(Kernel kernel)
    {
        public ChatHistory History { get; } = new();

        public void AddUserMessage(string content) => History.AddUserMessage(content);

        public async Task<string> GetChatMessageContentsAsync()
        {
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            var complations = new ChatHistory();
            while (!complations.Any(mbox => "stop" == mbox?.Metadata?["FinishReason"]?.ToString()?.ToLower()))
            {
                var completion = await chatCompletionService.GetChatMessageContentsAsync(History, new GeminiPromptExecutionSettings(), kernel);
                History.AddRange(completion);
                complations.AddRange(completion);
            }
            return string.Join("\n", complations.Where(content => content.Role == AuthorRole.Assistant)
                .Select(content => content.Content ?? "")).Replace("\n", "\r\n");
        }

    }
}
