using Backend.Services.Customer;
using ClassLibrary.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Customer;

/// <summary>
/// Manages customer orders and order operations.
/// Provides CRUD endpoints for managing orders including creation, retrieval, updating, and deletion.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _service;

    public OrderController(IOrderService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves all orders.
    /// </summary>
    /// <returns>A list of all orders in the system.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllOrdersAsync());
    }


    /// <summary>
    /// Retrieves a specific order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order to retrieve.</param>
    /// <returns>The order if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vendor = await _service.GetOrderByIdAsync(id);
        return vendor is null ? NotFound() : Ok(vendor);
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="order">The order to create.</param>
    /// <returns>The created order with an assigned ID.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (dto == null)
        {
            return BadRequest("Order data is missing.");
        }

        var result = await _service.CreateOrderAsync(dto);
        return Ok(result);
    }


    /// <summary>
    /// Updates an existing order.
    /// </summary>
    /// <param name="order">The order with updated information.</param>
    /// <returns>The updated order.</returns>
    [HttpPut]
    public async Task<IActionResult> Update(UpdateOrderDto dto)
    { 
        return Ok(await _service.UpdateOrderAsync(dto));
    }


    /// <summary>
    /// Deletes an order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order to delete.</param>
    /// <returns>A boolean indicating whether the deletion was successful.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    { 
        return Ok(await _service.DeleteOrderAsync(id)); 
    }

}
