namespace ECommerceProductsAPI.Models;

public class ProductAttribute
{
    public int Id { get; set; }
    public required string Type { get; set; }
    public required string Value { get; set; }
    public List<ProductVariationAttribute> VariationAttributes { get; set; } = [];
}