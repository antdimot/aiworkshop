// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace WorkflowAgentsInWorkflowsSample;

/// <summary>
/// This sample introduces the use of AI agents as executors within a workflow,
/// using <see cref="AgentWorkflowBuilder"/> to compose the agents into one of
/// several common patterns.
/// </summary>
/// <remarks>
/// Pre-requisites:
/// - An Azure OpenAI chat completion deployment must be configured.
/// </remarks>
public static class Program
{
    private static async Task Main()
    {
        // Set up the Azure OpenAI client.
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "https://models.github.ai/inference";
        var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? "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";
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "openai/gpt-4o-mini";
        var chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey)).GetChatClient(deploymentName).AsIChatClient();

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
        await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, new ChatMessage(ChatRole.User, "Hello World!"));

        // Must send the turn token to trigger the agents.
        // The agents are wrapped as executors. When they receive messages,
        // they will cache the messages and only start processing when they receive a TurnToken.
        await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
        await foreach (WorkflowEvent evt in run.WatchStreamAsync())
        {
            if (evt is ExecutorFailedEvent failed)
            {
                Console.WriteLine($"Workflow failed due to an error in executor {failed.ExecutorId}: {failed.Data.Message}");
            }

            if (evt is AgentResponseUpdateEvent executorComplete)
            {
                Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
            }
        }
    }

    /// <summary>
    /// Creates a translation agent for the specified target language.
    /// </summary>
    /// <param name="targetLanguage">The target language for translation</param>
    /// <param name="chatClient">The chat client to use for the agent</param>
    /// <returns>A ChatClientAgent configured for the specified language</returns>
    private static ChatClientAgent GetTranslationAgent(string targetLanguage, IChatClient chatClient) =>
        new(chatClient, $"You are a translation assistant that translates the provided text to {targetLanguage}.");
} 