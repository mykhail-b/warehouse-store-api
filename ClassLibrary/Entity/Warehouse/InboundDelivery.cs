using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Entity.Warehouse;

[Table("InboundDelivery", Schema = "Warehouse")]
public class InboundDelivery
{
    public long Id { get; set; }

    [MaxLength(50)]
    public required string DeliveryNumber { get; set; }

    public DateTime ArrivalDate { get; set; }

    public long VendorId { get; set; }
    public virtual Vendor Vendor { get; set; } = null!;

    public long? VehicleId { get; set; }
    public virtual Vehicle? Vehicle { get; set; }

    [MaxLength(500)]
    public string Note { get; set; } = string.Empty;

    public virtual ICollection<InboundDeliveryItem> Items { get; set; } = new List<InboundDeliveryItem>();
}

[Table("InboundDeliveryItem", Schema = "Warehouse")]
public class InboundDeliveryItem
{
    public long Id { get; set; }

    public long InboundDeliveryId { get; set; }
    public virtual InboundDelivery InboundDelivery { get; set; } = null!;

    public long WarehouseItemId { get; set; }
    public virtual WarehouseItem WarehouseItem { get; set; } = null!;

    public int QuantityReceived { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchasePrice { get; set; }
}