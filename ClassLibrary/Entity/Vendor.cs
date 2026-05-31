using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Entity;

[Table("Vendor", Schema = "Warehouse")]
public class Vendor
{
    public long Id { get; set; }

    [MaxLength(150)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(30)]
    public string TelNumber { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(50)]
    public required string Country { get; set; }
}
