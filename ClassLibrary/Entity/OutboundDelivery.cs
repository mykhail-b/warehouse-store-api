using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ClassLibrary.Entity;

/// <summary>
/// Represents an outbound delivery to customers.
/// Tracks outgoing shipments with destination and recipient information.
/// </summary>
[Table("OutboundDelivery", Schema = "Warehouse")]
public class OutboundDelivery
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    [JsonPropertyName("shippingNumber")]
    [MaxLength(50)]
    public required string ShippingNumber { get; set; }
    
    [JsonPropertyName("departureDate")]
    public DateTime DepartureDate { get; set; }

    [JsonPropertyName("destinationAddress")]
    [MaxLength(250)]
    public required string DestinationAddress { get; set; }

    [JsonPropertyName("recipientName")]
    [MaxLength(100)]
    public string RecipientName { get; set; } = string.Empty;

    public virtual ICollection<OutboundDeliveryItem> Items { get; set; } = new List<OutboundDeliveryItem>();
}

/// <summary>
/// Represents an item within an outbound delivery.
/// </summary>
[Table("OutboundDeliveryItem", Schema = "Warehouse")]
public class OutboundDeliveryItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("outboundDeliveryId")]
    public int OutboundDeliveryId { get; set; }

    public virtual OutboundDelivery OutboundDelivery { get; set; } = null!;

    [JsonPropertyName("warehouseItemId")]
    public int WarehouseItemId { get; set; }

    public virtual WarehouseItem WarehouseItem { get; set; } = null!;

    [JsonPropertyName("quantityShipped")]
    public int QuantityShipped { get; set; }
}