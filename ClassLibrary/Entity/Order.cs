using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ClassLibrary.Entity;

/// <summary>
/// Represents a customer order.
/// Tracks order metadata including creation date, status, and associated items.
/// </summary>
[Table("Order", Schema = "Warehouse")]
public class Order
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("userId")]

    // For registered users, we can link to their account via UserId. For guest customers, this will be null.
    public string? UserId { get; set; }
    public virtual UserAccount? User { get; set; }

    // For guest customers, we can store their name directly in the order record
    [JsonPropertyName("customerName")]
    public string? CustomerName { get; set; }
    [JsonPropertyName("customerEmail")]
    public string? CustomerEmail { get; set; }

    [JsonPropertyName("shippingAddress")]
    public string ShippingAddress { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("status")]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

/// <summary>
/// Represents an item within an order.
/// </summary>
[Table("OrderItem", Schema = "Warehouse")]
public class OrderItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }
    [JsonIgnore]
    public virtual Order Order { get; set; } = null!;

    [JsonPropertyName("warehouseItemId")]
    public int WarehouseItemId { get; set; }
    [JsonIgnore]
    public virtual WarehouseItem WarehouseItem { get; set; } = null!;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("price")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
}

/// <summary>
/// Specifies the status of an order.
/// </summary>
public enum OrderStatus
{
    Pending,
    Shipped,
    Delivered,
    Cancelled
}
