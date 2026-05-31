using ClassLibrary.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class AppDbContext : IdentityDbContext<UserAccount>
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected AppDbContext()
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<WarehouseItem> WarehouseItems { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<InboundDelivery> InboundDeliveries { get; set; }
    public DbSet<OutboundDelivery> OutboundDeliveries { get; set; }
    public DbSet<Order> Orders { get; set; }


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

