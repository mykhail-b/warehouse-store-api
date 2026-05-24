using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Entity.Warehouse;

public enum CurrencyType
{
    EUR,
    USD
}

[Table("Item", Schema = "Warehouse")]
public class WarehouseItem
{
    public long Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    public int CurrentQuantity { get; set; }
    public int MinQuantity { get; set; }
    public int MaxQuantity { get; set; }

    [Column(TypeName = "decimal(12,3)")]
    public decimal WeightKg { get; set; }

    [Column(TypeName = "decimal(12,4)")]
    public decimal VolumeCbm { get; set; } // Cubic Meters

    [Column(TypeName = "decimal(18,2)")]
    public decimal Cost { get; set; }
    public CurrencyType Currency { get; set; } = CurrencyType.EUR;

    [MaxLength(50)]
    public required string ItemCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ChangedAt { get; set; }
    public bool IsAvailable { get; set; }
}
