using Backend.Data;
using ClassLibrary.Dto;
using ClassLibrary.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

/// <summary>
/// Defines operations for managing company data with different access levels.
/// </summary>
public interface ICompanyDataService
{
    /// <summary>
    /// Retrieves detailed company information including sensitive data.
    /// </summary>
    /// <returns>A collection of detailed company data records.</returns>
    Task<IEnumerable<ComapnyDataDetailDto>> GetCompanyData();

    /// <summary>
    /// Creates or updates company data based on registry number.
    /// </summary>
    /// <param name="dto">The company data to save or update.</param>
    /// <returns>The saved company data.</returns>
    Task<ComapnyDataDetailDto> SetCompanyData(ComapnyDataDetailDto dto);

    /// <summary>
    /// Retrieves publicly available company information.
    /// </summary>
    /// <returns>A collection of public company data excluding sensitive information.</returns>
    Task<IEnumerable<ComapnyDataPublicDto>> GetPublicCompanyData();
}

/// <summary>
/// Implementation of <see cref="ICompanyDataService"/> for managing company data.
/// Provides both public and detailed access to company information with different data exposure levels.
/// </summary>
public class CompanyDataService : ICompanyDataService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyDataService"/> class.
    /// </summary>
    /// <param name="context">The database context for company data operations.</param>
    public CompanyDataService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves detailed company information including registry number and all contact details.
    /// </summary>
    /// <returns>A collection of detailed company data records.</returns>
    public async Task<IEnumerable<ComapnyDataDetailDto>> GetCompanyData()
    {
        return await _context.CompanyData
            .AsNoTracking()
            .Select(c => new ComapnyDataDetailDto
            {
                Name = c.Name,
                Country = c.Country,
                Address = c.Address,
                RegistryNumber = c.RegistryNumber,
                Email = c.Email,
                Phone = c.Phone
            })
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves publicly available company information without sensitive details.
    /// </summary>
    /// <returns>A collection of public company data excluding registry number and private contact info.</returns>
    public async Task<IEnumerable<ComapnyDataPublicDto>> GetPublicCompanyData()
    {
        return await _context.CompanyData
            .AsNoTracking()
            .Select(c => new ComapnyDataPublicDto
            {
                Name = c.Name,
                Address = c.Address,
                Email = c.Email,
                Phone = c.Phone
            })
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new company record or updates an existing one based on registry number.
    /// </summary>
    /// <param name="dto">The company data to save or update.</param>
    /// <returns>The saved company data.</returns>
    /// <remarks>If a company with the same registry number exists, it is updated. Otherwise, a new record is created.</remarks>
    public async Task<ComapnyDataDetailDto> SetCompanyData(ComapnyDataDetailDto dto)
    {
        var existingCompany = await _context.CompanyData
            .FirstOrDefaultAsync(c => c.RegistryNumber == dto.RegistryNumber);

        if (existingCompany == null)
        {
            var newCompany = new CompanyData
            {
                Name = dto.Name,
                Country = dto.Country,
                Address = dto.Address,
                RegistryNumber = dto.RegistryNumber,
                Email = dto.Email,
                Phone = dto.Phone
            };

            _context.CompanyData.Add(newCompany);
            await _context.SaveChangesAsync();

            return dto;
        }
        else
        {
            existingCompany.Name = dto.Name;
            existingCompany.Country = dto.Country;
            existingCompany.Address = dto.Address;
            existingCompany.Email = dto.Email;
            existingCompany.Phone = dto.Phone;

            await _context.SaveChangesAsync();

            return dto;
        }
    }
}
