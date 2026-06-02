namespace ClassLibrary.Dto;

/// <summary>
/// Data transfer object for a shopping cart item.
/// </summary>
public class CartItemDto
{
    /// <summary>Gets or sets the warehouse item ID.</summary>
    public int ItemId { get; set; }

    /// <summary>Gets or sets the item's name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the quantity in the cart.</summary>
    public int Quantity { get; set; }

    /// <summary>Gets or sets the price per unit.</summary>
    public decimal Price { get; set; }
}
