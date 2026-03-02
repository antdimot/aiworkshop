using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ModelContextProtocol.Server;
using MCPServer.Models;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;

namespace MCPServer;

[McpServerToolType, Description("Tool for managing customers, products and orders in the database.")]
public class MCPTool
{
    [McpServerTool(Name = "GetCustomers"), Description("Retrieve all customers from the database.")]
    public static async Task<object> GetCustomers(AppDbContext db, CancellationToken ct, ILogger<MCPTool> logger, [Description("Maximum number of customers to retrieve.")] int max = 100 ){
        logger.LogDebug( $"Executing GetCustomers tool with max : {max }" );
        
        var query = db.Customers.AsNoTracking();

		return  await query.OrderBy(c => c.LastName)
                            .Take(max)
                            .ToListAsync(ct);
    }

    [McpServerTool(Name = "GetCustomer"), Description("Retrieve a single customer from the database.")]
    public static async Task<object> GetCustomer(AppDbContext db, CancellationToken ct, ILogger<MCPTool> logger, [Description("Full name of the customer to retrieve.")] string fullName ){
        logger.LogDebug( $"Executing GetCustomer tool with full name : {fullName }" );
        
        var query = db.Customers.AsNoTracking();

		var result = await query.Where(c => (c.FirstName + " " + c.LastName) == fullName)
                                .FirstOrDefaultAsync(ct);

        if (result == null)
            return $"Customer with full name {fullName} not found.";

        return result;
    }

    [McpServerTool(Name = "GetProducts"), Description("Retrieve all products from the database.")]
    public static async Task<List<Product>> GetProducts(AppDbContext db, CancellationToken ct, [Description("Maximum number of products to retrieve.")] int max = 100 ) {
        var query = db.Products.AsNoTracking();

		return await query.OrderBy(p => p.Type)
                            .ThenBy(p => p.Title)
                            .Take(max)
                            .ToListAsync(ct);
    }

    [McpServerTool(Name = "GetOrders"), Description("Retrieve all orders with details from the database.")]
    public static async Task<List<Order>> GetOrders(AppDbContext db, CancellationToken ct ) {
		var query = db.Orders
			.Include(o => o.Customer)
			.Include(o => o.Details)
				.ThenInclude(d => d.Product)
			.AsNoTracking();

        return await query.ToListAsync(ct);
    }

    [McpServerTool(Name = "AddOrder"), Description("Add a new order to the database.")]
    public static async Task<object> AddOrder(AppDbContext db, McpServer server, CancellationToken ct) {
         // Check if the client supports elicitation
        if (server.ClientCapabilities?.Elicitation == null)
           return "Client does not support elicitation";

        var result = await server.ElicitAsync(new ElicitRequestParams
        {
            Message = "Give me the details for the new order",
            RequestedSchema = new ElicitRequestParams.RequestSchema
            {
                Properties = new Dictionary<string, ElicitRequestParams.PrimitiveSchemaDefinition>
                {
                    ["firstName"] = new ElicitRequestParams.StringSchema
                    {
                        Description = "First name?",
                        Default = null
                    },
                    ["lastName"] = new ElicitRequestParams.StringSchema
                    {
                        Description = "Last name?",
                        Default = null
                    },
                    ["productCode"] = new ElicitRequestParams.StringSchema
                    {
                        Description = "Product code?",
                        Default = null
                    },
                    ["productQuantity"] = new ElicitRequestParams.NumberSchema
                    {
                        Description = "Product quantity?",
                        Default = 1
                    }
                }
            }
        }, ct);

        if(result.Content?["firstName"] == null || result.Content?["lastName"] == null || result.Content?["productCode"] == null)
        {
            return "Missing required information. Order creation cancelled.";
        }
        else
        {
            var newOrder = new Order()
            {
                Details = new List<OrderDetail>(),
                Created = DateTime.UtcNow
            };

            // check if customer exists, if not return error (for simplicity, we don't create new customers here)
            var customer = await db.Customers.FirstOrDefaultAsync(c => c.FirstName == result.Content["firstName"].ToString() && c.LastName == result.Content["lastName"].ToString(), ct);
            if(customer != null)  
                newOrder.Customer = customer;
            else
                return "Customer not found.";
            
            // check if product exists and has enough quantity, if not return error
            var product = await db.Products.FirstOrDefaultAsync(p => p.Code == result.Content["productCode"].ToString(), ct);
            if(product != null && product.Quantity >= result.Content["productQuantity"].GetInt32())
            {
                newOrder.Details.Add(new OrderDetail()
                {
                    Product = product,
                    Quantity = result.Content["productQuantity"].GetInt32(),
                    UnitPrice = product.Price,
                    TotalPrice = product.Price * result.Content["productQuantity"].GetInt32()
                });
                newOrder.Status = "NEW";
            }
            else
                return "Product not found or insufficient quantity.";


            await using var tx = await db.Database.BeginTransactionAsync(ct);
            try {
                product.Quantity -= result.Content["productQuantity"].GetInt32();

                db.Products.Update(product);
                db.Orders.Add(newOrder);

                await db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                return newOrder;
            }
            catch(Exception ex)
            {
                await tx.RollbackAsync(ct);
                throw new McpException("Error occurred while adding the order.", ex);
            }
        }
    }
}