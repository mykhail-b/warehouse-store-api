using Backend.Data;
using ClassLibrary.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Customer;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(Order order);
    Task<Order?> GetOrderByIdAsync(long id);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<bool> DeleteOrderAsync(long id);
    Task<Order> UpdateOrderAsync(Order order);
}
public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<bool> DeleteOrderAsync(long id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(order => order.Id == id);
        if (order is null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(long id)
    {
        return await _context.Orders.FirstOrDefaultAsync(order => order.Id == id);
    }

    public async Task<Order> UpdateOrderAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }
}
