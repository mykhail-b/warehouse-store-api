using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Entity;

/// <summary>
/// Specifies the currency type for pricing.
/// </summary>
public enum CurrencyType
{
    /// <summary>Euro currency</summary>
    EUR,
    /// <summary>US Dollar currency</summary>
    USD
}

/// <summary>
/// Represents an item in the warehouse inventory.
/// Tracks item details, quantity, dimensions, cost, and availability status.
/// </summary>
[Table("Item", Schema = "Warehouse")]
public class WarehouseItem
{
    /// <summary>
    /// Gets or sets the item's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the item's name (max 100 characters).
    /// </summary>
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the item's detailed description (max 500 characters).
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the item's category (max 50 characters).
    /// </summary>
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current quantity in stock.
    /// </summary>
    public int CurrentQuantity { get; set; }

    /// <summary>
    /// Gets or sets the minimum quantity threshold for reordering.
    /// </summary>
    public int MinQuantity { get; set; }

    /// <summary>
    /// Gets or sets the maximum quantity allowed in stock.
    /// </summary>
    public int MaxQuantity { get; set; }

    /// <summary>
    /// Gets or sets the item's weight in kilograms.
    /// </summary>
    [Column(TypeName = "decimal(12,3)")]
    public decimal WeightKg { get; set; }

    /// <summary>
    /// Gets or sets the item's volume in cubic meters.
    /// </summary>
    [Column(TypeName = "decimal(12,4)")]
    public decimal VolumeCbm { get; set; }

    /// <summary>
    /// Gets or sets the item's cost.
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Cost { get; set; }

    /// <summary>
    /// Gets or sets the currency type for the cost.
    /// </summary>
    public CurrencyType Currency { get; set; } = CurrencyType.EUR;

    /// <summary>
    /// Gets or sets the unique item code.
    /// </summary>
    [MaxLength(50)]
    public required string ItemCode { get; set; }

    /// <summary>
    /// Gets or sets the creation date and time.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the last modification date and time.
    /// </summary>
    public DateTime ChangedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item is available for use.
    /// </summary>
    public bool IsAvailable { get; set; }
}
