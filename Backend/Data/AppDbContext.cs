using ClassLibrary.Entity.Warehouse;
using Microsoft.AspNetCore.Identity;
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

    public DbSet<WarehouseItem> WarehouseItems { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<InboundDelivery> InboundDeliveries { get; set; }
    public DbSet<OutboundDelivery> OutboundDeliveries { get; set; }
}

public class UserAccount : IdentityUser
{

}