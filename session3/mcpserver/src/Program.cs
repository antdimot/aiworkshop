using ModelContextProtocol.Server;
using System.ComponentModel;
using MCPServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
// Add logging providers
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithPrompts<MCPPrompt>()
    .WithToolsFromAssembly();

// Configure DbContext and DataManager using connection string from appsettings
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

app.MapMcp();

app.Run();