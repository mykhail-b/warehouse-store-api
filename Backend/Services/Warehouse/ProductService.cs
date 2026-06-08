using Backend.Data;
using ClassLibrary.Dto;
using ClassLibrary.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Warehouse;


public interface IProductService
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> CreateAsync(Product item);
    Task<Product> UpdateAsync(Product item);
    Task<bool> DeleteItemAsync(int id);

    Task<IEnumerable<ProductSummaryDto>> GetSummaryListAsync(int page, int pageSize);
    Task<ProductDetailDto?> GetDetailByIdAsync(int id);
}

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id)
        => await _context.Products.FindAsync(id);


    public async Task<IEnumerable<Product>> GetAllAsync()
        => await _context.Products.ToListAsync();


    public async Task<Product> CreateAsync(Product item)
    {
        item.CreatedAt = DateTime.UtcNow;
        // Temporary code to allow ID generation
        item.ItemCode = $"ITEM-{DateTime.UtcNow:yyyyMMdd}";

        _context.Products.Add(item);
        await _context.SaveChangesAsync();

        // Update code with unique ID
        item.ItemCode += $"-{item.Id}";
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<Product> UpdateAsync(Product item)
    {
        item.ChangedAt = DateTime.UtcNow;
        _context.Products.Update(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await _context.Products.FindAsync(id);
        if (item is null) return false;

        // Soft delete implementation
        item.IsAvailable = false;
        await _context.SaveChangesAsync();
        return true;
    }


    public async Task<IEnumerable<ProductSummaryDto>> GetSummaryListAsync(int page, int pageSize)
    {
        return await _context.Products
            .AsNoTracking()
            .OrderBy(i => i.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => new ProductSummaryDto
            {
                Id = i.Id,
                Name = i.Name,
            })
            .ToListAsync();
    }


    public async Task<ProductDetailDto?> GetDetailByIdAsync(int id)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new ProductDetailDto
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