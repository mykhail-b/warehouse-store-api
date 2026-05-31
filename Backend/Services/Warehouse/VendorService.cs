using Backend.Data;
using ClassLibrary.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Warehouse;

/**/

public interface IVendorService
{
    Task<Vendor?> GetByIdAsync(long id);
    Task<IEnumerable<Vendor>> GetAllAsync();
    Task<Vendor> CreateAsync(Vendor vendor);
    Task<Vendor> UpdateAsync(Vendor vendor);
    Task<bool> DeleteAsync(long id);
}
/// <summary>
/// 
/// </summary>
public class VendorService : IVendorService
{
    private readonly AppDbContext _context;

    public VendorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Vendor?> GetByIdAsync(long id)
        => await _context.Vendors.FindAsync(id);

    public async Task<IEnumerable<Vendor>> GetAllAsync()
        => await _context.Vendors.ToListAsync();

    public async Task<Vendor> CreateAsync(Vendor vendor)
    {
        _context.Vendors.Add(vendor);
        await _context.SaveChangesAsync();
        return vendor;
    }

    public async Task<Vendor> UpdateAsync(Vendor vendor)
    {
        _context.Vendors.Update(vendor);
        await _context.SaveChangesAsync();
        return vendor;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var vendor = await _context.Vendors.FindAsync(id);
        if (vendor is null) return false;
        _context.Vendors.Remove(vendor);
        await _context.SaveChangesAsync();
        return true;
    }
}
