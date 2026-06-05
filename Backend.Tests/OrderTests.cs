using Backend.Data;
using Backend.Services.Customer;
using Backend.Services.Warehouse;
using ClassLibrary.Dto;
using ClassLibrary.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace Backend.Tests;

public class OrderTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly OrderService _service;
    private readonly WarehouseItemService _itemService;

    public OrderTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new AppDbContext(options);
        _itemService = new WarehouseItemService(_context);

        var deliveryService = new WarehouseDeliveryService(_context);
        _service = new OrderService(_context, deliveryService);
    }

    // --- Helpers ---

    private async Task<WarehouseItem> CreateWarehouseItemAsync(int quantity = 100)
    {
        var item = new WarehouseItem
        {
            Name = "Test Item",
            ItemCode = "TEMP",
            CurrentQuantity = quantity,
            IsAvailable = true
        };
        return await _itemService.CreateAsync(item);
    }

    private CreateOrderDto MakeOrderDto(int warehouseItemId, int quantity = 5) => new()
    {
        UserId = "user123",
        ShippingAddress = "Test Street 123",
        Items = new List<CreateOrderItemDto>
        {
            new() { WarehouseItemId = warehouseItemId, Quantity = quantity, Price = 99.99m }
        }
    };

    // --- CreateOrderAsync ---

    [Fact]
    public async Task CreateOrderAsync_WithValidData_ReturnsOrderWithId()
    {
        var item = await CreateWarehouseItemAsync();
        var dto = MakeOrderDto(item.Id);

        var result = await _service.CreateOrderAsync(dto);

        Assert.True(result.Id > 0);
        Assert.Equal("Test Street 123", result.ShippingAddress);
        Assert.Equal(OrderStatus.Pending, result.Status);
    }

    [Fact]
    public async Task CreateOrderAsync_CreatesOutboundDelivery()
    {
        var item = await CreateWarehouseItemAsync();
        var dto = MakeOrderDto(item.Id);

        var order = await _service.CreateOrderAsync(dto);

        var delivery = await _context.OutboundDeliveries
            .FirstOrDefaultAsync(d => d.OrderId == order.Id);
        Assert.NotNull(delivery);
    }

    [Fact]
    public async Task CreateOrderAsync_DecreasesWarehouseStock()
    {
        var item = await CreateWarehouseItemAsync(quantity: 100);
        var dto = MakeOrderDto(item.Id, quantity: 10);

        await _service.CreateOrderAsync(dto);

        var updated = await _itemService.GetByIdAsync(item.Id);
        Assert.Equal(90, updated!.CurrentQuantity);
    }

    [Fact]
    public async Task CreateOrderAsync_WithMultipleItems_CreatesAllItems()
    {
        var item1 = await CreateWarehouseItemAsync(50);
        var item2 = await CreateWarehouseItemAsync(30);

        var dto = new CreateOrderDto
        {
            UserId = "user123",
            ShippingAddress = "Test Street 123",
            Items = new List<CreateOrderItemDto>
            {
                new() { WarehouseItemId = item1.Id, Quantity = 5, Price = 100m },
                new() { WarehouseItemId = item2.Id, Quantity = 3, Price = 200m }
            }
        };

        var result = await _service.CreateOrderAsync(dto);

        Assert.Equal(2, result.Items.Count);

        var updated1 = await _itemService.GetByIdAsync(item1.Id);
        var updated2 = await _itemService.GetByIdAsync(item2.Id);
        Assert.Equal(45, updated1!.CurrentQuantity);
        Assert.Equal(27, updated2!.CurrentQuantity);
    }

    [Fact]
    public async Task CreateOrderAsync_InsufficientStock_ThrowsInvalidOperationException()
    {
        var item = await CreateWarehouseItemAsync(quantity: 5);
        var dto = MakeOrderDto(item.Id, quantity: 10);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateOrderAsync(dto));
    }

    [Fact]
    public async Task CreateOrderAsync_InsufficientStock_DoesNotSaveOrder()
    {
        var item = await CreateWarehouseItemAsync(quantity: 5);
        var dto = MakeOrderDto(item.Id, quantity: 10);

        try { await _service.CreateOrderAsync(dto); } catch { }

        var orders = await _service.GetAllOrdersAsync();
        Assert.Empty(orders);
    }

    // --- GetOrderByIdAsync ---

    [Fact]
    public async Task GetOrderByIdAsync_ExistingOrder_ReturnsOrder()
    {
        var item = await CreateWarehouseItemAsync();
        var order = await _service.CreateOrderAsync(MakeOrderDto(item.Id));

        var result = await _service.GetOrderByIdAsync(order.Id);

        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
    }

    [Fact]
    public async Task GetOrderByIdAsync_NonExisting_ReturnsNull()
    {
        var result = await _service.GetOrderByIdAsync(999);

        Assert.Null(result);
    }

    // --- GetAllOrdersAsync ---

    [Fact]
    public async Task GetAllOrdersAsync_ReturnsAllOrders()
    {
        var item = await CreateWarehouseItemAsync(1000);

        await _service.CreateOrderAsync(MakeOrderDto(item.Id, 1));
        await _service.CreateOrderAsync(MakeOrderDto(item.Id, 1));
        await _service.CreateOrderAsync(MakeOrderDto(item.Id, 1));

        var result = await _service.GetAllOrdersAsync();

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetAllOrdersAsync_EmptyDatabase_ReturnsEmptyList()
    {
        var result = await _service.GetAllOrdersAsync();

        Assert.Empty(result);
    }

    // --- UpdateOrderAsync ---

    [Fact]
    public async Task UpdateOrderAsync_WithValidData_UpdatesFields()
    {
        var item = await CreateWarehouseItemAsync();
        var order = await _service.CreateOrderAsync(MakeOrderDto(item.Id));

        var dto = new UpdateOrderDto
        {
            Id = order.Id,
            UserId = "user123",
            ShippingAddress = "New Address 456",
            Status = OrderStatus.Shipped
        };

        var result = await _service.UpdateOrderAsync(dto);

        Assert.Equal(OrderStatus.Shipped, result.Status);
        Assert.Equal("New Address 456", result.ShippingAddress);
    }

    [Fact]
    public async Task UpdateOrderAsync_NonExistingOrder_ThrowsKeyNotFoundException()
    {
        var dto = new UpdateOrderDto
        {
            Id = 999,
            UserId = "user123",
            ShippingAddress = "Address",
            Status = OrderStatus.Shipped
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.UpdateOrderAsync(dto));
    }

    // --- DeleteOrderAsync ---

    [Fact]
    public async Task DeleteOrderAsync_ExistingOrder_ReturnsTrue()
    {
        var item = await CreateWarehouseItemAsync();
        var order = await _service.CreateOrderAsync(MakeOrderDto(item.Id));

        var result = await _service.DeleteOrderAsync(order.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteOrderAsync_ExistingOrder_RemovesFromDatabase()
    {
        var item = await CreateWarehouseItemAsync();
        var order = await _service.CreateOrderAsync(MakeOrderDto(item.Id));

        await _service.DeleteOrderAsync(order.Id);

        var deleted = await _service.GetOrderByIdAsync(order.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteOrderAsync_NonExistingOrder_ReturnsFalse()
    {
        var result = await _service.DeleteOrderAsync(999);

        Assert.False(result);
    }

    public void Dispose() => _context.Dispose();
}