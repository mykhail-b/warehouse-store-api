using ClassLibrary.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

/// <summary>
/// The main database context for the warehouse management system.
/// Extends IdentityDbContext to include ASP.NET Core Identity functionality for user authentication.
/// </summary>
/// <remarks>
/// This context manages all entities in the system including employees, warehouse items, vendors,
/// deliveries, orders, and company data. It also handles user authentication through Identity.
/// </remarks>
public class AppDbContext : IdentityDbContext<UserAccount>
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected AppDbContext()
    {
    }

    public DbSet<WarehouseItem> WarehouseItems { get; set; }
    public DbSet<OutboundDelivery> OutboundDeliveries { get; set; }
    public DbSet<Order> Orders { get; set; }

    /// <summary>
    /// Configures the model relationships and constraints.
    /// </summary>
    /// <param name="builder">The model builder for configuring the model.</param>
    /// <remarks>
    /// Configures cascading delete behavior for Order-OrderItem relationships.
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

