using ClassLibrary.Dto;
using Stripe.Checkout;
using System.Text.Json;

namespace Backend.Services.Customer;

public interface IPaymentService
{
    Task<string> CreateCheckoutSessionAsync(CartCheckoutDto order);
}

public class PaymentService : IPaymentService
{
    public async Task<string> CreateCheckoutSessionAsync(CartCheckoutDto cart)
    {
        // Calculate the total order amount based on the list of products
        decimal totalPrice = cart.Items.Sum(item => item.Price * item.Quantity);

        // Converting a list of products into a regular string (JSON), 
        string serializedItems = JsonSerializer.Serialize(cart.Items);

        // Configuring session parameters Stripe
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },

            CustomerEmail = cart.CustomerEmail,

            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(totalPrice * 100),
                        Currency = "pln",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Paying for an order at Computer Shop",
                            Description = $"Buyer: {cart.CustomerName}"
                        },
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",

            SuccessUrl = "http://localhost:3000/payment/success?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = "http://localhost:3000/payment/cancel",

            Metadata = new Dictionary<string, string>
            {
                { "UserId", cart.UserId ?? string.Empty },
                { "CustomerName", cart.CustomerName },
                { "CustomerEmail", cart.CustomerEmail },
                { "ShippingAddress", cart.ShippingAddress },
                { "TotalPrice", totalPrice.ToString() },
                { "SerializedItems", serializedItems }
            }
        };

        // Sending a request to Stripe
        var service = new SessionService();
        Session session = await service.CreateAsync(options);

        // Return the generated link (https://checkout.stripe.com/...)
        return session.Url;
    }
}
