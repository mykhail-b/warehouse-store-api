using Backend.Data;
using ClassLibrary.Dto;
using ClassLibrary.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Warehouse;

/// <summary>
/// Defines operations for managing warehouse items, including administrative 
/// management and public-facing catalog retrieval.
/// </summary>
public interface IWarehouseItemService
{
    // --- Admin/Employee Operations ---
    Task<WarehouseItem?> GetByIdAsync(int id);
    Task<IEnumerable<WarehouseItem>> GetAllAsync();
    Task<WarehouseItem> CreateAsync(WarehouseItem item);
    Task<WarehouseItem> UpdateAsync(WarehouseItem item);
    Task<bool> DeleteItemAsync(int id);

    /// <summary>
    /// Retrieves a paginated list of items for the public product catalog.
    /// </summary>
    Task<IEnumerable<WarehouseItemSummaryDto>> GetSummaryListAsync(int page, int pageSize);

    /// <summary>
    /// Retrieves full details for a specific item for public viewing.
    /// </summary>
    Task<WarehouseItemDetailDto?> GetDetailByIdAsync(int id);
}

/// <summary>
/// Provides implementation for warehouse item management using Entity Framework Core.
/// Includes both administrative item management and public-facing catalog operations.
/// </summary>
public class WarehouseItemService : IWarehouseItemService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="WarehouseItemService"/> class.
    /// </summary>
    /// <param name="context">The database context for warehouse item operations.</param>
    public WarehouseItemService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a warehouse item by its ID.
    /// </summary>
    /// <param name="id">The ID of the warehouse item to retrieve.</param>
    /// <returns>The warehouse item if found; null otherwise.</returns>
    public async Task<WarehouseItem?> GetByIdAsync(int id)
        => await _context.WarehouseItems.FindAsync(id);

    /// <summary>
    /// Retrieves all warehouse items.
    /// </summary>
    /// <returns>A collection of all warehouse items.</returns>
    public async Task<IEnumerable<WarehouseItem>> GetAllAsync()
        => await _context.WarehouseItems.ToListAsync();

    /// <summary>
    /// Creates a new warehouse item with an auto-generated item code.
    /// </summary>
    /// <param name="item">The warehouse item to create.</param>
    /// <returns>The created item with assigned ID and item code.</returns>
    /// <remarks>The item code is generated in format: ITEM-yyyyMMdd-{id}</remarks>
    public async Task<WarehouseItem> CreateAsync(WarehouseItem item)
    {
        item.CreatedAt = DateTime.UtcNow;
        // Temporary code to allow ID generation
        item.ItemCode = $"ITEM-{DateTime.UtcNow:yyyyMMdd}";

        _context.WarehouseItems.Add(item);
        await _context.SaveChangesAsync();

        // Update code with unique ID
        item.ItemCode += $"-{item.Id}";
        await _context.SaveChangesAsync();

        return item;
    }

    /// <summary>
    /// Updates an existing warehouse item and records the change timestamp.
    /// </summary>
    /// <param name="item">The warehouse item with updated information.</param>
    /// <returns>The updated warehouse item.</returns>
    public async Task<WarehouseItem> UpdateAsync(WarehouseItem item)
    {
        item.ChangedAt = DateTime.UtcNow;
        _context.WarehouseItems.Update(item);
        await _context.SaveChangesAsync();
        return item;
    }

    /// <summary>
    /// Performs a soft delete by marking the item as unavailable.
    /// </summary>
    /// <param name="id">The ID of the warehouse item to delete.</param>
    /// <returns>True if the item was found and marked as unavailable; false otherwise.</returns>
    /// <remarks>This implements soft delete - the item is not physically removed from the database.</remarks>
    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await _context.WarehouseItems.FindAsync(id);
        if (item is null) return false;

        // Soft delete implementation
        item.IsAvailable = false;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Retrieves a paginated list of items for the public product catalog.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated list of warehouse item summaries.</returns>
    public async Task<IEnumerable<WarehouseItemSummaryDto>> GetSummaryListAsync(int page, int pageSize)
    {
        return await _context.WarehouseItems
            .AsNoTracking()
            .OrderBy(i => i.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => new WarehouseItemSummaryDto
            {
                Id = i.Id,
                Name = i.Name,
            })
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves full details for a specific item for public viewing.
    /// </summary>
    /// <param name="id">The ID of the warehouse item to retrieve.</param>
    /// <returns>The item details if found; null otherwise.</returns>
    public async Task<WarehouseItemDetailDto?> GetDetailByIdAsync(int id)
    {
        return await _context.WarehouseItems
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new WarehouseItemDetailDto
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                IsAvailable = i.IsAvailable,
                Cost = i.Cost,
                Currency = i.Currency
            })
            .FirstOrDefaultAsync();
    }
}