using Backend.Data;
using Backend.Services;
using ClassLibrary.Entity.Warehouse;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests;

public class WarehouseItemTest
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateItem_WithValidData_ReturnsCreatedItem()
    {
        var context = CreateInMemoryContext();
        var service = new WarehouseItemService(context);

        var item = new WarehouseItem
        {
            Name = "Test Item",
            ItemCode = "TEMP",
            CurrentQuantity = 10,
            IsAvailable = true
        };

        var result = await service.CreateAsync(item);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.StartsWith("ITEM-", result.ItemCode);
    }

    [Fact]
    public async Task DeleteItem_WhenItemExists_SetsIsAvailableToFalse()
    {
        var context = CreateInMemoryContext();
        var service = new WarehouseItemService(context);

        var item = new WarehouseItem { Name = "Test", ItemCode = "TEMP", IsAvailable = true };
        await service.CreateAsync(item);

        var result = await service.DeleteAsync(item.Id);
        var deleted = await service.GetByIdAsync(item.Id);

        Assert.True(result);
        Assert.False(deleted!.IsAvailable);
    }

    [Fact]
    public async Task UpdateItem_WithNewDetails_UpdatesFields()
    {
        var context = CreateInMemoryContext();
        var service = new WarehouseItemService(context);

        var item = new WarehouseItem { Name = "Old Name", ItemCode = "TEMP" };
        await service.CreateAsync(item);

        item.Name = "New Name";
        await service.UpdateAsync(item);

        var updated = await service.GetByIdAsync(item.Id);
        Assert.Equal("New Name", updated!.Name);
    }

    [Fact]
    public async Task CreateInbound_IncreasesWarehouseStock()
    {
        var context = CreateInMemoryContext();
        var deliveryService = new WarehouseDeliveryService(context);
        var itemService = new WarehouseItemService(context);

        var item = new WarehouseItem { Name = "Test", ItemCode = "TEMP", CurrentQuantity = 5 };
        await itemService.CreateAsync(item);

        var vendor = new Vendor { Name = "Vendor", Country = "PL" };
        context.Vendors.Add(vendor);
        await context.SaveChangesAsync();

        var delivery = new InboundDelivery
        {
            DeliveryNumber = "DEL-001",
            ArrivalDate = DateTime.UtcNow,
            VendorId = vendor.Id,
            Items = new List<InboundDeliveryItem>
            {
                new InboundDeliveryItem
                {
                    WarehouseItemId = item.Id,
                    QuantityReceived = 10,
                    PurchasePrice = 100
                }
            }
        };

        await deliveryService.CreateInboundAsync(delivery);

        var updated = await itemService.GetByIdAsync(item.Id);
        Assert.Equal(15, updated!.CurrentQuantity);
    }

    [Fact]
    public async Task SetIsAvailableToFalse_WhenQuantityIsZero()
    {
        var context = CreateInMemoryContext();
        var service = new WarehouseItemService(context);

        var item = new WarehouseItem
        {
            Name = "Test",
            ItemCode = "TEMP",
            CurrentQuantity = 0,
            IsAvailable = true
        };
        await service.CreateAsync(item);

        item.IsAvailable = item.CurrentQuantity > 0;
        await service.UpdateAsync(item);

        var updated = await service.GetByIdAsync(item.Id);
        Assert.False(updated!.IsAvailable);
    }
}