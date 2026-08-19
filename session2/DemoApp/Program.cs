using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OllamaSharp;
using OpenAI;
using System.ClientModel;

var useLmStudio = args.Contains("--lmstudio");

var model = "mlx-community/Phi-3-mini-128k-instruct-4bit";

IChatClient chatClient = useLmStudio
    ? new OpenAIClient(
              new ApiKeyCredential("lm-studio"),
              new OpenAIClientOptions { Endpoint = new Uri("http://localhost:1234/v1") })
          .GetChatClient(model).AsIChatClient()
    : new OllamaApiClient(new Uri("http://localhost:11434/"), model);

// Console.WriteLine(useLmStudio ? "Backend: LM Studio (http://localhost:1234)" : "Backend: Ollama (http://localhost:11434)");

// Console.WriteLine("Connecting to MCP server at http://localhost:5186...");
// await using var mcpClient = await McpClient.CreateAsync(
//     new HttpClientTransport(new HttpClientTransportOptions
//     {
//         Endpoint = new Uri("http://localhost:5186"),
//         Name = "DemoAppMCP"
//     }));

// var tools = await mcpClient.ListToolsAsync();
// Console.WriteLine($"Loaded {tools.Count} tools from the MCP server.");
// foreach (var tool in tools)
// {
//     Console.WriteLine($"- {tool.Name}: {tool.Description}");
// }

// Start the conversation with context for the AI model
List<ChatMessage> chatHistory = new();

while (true)
{
    // Get user prompt and add to chat history
    Console.WriteLine("Your prompt:");
    var userPrompt = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userPrompt))
    {
        continue;
    }

    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

    // Stream the AI response and add to chat history
    Console.WriteLine("AI Response:");
    var response = "";
    // var options = new ChatOptions { Tools = [.. tools] };
    var options = new ChatOptions();
    await foreach (ChatResponseUpdate item in
        chatClient.GetStreamingResponseAsync(chatHistory, options))
    {
        Console.Write(item.Text);
        response += item.Text;
    }
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
    Console.WriteLine();
}