﻿// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace WorkflowDemo2;

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
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "https://adm-openai.openai.azure.com/";
        var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? "";
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4.1-nano";
        var chatClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey)).GetChatClient(deploymentName).AsIChatClient();

        Console.Write("Choose workflow type ('sequential', 'concurrent'): ");
        switch (Console.ReadLine())
        {
            case "sequential":
                await RunWorkflowAsync(
                    AgentWorkflowBuilder.BuildSequential(from lang in (string[])["French","English", "Spanish" ] select GetTranslationAgent(lang, chatClient)),
                    [new(ChatRole.User, "Ciao Mondo!")]);
                break;

            case "concurrent":
                await RunWorkflowAsync(
                    AgentWorkflowBuilder.BuildConcurrent(from lang in (string[])["French", "English", "Spanish", ] select GetTranslationAgent(lang, chatClient)),
                    [new(ChatRole.User, "Ciao Mondo!")]);
                break;

            default:
                throw new InvalidOperationException("Invalid workflow type.");
        }

        static async Task<List<ChatMessage>> RunWorkflowAsync(Workflow workflow, List<ChatMessage> messages)
        {
            string? lastExecutorId = null;

            await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, messages);
            await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
            await foreach (WorkflowEvent evt in run.WatchStreamAsync())
            {
                if (evt is AgentResponseUpdateEvent e)
                {
                    if (e.ExecutorId != lastExecutorId)
                    {
                        lastExecutorId = e.ExecutorId;
                        Console.WriteLine();
                        Console.WriteLine(e.ExecutorId);
                    }

                    Console.Write(e.Update.Text);
                    if (e.Update.Contents.OfType<FunctionCallContent>().FirstOrDefault() is FunctionCallContent call)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"  [Calling function '{call.Name}' with arguments: {JsonSerializer.Serialize(call.Arguments)}]");
                    }
                }
                else if (evt is WorkflowOutputEvent output)
                {
                    Console.WriteLine();
                    return output.As<List<ChatMessage>>()!;
                }
            }

            return [];
        }
    }

    /// <summary>Creates a translation agent for the specified target language.</summary>
    private static ChatClientAgent GetTranslationAgent(string targetLanguage, IChatClient chatClient) =>
        new(chatClient,
            $"You are a translation assistant who only responds in {targetLanguage}. Respond to any " +
            $"input by outputting the name of the input language and then translating the input to {targetLanguage}.");
}