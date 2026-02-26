using Microsoft.EntityFrameworkCore;
using MCPServer.Models;

namespace MCPServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customer", schema: "public");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FirstName).HasColumnName("firstname").HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasColumnName("lastname").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Created).HasColumnName("created");
            entity.Property(e => e.Updated).HasColumnName("updated");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("product", schema: "public");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").IsRequired();
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.Created).HasColumnName("created");
            entity.Property(e => e.Updated).HasColumnName("updated");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("order", schema: "public");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(10).IsRequired();
            entity.Property(e => e.Created).HasColumnName("created");
            entity.Property(e => e.Updated).HasColumnName("updated");
            entity.HasOne(e => e.Customer).WithMany().HasForeignKey(e => e.CustomerId);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("order_detail", schema: "public");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice).HasColumnName("unit_price");
            entity.Property(e => e.TotalPrice).HasColumnName("total_price");
            entity.Property(e => e.Created).HasColumnName("created");
            entity.Property(e => e.Updated).HasColumnName("updated");
            entity.HasOne(e => e.Order).WithMany(o => o.Details).HasForeignKey(e => e.OrderId);
            entity.HasOne(e => e.Product).WithMany().HasForeignKey(e => e.ProductId);
        });
    }
}
