using Backend.Data;
using ClassLibrary.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Warehouse;

/// <summary>
/// Provides services for managing simple warehouse logistics
/// and outbound (shipping) delivery operations.
/// </summary>
public interface IWarehouseDeliveryService
{
    Task<OutboundDelivery> CreateOutboundAsync(int newOrderId, IEnumerable<OrderItem> items);
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

    public WarehouseDeliveryService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new outbound delivery and automatically updates warehouse stock levels.
    /// </summary>
    /// <param name="delivery">The outbound delivery containing items and quantities to ship.</param>
    /// <returns>The created outbound delivery record.</returns>
    /// <remarks>Automatically decrements warehouse item quantities based on shipped amounts.</remarks>
    public async Task<OutboundDelivery> CreateOutboundAsync(int newOrderId, IEnumerable<OrderItem> orderItems)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == newOrderId);

        if (order == null)
            throw new KeyNotFoundException($"Order with ID {newOrderId} not found.");

        string recipientName = order.User != null
            ? $"{order.User.FirstName} {order.User.LastName}"
            : "Guest Customer";

        var delivery = new OutboundDelivery
        {
            OrderId = newOrderId,
            DepartureDate = DateTime.UtcNow,
            ShippingNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
            DestinationAddress = order.ShippingAddress,
            RecipientName = recipientName,
            Items = orderItems.Select(oi => new OutboundDeliveryItem
            {
                WarehouseItemId = oi.WarehouseItemId,
                QuantityShipped = oi.Quantity
            }).ToList()
        };

        var itemsToValidate = new List<(WarehouseItem Item, int Quantity)>();

        foreach (var deliveryItem in delivery.Items)
        {
            var warehouseItem = await _context.WarehouseItems.FindAsync(deliveryItem.WarehouseItemId);
            if (warehouseItem == null)
                throw new KeyNotFoundException($"Warehouse item {deliveryItem.WarehouseItemId} not found.");

            if (warehouseItem.CurrentQuantity < deliveryItem.QuantityShipped)
                return null!;

            itemsToValidate.Add((warehouseItem, deliveryItem.QuantityShipped));
        }

        foreach (var pair in itemsToValidate)
        {
            pair.Item.CurrentQuantity -= pair.Quantity;
            if (pair.Item.CurrentQuantity == 0)
                pair.Item.IsAvailable = false;
        }

        _context.OutboundDeliveries.Add(delivery);
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