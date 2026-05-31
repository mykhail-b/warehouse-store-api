using ClassLibrary.Entity;

namespace ClassLibrary.Dto;

public class WarehouseItemDetailDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public decimal Cost { get; set; }
    public CurrencyType Currency { get; set; }
}
public class WarehouseItemSummaryDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public decimal Cost { get; set; }
    public CurrencyType Currency { get; set; }
}

