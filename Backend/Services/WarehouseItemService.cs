using Backend.Data;
using ClassLibrary.Entity.Warehouse;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;
/*
 
 */
public interface IWarehouseItemService
{
    Task<WarehouseItem?> GetByIdAsync(long id);
    Task<IEnumerable<WarehouseItem>> GetAllAsync();
    Task<WarehouseItem> CreateAsync(WarehouseItem item);
    Task<WarehouseItem> UpdateAsync(WarehouseItem item);
    Task<bool> DeleteAsync(long id);
}

/// <summary>
/// 
/// </summary>

public class WarehouseItemService : IWarehouseItemService
{
    private readonly AppDbContext _context;

    public WarehouseItemService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WarehouseItem?> GetByIdAsync(long id)
        => await _context.WarehouseItems.FindAsync(id);

    public async Task<IEnumerable<WarehouseItem>> GetAllAsync()
        => await _context.WarehouseItems.ToListAsync();

    public async Task<WarehouseItem> CreateAsync(WarehouseItem item)
    {
        item.CreatedAt = DateTime.UtcNow;
        item.ItemCode = $"ITEM-{DateTime.UtcNow:yyyyMMdd}";
        _context.WarehouseItems.Add(item);
        await _context.SaveChangesAsync();
        item.ItemCode += $"-{item.Id}";
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<WarehouseItem> UpdateAsync(WarehouseItem item)
    {
        item.ChangedAt = DateTime.UtcNow;
        _context.WarehouseItems.Update(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var item = await _context.WarehouseItems.FindAsync(id);
        if (item is null) return false;
        item.IsAvailable = false;
        await _context.SaveChangesAsync();
        return true;
    }
}