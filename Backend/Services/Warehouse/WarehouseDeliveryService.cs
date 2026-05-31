using Backend.Data;
using ClassLibrary.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Warehouse;

/*
 
 */

public interface IWarehouseDeliveryService
{
    // Inbound
    Task<InboundDelivery> CreateInboundAsync(InboundDelivery delivery);
    Task<InboundDelivery?> GetInboundByIdAsync(long id);
    Task<IEnumerable<InboundDelivery>> GetAllInboundAsync();

    // Outbound
    Task<OutboundDelivery> CreateOutboundAsync(OutboundDelivery delivery);
    Task<OutboundDelivery?> GetOutboundByIdAsync(long id);
    Task<IEnumerable<OutboundDelivery>> GetAllOutboundAsync();
}

/// <summary>
/// 
/// </summary>
public class WarehouseDeliveryService : IWarehouseDeliveryService
{
    private readonly AppDbContext _context;

    public WarehouseDeliveryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<InboundDelivery> CreateInboundAsync(InboundDelivery delivery)
    {
        _context.InboundDeliveries.Add(delivery);
        foreach (var item in delivery.Items)
        {
            var warehouseItem = await _context.WarehouseItems.FindAsync(item.WarehouseItemId);
            if (warehouseItem is not null)
                warehouseItem.CurrentQuantity += item.QuantityReceived;
        }
        await _context.SaveChangesAsync();
        return delivery;
    }

    public async Task<InboundDelivery?> GetInboundByIdAsync(long id)
    {
        return await _context.InboundDeliveries
            .Include(d => d.Items)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
    

    public async Task<IEnumerable<InboundDelivery>> GetAllInboundAsync()
    {
        return await _context.InboundDeliveries
            .Include(d => d.Items)
            .ToListAsync();
    }
    

    public async Task<OutboundDelivery> CreateOutboundAsync(OutboundDelivery delivery)
    {
        _context.OutboundDeliveries.Add(delivery);
        foreach (var item in delivery.Items)
        {
            var warehouseItem = await _context.WarehouseItems.FindAsync(item.WarehouseItemId);
            if (warehouseItem is not null)
                warehouseItem.CurrentQuantity -= item.QuantityShipped;
        }
        await _context.SaveChangesAsync();
        return delivery;
    }

    public async Task<OutboundDelivery?> GetOutboundByIdAsync(long id)
    { 
       return await _context.OutboundDeliveries
            .Include(d => d.Items)
            .FirstOrDefaultAsync(d => d.Id == id);
    }


    public async Task<IEnumerable<OutboundDelivery>> GetAllOutboundAsync()
    {
        return await _context.OutboundDeliveries
            .Include(d => d.Items)
            .ToListAsync();
    }
    
}
