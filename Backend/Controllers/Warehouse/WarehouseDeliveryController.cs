using Backend.Services.Warehouse;
using ClassLibrary.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Admin;

/// <summary>
/// Manages warehouse deliveries including inbound and outbound operations.
/// Provides endpoints for tracking incoming and outgoing inventory movements.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class WarehouseDeliveryController : ControllerBase
{
    private readonly IWarehouseDeliveryService _service;

    public WarehouseDeliveryController(IWarehouseDeliveryService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves all outbound deliveries.
    /// </summary>
    /// <returns>A list of all outbound deliveries in the system.</returns>
    [HttpGet("outbound")]
    public async Task<ActionResult<IEnumerable<DeliveryDto>>> GetAllOutbound()
    {
        return Ok(await _service.GetAllOutboundAsync());
    }


    /// <summary>
    /// Retrieves a specific outbound delivery by its ID.
    /// </summary>
    /// <param name="id">The ID of the outbound delivery to retrieve.</param>
    /// <returns>The outbound delivery if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet("outbound/{id}")]
    public async Task<ActionResult<DeliveryDto>> GetOutboundById(int id)
    {
        var delivery = await _service.GetOutboundByIdAsync(id);
        return delivery is null ? NotFound() : Ok(delivery);
    }
}