using Backend.Data;
using Backend.Services.Warehouse;
using ClassLibrary.Dto;
using ClassLibrary.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace Backend.Tests;

public class OrderServiceTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateOrderAsync_ValidDto_CreatesOrderAndDecrementsWarehouseQuantity()
    {
        using var context = GetDbContext();

        var product = new Product { Id = 1, CurrentQuantity = 10, ItemCode = $"ITEM-{DateTime.UtcNow:yyyyMMdd}" };
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var orderService = new OrderService(context);

        var itemsDto = new List<OrderItemDto>
        {
            new OrderItemDto { ProductId = 1, Quantity = 3, Price = 100m }
        };
        var serializedItems = JsonSerializer.Serialize(itemsDto);

        var orderCreateDto = new OrderCreateDto
        {
            UserId = "user_123",
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            ShippingAddress = "Test Address",
            StripeSessionId = "session_999",
            SerializedItems = serializedItems
        };

        var result = await orderService.CreateOrderAsync(orderCreateDto);

        Assert.NotNull(result);
        Assert.Equal("user_123", result.UserId);
        Assert.Equal(OrderStatus.Pending, result.Status);
        Assert.Single(result.Items);

        var updatedProduct = await context.Products.FindAsync(1);
        Assert.Equal(7, updatedProduct.CurrentQuantity);

        var orderInDb = await context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == result.Id);
        Assert.NotNull(orderInDb);
        Assert.Equal(300m, orderInDb.TotalPrice);
    }

    [Fact]
    public async Task CreateOrderAsync_NotEnoughStock_ThrowsInvalidOperationException()
    {
        using var context = GetDbContext();

        var product = new Product { Id = 1, CurrentQuantity = 2, ItemCode = $"ITEM-{DateTime.UtcNow:yyyyMMdd}" };
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var orderService = new OrderService(context);

        var itemsDto = new List<OrderItemDto>
        {
            new OrderItemDto { ProductId = 1, Quantity = 5, Price = 100m }
        };
        var serializedItems = JsonSerializer.Serialize(itemsDto);

        var orderCreateDto = new OrderCreateDto
        {
            CustomerName = "Test Johnson",
            CustomerEmail = "test@test.com",
            ShippingAddress = "Test Address",
            SerializedItems = serializedItems
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            orderService.CreateOrderAsync(orderCreateDto)
        );

        Assert.Contains("Not enough goods in stock", exception.Message);

        var updatedProduct = await context.Products.FindAsync(1);
        Assert.Equal(2, updatedProduct.CurrentQuantity);

        var ordersCount = await context.Orders.CountAsync();
        Assert.Equal(0, ordersCount);
    }
}