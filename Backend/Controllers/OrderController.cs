using Backend.Services.Customer;
using ClassLibrary.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _service;

    public OrderController(IOrderService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllOrdersAsync());
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var vendor = await _service.GetOrderByIdAsync(id);
        return vendor is null ? NotFound() : Ok(vendor);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
        return Ok(await _service.CreateOrderAsync(order));
    }


    [HttpPut]
    public async Task<IActionResult> Update(Order order)
    { 
        return Ok(await _service.UpdateOrderAsync(order));
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    { 
        return Ok(await _service.DeleteOrderAsync(id)); 
    }

}
