using Backend.Data;
using ClassLibrary.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Warehouse;

/// <summary>
/// Provides services for managing warehouse logistics, including inbound (receiving) 
/// and outbound (shipping) delivery operations.
/// </summary>
public interface IWarehouseDeliveryService
{
    // --- Inbound Operations ---
    Task<InboundDelivery> CreateInboundAsync(InboundDelivery delivery);
    Task<InboundDelivery?> GetInboundByIdAsync(int id);
    Task<IEnumerable<InboundDelivery>> GetAllInboundAsync();

    // --- Outbound Operations ---
    Task<OutboundDelivery> CreateOutboundAsync(OutboundDelivery delivery);
    Task<OutboundDelivery?> GetOutboundByIdAsync(int id);
    Task<IEnumerable<OutboundDelivery>> GetAllOutboundAsync();
}

/// <summary>
/// Implementation of <see cref="IWarehouseDeliveryService"/> handling delivery processing 
/// and automatic stock updates for both inbound and outbound operations.
/// </summary>
public class WarehouseDeliveryService : IWarehouseDeliveryService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="WarehouseDeliveryService"/> class.
    /// </summary>
    /// <param name="context">The database context for delivery operations.</param>
    public WarehouseDeliveryService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new inbound delivery and automatically updates warehouse stock levels.
    /// </summary>
    /// <param name="delivery">The inbound delivery containing items and quantities received.</param>
    /// <returns>The created inbound delivery record.</returns>
    /// <remarks>Automatically increments warehouse item quantities based on received amounts.</remarks>
    public async Task<InboundDelivery> CreateInboundAsync(InboundDelivery delivery)
    {
        _context.InboundDeliveries.Add(delivery);

        // Update stock levels automatically
        foreach (var item in delivery.Items)
        {
            var warehouseItem = await _context.WarehouseItems.FindAsync(item.WarehouseItemId);
            if (warehouseItem is not null)
                warehouseItem.CurrentQuantity += item.QuantityReceived;
        }

        await _context.SaveChangesAsync();
        return delivery;
    }

    /// <summary>
    /// Retrieves a specific inbound delivery by its ID with related items.
    /// </summary>
    /// <param name="id">The ID of the inbound delivery to retrieve.</param>
    /// <returns>The inbound delivery if found; null otherwise.</returns>
    public async Task<InboundDelivery?> GetInboundByIdAsync(int id)
    => await _context.InboundDeliveries
        .AsNoTracking()
        .Include(d => d.Items)
        .FirstOrDefaultAsync(d => d.Id == id);

    /// <summary>
    /// Retrieves all inbound deliveries with their related items.
    /// </summary>
    /// <returns>A collection of all inbound deliveries.</returns>
    public async Task<IEnumerable<InboundDelivery>> GetAllInboundAsync()
        => await _context.InboundDeliveries
            .AsNoTracking()
            .Include(d => d.Items)
            .ToListAsync();

    /// <summary>
    /// Creates a new outbound delivery and automatically updates warehouse stock levels.
    /// </summary>
    /// <param name="delivery">The outbound delivery containing items and quantities to ship.</param>
    /// <returns>The created outbound delivery record.</returns>
    /// <remarks>Automatically decrements warehouse item quantities based on shipped amounts.</remarks>
    public async Task<OutboundDelivery> CreateOutboundAsync(OutboundDelivery delivery)
    {
        _context.OutboundDeliveries.Add(delivery);

        // Update stock levels automatically
        foreach (var item in delivery.Items)
        {
            var warehouseItem = await _context.WarehouseItems.FindAsync(item.WarehouseItemId);
            if (warehouseItem is not null)
                warehouseItem.CurrentQuantity -= item.QuantityShipped;
        }

        await _context.SaveChangesAsync();
        return delivery;
    }

    /// <summary>
    /// Retrieves a specific outbound delivery by its ID with related items.
    /// </summary>
    /// <param name="id">The ID of the outbound delivery to retrieve.</param>
    /// <returns>The outbound delivery if found; null otherwise.</returns>
    public async Task<OutboundDelivery?> GetOutboundByIdAsync(int id)
        => await _context.OutboundDeliveries
            .AsNoTracking()
            .Include(d => d.Items)
            .FirstOrDefaultAsync(d => d.Id == id);

    /// <summary>
    /// Retrieves all outbound deliveries with their related items.
    /// </summary>
    /// <returns>A collection of all outbound deliveries.</returns>
    public async Task<IEnumerable<OutboundDelivery>> GetAllOutboundAsync()
        => await _context.OutboundDeliveries
            .AsNoTracking()
            .Include(d => d.Items)
            .ToListAsync();
}