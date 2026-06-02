using Backend.Services.Warehouse;
using ClassLibrary.Entity;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="WarehouseDeliveryController"/> class.
    /// </summary>
    /// <param name="service">The warehouse delivery service for handling delivery operations.</param>
    public WarehouseDeliveryController(IWarehouseDeliveryService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves all inbound deliveries.
    /// </summary>
    /// <returns>A list of all inbound deliveries in the system.</returns>
    [HttpGet("inbound")]
    public async Task<IActionResult> GetAllInbound()
    {
        return Ok(await _service.GetAllInboundAsync());
    }


    /// <summary>
    /// Retrieves a specific inbound delivery by its ID.
    /// </summary>
    /// <param name="id">The ID of the inbound delivery to retrieve.</param>
    /// <returns>The inbound delivery if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet("inbound/{id}")]
    public async Task<IActionResult> GetInboundById(int id)
    {
        var delivery = await _service.GetInboundByIdAsync(id);
        return delivery is null ? NotFound() : Ok(delivery);
    }

    /// <summary>
    /// Creates a new inbound delivery and updates warehouse inventory.
    /// </summary>
    /// <param name="delivery">The inbound delivery details including items and quantities.</param>
    /// <returns>The created inbound delivery record.</returns>
    [HttpPost("inbound")]
    public async Task<IActionResult> CreateInbound(InboundDelivery delivery)
    {
        return Ok(await _service.CreateInboundAsync(delivery));
    }


    /// <summary>
    /// Retrieves all outbound deliveries.
    /// </summary>
    /// <returns>A list of all outbound deliveries in the system.</returns>
    [HttpGet("outbound")]
    public async Task<IActionResult> GetAllOutbound()
    {
        return Ok(await _service.GetAllOutboundAsync());
    }


    /// <summary>
    /// Retrieves a specific outbound delivery by its ID.
    /// </summary>
    /// <param name="id">The ID of the outbound delivery to retrieve.</param>
    /// <returns>The outbound delivery if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet("outbound/{id}")]
    public async Task<IActionResult> GetOutboundById(int id)
    {
        var delivery = await _service.GetOutboundByIdAsync(id);
        return delivery is null ? NotFound() : Ok(delivery);
    }

    /// <summary>
    /// Creates a new outbound delivery and updates warehouse inventory.
    /// </summary>
    /// <param name="delivery">The outbound delivery details including items and quantities.</param>
    /// <returns>The created outbound delivery record.</returns>
    [HttpPost("outbound")]
    public async Task<IActionResult> CreateOutbound(OutboundDelivery delivery)
    {
        return Ok(await _service.CreateOutboundAsync(delivery));
    }

}