using Backend.Services.Warehouse;
using ClassLibrary.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Admin;

/// <summary>
/// Manages warehouse inventory items and their operations.
/// Provides CRUD endpoints for managing warehouse items including creation, retrieval, updating, and deletion.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class WarehouseItemController : ControllerBase
{
    private readonly IWarehouseItemService _service;

    public WarehouseItemController(IWarehouseItemService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves all warehouse items.
    /// </summary>
    /// <returns>A list of all warehouse items in the system.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    { 
         return  Ok(await _service.GetAllAsync());
    }
   

    /// <summary>
    /// Retrieves a specific warehouse item by its ID.
    /// </summary>
    /// <param name="id">The ID of the warehouse item to retrieve.</param>
    /// <returns>The warehouse item if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Creates a new warehouse item.
    /// </summary>
    /// <param name="item">The warehouse item to create.</param>
    /// <returns>The created warehouse item with an assigned ID.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(WarehouseItem item)
    { 
         return  Ok(await _service.CreateAsync(item));
    }


    /// <summary>
    /// Updates an existing warehouse item.
    /// </summary>
    /// <param name="item">The warehouse item with updated information.</param>
    /// <returns>The updated warehouse item.</returns>
    [HttpPut]
    public async Task<IActionResult> Update(WarehouseItem item)
    { 
        return  Ok(await _service.UpdateAsync(item));
    }


    /// <summary>
    /// Deletes a warehouse item by marking it as unavailable.
    /// </summary>
    /// <param name="id">The ID of the warehouse item to delete.</param>
    /// <returns>A boolean indicating whether the deletion was successful.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    { 
        return  Ok(await _service.DeleteItemAsync(id));
    }
    
}