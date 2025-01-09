namespace ECommerceProductsAPI.Models;

public class ProductVariationAttribute
{
    public int Id { get; set; }
    public int VariationId { get; set; }
    public int AttributeId { get; set; }

    public ProductVariation Variation { get; set; } = null!;
    public ProductAttribute Attribute { get; set; } = null!;
}
