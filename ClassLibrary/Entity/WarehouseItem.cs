using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ClassLibrary.Entity;

/// <summary>
/// Specifies the currency type for pricing.
/// </summary>
public enum CurrencyType
{
    EUR,
    USD
}

/// <summary>
/// Represents an item in the warehouse inventory.
/// Tracks item details, quantity, dimensions, cost, and availability status.
/// </summary>
[Table("Item", Schema = "Warehouse")]
public class WarehouseItem
{

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("category")]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("currentQuantity")]
    public int CurrentQuantity { get; set; }

    [JsonPropertyName("minQuantity")]
    public int MinQuantity { get; set; }

    [JsonPropertyName("maxQuantity")]
    public int MaxQuantity { get; set; }

    [JsonPropertyName("weightKg")]
    [Column(TypeName = "decimal(12,3)")]
    public decimal WeightKg { get; set; }

    [JsonPropertyName("volumeCbm")]
    [Column(TypeName = "decimal(12,4)")]
    public decimal VolumeCbm { get; set; }
    
    [JsonPropertyName("cost")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Cost { get; set; }

    [JsonPropertyName("currency")]
    public CurrencyType Currency { get; set; } = CurrencyType.EUR;

    [JsonPropertyName("itemCode")]
    [MaxLength(50)]
    public required string ItemCode { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("changedAt")]
    public DateTime ChangedAt { get; set; }

    [JsonPropertyName("isAvailable")]
    public bool IsAvailable { get; set; }
}
