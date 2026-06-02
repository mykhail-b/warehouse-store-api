using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Entity;

/// <summary>
/// Represents an outbound delivery to customers.
/// Tracks outgoing shipments with destination and recipient information.
/// </summary>
[Table("OutboundDelivery", Schema = "Warehouse")]
public class OutboundDelivery
{
    /// <summary>
    /// Gets or sets the delivery's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the shipping number for tracking (max 50 characters).
    /// </summary>
    [MaxLength(50)]
    public required string ShippingNumber { get; set; }

    /// <summary>
    /// Gets or sets the shipment departure date.
    /// </summary>
    public DateTime DepartureDate { get; set; }

    /// <summary>
    /// Gets or sets the destination address (max 250 characters).
    /// </summary>
    [MaxLength(250)]
    public required string DestinationAddress { get; set; }

    /// <summary>
    /// Gets or sets the recipient name (max 100 characters).
    /// </summary>
    [MaxLength(100)]
    public string RecipientName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of items in this shipment.
    /// </summary>
    public virtual ICollection<OutboundDeliveryItem> Items { get; set; } = new List<OutboundDeliveryItem>();
}

/// <summary>
/// Represents an item within an outbound delivery.
/// </summary>
[Table("OutboundDeliveryItem", Schema = "Warehouse")]
public class OutboundDeliveryItem
{
    /// <summary>
    /// Gets or sets the item's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the outbound delivery ID this item belongs to.
    /// </summary>
    public int OutboundDeliveryId { get; set; }

    /// <summary>
    /// Gets or sets the associated outbound delivery.
    /// </summary>
    public virtual OutboundDelivery OutboundDelivery { get; set; } = null!;

    /// <summary>
    /// Gets or sets the warehouse item ID.
    /// </summary>
    public int WarehouseItemId { get; set; }

    /// <summary>
    /// Gets or sets the associated warehouse item.
    /// </summary>
    public virtual WarehouseItem WarehouseItem { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quantity shipped.
    /// </summary>
    public int QuantityShipped { get; set; }
}