// Copyright (c) Microsoft. All rights reserved.

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

        Console.Write("Choose workflow type ('handoffs', 'groupchat'): ");
        switch (Console.ReadLine())
        {
            case "handoffs":
                ChatClientAgent historyTutor = new(chatClient,
                    "You provide assistance with historical queries. Explain important events and context clearly. Only respond about history.",
                    "history_tutor",
                    "Specialist agent for historical questions");
                ChatClientAgent mathTutor = new(chatClient,
                    "You provide help with math problems. Explain your reasoning at each step and include examples. Only respond about math.",
                    "math_tutor",
                    "Specialist agent for math questions");
                ChatClientAgent triageAgent = new(chatClient,
                    "You determine which agent to use based on the user's homework question. ALWAYS handoff to another agent.",
                    "triage_agent",
                    "Routes messages to the appropriate specialist agent");
                var workflow = AgentWorkflowBuilder.CreateHandoffBuilderWith(triageAgent)
                    .WithHandoffs(triageAgent, [mathTutor, historyTutor])
                    .WithHandoffs([mathTutor, historyTutor], triageAgent)
                    .Build();

                List<ChatMessage> messages = [];
                while (true)
                {
                    Console.Write("Q: ");
                    messages.Add(new(ChatRole.User, Console.ReadLine()));
                    messages.AddRange(await RunWorkflowAsync(workflow, messages));
                }

            case "groupchat":
                // Create a copywriter agent
                ChatClientAgent writer = new(chatClient,
                    "You are a creative copywriter. Generate catchy slogans and marketing copy. Be concise and impactful.",
                    "CopyWriter",
                    "A creative copywriter agent");

                // Create a reviewer agent
                ChatClientAgent reviewer = new(chatClient,
                    "You are a marketing reviewer. Evaluate slogans for clarity, impact, and brand alignment. " +
                    "Provide constructive feedback or approval.",
                    "Reviewer",
                    "A marketing review agent");

                await RunWorkflowAsync(
                    AgentWorkflowBuilder.CreateGroupChatBuilderWith(agents =>
                            new RoundRobinGroupChatManager(agents)
                            {
                                MaximumIterationCount = 5  // Maximum number of turns
                            })
                        .AddParticipants(writer, reviewer)
                        .Build(),
                    [new(ChatRole.User, "Create a slogan for an eco-friendly electric vehicle.")]);
                break;

            default:
                throw new InvalidOperationException("Invalid workflow type.");
        }

        static async Task<List<ChatMessage>> RunWorkflowAsync(Workflow workflow, List<ChatMessage> messages)
        {
            await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, messages);

            await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

            await foreach (WorkflowEvent evt in run.WatchStreamAsync())
            {
                if (evt is AgentResponseUpdateEvent update)
                {
                    // Process streaming agent responses
                    AgentResponse response = update.AsResponse();
                    foreach (ChatMessage message in response.Messages)
                    {
                        Console.WriteLine($"[{update.ExecutorId}]: {message.Text}");
                    }
                }
                else if (evt is WorkflowOutputEvent output)
                {
                    // Workflow completed
                    var conversationHistory = output.As<List<ChatMessage>>();
                    Console.WriteLine("\n=== Final Conversation ===");
                    foreach (var message in conversationHistory)
                    {
                        Console.WriteLine($"{message.AuthorName}: {message.Text}");
                    }
                    break;
                }
            }

            return [];
        }
    }
}