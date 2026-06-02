using Backend.Data;
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
    Task<Order> CreateOrderAsync(Order order);

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
    Task<Order> UpdateOrderAsync(Order order);
}

/// <summary>
/// Implementation of <see cref="IOrderService"/> for managing customer orders using Entity Framework Core.
/// </summary>
public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderService"/> class.
    /// </summary>
    /// <param name="context">The database context for order operations.</param>
    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new order in the database.
    /// </summary>
    /// <param name="order">The order to create.</param>
    /// <returns>The created order with an assigned ID.</returns>
    public async Task<Order> CreateOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return order;
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
    public async Task<Order> UpdateOrderAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }
}
