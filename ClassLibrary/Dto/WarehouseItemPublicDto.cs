using ClassLibrary.Entity;

namespace ClassLibrary.Dto;

public class ProductDetailDto
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public required string Description { get; set; }

    public bool IsAvailable { get; set; }

    public decimal Cost { get; set; }
    public CurrencyType Currency { get; set; }
}

public class ProductSummaryDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public CurrencyType Currency { get; set; }
}

