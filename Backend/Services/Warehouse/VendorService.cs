using Backend.Data;
using ClassLibrary.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Warehouse;

/// <summary>
/// Defines operations for managing vendor profiles, including CRUD functionality.
/// </summary>
public interface IVendorService
{
    /// <summary>
    /// Retrieves a vendor by its ID.
    /// </summary>
    /// <param name="id">The ID of the vendor to retrieve.</param>
    /// <returns>The vendor if found; null otherwise.</returns>
    Task<Vendor?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves all vendors.
    /// </summary>
    /// <returns>A collection of all vendors in the system.</returns>
    Task<IEnumerable<Vendor>> GetAllAsync();

    /// <summary>
    /// Creates a new vendor.
    /// </summary>
    /// <param name="vendor">The vendor to create.</param>
    /// <returns>The created vendor with an assigned ID.</returns>
    Task<Vendor> CreateAsync(Vendor vendor);

    /// <summary>
    /// Updates an existing vendor.
    /// </summary>
    /// <param name="vendor">The vendor with updated information.</param>
    /// <returns>The updated vendor.</returns>
    Task<Vendor> UpdateAsync(Vendor vendor);

    /// <summary>
    /// Deletes a vendor by its ID.
    /// </summary>
    /// <param name="id">The ID of the vendor to delete.</param>
    /// <returns>True if the vendor was deleted; false if not found.</returns>
    Task<bool> DeleteAsync(int id);
}

/// <summary>
/// Implementation of <see cref="IVendorService"/> for managing vendor data.
/// </summary>
public class VendorService : IVendorService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="VendorService"/> class.
    /// </summary>
    /// <param name="context">The database context for vendor operations.</param>
    public VendorService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a vendor by its ID.
    /// </summary>
    /// <param name="id">The ID of the vendor to retrieve.</param>
    /// <returns>The vendor if found; null otherwise.</returns>
    public async Task<Vendor?> GetByIdAsync(int id)
        => await _context.Vendors.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

    /// <summary>
    /// Retrieves all vendors.
    /// </summary>
    /// <returns>A collection of all vendors.</returns>
    public async Task<IEnumerable<Vendor>> GetAllAsync()
        => await _context.Vendors.AsNoTracking().ToListAsync();

    /// <summary>
    /// Creates a new vendor.
    /// </summary>
    /// <param name="vendor">The vendor to create.</param>
    /// <returns>The created vendor with an assigned ID.</returns>
    public async Task<Vendor> CreateAsync(Vendor vendor)
    {
        _context.Vendors.Add(vendor);
        await _context.SaveChangesAsync();
        return vendor;
    }

    /// <summary>
    /// Updates an existing vendor.
    /// </summary>
    /// <param name="vendor">The vendor with updated information.</param>
    /// <returns>The updated vendor.</returns>
    public async Task<Vendor> UpdateAsync(Vendor vendor)
    {
        _context.Vendors.Update(vendor);
        await _context.SaveChangesAsync();
        return vendor;
    }

    /// <summary>
    /// Deletes a vendor if it exists.
    /// </summary>
    /// <param name="id">The ID of the vendor to delete.</param>
    /// <returns>True if the vendor was found and deleted; false otherwise.</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var vendor = await _context.Vendors.FindAsync(id);
        if (vendor is null) return false;

        _context.Vendors.Remove(vendor);
        await _context.SaveChangesAsync();
        return true;
    }
}