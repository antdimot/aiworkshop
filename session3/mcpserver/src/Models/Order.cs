using System;
using System.Collections.Generic;

namespace MCPServer.Models;

public class Order
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? Updated { get; set; }

    public Customer? Customer { get; set; }
    public List<OrderDetail> Details { get; set; } = new();
}