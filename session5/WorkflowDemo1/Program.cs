// Copyright (c) Microsoft. All rights reserved.

using System.ClientModel;
using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OpenAI;

namespace WorkflowDemo1;

   
public static class Program
{
    private static ChatClientAgent GetTranslationAgent(string targetLanguage, IChatClient chatClient) =>
        new(chatClient, $"You are a translation assistant that translates the provided text to {targetLanguage}.");


    private static async Task Main(string[] args)
    {
        var useLmStudio = args.Contains("--lmstudio");

        var model = "google/gemma-4-e2b";

        IChatClient chatClient = useLmStudio
            ? new OpenAIClient(
                    new ApiKeyCredential("lm-studio"),
                    new OpenAIClientOptions { Endpoint = new Uri("http://localhost:1234/v1") })
                .GetChatClient(model).AsIChatClient()
            : new OllamaApiClient(new Uri("http://localhost:11434/"), model);

        Console.WriteLine(useLmStudio ? "Backend: LM Studio (http://localhost:1234)" : "Backend: Ollama (http://localhost:11434)");

        // Set up the Azure OpenAI client.
        // var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "https://adm-openai.openai.azure.com/";
        // var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? "";
        // var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4.1-nano";
        // var chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey)).GetChatClient(deploymentName).AsIChatClient();

       // Create agents
        AIAgent frenchAgent = GetTranslationAgent("French", chatClient);
        AIAgent spanishAgent = GetTranslationAgent("Spanish", chatClient);
        AIAgent englishAgent = GetTranslationAgent("English", chatClient);

        // Build the workflow by adding executors and connecting them
        var workflow = new WorkflowBuilder(frenchAgent)
            .AddEdge(frenchAgent, spanishAgent)
            .AddEdge(spanishAgent, englishAgent)
            .Build();

        // Execute the workflow
        await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, new ChatMessage(ChatRole.User, "Ciao Mondo!"));

        await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
        await foreach (WorkflowEvent evt in run.WatchStreamAsync())
        {
            if (evt is ExecutorFailedEvent failed)
            {
                Console.WriteLine($"Workflow failed due to an error in executor {failed.ExecutorId}: {failed.Data?.Message}");
            }

            if (evt is AgentResponseUpdateEvent executorComplete)
            {
                Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
            }
        }
    }
} 