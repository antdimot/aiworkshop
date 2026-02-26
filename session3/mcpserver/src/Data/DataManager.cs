using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MCPServer.Models;

namespace MCPServer.Data;

public class DataManager
{
	private readonly AppDbContext _db;

	public DataManager(AppDbContext db)
	{
		_db = db;
	}

	public Task<List<Customer>> GetCustomersAsync(int? limit = null)
	{
		var query = _db.Customers.AsNoTracking();

		if (limit.HasValue) query = query.Take(limit.Value);

		return query.ToListAsync();
	}

	public Task<List<Product>> GetProductsAsync(int? limit = null)
	{
		var query = _db.Products.AsNoTracking();

		if (limit.HasValue) query = query.Take(limit.Value);

		return query.ToListAsync();
	}

	public Task<List<Order>> GetOrdersWithDetailsAsync(int? limit = null)
	{
		var query = _db.Orders
			.Include(o => o.Customer)
			.Include(o => o.Details)
				.ThenInclude(d => d.Product)
			.AsNoTracking();

		if (limit.HasValue) query = query.Take(limit.Value);

		return query.ToListAsync();
	}
}
