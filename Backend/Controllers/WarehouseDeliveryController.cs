using Backend.Services;
using ClassLibrary.Entity.Warehouse;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseDeliveryController : ControllerBase
{
    private readonly IWarehouseDeliveryService _service;

    public WarehouseDeliveryController(IWarehouseDeliveryService service)
    {
        _service = service;
    }

    [HttpGet("inbound")]
    public async Task<IActionResult> GetAllInbound()
        => Ok(await _service.GetAllInboundAsync());

    [HttpGet("inbound/{id}")]
    public async Task<IActionResult> GetInboundById(long id)
    {
        var delivery = await _service.GetInboundByIdAsync(id);
        return delivery is null ? NotFound() : Ok(delivery);
    }

    [HttpPost("inbound")]
    public async Task<IActionResult> CreateInbound(InboundDelivery delivery)
        => Ok(await _service.CreateInboundAsync(delivery));

    [HttpGet("outbound")]
    public async Task<IActionResult> GetAllOutbound()
        => Ok(await _service.GetAllOutboundAsync());

    [HttpGet("outbound/{id}")]
    public async Task<IActionResult> GetOutboundById(long id)
    {
        var delivery = await _service.GetOutboundByIdAsync(id);
        return delivery is null ? NotFound() : Ok(delivery);
    }

    [HttpPost("outbound")]
    public async Task<IActionResult> CreateOutbound(OutboundDelivery delivery)
        => Ok(await _service.CreateOutboundAsync(delivery));
}