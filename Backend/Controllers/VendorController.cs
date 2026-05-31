using Backend.Services.Warehouse;
using ClassLibrary.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VendorController : ControllerBase
{
    private readonly IVendorService _service;

    public VendorController(IVendorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }
    

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var vendor = await _service.GetByIdAsync(id);
        return vendor is null ? NotFound() : Ok(vendor);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Vendor vendor)
        => Ok(await _service.CreateAsync(vendor));

    [HttpPut]
    public async Task<IActionResult> Update(Vendor vendor)
        => Ok(await _service.UpdateAsync(vendor));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
        => Ok(await _service.DeleteAsync(id));
}