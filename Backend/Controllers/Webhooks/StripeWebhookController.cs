using Backend.Services.Warehouse;
using ClassLibrary.Dto;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Backend.Controllers.Webhooks;

[Route("api/webhook")]
[ApiController]
public class StripeWebhookController : ControllerBase
{
    private readonly IOrderService _orderService;

    // Webhook Secret (for local testing via Stripe CLI)
    private const string WebhookSecret = "whsec_your_stripe_webhook_secret";

    public StripeWebhookController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            // Verify Stripe signature for security
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                WebhookSecret
            );

            // If the payment session is successfully completed on the Stripe side
            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                // We get the flat metadata dictionary that we packed into PaymentService
                var metadata = session.Metadata;

                var orderCreateDto = new OrderCreateDto
                {
                    // If UserId is empty (guest), null will be written
                    UserId = string.IsNullOrEmpty(metadata["UserId"]) ? null : metadata["UserId"],
                    CustomerName = metadata["CustomerName"],
                    CustomerEmail = metadata["CustomerEmail"],
                    ShippingAddress = metadata["ShippingAddress"],
                    TotalPrice = decimal.Parse(metadata["TotalPrice"]),

                    // We extract a JSON string with order products
                    SerializedItems = metadata["SerializedItems"],

                    // Save the Stripe session ID (very useful for logs and history in the database)
                    StripeSessionId = session.Id
                };

                // We transfer the completed DTO to the order service
                await _orderService.CreateOrderAsync(orderCreateDto);
            }

            // 200 OK for Stripe that the request has been delivered
            return Ok();
        }
        catch (StripeException)
        {
            // If the signature does not match, we return an error.
            return BadRequest();
        }
    }
}