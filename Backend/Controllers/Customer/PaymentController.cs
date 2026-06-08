using Backend.Services.Customer;
using ClassLibrary.Dto;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Backend.Controllers.Customer;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("create-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CartCheckoutDto dto)
    {
        if (dto == null || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Call the service and get the URL of the Stripe page
            string checkoutUrl = await _paymentService.CreateCheckoutSessionAsync(dto);

            // Returning a link to the frontend
            return Ok(new { url = checkoutUrl });
        }
        catch (StripeException e)
        {
            // If Stripe's servers are down or your API key is invalid
            return BadRequest(new { error = e.Message });
        }
    }
}