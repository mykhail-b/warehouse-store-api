namespace ClassLibrary.Dto;

public class DeliveryDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public DateTime ShippedAt { get; set; } = DateTime.UtcNow;

    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }

    public List<OrderItemDto> Items { get; set; } = new();
}
