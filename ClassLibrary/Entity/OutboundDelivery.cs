using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Entity;

[Table("OutboundDelivery", Schema = "Warehouse")]
public class OutboundDelivery
{
    public long Id { get; set; }

    [MaxLength(50)]
    public required string ShippingNumber { get; set; }

    public DateTime DepartureDate { get; set; }

    [MaxLength(250)]
    public required string DestinationAddress { get; set; }

    [MaxLength(100)]
    public string RecipientName { get; set; } = string.Empty;

    public virtual ICollection<OutboundDeliveryItem> Items { get; set; } = new List<OutboundDeliveryItem>();
}

[Table("OutboundDeliveryItem", Schema = "Warehouse")]
public class OutboundDeliveryItem
{
    public long Id { get; set; }

    public long OutboundDeliveryId { get; set; }
    public virtual OutboundDelivery OutboundDelivery { get; set; } = null!;

    public long WarehouseItemId { get; set; }
    public virtual WarehouseItem WarehouseItem { get; set; } = null!;

    public int QuantityShipped { get; set; }
}