using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ClassLibrary.Entity;

[Table("Order", Schema = "Warehouse")]
public class Order
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("userId")]
    public string? UserId { get; set; }
    public virtual UserAccount? User { get; set; }

    [JsonPropertyName("customerName")]
    public string? CustomerName { get; set; }

    [JsonPropertyName("customerEmail")]
    public string? CustomerEmail { get; set; }

    [JsonPropertyName("shippingAddress")]
    public required string ShippingAddress { get; set; }

    [JsonPropertyName("shippingNumber")]
    public required string ShippingNumber { get; set; }

    [JsonPropertyName("stripeSessionId")]
    public string? StripeSessionId { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("status")]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    [JsonPropertyName("totalPrice")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice => Items?.Sum(item => item.Price * item.Quantity) ?? 0;
}


[Table("OrderItem", Schema = "Warehouse")]
public class OrderItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }
    [JsonIgnore]
    public virtual Order? Order { get; set; }

    [JsonPropertyName("productId")]
    public int ProductId { get; set; }

    [JsonIgnore]
    public virtual Product? Product { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("price")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
}

public enum OrderStatus
{
    Pending,
    Shipped,
    Delivered,
    Cancelled
}
