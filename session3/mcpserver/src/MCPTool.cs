using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using MCPServer.Models;
using MCPServer.Data;
using Microsoft.EntityFrameworkCore;

namespace MCPServer;

[McpServerToolType, Description("Tool for managing customers, products and orders in the database.")]
public class MCPTool
{
    private readonly DataManager _dataManager;

    public MCPTool(DataManager dataManager)
    {
        _dataManager = dataManager;
    }

    [McpServerTool(Name = "GetCustomers"), Description("Retrieve all customers from the database.")]
    public async Task<List<Customer>> GetCustomers(int? limit = null) {
        return await _dataManager.GetCustomersAsync(limit);
    }

    [McpServerTool(Name = "GetProducts"), Description("Retrieve all products from the database.")]
    public async Task<List<Product>> GetProducts(int? limit = null) {
        return await _dataManager.GetProductsAsync(limit);
    }

    [McpServerTool(Name = "GetOrders"), Description("Retrieve all orders with details from the database.")]
    public async Task<List<Order>> GetOrders(int? limit = null) {
        return await _dataManager.GetOrdersWithDetailsAsync(limit);
    }
}