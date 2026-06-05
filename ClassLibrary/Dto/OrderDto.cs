using ClassLibrary.Entity;

namespace ClassLibrary.Dto;

/// <summary>
/// DTO for creating a new order.
/// </summary>
public class CreateOrderDto
{
    public string? UserId { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

/// <summary>
/// DTO for creating an order item.
/// </summary>
public class CreateOrderItemDto
{
    public int WarehouseItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

/// <summary>
/// DTO for updating an existing order.
/// </summary>
public class UpdateOrderDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
}

/// <summary>
/// DTO for returning order data to the client.
/// </summary>
public class OrderResponseDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}

/// <summary>
/// DTO for returning order item data to the client.
/// </summary>
public class OrderItemResponseDto
{
    public int Id { get; set; }
    public int WarehouseItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}