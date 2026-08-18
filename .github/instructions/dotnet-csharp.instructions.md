---
description: "Use when writing, reviewing, or refactoring C# or .NET code. Covers naming conventions, async/await, null handling, exceptions, dependency injection, LINQ, project structure, and Microsoft.Extensions.AI patterns."
applyTo: "**/*.cs"
---

# .NET & C# Coding Instructions

## Naming Conventions

- **Types** (classes, records, structs, interfaces, enums): `PascalCase` — `ChatService`, `IMessageHandler`
- **Methods and properties**: `PascalCase` — `GetResponseAsync`, `ChatHistory`
- **Local variables and parameters**: `camelCase` — `chatClient`, `userPrompt`
- **Private fields**: `_camelCase` — `_chatClient`, `_chatHistory`
- **Constants**: `PascalCase` — `MaxRetries`, `DefaultTimeout`
- **Async methods**: always suffix with `Async` — `SendMessageAsync`, `LoadModelAsync`
- **Interfaces**: prefix with `I` — `IChatClient`, `IModelProvider`

## Async / Await

- Always use `async`/`await` — never `.Result` or `.Wait()` (causes deadlocks)
- Return `Task` for void-returning async methods; `Task<T>` for values; `IAsyncEnumerable<T>` for streams
- Use `ConfigureAwait(false)` in library code; omit it in application code (ASP.NET Core, console apps)
- Prefer `await foreach` for `IAsyncEnumerable<T>` streams:

```csharp
await foreach (var update in chatClient.GetStreamingResponseAsync(chatHistory))
{
    Console.Write(update.Text);
}
```

- Always pass `CancellationToken` through async call chains; accept it as the last parameter

## Null Handling (Nullable Reference Types)

- Enable nullable reference types: `<Nullable>enable</Nullable>` in the project file
- Use `?` to explicitly declare nullable: `string? userPrompt`
- Prefer null-coalescing patterns over null checks where readable: `value ?? defaultValue`
- Use `ArgumentNullException.ThrowIfNull(param)` for guard clauses at method boundaries
- Avoid suppressing nulls with `!` unless you have certain knowledge the value is non-null

## Exception Handling

- Catch specific exceptions, not `Exception` as a catch-all unless logging and rethrowing
- Always include context in exception messages: what failed, what the input was
- Use `try/catch` only at logical boundaries — let exceptions propagate otherwise
- Prefer `throw;` (re-throw) over `throw ex;` (which resets the stack trace)
- For expected failure paths, prefer `bool TryX(out T result)` patterns or `Result<T>` types over exceptions

## Dependency Injection

- Register services in `IServiceCollection` using the appropriate lifetime:
  - `AddSingleton` — stateless, shared state (e.g., `IChatClient`)
  - `AddScoped` — per-request (e.g., in ASP.NET Core)
  - `AddTransient` — stateless, lightweight (e.g., validators)
- Accept dependencies via constructor injection; never use `ServiceLocator` anti-pattern
- Depend on interfaces/abstractions, not concrete types

## LINQ

- Prefer method syntax over query syntax for short expressions
- Avoid multiple enumeration of `IEnumerable<T>` — materialize with `.ToList()` or `.ToArray()` once
- Use `.FirstOrDefault()` with a null-check rather than `.First()` when the element may be missing
- Prefer `.Select()`, `.Where()`, `.Any()` over equivalent `foreach` loops for transformations and checks

## Project & File Structure

- Use **file-scoped namespaces**: `namespace MyApp.Services;` (not block-scoped)
- One public type per file; filename matches the type name
- Use **top-level statements** for entry points (`Program.cs`) — no boilerplate `Main` method
- Group files by feature/domain, not by type (avoid `Models/`, `Controllers/` as top-level folders)
- Use **primary constructors** (C# 12+) for simple types with injected dependencies

## Microsoft.Extensions.AI Patterns

- Use `IChatClient` as the abstraction — never depend directly on a provider (`OllamaApiClient`, Azure client, etc.)
- Maintain conversation context via `List<ChatMessage>` and append both user and assistant turns
- Use `GetStreamingResponseAsync` for user-facing output to avoid blocking on full responses
- Wire up `IChatClient` through DI using `AddChatClient()` extensions when using a host
- Pass the full `chatHistory` list on every request — models are stateless between calls

```csharp
// Preferred: streaming with history
var response = "";
await foreach (var update in chatClient.GetStreamingResponseAsync(chatHistory))
{
    Console.Write(update.Text);
    response += update.Text;
}
chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
```
