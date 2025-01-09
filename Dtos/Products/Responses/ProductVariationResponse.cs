namespace ECommerceProductsAPI.Dtos.Products.Responses;

public class ProductVariationResponse
{
    public int Id { get; set; }
    public string? SKU { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public List<ProductAttributeResponse>? Attributes { get; set; }
}