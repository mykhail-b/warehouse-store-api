using ClassLibrary.Entity;

namespace ClassLibrary.Dto;

/// <summary>
/// Data transfer object for detailed public warehouse item information.
/// </summary>
public class WarehouseItemDetailDto
{
    /// <summary>Gets or sets the item ID.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the item name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the item description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets whether the item is available.</summary>
    public bool IsAvailable { get; set; }

    /// <summary>Gets or sets the item cost.</summary>
    public decimal Cost { get; set; }

    /// <summary>Gets or sets the currency type.</summary>
    public CurrencyType Currency { get; set; }
}

/// <summary>
/// Data transfer object for summarized warehouse item information.
/// </summary>
public class WarehouseItemSummaryDto
{
    /// <summary>Gets or sets the item ID.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the item name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets whether the item is available.</summary>
    public bool IsAvailable { get; set; }

    /// <summary>Gets or sets the item category.</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>Gets or sets the item cost.</summary>
    public decimal Cost { get; set; }

    /// <summary>Gets or sets the currency type.</summary>
    public CurrencyType Currency { get; set; }
}

