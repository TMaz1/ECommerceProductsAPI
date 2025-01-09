using System.ComponentModel.DataAnnotations.Schema;
using ECommerceProductsAPI.Models.Enums;

namespace ECommerceProductsAPI.Models;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    public ProductType? ProductType { get; set; }
    public bool IsSubscription { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<ProductVariation> Variations { get; set; } = [];
    public List<GroupedProductItem> GroupedProductItems { get; set; } = [];
}