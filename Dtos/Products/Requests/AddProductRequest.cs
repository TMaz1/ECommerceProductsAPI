using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ECommerceProductsAPI.Models.Enums;

namespace ECommerceProductsAPI.Dtos.Products.Requests;

public class AddProductRequest
{
    public required string Name { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public required decimal Price { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ProductType ProductType { get; set; }
    public bool IsSubscription { get; set; } = false;
}