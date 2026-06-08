using Backend.Services.Warehouse;
using ClassLibrary.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Warehouse;


[Route("api/[controller]")]
[ApiController]
public class DeliveryController : ControllerBase
{
    private readonly IDeliveryService _service;

    public DeliveryController(IDeliveryService service)
    {
        _service = service;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeliveryDto>>> GetAll()
    {
        var deliveries = await _service.GetAllDeliveriesAsync();
        return Ok(deliveries);
    }



    [HttpGet("{id}")]
    public async Task<ActionResult<DeliveryDto>> GetById(int id)
    {
        var delivery = await _service.GetDeliveriesByIdAsync(id);

        if (delivery is null)
        {
            return NotFound(new { message = $"Delivery option with ID {id} not found." });
        }

        return Ok(delivery);
    }
}