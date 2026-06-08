using Backend.Services.Customer;
using ClassLibrary.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Customer;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var cart = await _cartService.GetCartAsync();
        return Ok(cart);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddItem([FromBody] CartItemDto item)
    {
        await _cartService.AddItemAsync(item);
        return Ok();
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveItem([FromBody] int itemId)
    {
        await _cartService.RemoveItemAsync(itemId);
        return Ok();
    }

    [HttpPost("clear")]
    public async Task<IActionResult> ClearCart()
    {
        await _cartService.ClearCartAsync();
        return Ok();
    }
}
