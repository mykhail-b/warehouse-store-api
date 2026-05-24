using Backend.Services;
using ClassLibrary.Entity.Warehouse;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseItemController : ControllerBase
{
    private readonly IWarehouseItemService _service;

    public WarehouseItemController(IWarehouseItemService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(WarehouseItem item)
        => Ok(await _service.CreateAsync(item));

    [HttpPut]
    public async Task<IActionResult> Update(WarehouseItem item)
        => Ok(await _service.UpdateAsync(item));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
        => Ok(await _service.DeleteAsync(id));
}