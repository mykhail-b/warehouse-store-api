using Backend.Services.Warehouse;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Warehouse;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _service;

    public OrderController(IOrderService service)
    {
        _service = service;
    }

    // Get all orders (for admin panel)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _service.GetAllOrdersAsync();
        return Ok(orders);
    }

    // Get a specific order by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {        var order = await _service.GetOrderByIdAsync(id);
        return order is null ? NotFound(new { message = $"Order with ID {id} not found." }) : Ok(order);
    }

    // Delete order
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var isDeleted = await _service.DeleteOrderAsync(id);
        if (!isDeleted)
        {
            return NotFound(new { message = $"Order with ID {id} not found for deletion." });
        }

        return Ok(new { message = "Order successfully deleted." });
    }
}