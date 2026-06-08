using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ClassLibrary.Entity;

// TODO: Rewrite summary to updated entity

[Table("Delivery", Schema = "Warehouse")]
public class Delivery
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("orderId")]
    public required int OrderId { get; set; }
    public virtual Order? Order { get; set; }


    [JsonPropertyName("shippedAt")]
    public DateTime ShippedAt { get; set; } = DateTime.UtcNow;
}