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
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for configuring the context.</param>
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class with no parameters.
    /// Typically used for design-time scenarios.
    /// </summary>
    protected AppDbContext()
    {
    }

    /// <summary>
    /// Gets or sets the collection of employees.
    /// </summary>
    public DbSet<Employee> Employees { get; set; }

    /// <summary>
    /// Gets or sets the collection of warehouse items in inventory.
    /// </summary>
    public DbSet<WarehouseItem> WarehouseItems { get; set; }

    /// <summary>
    /// Gets or sets the collection of vendors.
    /// </summary>
    public DbSet<Vendor> Vendors { get; set; }

    /// <summary>
    /// Gets or sets the collection of inbound deliveries (incoming inventory).
    /// </summary>
    public DbSet<InboundDelivery> InboundDeliveries { get; set; }

    /// <summary>
    /// Gets or sets the collection of outbound deliveries (outgoing inventory).
    /// </summary>
    public DbSet<OutboundDelivery> OutboundDeliveries { get; set; }

    /// <summary>
    /// Gets or sets the collection of customer orders.
    /// </summary>
    public DbSet<Order> Orders { get; set; }

    /// <summary>
    /// Gets or sets the collection of company data and information.
    /// </summary>
    public DbSet<CompanyData> CompanyData { get; set; }


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

