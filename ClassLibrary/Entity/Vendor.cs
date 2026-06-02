using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Entity;

/// <summary>
/// Represents a vendor or supplier of goods.
/// Stores vendor contact information and operational details.
/// </summary>
[Table("Vendor", Schema = "Warehouse")]
public class Vendor
{
    /// <summary>
    /// Gets or sets the vendor's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the vendor's name (max 150 characters).
    /// </summary>
    [MaxLength(150)]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the vendor's description (max 500 characters).
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vendor's telephone number (max 30 characters).
    /// </summary>
    [MaxLength(30)]
    public string TelNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vendor's email address (max 100 characters).
    /// </summary>
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vendor's country (max 50 characters).
    /// </summary>
    [MaxLength(50)]
    public required string Country { get; set; }
}
