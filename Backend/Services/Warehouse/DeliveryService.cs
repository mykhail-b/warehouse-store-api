using Backend.Data;
using ClassLibrary.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Warehouse;

public interface IDeliveryService
{
    Task<Delivery> CreateDeliveryAsync(int newOrderId);
    Task<Delivery?> GetDeliveriesByIdAsync(int id);
    Task<IEnumerable<Delivery>> GetAllDeliveriesAsync();
}

public class DeliveryService : IDeliveryService
{
    private readonly AppDbContext _context;

    public DeliveryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Delivery> CreateDeliveryAsync(int newOrderId)
    {
        var order = await _context.Orders.FindAsync(newOrderId);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {newOrderId} not found.");

        var delivery = new Delivery
        {
            OrderId = newOrderId,
            ShippedAt = DateTime.UtcNow
        };

        _context.Deliveries.Add(delivery);
        await _context.SaveChangesAsync();

        return delivery;
    }

    public async Task<Delivery?> GetDeliveriesByIdAsync(int id)
        => await _context.Deliveries.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);

    public async Task<IEnumerable<Delivery>> GetAllDeliveriesAsync()
        => await _context.Deliveries.AsNoTracking().ToListAsync();
}