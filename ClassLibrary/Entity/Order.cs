using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Entity;

[Table("Order", Schema = "Warehouse")]
public class Order
{
    public long Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public virtual UserAccount User { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

[Table("OrderItem", Schema = "Warehouse")]
public class OrderItem
{
    public long Id { get; set; }

    public long OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    public long WarehouseItemId { get; set; }
    public virtual WarehouseItem WarehouseItem { get; set; } = null!;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
}

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}
