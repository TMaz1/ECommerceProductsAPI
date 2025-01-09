using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ECommerceProductsAPI.Models.Enums;

namespace ECommerceProductsAPI.Dtos.Products.Requests;

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProductType? ProductType { get; set; }
    public bool IsSubscription { get; set; } = false;
}