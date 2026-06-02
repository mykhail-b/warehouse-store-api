using Backend.Services.Warehouse;
using ClassLibrary.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Admin;

/// <summary>
/// Manages vendor information and operations.
/// Provides CRUD endpoints for managing vendors including creation, retrieval, updating, and deletion.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class VendorController : ControllerBase
{
    private readonly IVendorService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="VendorController"/> class.
    /// </summary>
    /// <param name="service">The vendor service for handling vendor operations.</param>
    public VendorController(IVendorService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves all vendors.
    /// </summary>
    /// <returns>A list of all vendors in the system.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }


    /// <summary>
    /// Retrieves a specific vendor by its ID.
    /// </summary>
    /// <param name="id">The ID of the vendor to retrieve.</param>
    /// <returns>The vendor if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vendor = await _service.GetByIdAsync(id);
        return vendor is null ? NotFound() : Ok(vendor);
    }

    /// <summary>
    /// Creates a new vendor.
    /// </summary>
    /// <param name="vendor">The vendor to create.</param>
    /// <returns>The created vendor with an assigned ID.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(Vendor vendor)
    {
        return Ok(await _service.CreateAsync(vendor));
    }


    /// <summary>
    /// Updates an existing vendor.
    /// </summary>
    /// <param name="vendor">The vendor with updated information.</param>
    /// <returns>The updated vendor.</returns>
    [HttpPut]
    public async Task<IActionResult> Update(Vendor vendor)
    {
        return Ok(await _service.UpdateAsync(vendor));
    }


    /// <summary>
    /// Deletes a vendor by its ID.
    /// </summary>
    /// <param name="id">The ID of the vendor to delete.</param>
    /// <returns>A boolean indicating whether the deletion was successful.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        return Ok(await _service.DeleteAsync(id));
    }

}