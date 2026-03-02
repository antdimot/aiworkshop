using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using MCPServer.Models;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using Microsoft.Extensions.AI;

namespace MCPServer;

[McpServerPromptType, Description("Prompt for suggesting products based on customer preferences and past purchases.")]
public class MCPPrompt
{
    [McpServerPrompt, Description("Generate product suggestion for new order prompt")]
    public static IEnumerable<ChatMessage> SuggestProductByCustomer(
        [Description("Full name of the customer")] string fullName)
    {
        return [
            new(ChatRole.System, "You are an assistant for an e-commerce platform. You help customers find products based on their preferences and past purchases."),
            new(ChatRole.User, $"Please suggest products for the customer with full name: {fullName}"),
            new(ChatRole.Assistant, "To suggest products for the customer, I will need to look up their past purchases and preferences in the database. I will use the GetCustomer and GetOrders tools to retrieve this information and then analyze it to find suitable product suggestions.")
        ];
    }
}