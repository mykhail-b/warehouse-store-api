using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Entity;

/// <summary>
/// Represents an inbound delivery from a vendor.
/// Tracks incoming inventory with vendor information and received items.
/// </summary>
[Table("InboundDelivery", Schema = "Warehouse")]
public class InboundDelivery
{
    /// <summary>
    /// Gets or sets the delivery's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the delivery number for reference (max 50 characters).
    /// </summary>
    [MaxLength(50)]
    public required string DeliveryNumber { get; set; }

    /// <summary>
    /// Gets or sets the delivery arrival date.
    /// </summary>
    public DateTime ArrivalDate { get; set; }

    /// <summary>
    /// Gets or sets the vendor ID.
    /// </summary>
    public int VendorId { get; set; }

    /// <summary>
    /// Gets or sets the associated vendor.
    /// </summary>
    public virtual Vendor Vendor { get; set; } = null!;

    /// <summary>
    /// Gets or sets optional notes about the delivery (max 500 characters).
    /// </summary>
    [MaxLength(500)]
    public string Note { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of items in this delivery.
    /// </summary>
    public virtual ICollection<InboundDeliveryItem> Items { get; set; } = new List<InboundDeliveryItem>();
}

/// <summary>
/// Represents an item within an inbound delivery.
/// </summary>
[Table("InboundDeliveryItem", Schema = "Warehouse")]
public class InboundDeliveryItem
{
    /// <summary>
    /// Gets or sets the item's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the inbound delivery ID this item belongs to.
    /// </summary>
    public int InboundDeliveryId { get; set; }

    /// <summary>
    /// Gets or sets the associated inbound delivery.
    /// </summary>
    public virtual InboundDelivery InboundDelivery { get; set; } = null!;

    /// <summary>
    /// Gets or sets the warehouse item ID.
    /// </summary>
    public int WarehouseItemId { get; set; }

    /// <summary>
    /// Gets or sets the associated warehouse item.
    /// </summary>
    public virtual WarehouseItem WarehouseItem { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quantity received.
    /// </summary>
    public int QuantityReceived { get; set; }

    /// <summary>
    /// Gets or sets the purchase price per unit.
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchasePrice { get; set; }
}