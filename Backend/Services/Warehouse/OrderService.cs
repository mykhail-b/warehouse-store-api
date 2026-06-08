using Backend.Data;
using Backend.Services.Infrastructure;
using ClassLibrary;
using ClassLibrary.Dto;
using ClassLibrary.Entity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Backend.Services.Warehouse;


public interface IOrderService
{
    Task<Order> CreateOrderAsync(OrderCreateDto dto);

    Task<Order?> GetOrderByIdAsync(int id);

    Task<IEnumerable<Order>> GetAllOrdersAsync();

    Task<bool> DeleteOrderAsync(int id);
}

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly IMailService _mailService;

    private static string GenerateShippingNumber()
        => $"SHIP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

    public OrderService(AppDbContext context, IMailService mailService)
    {
        _context = context;
        _mailService = mailService;
    }

    public async Task<Order> CreateOrderAsync(OrderCreateDto dto)
    {
        // Deserialize the string with products back into a list of objects
        var items = JsonSerializer.Deserialize<List<OrderItemDto>>(dto.SerializedItems);

        if (items == null || !items.Any())
        {
            throw new InvalidOperationException("Order must contain at least one item.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Check the warehouse and reduce the quantity of certain products
            foreach (var item in items)
            {
                var warehouseItem = await _context.Products.FindAsync(item.ProductId);

                if (warehouseItem is null || warehouseItem.CurrentQuantity < item.Quantity)
                {
                    throw new InvalidOperationException($"Not enough goods in stock! ID: {item.ProductId}");
                }

                warehouseItem.CurrentQuantity -= item.Quantity;
            }

            // Create an order entity object
            var newOrder = new Order
            {
                UserId = dto.UserId,
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                ShippingAddress = dto.ShippingAddress,
                ShippingNumber = GenerateShippingNumber(),
                StripeSessionId = dto.StripeSessionId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,

                Items = items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            // Save order to the database
            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            // Commit the transaction
            await transaction.CommitAsync();

            await _mailService.SendOrderConfirmationEmailAsync(new OrderConfirmationMail
            {
                To = newOrder.CustomerEmail,
                RecipientName = newOrder.CustomerName,
                OrderId = newOrder.Id,
                ShippingAddress = newOrder.ShippingAddress,
                Items = newOrder.Items.Select(i => new OrderMailItem
                {
                    Name = $"Product #{i.ProductId}",
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            });

            return newOrder;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(order => order.Id == id);
        if (order is null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders.Include(o => o.Items).ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(order => order.Id == id);
    }
}