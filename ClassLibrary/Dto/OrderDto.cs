using ClassLibrary.Entity;

namespace ClassLibrary.Dto;

// For request a payment link
public class CartCheckoutDto
{
    public string? UserId { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public required string ShippingAddress { get; set; }
    public required List<OrderItemDto> Items { get; set; }
}

// To create a record in the database
public class OrderCreateDto
{
    public string? UserId { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public required string ShippingAddress { get; set; }
    public decimal TotalPrice { get; set; }

    // Instead of a complex object, we store serialized JSON with a list of products for the database.
    public string SerializedItems { get; set; }
    public string StripeSessionId { get; set; }
}

// For return the finished data to the frontend
public class OrderResponseDto
{
    public int OrderId { get; set; }
    public string? UserId { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public required string ShippingAddress { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

// DTO for product items
public class OrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}