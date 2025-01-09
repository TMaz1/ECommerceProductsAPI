using ECommerceProductsAPI.Models.Enums;

namespace ECommerceProductsAPI.Dtos.Products.Responses;

public class BasicProductResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public string ProductType { get; set; } = string.Empty;
    public bool IsSubscription { get; set; } = false;
    
}