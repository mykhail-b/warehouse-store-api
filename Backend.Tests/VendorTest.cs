using Backend.Data;
using Backend.Services;
using ClassLibrary.Entity.Warehouse;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests;

public class VendorTest
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateVendor_WithValidData_SavesVendorToDatabase()
    {
        var context = CreateInMemoryContext();
        var service = new VendorService(context);

        var vendor = new Vendor
        {
            Name = "Test Vendor",
            Country = "PL",
            Email = "test@vendor.com",
            TelNumber = "+48123456789"
        };

        var result = await service.CreateAsync(vendor);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Test Vendor", result.Name);
    }

    [Fact]
    public async Task DeleteVendor_WhenVendorExists_RemovesVendorSuccessfully()
    {
        var context = CreateInMemoryContext();
        var service = new VendorService(context);

        var vendor = new Vendor { Name = "Test Vendor", Country = "PL" };
        await service.CreateAsync(vendor);

        var result = await service.DeleteAsync(vendor.Id);
        var deleted = await service.GetByIdAsync(vendor.Id);

        Assert.True(result);
        Assert.Null(deleted);
    }
}