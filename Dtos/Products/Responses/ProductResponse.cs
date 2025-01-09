using ECommerceProductsAPI.Models.Enums;

namespace ECommerceProductsAPI.Dtos.Products.Responses;

public class ProductResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ProductType { get; set; }
    public bool IsSubscription { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public List<ProductVariationResponse> Variations { get; set; } = [];
    public List<GroupedProductItemResponse> GroupedProductItems { get; set; } = [];
}