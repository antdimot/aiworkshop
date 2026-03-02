using System;
using System.Text.Json.Serialization;

namespace MCPServer.Models;

public class OrderDetail
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? Updated { get; set; }

    [JsonIgnore]
    public Order? Order { get; set; }
    public Product? Product { get; set; }
}