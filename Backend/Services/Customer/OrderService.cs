using Backend.Data;
using Backend.Services.Warehouse;
using ClassLibrary.Dto;
using ClassLibrary.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Customer;

/// <summary>
/// Defines operations for managing customer orders.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="order">The order to create.</param>
    /// <returns>The created order with an assigned ID.</returns>
    Task<Order> CreateOrderAsync(CreateOrderDto dto);

    /// <summary>
    /// Retrieves a specific order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order to retrieve.</param>
    /// <returns>The order if found; null otherwise.</returns>
    Task<Order?> GetOrderByIdAsync(int id);

    /// <summary>
    /// Retrieves all orders in the system.
    /// </summary>
    /// <returns>A collection of all orders.</returns>
    Task<IEnumerable<Order>> GetAllOrdersAsync();

    /// <summary>
    /// Deletes an order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order to delete.</param>
    /// <returns>True if the order was deleted; false if not found.</returns>
    Task<bool> DeleteOrderAsync(int id);

    /// <summary>
    /// Updates an existing order.
    /// </summary>
    /// <param name="order">The order with updated information.</param>
    /// <returns>The updated order.</returns>
    Task<Order> UpdateOrderAsync(UpdateOrderDto dto);
}

/// <summary>
/// Implementation of <see cref="IOrderService"/> for managing customer orders using Entity Framework Core.
/// </summary>
public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly IWarehouseDeliveryService _warehouseDeliveryService;

    public OrderService(AppDbContext context, IWarehouseDeliveryService warehouseDeliveryService)
    {
        _context = context;
        _warehouseDeliveryService = warehouseDeliveryService;
    }

    /// <summary>
    /// Creates a new order in the database.
    /// </summary>
    /// <param name="order">The order to create.</param>
    /// <returns>The created order with an assigned ID.</returns>
    public async Task<Order> CreateOrderAsync(CreateOrderDto dto)
    {
        foreach (var item in dto.Items)
        {
            var warehouseItem = await _context.WarehouseItems.FindAsync(item.WarehouseItemId);
            if (warehouseItem is null || warehouseItem.CurrentQuantity < item.Quantity)
                throw new InvalidOperationException($"Not enough goods in stock! ID: {item.WarehouseItemId}");
        }

        var newOrder = new Order {
            UserId = dto.UserId,
            ShippingAddress = dto.ShippingAddress,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Items = dto.Items.Select(i => new OrderItem
            {
                WarehouseItemId = i.WarehouseItemId,
                Quantity = i.Quantity
            }).ToList()
        };

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();
            await _warehouseDeliveryService.CreateOutboundAsync(newOrder.Id, newOrder.Items);
            await transaction.CommitAsync();
            return newOrder;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Deletes an order if it exists.
    /// </summary>
    /// <param name="id">The ID of the order to delete.</param>
    /// <returns>True if the order was found and deleted; false otherwise.</returns>
    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(order => order.Id == id);
        if (order is null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Retrieves all orders from the database.
    /// </summary>
    /// <returns>A collection of all orders.</returns>
    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    /// <summary>
    /// Retrieves a specific order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order to retrieve.</param>
    /// <returns>The order if found; null if not found.</returns>
    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _context.Orders.FirstOrDefaultAsync(order => order.Id == id);
    }

    /// <summary>
    /// Updates an existing order in the database.
    /// </summary>
    /// <param name="order">The order with updated information.</param>
    /// <returns>The updated order.</returns>
    public async Task<Order> UpdateOrderAsync(UpdateOrderDto dto)
    {
        var existing = await _context.Orders.FindAsync(dto.Id);
        if (existing is null)
            throw new KeyNotFoundException($"Order {dto.Id} not found");

        existing.Status = dto.Status;
        existing.ShippingAddress = dto.ShippingAddress;
        existing.UserId = dto.UserId;

        await _context.SaveChangesAsync();
        return existing;
    }
}
