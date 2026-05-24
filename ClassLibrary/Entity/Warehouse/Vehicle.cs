using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Entity.Warehouse;

[Table("Vehicle", Schema = "Logistics")] // Совет: схему лучше назвать "Logistics" или "Transport", в "Warehouse" машинам тесновато :)
public class Vehicle
{
    public long Id { get; set; }

    public long VendorId { get; set; }
    public virtual Vendor Vendor { get; set; } = null!;

    [MaxLength(150)]
    public required string DriverName { get; set; }

    [MaxLength(20)]
    public required string PlateNumber { get; set; }

    [MaxLength(50)]
    public string Brand { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Model { get; set; } = string.Empty;

    [Column(TypeName = "decimal(12,3)")]
    public decimal MaxWeightCapacityKg { get; set; }

    [Column(TypeName = "decimal(12,4)")]
    public decimal MaxVolumeCapacityCbm { get; set; }
}
