using Microsoft.Extensions.AI;
using OllamaSharp;
using OpenAI;
using System.ClientModel;

var useLmStudio = args.Contains("--lmstudio");

var model = "google/gemma-4-e2b";

IChatClient chatClient = useLmStudio
    ? new OpenAIClient(
              new ApiKeyCredential("lm-studio"),
              new OpenAIClientOptions { Endpoint = new Uri("http://localhost:1234/v1") })
          .GetChatClient(model).AsIChatClient()
    : new OllamaApiClient(new Uri("http://localhost:11434/"), model);

Console.WriteLine(useLmStudio ? "Backend: LM Studio (http://localhost:1234)" : "Backend: Ollama (http://localhost:11434)");

// Start the conversation with context for the AI model
List<ChatMessage> chatHistory = new();

while (true)
{
    // Get user prompt and add to chat history
    Console.WriteLine("Your prompt:");
    var userPrompt = Console.ReadLine();
    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

    // Stream the AI response and add to chat history
    Console.WriteLine("AI Response:");
    var response = "";
    await foreach (ChatResponseUpdate item in
        chatClient.GetStreamingResponseAsync(chatHistory))
    {
        Console.Write(item.Text);
        response += item.Text;
    }
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
    Console.WriteLine();
}