using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Entity;

/// <summary>
/// Represents a customer order.
/// Tracks order metadata including creation date, status, and associated items.
/// </summary>
[Table("Order", Schema = "Warehouse")]
public class Order
{
    /// <summary>
    /// Gets or sets the order's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the user ID who placed the order.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the associated user account.
    /// </summary>
    public virtual UserAccount User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the order creation date and time.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the current status of the order.
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>
    /// Gets or sets the collection of items in this order.
    /// </summary>
    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

/// <summary>
/// Represents an item within an order.
/// </summary>
[Table("OrderItem", Schema = "Warehouse")]
public class OrderItem
{
    /// <summary>
    /// Gets or sets the item's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the order ID this item belongs to.
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the associated order.
    /// </summary>
    public virtual Order Order { get; set; } = null!;

    /// <summary>
    /// Gets or sets the warehouse item ID.
    /// </summary>
    public int WarehouseItemId { get; set; }

    /// <summary>
    /// Gets or sets the associated warehouse item.
    /// </summary>
    public virtual WarehouseItem WarehouseItem { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quantity ordered.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the price per unit at the time of order.
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
}

/// <summary>
/// Specifies the status of an order.
/// </summary>
public enum OrderStatus
{
    /// <summary>Order pending processing</summary>
    Pending,
    /// <summary>Order currently being processed</summary>
    Processing,
    /// <summary>Order has been shipped</summary>
    Shipped,
    /// <summary>Order has been delivered</summary>
    Delivered,
    /// <summary>Order has been cancelled</summary>
    Cancelled
}
